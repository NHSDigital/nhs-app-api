import Foundation

struct BiometricResultStrings {
    let HeaderTextText: String
    let BoxTextText: String
    
    init(_ biometricType: BiometricType, _ isRegistration: Bool) {
        switch biometricType {
        case .faceID:
            if(isRegistration) {
                HeaderTextText = NSLocalizedString("FaceIdTurnedOnHeader", comment: "")
                BoxTextText = NSLocalizedString("FaceIdTurnedOn", comment: "")
            } else {
                HeaderTextText = NSLocalizedString("FaceIdTurnedOffHeader", comment: "")
                BoxTextText = NSLocalizedString("FaceIdTurnedOff", comment: "")
            }
        case .touchID:
            if(isRegistration) {
                HeaderTextText = NSLocalizedString("TouchIdTurnedOnHeader", comment: "")
                BoxTextText = NSLocalizedString("TouchIdTurnedOn", comment: "")
            } else {
                HeaderTextText = NSLocalizedString("TouchIdTurnedOffHeader", comment: "")
                BoxTextText = NSLocalizedString("TouchIdTurnedOff", comment: "")
            }
        }
    }
}
