import Foundation
import FidoClientIOS

struct BiometricErrors {
    var errorMessage: String
    var appWebInterface: AppWebInterface?
    var action: String
    var errorType: FidoError
    var outcome: String
    
    init(errorType: FidoError, action: String, appWebInterface: AppWebInterface) {
        self.errorType = errorType
        self.appWebInterface = appWebInterface
        self.action = action
        switch errorType {
            case .userCancelled:
                self.errorMessage = "User cancelled the toggle"
                self.outcome = BiometricOutcomes.cancelled.rawValue
            case .invalidBiometrics:
                self.errorMessage = "Invalid Biometrics used"
                self.outcome = BiometricOutcomes.failed.rawValue
            case .keyRetrievalError:
                self.errorMessage = "An error occurred during key retrieval"
            self.outcome = BiometricOutcomes.failed.rawValue
            case .parsingError:
                self.errorMessage = "A parsing error occurred"
            self.outcome = BiometricOutcomes.failed.rawValue
            case .encryptionError:
                self.errorMessage = "An encryption error occurred"
                self.outcome = BiometricOutcomes.failed.rawValue
            case .networkRequestError:
                self.errorMessage = "A network error occurred"
            self.outcome = BiometricOutcomes.failed.rawValue
            case .accessTokenError:
                self.errorMessage = "An access token error occurred"
                self.outcome = BiometricOutcomes.failed.rawValue
            case .genericError:
                self.errorMessage = "A generic error occurred"
                self.outcome = BiometricOutcomes.failed.rawValue
            
        }
    }
    
    func updateWeb(biometricType: BiometricType) {
        // handle authentication actions that are not to cancel
        if (action == BiometricActions.authentication.rawValue && outcome != BiometricOutcomes.cancelled.rawValue) {
            appWebInterface?.biometricLoginFailure()
            return
        }
        
        // handle cancel on face ID which should cause login biometric error
        if (action == BiometricActions.authentication.rawValue && outcome == BiometricOutcomes.cancelled.rawValue && biometricType == BiometricType.faceID) {
            appWebInterface?.biometricLoginFailure()
             return
        }
        
        // handle reg/dereg actions
        if ((action == BiometricActions.register.rawValue || action == BiometricActions.deregister.rawValue) && outcome == BiometricOutcomes.cancelled.rawValue) {
            if (biometricType == BiometricType.touchID) {
                appWebInterface?.biometricCompletion(action: action, outcome: outcome, errorCode: "")
                return
            } else {
                appWebInterface?.biometricCompletion(action: action, outcome: BiometricOutcomes.failed.rawValue, errorCode: BiometricErrorCodes.cannotChangeBiometrics.rawValue)
                return
            }
        }
        
        // default throw up cant change
        appWebInterface?.biometricCompletion(action: action, outcome: outcome, errorCode: BiometricErrorCodes.cannotChangeBiometrics.rawValue)
    }
}

