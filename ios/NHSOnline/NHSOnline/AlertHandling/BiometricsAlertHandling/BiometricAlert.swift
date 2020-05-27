import Foundation

struct BiometricAlert {
    let title: String
    let message: String
    
    init(alertType: (type: BiometricAlertType, biometricType: BiometricType)) {
        switch alertType.biometricType {
            case .faceID:
                switch alertType.type {
                    case .NotSupported:
                        self.title = NSLocalizedString("FaceIdNotSupportedTitle", comment: "")
                        self.message = NSLocalizedString("FaceIdNotSupportedMessage", comment: "")
                    case .OSNotSupported:
                        self.title = NSLocalizedString("FaceIdOSNotSupportedTitle", comment: "")
                        self.message = NSLocalizedString("OSNotSupportedMessage", comment: "")
                    case .NoBiometrics:
                        self.title = NSLocalizedString("NoFaceIdTitle", comment: "")
                        self.message = NSLocalizedString("NoFaceIdMessage", comment: "")
                    case .BiometricsInvalidated:
                        self.title = NSLocalizedString("FaceIdInvalidatedTitle", comment: "")
                        self.message = NSLocalizedString("FaceIdInvalidatedMessage", comment: "")
                    case .BiometricLockout:
                        self.title = NSLocalizedString("FaceIdLockedOutTitle", comment: "")
                        self.message = NSLocalizedString("FaceIdLockedOutMessage", comment: "")
                    
            }
            case .touchID:
                switch alertType.type {
                    case .NotSupported:
                        self.title = NSLocalizedString("TouchIdNotSupportedTitle", comment: "")
                        self.message = NSLocalizedString("TouchIdNotSupportedMsg", comment: "")
                    case .OSNotSupported:
                        self.title = NSLocalizedString("TouchIdOSNotSupportedTitle", comment: "")
                        self.message = NSLocalizedString("OSNotSupportedMessage", comment: "")
                    case .NoBiometrics:
                        self.title = NSLocalizedString("NoTouchIdTitle", comment: "")
                        self.message = NSLocalizedString("NoTouchIdMessage", comment: "")
                    case .BiometricsInvalidated:
                        self.title = NSLocalizedString("TouchIdInvalidatedTitle", comment: "")
                        self.message = NSLocalizedString("TouchIdInvalidatedMessage", comment: "")
                    case .BiometricLockout:
                        self.title = NSLocalizedString("TouchIdLockedOutTitle", comment: "")
                        self.message = NSLocalizedString("TouchIdLockedOutMessage", comment: "")
            }
        }
    }
}
