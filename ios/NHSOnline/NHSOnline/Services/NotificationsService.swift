import Foundation
import os.log
import UIKit
import UserNotifications

class NotificationsService {
    private let appWebInterface: AppWebInterface
    private var trigger: String = ""
    private let cookieHandler: CookieHandler
    
    init(appWebInterface: AppWebInterface, cookieHandler: CookieHandler) {
        self.appWebInterface = appWebInterface
        self.cookieHandler = cookieHandler
    }

    func registerForPushNotifications(trigger: String) {
        if #available(iOS 10.0, *) {
            self.trigger = trigger
            UNUserNotificationCenter.current()
                .requestAuthorization(options: [.alert, .sound, .badge]) {
                    [weak self] granted, error in
                    guard granted else {
                        self?.unauthorised()
                        return
                    }

                    DispatchQueue.main.async {
                        UIApplication.shared.registerForRemoteNotifications()
                    }
                }
        } else {
            logNotSupported()
        }
    }
    
    func getNotificationsStatus() {
        if #available(iOS 10.0, *) {
            UNUserNotificationCenter.current().getNotificationSettings { settings in
                let status: String
                
                if #available(iOS 12.0, *) {
                    status = self.getNotificationsStatus(with: compatibility.iOS12(), authorizationStatus: settings.authorizationStatus)
                } else {
                    status = self.getNotificationsStatus(with: compatibility.iOS10(), authorizationStatus: settings.authorizationStatus)
                }
                
                self.appWebInterface.getNotificationsStatus(status: status)
            }
        }
    }
    
    @available(iOS 10, *)
    func getNotificationsStatus(with _: compatibility.iOS10, authorizationStatus: UNAuthorizationStatus) -> String {
        var status: ExhaustiveAuthorisationStatus;
        
        switch authorizationStatus {
        case .authorized:
            status = .authorised
        case .notDetermined:
            status = .notDetermined
        case .denied:
            status = .denied
        default:
            status = .unknown
        }
        return self.mapAuthorisationStatus(status: status)
    }
    
    @available(iOS 12, *)
    func getNotificationsStatus(with _: compatibility.iOS12, authorizationStatus: UNAuthorizationStatus) -> String {
        var status: ExhaustiveAuthorisationStatus;
        
        switch authorizationStatus {
        case .authorized:
            status = .authorised
        case .provisional:
            status = .provisional
        case .notDetermined:
            status = .notDetermined
        case .denied:
            status = .denied
        @unknown default:
            status = .unknown
        }
        return self.mapAuthorisationStatus(status: status)
    }
    
    private func mapAuthorisationStatus(status: ExhaustiveAuthorisationStatus) -> String {
        var outgoingStatus: OutgoingAuthorisationStatus;
        
        switch status {
        case .authorised, .provisional:
            Logger.logInfo(message: "Allow notifications is enabled")
           outgoingStatus = .authorised
        case .notDetermined:
            Logger.logInfo(message: "Authorisation request has not been made yet")
            outgoingStatus = .notDetermined
        case .denied, .unknown:
            Logger.logInfo(message: "Allow notifications is disabled")
            outgoingStatus = .denied
        }

        return outgoingStatus.rawValue
    }

    func authorised(deviceToken: Data) {
        let devicePns = deviceToken.reduce("", { $0 + String(format: "%02X", $1) })

        Logger.logInfo(message: "Notifications permission granted")
        appWebInterface.notificationsAuthorised(devicePns: devicePns, trigger: trigger)
        resetTrigger()
    }

    func failedToRegister() {
        Logger.logInfo(message: "Failed to register for notifications")
        appWebInterface.notificationsUnauthorised()
        resetTrigger()
    }

    private func unauthorised() {
        Logger.logInfo(message: "Notifications permission not granted")
        appWebInterface.notificationsUnauthorised()
        resetTrigger()
    }

    private func logNotSupported() {
        Logger.logInfo(message: "Os too old, not supported")
    }

    private func resetTrigger() {
        trigger = "load"
    }

    private enum ExhaustiveAuthorisationStatus: String {
        case notDetermined
        case denied
        case authorised
        case provisional
        case unknown
    }
        
    private enum OutgoingAuthorisationStatus: String {
        case notDetermined
        case denied
        case authorised
    }
}
