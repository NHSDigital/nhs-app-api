import LocalAuthentication
import WebKit
import os.log
import FidoClientIOS

protocol BiometricProtocol {
    @available(iOS 10.0, *)
    func register()
    @available(iOS 10.0, *)
    func authenticate()
    @available(iOS 10.0, *)
    func deRegister(deregisterFidoCredentials: Bool)
}

class BiometricService: BiometricProtocol {
    let homeViewController: HomeViewController
    let biometricViewController: BiometricsViewController
    let configurationServiceProvider: ConfigurationServiceProtocol
    let endpointHelper: FidoEndpointHelper
    let privateKeyLabel: String = config().PrivateKeyLabel
    let BiometricsAssertionScheme: String = config().BiometricsAssertionScheme
    let aaid: String = config().AAID
    let keyIDPrefix = "nhs-app-key-"

    init(homeViewController controller: HomeViewController, biometricViewController biometricController: BiometricsViewController,
         configurationServiceProvider: ConfigurationServiceProtocol) {
        self.homeViewController = controller
        self.biometricViewController = biometricController
        self.configurationServiceProvider = configurationServiceProvider

        endpointHelper = FidoEndpointHelper(
                configurationServiceProvider: self.configurationServiceProvider,
                config: config())
    }
    
    @available(iOS 10.0, *)
    func register() {
        do {
            try endpointHelper.configure()
            let registrationUrl: String = endpointHelper.requestRequestEndpoint!
            let accessToken: String = try UserDefaultsManager.getAccessToken()
            let registrationResponseEndpoint: String = endpointHelper.registrationResponseEndpoint!
            let success = try FidoClient().register(aaid: aaid, BiometricsAssertionScheme: BiometricsAssertionScheme, accessToken: accessToken, registrationUrl: registrationUrl, privateKeyLabel: privateKeyLabel, registrationResponseEndpoint: registrationResponseEndpoint, keyIDPrefix: keyIDPrefix)
            if success {
                return  biometricViewController.goToResultsPage()
            }
        } catch let error as FidoError {
            handleError(error)
            if error == FidoError.invalidBiometrics {
               return biometricViewController.biometricToggle.isUserInteractionEnabled = true
            }
        } catch {
            Logger.logError(message: "An unknown error occurred")
        }
        biometricViewController.showBiometricRegistrationError()
    }
    
    @available(iOS 10.0, *)
    func authenticate() {
        do {
            homeViewController.showWebViewContainer()

            try endpointHelper.configure()
            let authenticationUrl: String = endpointHelper.authenticationRequestEndpoint!
            
            let base64Response = try FidoClient().completeAuthorisationRequestAndRetrieveBase64Response(aaid: aaid, BiometricsAssertionScheme: BiometricsAssertionScheme, privateKeyLabel: privateKeyLabel, authenticationUrl: authenticationUrl)
            
            let currenturl = homeViewController.webViewController?.webView.url
            var urlComponents = URLComponents(url: currenturl!, resolvingAgainstBaseURL: false)
            
            var queryItems = urlComponents?.queryItems ?? []
            queryItems.append(URLQueryItem(name: config().BiometricAuthResponseParam,value: base64Response))
            urlComponents?.queryItems = queryItems
            
            homeViewController.webViewController?.loadPage(url: (urlComponents?.url)!)
            homeViewController.clearSelectedTab()
        } catch let error as FidoError {
            handleError(error)
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
                try FidoClient().doDeregistration(aaid: aaid, privateKeyLabel: privateKeyLabel, deregistrationRequestEndpoint: deregistrationRequestEndpoint, authToken: accessToken)
            }
        } catch let error as FidoError {
            handleError(error)
        } catch {
            Logger.logError(message: "An unknown error occurred")
        }
    }
    
    @available(iOS 10.0, *)
    func handleError(_ error: FidoError){
        switch error {
        case .invalidBiometrics:
            Logger.logError(message: "Invalid Biometrics used")
        case .keyRetrievalError:
            Logger.logError(message: "An error occurred during key retrieval")
        case .parsingError:
            Logger.logError(message: "A parsing error occurred")
        case .encryptionError:
            Logger.logError(message: "An encryption error occurred")
        case .networkRequestError:
            Logger.logError(message: "A network error occurred")
        case .accessTokenError:
            Logger.logError(message: "An access token error occurred")
        case .genericError:
            Logger.logError(message: "A generic error occurred")
        }
    }
}
