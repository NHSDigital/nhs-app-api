import Foundation

struct BiometricErrorStrings {
    let BiometricRegistrationPageHeader: String
    let BiometricRegistrationErrorMessage: String
    
    init(_ biometricType: BiometricType) {
     BiometricRegistrationPageHeader = NSLocalizedString("BiometricRegistrationErrorHeader", comment: "")
        switch biometricType {
            case .faceID:
                BiometricRegistrationErrorMessage = NSLocalizedString("FaceIdRegistrationErrorMessage", comment: "")

            case .touchID:
                BiometricRegistrationErrorMessage = NSLocalizedString("TouchIdRegistrationErrorMessage", comment: "")
        }
    }    
}
