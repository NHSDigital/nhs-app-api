import Foundation
import LocalAuthentication
import FidoClientIOS

class UserDefaultsManager {
    static let userDefaults: UserDefaults = UserDefaults.standard
    static let BiometricsKey: String = "BiometricsEnabled"
    static let DomainStateKey: String = "DomainState"
    static let KeyIDKey: String = "keyID"
    static let accessTokenKey: String = "accessToken"
    static let appIDKey: String = "appID"

    static func setBiometricState(_ domainState: Data?){
        userDefaults.set(domainState, forKey: DomainStateKey)
    }
    
    static func getBiometricState() -> Data?{
        return UserDefaults.standard.data(forKey: DomainStateKey)
    }
    
    static func setAccessToken(_ accessToken: String?) {
        userDefaults.set(accessToken, forKey: accessTokenKey)
    }
    
    static func getAccessToken() throws -> String {
        if let accessToken = UserDefaults.standard.string(forKey: accessTokenKey) {
            return accessToken
        }
        throw FidoError.accessTokenError
        
    }
    
    @available(iOS 11.0, *)
    static func getBiometricType() -> BiometricType {
        let context = LAContext()
        context.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: nil)
        
        if context.biometryType == .faceID {
            return BiometricType.faceID
        } else {
            return BiometricType.touchID
        }
    }
    
    static func getBiometricAvailability() -> BiometricState {
        let context = LAContext()
        context.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: nil)
        
        let newState = context.evaluatedPolicyDomainState
        let oldState = getBiometricState()
        
        if(oldState == nil){
            return BiometricState.Not_Registered
        }
        
        if(oldState == newState){
            return BiometricState.Registered
        }
        
        return BiometricState.Invalidated
        
    }
}
