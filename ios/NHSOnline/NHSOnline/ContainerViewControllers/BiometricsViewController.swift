import UIKit
import WebKit
import LocalAuthentication
import os.log

class BiometricsViewController: UIViewController {
    
    @IBOutlet weak var HeaderTextView: UITextView!
    @IBOutlet weak var ContentTextView: ErrorTextView!
    @IBOutlet weak var TitleLabel: UILabel!
    @IBOutlet weak var ContentLabel: UILabel!
    @IBOutlet weak var biometricToggle: UISwitch!
    @IBOutlet weak var BiometricLabel: UILabel!
    
    var webViewController: WebViewController?
    var homeViewController: HomeViewController?
    let laContext: LAContext = LAContext()
    var biometricService: BiometricService?
    let ExpTime = TimeInterval(60 * 60 * 24 * 365 * 5)
    
    @IBAction func biometricToggleChanged(_ sender: UISwitch) {
        setCookie(key: "HideBiometricBanner", value: "true" as AnyObject)

        if(sender.isOn) {
            DispatchQueue.main.async {
                sender.setOn(false, animated: false)
            }
            return attemptRegistration()
        } else if(!sender.isOn) {
            attemptDeregistration()
        }
    }
    
    override func viewDidAppear(_ animated: Bool) {
        setupViewController()
    }
    
    func setupViewController() {
        let stringsObject = BiometricStringHandler().getControllerStrings()
        HeaderTextView.text = stringsObject.HeaderTextViewText
        ContentTextView.text = stringsObject.ContentTextViewText
        TitleLabel.text = stringsObject.TitleLabelText
        ContentLabel.text = stringsObject.ContentLabelText
        BiometricLabel.text = stringsObject.BiometricLabelText
        ContentTextView.layer.addTopBorder(color: UIColor(red: 255, green: 237, blue: 0, alpha: 1), thickness: 3)
        ContentTextView.resizeErrorTextView()
        biometricToggle.isUserInteractionEnabled = true
        biometricToggle.setOn(UserDefaultsManager.getBiometricAvailability() == BiometricState.Registered, animated: false)
    }

    func attemptRegistration() {
        if #available(iOS 10.0, *) {
            var authError: NSError?
            if laContext.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: &authError) {
                biometricToggle.isUserInteractionEnabled = false
                biometricService?.register()
                return
            } else if authError?.code == LAError.touchIDNotAvailable.rawValue  { //same raw value as LAError.BiometryNotAvailable
                showAlert(alertType: .NotSupported)
                return
            }  else if authError?.code == LAError.touchIDNotEnrolled.rawValue { //same raw value as LAError.BiometryNotEnrolled
                showAlert(alertType: .NoBiometrics)
                return
            } else if authError?.code == LAError.touchIDLockout.rawValue {
                showAlert(alertType: .BiometricLockout)
                return
            }
        } else {
            showAlert(alertType: .OSNotSupported)
            return
        }
    }
    
    func attemptDeregistration() {
         if #available(iOS 10.0, *) {
            biometricService?.deRegister()
            homeViewController?.showBiometricResultsContainer(registration: false)
        }
    }
    
    func showAlert(alertType: BiometricAlertType) {
        let alert = BiometricStringHandler().getBiometricAlert(type: alertType)
        if(alertType == BiometricAlertType.NoBiometrics) {
            let help = UIAlertAction(title: NSLocalizedString("BiometricsDialogHelp", comment: ""), style: .default) { (action) -> Void in
                self.homeViewController?.webViewController?.webView.loadPage(url: config().BiometricHelpURL)
            }
            alert.addAction(help)
        
        }
        self.present(alert, animated: true, completion: nil)
    }
    
    func getAlert(title: String, message: String) -> UIAlertController {
        
        let alertController = UIAlertController(title: title, message: message, preferredStyle: .alert)
        let cancel = UIAlertAction(title: "Cancel", style: .cancel) { (action) -> Void in
            alertController.dismiss(animated: true, completion: nil)
        }
        alertController.addAction(cancel)
        return alertController
    }
    
    func showBiometricRegistrationError() {
        biometricToggle.isUserInteractionEnabled = true
        homeViewController?.showBiometricsRegistrationError()
    }
    
    func storeBiometricState(){
        if let domainState = laContext.evaluatedPolicyDomainState {
            UserDefaultsManager.setBiometricState(domainState)
        }
    }
    
    func goToResultsPage() {
        biometricToggle.isUserInteractionEnabled = true
        biometricToggle.setOn(true, animated: true)
        storeBiometricState()
        homeViewController?.showBiometricResultsContainer(registration: true)
    }
    
    func setCookie(key: String, value: AnyObject) {
        let domain = config().HomeHost.dropLast()
        
        let cookie = HTTPCookie(properties: [
            .domain: String(domain),
            .path: "/",
            .name: key,
            .value: value,
            .secure: "TRUE",
            .expires: NSDate(timeIntervalSinceNow: 31556926)
            ])!

        if #available(iOS 11.0, *) {
            setWKCookie(cookie)
        }
        else {
            setHTTPCookie(cookie)
        }
    }
    @available(iOS 11.0, *)
    func setWKCookie(_ cookie: HTTPCookie) {
        let cookieStore = WKWebsiteDataStore.default().httpCookieStore
        cookieStore.setCookie(cookie) { }
    }
    
    func setHTTPCookie(_ cookie: HTTPCookie) {
        HTTPCookieStorage.shared.cookieAcceptPolicy = HTTPCookie.AcceptPolicy.always
        HTTPCookieStorage.shared.setCookie(cookie)
    }
}
