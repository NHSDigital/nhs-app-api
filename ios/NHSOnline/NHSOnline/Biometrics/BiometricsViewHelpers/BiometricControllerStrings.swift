import Foundation

struct BiometricControllerStrings {
    let HeaderTextViewText: String
    let ContentTextViewText: String
    let TitleLabelText: String
    let ContentLabelText: String
    let BiometricLabelText: String
    
    init(_ biometricType: BiometricType) {
        switch biometricType {
            case .faceID:
                HeaderTextViewText = NSLocalizedString("BiometricWarningHeader", comment: "")
                ContentTextViewText = NSLocalizedString("FaceIdWarningContent", comment: "")
                TitleLabelText = NSLocalizedString("FaceIdInfoMessage1", comment: "")
                ContentLabelText = NSLocalizedString("BiometricInfoMessage2", comment: "")
                BiometricLabelText = NSLocalizedString("FaceId", comment: "")
            case .touchID:
                HeaderTextViewText = NSLocalizedString("BiometricWarningHeader", comment: "")
                ContentTextViewText = NSLocalizedString("TouchIdWarningContent", comment: "")
                TitleLabelText = NSLocalizedString("TouchIdInfoMessage1", comment: "")
                ContentLabelText = NSLocalizedString("BiometricInfoMessage2", comment: "")
                BiometricLabelText = NSLocalizedString("TouchId", comment: "")
        }
    }
}
