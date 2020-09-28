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
                
                status = AuthorisationStatus.notDetermined
                
                if (settings.authorizationStatus == .notDetermined) {
                    Logger.logInfo(message: "Authorisation request has not been made yet")
                }
                
                if (settings.authorizationStatus == .denied) {
                    Logger.logInfo(message: "Authorisation request has not been made yet")
                }
                
                if (settings.authorizationStatus == .authorized) {
                    status = AuthorisationStatus.authorised
                    Logger.logInfo(message: "Allow notifications is enabled")
                }
                
                if #available(iOS 12.0, *) {
                    if (settings.authorizationStatus == .provisional) {
                        status = AuthorisationStatus.authorised
                        Logger.logInfo(message: "Allow notifications is enabled")
                    }
                }
                
                // when everyone is on xcode 12 and azure fixes app problem with xcode 12, here we can add the check for status .ephemeral

                self.appWebInterface.getNotificationsStatus(status: status.rawValue)
            }
        }
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

    private enum AuthorisationStatus: String {
        case notDetermined
        case denied
        case authorised
    }
}
