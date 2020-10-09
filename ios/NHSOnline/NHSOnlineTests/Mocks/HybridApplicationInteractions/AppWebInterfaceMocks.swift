import Foundation
@testable import NHSOnline

class AppWebInterfaceMocks: AppWebInterface {
    var biometricCompletionCalled = false
    var biometricLoginFailureCalled = false
    var biometricSpecRequestCalled = false
    var biometricAction = ""
    var biometricOutcome = ""
    var biometricErrorCode = ""
    var biometricTypeRef = ""
    var biometricEnabled = false
    var paycassoCallbackCalled = false
    var paycassoFaceMatched = false
    var paycassoErrorcode = 0
    var paycassoErrorMessage = ""
    var paycassoTransactionId = ""
    var paycassoTransactionType = ""
    var notificationCookieFound = false

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
    
    override func paycassoResponseFailureCallback(isFaceMatched: Bool, errorCode: Int, errorMessage: String) {
        paycassoCallbackCalled = true
        paycassoFaceMatched = isFaceMatched
        paycassoErrorcode = errorCode
        paycassoErrorMessage = errorMessage
    }
    
    override func paycassoCustomFailureCallback(isFaceMatched: Bool, errorMessage: String) {
        paycassoCallbackCalled = true
        paycassoFaceMatched = isFaceMatched
        paycassoErrorMessage = errorMessage
    }
    
    override func paycassoSuccessCallback(isFaceMatched: Bool, transactionId: String, transactionType: String) {
        paycassoCallbackCalled = true
        paycassoFaceMatched = isFaceMatched
        paycassoTransactionId = transactionId
        paycassoTransactionType = transactionType
    }
    
    override func checkNotificationCookieExists(exists: Bool) {
        notificationCookieFound = exists
    }
}
