import Foundation
import os.log

protocol FidoClientProtocol {
    func startRegistration() throws -> String
    func completeRegistration(_ encodedResponse: String) throws -> Bool
    func startAuthorisation() throws -> String
    func completeAuthorisation(completion: @escaping (_: FidoResponse ) throws -> ()) throws
    func doDeregistration() throws
}

@available(iOS 10.0, *)
class FidoClient: FidoClientProtocol {
    let encodingHandler = FidoEncodingHandler()
    let requestHandler = FidoRequestHandler()
    let bundleID = "ios:bundle-id:"
    
    func register() throws -> Bool {
        do {
            let registrationResponse = try startRegistration()
            
            return try completeRegistration(registrationResponse)
        } catch let error as FidoError {
            throw error
        }
    }
 
    func completeAuthorisationRequestAndRetrieveBase64Response() throws -> String {
        do {
            let base64Response = try startAuthorisation()
            return base64Response
        } catch let error as FidoError {
            throw error
        }
    }
    
    func startRegistration() throws -> String {
        do {
            let registrationRequestData = try requestHandler.getRegistrationRequestWith(accessToken: UserDefaultsManager.getAccessToken())
            let request: RegistrationRequest = try RegistrationRequest(with: registrationRequestData[0])
            request.header.appID = try self.selectFacetIdWhenAppIDisEmpty(request: request, facetId: getFacetID())
            let encodedResponse = try encodingHandler.getEncodedRegistrationResponse(with: request)
            if !encodedResponse.isEmpty {
                return encodedResponse
            }
        throw FidoError.genericError
        } catch let error as FidoError {
            throw error
        }
    }
    
    func completeRegistration(_ encodedResponse: String) throws -> Bool {
        do {
            let fidoResponse = try requestHandler.clientSendRegistrationResponse(encodedResponse)
            if fidoResponse[0]["status"].string != nil && fidoResponse[0]["status"].string == "SUCCESS" {
                os_log("Registration Successful", log: OSLog.default, type: OSLogType.info)
                
                return true
            }
        } catch let error as FidoError {
            throw error
        }
        return false
    }
    
    func startAuthorisation() throws -> String {
        do {
            var base64Response = String()
            let semaphore = DispatchSemaphore(value: 0)
            try completeAuthorisation(completion: { authenticationResponse in
                let response: [FidoResponse] = [authenticationResponse]
                let jsonResponse = try JSONEncoder().encode(response)
                if let message = String(data: jsonResponse, encoding: .utf8) {
                    if let authenticationResponseB64 = message.data(using: .utf8)?.base64EncodedDataRFC4648() {
                       base64Response = authenticationResponseB64
                    }
                }
                semaphore.signal()
            })
            semaphore.wait()
            
            return base64Response
        } catch let error as FidoError {
            throw error
        }
    }
    
    func completeAuthorisation(completion: @escaping (_: FidoResponse ) throws -> ()) throws {
        do {
            let response = try requestHandler.getAuthRequest()
            var authResponse: FidoResponse
            let request: FidoRequest = try FidoRequest(with: response[0])
            request.header.appID = try selectFacetIdWhenAppIDisEmpty(request: request, facetId: getFacetID())
            authResponse = try requestHandler.getAuthenticationResponse(with: request)
            try completion(authResponse)
        } catch let error as FidoError {
            throw error
        }
    }
    
    func doDeregistration() throws {
        do {
            let keyId = try UserDefaultsManager.getKeyID()
            UserDefaultsManager.deleteKeyID()
            UserDefaultsManager.setBiometricState(nil)
            let deregistrationRequest = try requestHandler.generateDeRegisterRequest(keyId: keyId, facetId: getFacetID())
            let encodedRequest = try encodingHandler.encodeDeregistrationRequest(deregistrationRequest)
            if !encodedRequest.isEmpty{
                try requestHandler.clientSendDeRegistrationRequest(encodedRequest)
            }
        } catch let error as FidoError {
            throw error
        }
    }
    
    func selectFacetIdWhenAppIDisEmpty(request: FidoRequest, facetId: String) -> String {
        if (request.header.appID.isEmpty) {
            return facetId
        }
        return request.header.appID
    }
    
    func getFacetID() throws -> String {
        if let facetID = Bundle.main.bundleIdentifier {
            return bundleID + facetID
        }
        throw FidoError.genericError
    }   
}

