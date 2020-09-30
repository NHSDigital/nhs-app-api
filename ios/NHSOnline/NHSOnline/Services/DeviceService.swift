import Foundation
import os.log
import SafariServices
import LocalAuthentication
import UIKit
import WebKit
import DeviceKit
import FidoClientIOS
import EventKit
import EventKitUI

class DeviceService {
    
    let viewController: UIViewController
    let deviceInfoProtocol: DeviceInfoProtocol

    init(viewController controller: UIViewController, deviceInfoProtocol: DeviceInfoProtocol) {
        self.viewController = controller
        self.deviceInfoProtocol = deviceInfoProtocol
    }
    
    func performCompatibilityCheck() -> Compatibility {
        var compatibility = Compatibility(canUpdate: true, correctVersion: true)
        let version = deviceInfoProtocol.getIOSVersion()
        
        if (version >= 11) {
           return compatibility
        }
    
        compatibility.correctVersion = false

        let groupOfIncompatibleDevices: [Device] = [
            .iPhone5c, .simulator(.iPhone5c),
            .iPhone5, .simulator(.iPhone5),
            .iPhone4s, .simulator(.iPhone4s),
            .iPhone4, .simulator(.iPhone4),
            .iPad4, .simulator(.iPad4),
            .iPadMini, .simulator(.iPadMini),
            .iPad3, .simulator(.iPad3),
            .iPad2, .simulator(.iPad2),
            .iPodTouch5, .simulator(.iPodTouch5)
        ]
            
        // This checks iPhone (1st Gen), iPhone3, iPhone3GS,
        // iPad (1st Gen), iPad 3G and iPod touch Gen 1-4
        // as the DeviceKit does not
        // specify those models like above.
        let groupOfIncompatibleIdentifiers: [String] = ["iPhone1,1", "iPhone1,2",
                                                        "iPhone2,1", "iPad1,1",
                                                        "iPad1,2", "iPod1,1",
                                                        "iPod2,1", "iPod3,1",
                                                        "iPod4,1"]
        
        if groupOfIncompatibleDevices.contains(deviceInfoProtocol.getDeviceDescription()) ||
            groupOfIncompatibleIdentifiers.contains(
            deviceInfoProtocol.getDeviceIdentifier()) {
                compatibility.canUpdate = false
        }
        
        return compatibility
    }
    
    func appUpdateAlertForIOSVersion() {
        let compatibility = self.performCompatibilityCheck()
        var title = NSLocalizedString("AppUpdateRequiredIOSVersionTitle", comment: "")
        var message = NSLocalizedString("AppUpdateIOSVersionRequiredMessage", comment: "")
        let buttonText = NSLocalizedString("AppUpdateIOSVersionRequiredOKButtonText", comment: "")
        var logMessage = "Post Login iOS Compatibility Check Failure: Device does not have the correct iOS, but can be upgraded."
        
        
        if (compatibility.correctVersion) {
            return
        }
        
        if (!compatibility.canUpdate) {
            title = NSLocalizedString("AppUpdateRequiredIOSVersionTitle", comment: "")
            message = NSLocalizedString("AppUpdateIOSVersionRequiredNotCompatibleMessage", comment: "")
            logMessage = "Post Login iOS Compatibility Check Failure: Device does not have the correct iOS, and cannot be upgraded."
        }

        let alert = UIAlertController(title: title, message: message, preferredStyle: .alert)
        let alertButton = UIAlertAction(title: buttonText , style: .cancel) { (action) -> Void in alert.dismiss(animated: true, completion: nil)
        }
        
        alert.addAction(alertButton)
        
        NotificationCenter.default.addObserver(alert, selector: #selector(alert.close), name: CustomNotifications.dismissAllAlerts, object: nil)
        NotificationCenter.default.addObserver(alert, selector: #selector(alert.close), name: CustomNotifications.dismissIOSVersionUpdateAlert, object: nil)
        
        LoggingService().logError(message: logMessage)
        
        self.viewController.present(alert, animated: true, completion: nil)
    }
}
