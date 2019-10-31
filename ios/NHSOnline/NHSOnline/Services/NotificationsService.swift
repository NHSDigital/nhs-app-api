import Foundation
import os.log
import UIKit
import UserNotifications

class NotificationsService {
    private let appWebInterface: AppWebInterface
    private var trigger: String = ""
    
    init(appWebInterface: AppWebInterface) {
        self.appWebInterface = appWebInterface
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
                var status: AuthorisationStatus;
                
                switch settings.authorizationStatus {
                case .notDetermined:
                    status = AuthorisationStatus.notDetermined
                    Logger.logInfo(message: "Authorisation request has not been made yet")
                case .denied:
                    status = AuthorisationStatus.denied
                    Logger.logInfo(message: "Allow notifications is disabled")
                case .authorized,
                        .provisional:
                    status = AuthorisationStatus.authorised
                    Logger.logInfo(message: "Allow notifications is enabled")
                }
                
                self.appWebInterface.getNotificationsStatus(status: status.rawValue)
            }
        }
    }
    
    func authorised(deviceToken: Data) {
        let devicePns = deviceToken.reduce("", {$0 + String(format: "%02X", $1)})
        
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
    
    private enum AuthorisationStatus: String
    {
        case notDetermined
        case denied
        case authorised
    }
}
