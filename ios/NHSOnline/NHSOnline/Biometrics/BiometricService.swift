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
    let FidoServerUrl: String = ConfigurationService.shared().FidoServerUrl()
    lazy var endpointHelper = FidoEndpointHelper(
        FidoServerUrl: FidoServerUrl,
        BiometricsRegistrationRequestEndpoint: config().BiometricsRegistrationRequestEndpoint,
        BiometricsRegistrationResponseEndpoint: config().BiometricsRegistrationResponseEndpoint,
        BiometricsAuthenticationRequestEndpoint: config().BiometricsAuthenticationRequestEndpoint,
        BiometricsDeregistrationRequestEndpoint: config().BiometricsDeregistrationRequestEndpoint)
    let privateKeyLabel: String = config().PrivateKeyLabel
    let BiometricsAssertionScheme: String = config().BiometricsAssertionScheme
    let aaid: String = config().AAID
    let keyIDPrefix = "nhs-app-key-"

    init(homeViewController controller: HomeViewController, biometricViewController biometricController: BiometricsViewController) {
        self.homeViewController = controller
        self.biometricViewController = biometricController
    }
    
    @available(iOS 10.0, *)
    func register() {
        do {
            let registrationUrl: String = endpointHelper.requestRequestEndpoint
            let accessToken: String = try UserDefaultsManager.getAccessToken()
            let registrationResponseEndpoint: String = endpointHelper.registrationResponseEndpoint
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
            os_log("An unknown error occured", log: OSLog.default, type: OSLogType.error)
        }
         biometricViewController.showBiometricRegistrationError()
    }
    
    @available(iOS 10.0, *)
    func authenticate() {
        do {
            homeViewController.showWebViewContainer()
            let authenticationUrl: String = endpointHelper.authenticationRequestEndpoint
            
            let base64Response = try FidoClient().completeAuthorisationRequestAndRetrieveBase64Response(aaid: aaid, BiometricsAssertionScheme: BiometricsAssertionScheme, privateKeyLabel: privateKeyLabel, authenticationUrl: authenticationUrl)
            let url = config().BiometricRedirectURL + base64Response
            let validLoginUrl = config().HomeUrl + url
            homeViewController.webViewController?.loadPage(url: validLoginUrl)
            homeViewController.tabBar.selectedItem = nil
            homeViewController.selectedTab = nil
        } catch let error as FidoError {
            handleError(error)
        } catch {
            os_log("An unknown error occured", log: OSLog.default, type: OSLogType.error)
        }
    }
    
    @available(iOS 10.0, *)
    func deRegister(deregisterFidoCredentials: Bool) {
        do {
            let deregistrationRequestEndpoint: String = endpointHelper.deregistrationRequestEndpoint
            UserDefaultsManager.setBiometricState(nil)
            if deregisterFidoCredentials {
                let accessToken: String = try UserDefaultsManager.getAccessToken()
                try FidoClient().doDeregistration(aaid: aaid, privateKeyLabel: privateKeyLabel, deregistrationRequestEndpoint: deregistrationRequestEndpoint, authToken: accessToken)
            }
        } catch let error as FidoError {
            handleError(error)
        } catch {
            os_log("An unknown error occured", log: OSLog.default, type: OSLogType.error)
        }
    }
    
    @available(iOS 10.0, *)
    func handleError(_ error: FidoError){
        switch error {
        case .invalidBiometrics:
            os_log("Invalid Biometrics used", log: OSLog.default, type: OSLogType.error)
        case .keyRetrievalError:
            os_log("An error occured during key retrieval", log: OSLog.default, type: OSLogType.error)
        case .parsingError:
            os_log("A parsing error occured", log: OSLog.default, type: OSLogType.error)
        case .encryptionError:
            os_log("An encryption error occured", log: OSLog.default, type: OSLogType.error)
        case .networkRequestError:
            os_log("A network error occured", log: OSLog.default, type: OSLogType.error)
        case .accessTokenError:
            os_log("An access token error occured", log: OSLog.default, type: OSLogType.error)
        case .genericError:
            os_log("A generic error occured", log: OSLog.default, type: OSLogType.error)
        }
    }
}
