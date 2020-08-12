import LocalAuthentication
import WebKit
import os.log
import FidoClientIOS

protocol BiometricProtocol {
    @available(iOS 11.0, *)
    func checkBiometricCapability() -> Bool
    @available(iOS 11.0, *)
    func register()
    @available(iOS 11.0, *)
    func authenticate()
    @available(iOS 11.0, *)
    func deRegister(deregisterFidoCredentials: Bool)
}

class BiometricService: BiometricProtocol {
    let homeViewController: HomeViewController
    let configurationServiceProvider: ConfigurationServiceProtocol
    let endpointHelper: FidoEndpointHelper
    let privateKeyLabel: String = config().PrivateKeyLabel
    let BiometricsAssertionScheme: String = config().BiometricsAssertionScheme
    let aaid: String = config().AAID
    let keyIDPrefix = "nhs-app-key-"
    let appWebInterface: AppWebInterface
    let fidoClient: FidoClientProtocol?
    let laContext: LAContext

    init(homeViewController controller: HomeViewController,
         configurationServiceProvider: ConfigurationServiceProtocol,
         appWebInterface: AppWebInterface,
         fidoClient: FidoClientProtocol?,
         laContext: LAContext) {
        self.homeViewController = controller
        self.configurationServiceProvider = configurationServiceProvider
        self.appWebInterface = appWebInterface
        self.fidoClient = fidoClient
        self.laContext = laContext
        
        laContext.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: nil)

