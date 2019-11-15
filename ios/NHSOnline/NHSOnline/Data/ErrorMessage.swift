import Foundation

struct ErrorMessage{
        let title:String
        let message:String?
        let accessibleMessage:String?
        private(set) var type: ErrorType
    init(_ errorType:ErrorType ){
        type = errorType
        switch errorType {
        case .NoInternetConnection:
            title = NSLocalizedString("ConnectionErrorTitle", comment: "")
            message = NSLocalizedString("ConnectionErrorMessage", comment: "")
            accessibleMessage = NSLocalizedString("AccessibilityConnectionErrorMessage", comment: "")
        case .ServiceUnavailable:
            title = NSLocalizedString("ServiceUnavailableErrorHeader", comment: "")
            message = NSLocalizedString("ServiceUnavailableErrorBody", comment: "")
            accessibleMessage=NSLocalizedString("AccessibilityServiceUnavailableErrorBody", comment: "")
        case .APICallFailure:
            title = NSLocalizedString("ServiceUnavailableErrorMessage", comment: "")
            message = NSLocalizedString("APIUnavailableErrorMessage", comment: "")
            accessibleMessage = NSLocalizedString("AccessibilityAPIUnavailableErrorMessage", comment: "")
        case .BiometricRegistrationError:
             title = NSLocalizedString("BiometricRegistrationErrorHeader", comment: "")
             message = BiometricStringHandler().getErrorStrings().BiometricRegistrationErrorMessage
             accessibleMessage = BiometricStringHandler().getErrorStrings().BiometricRegistrationErrorMessage
        case .BiometricLoginSessionError:
            title = NSLocalizedString("BiometricSessionTimeoutHeader", comment: "")
            message = NSLocalizedString("BiometricSessionTimeoutMessage", comment: "")
            accessibleMessage = NSLocalizedString("AccessibilityBiometricsTimeoutMessage", comment: "")
        case .DownloadError:
            title = NSLocalizedString("DataDownloadErrorHeader", comment: "")
            message = NSLocalizedString("DataDownloadErrorMessage", comment: "")
            accessibleMessage = NSLocalizedString("DataDownloadErrorMessage", comment: "")
        }
}
    enum ErrorType {
        case NoInternetConnection
        case ServiceUnavailable
        case APICallFailure
        case BiometricRegistrationError
        case BiometricLoginSessionError
        case DownloadError
    }

}
