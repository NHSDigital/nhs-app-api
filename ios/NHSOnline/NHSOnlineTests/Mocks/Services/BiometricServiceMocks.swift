import Foundation
@testable import NHSOnline

class BiometricServiceMocks: BiometricService {
    var calledRegister = false
    var calledDeRegister = false
    var biometricsAvailable = true
    
    open func updateBiometricAvailability(available: Bool) {
        biometricsAvailable = available
    }
    
    override func register() {
        calledRegister = true
    }
    
    override func deRegister(deregisterFidoCredentials: Bool) {
        calledDeRegister = true
    }
    
    override func checkBiometricCapability() -> Bool {
        return biometricsAvailable
    }
}
