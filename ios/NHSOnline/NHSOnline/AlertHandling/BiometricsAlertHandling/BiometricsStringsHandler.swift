import Foundation
import LocalAuthentication
import UIKit
import os.log

class BiometricStringHandler {
    
    func getBiometricAlert(type: BiometricAlertType) -> UIAlertController {
        let biometricAlert = BiometricAlert(alertType: (type, getBiometricType()))
        let alertController = UIAlertController(title: biometricAlert.title, message: biometricAlert.message, preferredStyle: .alert)
        let cancel = UIAlertAction(title: NSLocalizedString("AppUpdateRequiredCloseButtonText", comment: ""), style: .cancel) { (action) -> Void in
            alertController.dismiss(animated: true, completion: nil)
        }
        
        alertController.addAction(cancel)
        
        return alertController
    }
    
    func getBiometricType() -> BiometricType {
        var authError: NSError?
        let laContext = LAContext()
        laContext.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: &authError)
        if #available(iOS 11.0, *) {
            if laContext.biometryType == .faceID {
                return BiometricType.faceID
            }
        }
        return BiometricType.touchID
    }
}