        endpointHelper = FidoEndpointHelper(
                configurationServiceProvider: self.configurationServiceProvider,
                config: config())
    }
    
    @available(iOS 10.0, *)
    func checkBiometricCapability() -> Bool {
        var authError: NSError?
        if !laContext.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: &authError) {
            switch authError?.code {
            case Int(kLAErrorBiometryNotAvailable):
                Logger.logError(message: "touch/face id is not available")
            case Int(kLAErrorBiometryNotEnrolled):
                Logger.logError(message: "touch/face id is not enrolled")
            case Int(kLAErrorBiometryLockout):
                Logger.logError(message: "touch/face id has been locked and requires a passcode entered")
            case .none:
                Logger.logError(message: "Local auth failed with error type .none ")
            case .some(_):
                Logger.logError(message: "Local auth failed with error type .some")
            }
            
            appWebInterface.biometricCompletion(
                action: BiometricActions.register.rawValue,
                outcome: BiometricOutcomes.failed.rawValue,
                errorCode: BiometricErrorCodes.cannotFindBiometrics.rawValue)
            
            return false
        }
        
        return true
    }
    
    @available(iOS 10.0, *)
    func register() {
        do {
            try endpointHelper.configure()
            let registrationUrl: String = endpointHelper.requestRequestEndpoint!
            let accessToken: String = try UserDefaultsManager.getAccessToken()
            let registrationResponseEndpoint: String = endpointHelper.registrationResponseEndpoint!
            let success = try fidoClient!.register(aaid: aaid, BiometricsAssertionScheme: BiometricsAssertionScheme, accessToken: accessToken, registrationUrl: registrationUrl, privateKeyLabel: privateKeyLabel, registrationResponseEndpoint: registrationResponseEndpoint, keyIDPrefix: keyIDPrefix)
            
            if !success {
                appWebInterface.biometricCompletion(
                    action: BiometricActions.register.rawValue,
                    outcome: BiometricOutcomes.failed.rawValue,
                    errorCode: BiometricErrorCodes.cannotChangeBiometrics.rawValue)
                return
            }
            
            CookieHandler().setCookie(key: "HideBiometricBanner", value: "true" as AnyObject)
            storeBiometricState()
            self.appWebInterface.biometricCompletion(
                action: BiometricActions.register.rawValue,
                outcome: BiometricOutcomes.success.rawValue,
                errorCode: "")
            return
            
        } catch let error as FidoError {
            let biometricError = BiometricErrors(
                errorType: error,
                action: BiometricActions.register.rawValue,
                appWebInterface: appWebInterface)
            biometricError.updateWeb(biometricType: getBiometricType())
        } catch {
            Logger.logError(message: "An unknown error occurred")
        }
    }
    
    @available(iOS 10.0, *)
    func authenticate() {
        do {
            homeViewController.showWebViewContainer()

            try endpointHelper.configure()
            let authenticationUrl: String = endpointHelper.authenticationRequestEndpoint!
            
            let base64Response = try fidoClient!.completeAuthorisationRequestAndRetrieveBase64Response(aaid: aaid, BiometricsAssertionScheme: BiometricsAssertionScheme, privateKeyLabel: privateKeyLabel, authenticationUrl: authenticationUrl)
            
            let currenturl = homeViewController.webViewController?.webView.url
            var urlComponents = URLComponents(url: currenturl!, resolvingAgainstBaseURL: false)
            
            var queryItems = urlComponents?.queryItems ?? []
            queryItems.append(URLQueryItem(name: config().BiometricAuthResponseParam,value: base64Response))
            urlComponents?.queryItems = queryItems
            
            homeViewController.webViewController?.loadPage(url: (urlComponents?.url)!)
            homeViewController.clearSelectedTab()
        } catch let error as FidoError {
            let biometricError = BiometricErrors(
                errorType: error,
                action: BiometricActions.authentication.rawValue,
                appWebInterface: appWebInterface)
            biometricError.updateWeb(biometricType: getBiometricType())
        } catch {
            Logger.logError(message: "An unknown error occurred")
        }
    }
    
    @available(iOS 10.0, *)
    func deRegister(deregisterFidoCredentials: Bool) {
        do {
            try endpointHelper.configure()
            let deregistrationRequestEndpoint: String = endpointHelper.deregistrationRequestEndpoint!

            UserDefaultsManager.setBiometricState(nil)
            if deregisterFidoCredentials {
                let accessToken: String = try UserDefaultsManager.getAccessToken()
                try fidoClient!.doDeregistration(aaid: aaid, privateKeyLabel: privateKeyLabel, deregistrationRequestEndpoint: deregistrationRequestEndpoint, authToken: accessToken)
            }
            
            self.appWebInterface.biometricCompletion(
                action: BiometricActions.deregister.rawValue,
                outcome: BiometricOutcomes.success.rawValue,
                errorCode: "")
        } catch let error as FidoError {
            let biometricError = BiometricErrors(
                errorType: error,
                action: BiometricActions.deregister.rawValue,
                appWebInterface: appWebInterface)
            biometricError.updateWeb(biometricType: getBiometricType())
        } catch {
            Logger.logError(message: "An unknown error occurred")
        }
    }
    
    func storeBiometricState(){
        laContext.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: nil)
        if let domainState = laContext.evaluatedPolicyDomainState {
            UserDefaultsManager.setBiometricState(domainState)
        }
    }
    
    func getBiometricType() -> BiometricType {
        
        if #available(iOS 11.0, *) {
            if laContext.biometryType == .faceID {
                return BiometricType.faceID
            }
        }
        
        return BiometricType.touchID
    }
    
    open func sendBiometricSpec(enabled: Bool) {
        if #available(iOS 11.0, *) {
            if (laContext.biometryType == .faceID) {
                self.appWebInterface.biometricSpec(biometricTypeReference: BiometricTypeWebReferences.faceId.rawValue, enabled: enabled)
            } else if (laContext.biometryType == .touchID) {
                self.appWebInterface.biometricSpec(biometricTypeReference: BiometricTypeWebReferences.touchId.rawValue, enabled: enabled)
            } else {
                self.appWebInterface.biometricSpec(biometricTypeReference: BiometricTypeWebReferences.touchId.rawValue, enabled: enabled)
            }
    }
}
}
