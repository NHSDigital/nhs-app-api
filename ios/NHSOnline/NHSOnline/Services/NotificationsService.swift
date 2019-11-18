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
        self.trigger = trigger
        UNUserNotificationCenter.current()
            .requestAuthorization(options: [.alert, .sound, .badge]) {
                [weak self] granted, error in
                guard granted else {
                    self?.unauthorised()
                    return
                }
                
                self?.areNotificationsEnabled { areEnabled in
                    if areEnabled {
                        DispatchQueue.main.async {
                            UIApplication.shared.registerForRemoteNotifications()
                        }
                    } else {
                        self?.unauthorised()
                    }
                }
        }
    }
    
    func areNotificationsEnabled() {
        areNotificationsEnabled { areEnabled in
            self.appWebInterface.areNotificationsEnabled(areEnabled: areEnabled)
        }
    }
    
    private func areNotificationsEnabled(callback: @escaping (Bool) -> Void) {
        UNUserNotificationCenter.current().getNotificationSettings { settings in
            guard settings.authorizationStatus == .authorized else {
                Logger.logInfo(message: "Allow notifications is disabled")
                callback(false)
                return
            }
            
            Logger.logInfo(message: "Allow notifications is enabled")
            callback(true)
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
    
    private func resetTrigger() {
        trigger = "load"
    }
}
