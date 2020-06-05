import Foundation
@testable import NHSOnline

class AppWebInterfaceMocks: AppWebInterface {
    var biometricCompletionCalled = false
    var biometricLoginFailureCalled = false
    var biometricSpecRequestCalled = false
    var biometricAction: String = ""
    var biometricOutcome: String = ""
    var biometricErrorCode: String = ""
    var biometricTypeRef: String = ""
    var biometricEnabled: Bool = false

    override func biometricCompletion(action: String, outcome: String, errorCode: String) {
        biometricCompletionCalled = true
        biometricAction = action
        biometricOutcome = outcome
        biometricErrorCode = errorCode
    }
    
    override func biometricLoginFailure() {
        biometricLoginFailureCalled = true
    }
    
    override func biometricSpec(biometricTypeReference: String, enabled: Bool) {
        biometricSpecRequestCalled = true
        biometricTypeRef = biometricTypeReference
        biometricEnabled = enabled
    }
}
