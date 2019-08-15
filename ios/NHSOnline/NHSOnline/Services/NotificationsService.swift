import Foundation
import os.log
import UIKit
import UserNotifications

class NotificationsService {
    private let appWebInterface: AppWebInterface
    
    init(appWebInterface: AppWebInterface) {
        self.appWebInterface = appWebInterface
    }
    
    func registerForPushNotifications() {
        if #available(iOS 10.0, *) {
            UNUserNotificationCenter.current()
                .requestAuthorization(options: [.alert, .sound, .badge]) {
                    [weak self] granted, error in
                    guard granted else {
                        self?.unauthorised()
                        return
                    }
                    self?.getNotificationSettings()
            }
        } else {
            logNotSupported()
        }
    }
    
    func getNotificationSettings() {
        if #available(iOS 10.0, *) {
            UNUserNotificationCenter.current().getNotificationSettings { settings in
                guard settings.authorizationStatus == .authorized else {
                    self.unauthorised()
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
    
    func authorised(deviceToken: Data) {
        let devicePns = deviceToken.reduce("", {$0 + String(format: "%02X", $1)})

        Logger.logInfo(message: "Notifications permission granted")
        self.appWebInterface.notificationsAuthorised(devicePns: devicePns)
    }
    
    func failedToRegister() {
        Logger.logInfo(message: "Failed to register for notifications")
        self.appWebInterface.notificationsUnauthorised()
    }
    
    private func unauthorised() {
        Logger.logInfo(message: "Notifications permission not granted")
        self.appWebInterface.notificationsUnauthorised()
    }

    private func logNotSupported() {
        Logger.logInfo(message: "Os too old, not supported")
    }
}
