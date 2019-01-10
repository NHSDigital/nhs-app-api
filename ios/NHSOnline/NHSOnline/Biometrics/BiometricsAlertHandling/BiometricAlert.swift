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
                        self.message = NSLocalizedString("NoFaceIdMsg", comment: "")
                    case .BiometricsInvalidated:
                        self.title = NSLocalizedString("FaceIdInvalidatedTitle", comment: "")
                        self.message = NSLocalizedString("FaceIdInvalidatedMessage", comment: "")
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
                }
        }
    }
}
