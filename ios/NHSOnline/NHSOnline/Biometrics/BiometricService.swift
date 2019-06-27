import LocalAuthentication
import WebKit
import os.log

protocol BiometricProtocol {
    @available(iOS 10.0, *)
    func register()
    @available(iOS 10.0, *)
    func authenticate()
    @available(iOS 10.0, *)
    func deRegister()
}

class BiometricService: BiometricProtocol {
    let homeViewController: HomeViewController
    let biometricViewController: BiometricsViewController
    
    init(homeViewController controller: HomeViewController, biometricViewController biometricController: BiometricsViewController) {
        self.homeViewController = controller
        self.biometricViewController = biometricController
    }
    
    @available(iOS 10.0, *)
    func register() {
        do {
            let success = try FidoClient().register()
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
            let base64Response = try FidoClient().completeAuthorisationRequestAndRetrieveBase64Response()
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
    func deRegister() {
        do {
            try FidoClient().doDeregistration()
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
