import Foundation
import FidoClientIOS
@testable import NHSOnline

class FidoClientMocks: FidoClientProtocol {
    var shouldThrowFidoError = false
    var userCancelled = false
    var fidoTokenErrorThrown = false
    
    
    func updateFidoTokenErrorStatus(showThrow: Bool) {
        self.fidoTokenErrorThrown = showThrow
    }
    func startRegistration(aaid: String, BiometricsAssertionScheme: String, accessToken: String, registrationUrl: String, privateKeyLabel: String, keyIDPrefix: String) throws -> String {
        return ""
    }
    
    func completeRegistration(_ encodedResponse: String, registrationResponseEndpoint: String) throws -> Bool {
        return true
    }
    
    func startAuthorisation(aaid: String, BiometricsAssertionScheme: String, privateKeyLabel: String, authenticationUrl: String) throws -> String {
        return ""
    }
    
    func completeAuthorisation(aaid: String, BiometricsAssertionScheme: String, privateKeyLabel: String, authenticationUrl: String, completion: @escaping (FidoResponse) throws -> ()) throws {
        do {
            let registrationResponse = try FidoResponse(from: Decoder.self as! Decoder)
            try completion(registrationResponse)
        } catch let error as FidoError {
            throw error
        }
    }
    
    func doDeregistration(aaid: String, privateKeyLabel: String, deregistrationRequestEndpoint: String, authToken: String) throws {
        if (shouldThrowFidoError) {
        throw FidoError.invalidBiometrics
        }
    }
    
    func completeAuthorisationRequestAndRetrieveBase64Response(aaid: String, BiometricsAssertionScheme: String, privateKeyLabel: String, authenticationUrl: String) throws -> String {
        if (fidoTokenErrorThrown) {
            throw FidoError.accessTokenError
        } else if (userCancelled) {
            throw FidoError.userCancelled
        }
        
        return ""
    }
    
    
    func register(aaid: String, BiometricsAssertionScheme: String, accessToken: String, registrationUrl: String, privateKeyLabel: String, registrationResponseEndpoint: String, keyIDPrefix: String) throws -> Bool {
        if (!shouldThrowFidoError && !userCancelled) {
        return true
        } else if (shouldThrowFidoError) {
            throw FidoError.invalidBiometrics
        } else {
            throw FidoError.userCancelled
        }
    }

}
