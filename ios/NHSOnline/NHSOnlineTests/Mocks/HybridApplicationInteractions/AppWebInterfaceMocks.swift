import Foundation
@testable import NHSOnline

class AppWebInterfaceMocks: AppWebInterface {
    var biometricCompletionCalled = false
    var biometricLoginFailureCalled = false
    var biometricAction: String = ""
    var biometricOutcome: String = ""
    var biometricErrorCode: String = ""

    override func biometricCompletion(action: String, outcome: String, errorCode: String) {
        biometricCompletionCalled = true
        biometricAction = action
        biometricOutcome = outcome
        biometricErrorCode = errorCode
    }
    
    override func biometricLoginFailure() {
        biometricLoginFailureCalled = true
    }
}
