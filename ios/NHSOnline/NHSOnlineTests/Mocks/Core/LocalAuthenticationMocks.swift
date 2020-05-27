import Foundation
import LocalAuthentication

class LocalAuthenticationMocks: LAContext {
    var biometricsCapable = false
    var shouldReturnLockoutError = false
    var shouldReturnNotAvailableError = false
    var shouldReturnNotEnrolled = false
    var error: NSError?
    var shouldUseFaceId = true
    
    func updateBiometricsCapability(capable: Bool) {
        biometricsCapable = capable
    }
    
    func updateShouldUseFaceId(shouldUse: Bool) {
        shouldUseFaceId = shouldUse
    }
    
    func updateShouldReturnLockoutError(shouldThrow: Bool) {
        shouldReturnLockoutError = shouldThrow
    }
    
    func updateShouldReturnNotAvailableError(shouldThrow: Bool) {
        shouldReturnNotAvailableError = shouldThrow
    }
    
    func updateShouldReturnNotEnrolled(shouldThrow: Bool) {
        shouldReturnNotEnrolled = shouldThrow
    }
    
    func errorExpected() -> Bool {
        return shouldReturnLockoutError
        || shouldReturnNotAvailableError
        || shouldReturnNotEnrolled
    }
    
    func getErrorCode() -> Int {
        if (shouldReturnLockoutError) {
            return -8
        } else if (shouldReturnNotAvailableError) {
            return -6
        } else if (shouldReturnNotEnrolled) {
            return -7
        }
        
        return 0
    }
    
    override open func canEvaluatePolicy(_ policy: LAPolicy, error: NSErrorPointer) -> Bool {
        if (errorExpected()){
            let expectedError = NSError(domain: "", code: getErrorCode(), userInfo: nil)
            error?.pointee = expectedError
            return false
        }
        return biometricsCapable
    }
    
    @available(iOS 11.0, *)
    override var biometryType : LABiometryType {
        get {
            if (shouldUseFaceId) {
                return LABiometryType.faceID
            } else {
                return LABiometryType.touchID
            }
        }
    }
}
