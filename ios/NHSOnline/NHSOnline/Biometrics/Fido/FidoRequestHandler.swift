import Foundation
import os.log

@available(iOS 10.0, *)
class FidoRequestHandler {
    let endpointHelper = FidoEndpointHelper()
    let keyManager: FidoKeyManager = FidoKeyManager()

    func getAuthenticationResponse(with authenticationRequest: FidoRequest) throws -> FidoResponse {
        var authenticationResponse = FidoResponse()
        var privateKey: SecKey
        do {
            try privateKey = keyManager.getPrivateKey()
            let builder: AuthenticationAssertionBuilder = AuthenticationAssertionBuilder(privateKey: privateKey)
            try authenticationResponse = processRequest(request: authenticationRequest, builder: builder)
            
            return authenticationResponse
        } catch let error as FidoError {
            throw error
        }
    }
    
    func getAuthRequest() throws -> JSON {
        do {
            return try FidoURLSessionManager.doRequest(with: generateAuthenticationRequest())
        } catch FidoError.networkRequestError {
            throw FidoError.networkRequestError
        } catch FidoError.parsingError {
            throw FidoError.parsingError
        } catch {
            throw FidoError.genericError
        }
    }
    
    func getRegistrationResponse(with registrationRequest: RegistrationRequest) throws -> FidoResponse {
        var registrationResponse = FidoResponse()
        var keyPair: KeyPair
        do {
            try keyPair = keyManager.generateKeyPair()
            let registrationAssertionBuilder: RegistrationAssertionBuilder = RegistrationAssertionBuilder(keyPair: keyPair)
            try registrationResponse = processRequest(request: registrationRequest, builder: registrationAssertionBuilder)
            return registrationResponse
        } catch let error as FidoError{
            throw error
        }
    }
    
    func getRegistrationRequestWith(accessToken: String) throws -> JSON {
        do {
            let request: URLRequest = try generateRegistrationRequest(accessToken: accessToken)
            let response = try FidoURLSessionManager.doRequest(with: request)
            
            return response
        } catch FidoError.parsingError {
            throw FidoError.parsingError
        }
    }
    
    func generateAuthenticationRequest() throws -> URLRequest {
        let authenticationUrl: String = endpointHelper.authenticationRequestEndpoint
        do {
            let request = try generateRequest(authenticationUrl)
            return request
        } catch FidoError.parsingError {
            throw FidoError.parsingError
        }
    }
    
    func generateRegistrationRequest(accessToken: String) throws -> URLRequest {
        let registrationUrl: String = endpointHelper.requestRequestEndpoint
        do {
            var request = try generateRequest(registrationUrl)
            request.setValue(accessToken, forHTTPHeaderField: "Authorization")
            return request
        } catch FidoError.parsingError {
            throw FidoError.parsingError
        }
    }
    
    func generateRequest(_ url: String) throws -> URLRequest {
        if var components = URLComponents(string: url) {
            let parameters: URLQueryItem = URLQueryItem(name: "http.protocol.handle-redirects", value: "false")
            components.queryItems?.append(parameters)
            components.percentEncodedQuery = components.percentEncodedQuery?.replacingOccurrences(of: "+", with: "%2B")
            var request = URLRequest(url: components.url!, cachePolicy: NSURLRequest.CachePolicy.reloadIgnoringLocalCacheData)
            request.httpMethod = "GET"
            
            return request
        }
        throw FidoError.parsingError
    }
    
    func generateDeRegisterRequest(keyId: String, facetId: String) -> DeregistrationRequest {
        keyManager.deletePrivateKey()
        let upv = Version(major: 1, minor: 0)
        let header = OperationHeader(upv,
                                     Operation.Dereg,
                                     facetId)
        
        let deregisterAuthenticator = DeregisterAuthenticator(aaid: config().AAID, keyID: keyId)
        let deregistrationRequest = DeregistrationRequest(header: header, authenticators: [deregisterAuthenticator])
        
        return deregistrationRequest
    }
    
    func clientSendRegistrationResponse(_ uafMessage: String) throws -> JSON {
        do {
            return try sendResponse(uafMessage: uafMessage, endpoint: endpointHelper.registrationResponseEndpoint)
        } catch let error as FidoError {
            throw error
        }
    }
    
    func clientSendDeRegistrationRequest(_ uafMessage: String) throws {
        do {
            let fidoResponse = try sendResponse(uafMessage: uafMessage, endpoint: endpointHelper.deregistrationRequestEndpoint)
            NSLog("Deregistration response: \(fidoResponse)")
        } catch FidoError.parsingError {
            throw FidoError.parsingError
        }
    }
    
    private func sendResponse(uafMessage: String, endpoint: String) throws -> JSON {
        do {
            let request: URLRequest = try generateRequest(uafMessage: uafMessage, endpoint: endpoint)
            
            return try FidoURLSessionManager.doRequest(with: request)
        } catch FidoError.parsingError {
            throw FidoError.parsingError
        }
    }
    
    private func generateRequest(uafMessage: String, endpoint: String) throws -> URLRequest {
        if let url = URL(string: endpoint) {
            var request: URLRequest = URLRequest(url: url, cachePolicy: NSURLRequest.CachePolicy.reloadIgnoringLocalCacheData)
            request.httpMethod = "POST"
            request.setValue("application/json", forHTTPHeaderField: "Content-Type")
            request.setValue("application/json", forHTTPHeaderField: "Accept")
            
            if let data = uafMessage.data(using: (.utf8)){
                request.httpBody = data
            }
            
            return request
        }
        throw FidoError.parsingError
    }
    
    func processRequest(request: FidoRequest, builder: FidoAssertionBuilder) throws -> FidoResponse {
        var response = FidoResponse()
        
        do {
            try response.header = OperationHeader(request: request)
            var finalChallengeParams = FinalChallengeParams()
            finalChallengeParams.appID = request.header.appID
            UserDefaultsManager.setAppID(finalChallengeParams.appID)
            finalChallengeParams.facetID = ""
            finalChallengeParams.challenge = request.challenge
            try response.assertions = setAssertions(response: response, builder: builder)
            let base64Params = try JSONEncoder().encode(finalChallengeParams).base64EncodedDataRFC4648()
            response.fcParams = base64Params
        } catch let error as FidoError {
            throw error
        }
        
        return response
    }

    private func setAssertions(response: FidoResponse, builder: FidoAssertionBuilder) throws -> [Assertion] {
        var assertion = Assertion()
        assertion.assertionScheme = config().BiometricsAssertionScheme
        do {
            try assertion.assertion = builder.getAssertions(response: response)
        } catch let error as FidoError {
            throw error
        }
        
        return [assertion]
    }
}
