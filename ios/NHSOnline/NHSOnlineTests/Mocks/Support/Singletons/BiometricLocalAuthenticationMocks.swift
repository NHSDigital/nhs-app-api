import Foundation
import LocalAuthentication
@testable import NHSOnline

class BiometricLocalAuthenticationMocks: BiometricLocalAuthentication {
    
    override static let biometricLAContext: LAContext = MockLocalAuthentication()

}
