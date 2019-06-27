import UIKit
import os.log
import SafariServices
import WebKit

class HomeViewController : UIViewController {
    private let showConstraintPriority = UILayoutPriority.init(rawValue: 900)
    private let hideConstraintPriority = UILayoutPriority.init(rawValue: 850)
    
    let applicationState = ApplicationState()
    
    @IBOutlet weak var headerBar: HeaderBar!
    @IBOutlet weak var headerBarSlim: HeaderBarSlim!
    
    @IBOutlet weak var tabBar: UITabBar!
    @IBOutlet weak var containerView: UIView!
    
    @IBOutlet weak var webviewHeaderTopConstraint: NSLayoutConstraint!
    @IBOutlet weak var webviewNavMenuBottomConstraint: NSLayoutConstraint!
    @IBOutlet weak var webviewHeaderSlimTopConstraint: NSLayoutConstraint!
    
    let knownServices = KnownServices(config: config())
    var lifecycleHandlers: LifecycleHandlers?
    var webViewController: WebViewController?
    var errorViewController: PageUnavailabilityViewController?
    var biometricViewController: BiometricsViewController?
    var biometricResultController: BiometricsResultViewController?
    var currentNativeViewController: UIViewController?
    var webViewDelegate: WebViewDelegate?
    var tabBarDelegate: TabBarDelegate?
    var pageUrl = config().HomeUrl
    var appVersionCheckError: Bool = false
    var apiConfigCallError: Bool = false
    var appWebInterface: AppWebInterface?
    var webAppInterface: WebAppInterface?
    var extendSessionOverdue: Bool = false
    var currentSessionDuration: Int?
    var isPresented: Bool = false
    var goingBack: Bool = false
    var biometricService: BiometricService?
    var configurationService: ConfigurationService?
    public var selectedTab: Int?
    
    override func viewWillAppear(_ animated: Bool) {
        self.navigationController?.navigationBar.isHidden = true
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        isPresented = true
        
        if (extendSessionOverdue) {
            displayExtendSessionDialogue(sessionDuration: currentSessionDuration!)
        }
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        isPresented = false
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()

        setupNhsLogo()
        setupBackArrow()
        setupMyAccountIcon()
        setupHelpIcon()
        
        webAppInterface = WebAppInterface(controller: self)
        webViewDelegate = WebViewDelegate(controller: self, knownServices: knownServices, webAppInterface: webAppInterface!)
        tabBarDelegate = TabBarDelegate(controller: self)
        tabBar.delegate = tabBarDelegate
        tabBar.setDefaultTabBarItemsAppearance()
        
        setUpControllers()
        self.addChildViewController(self.webViewController!)
        self.addSubview(subView: (self.webViewController?.view)!, toView: self.containerView)
        
        lifecycleHandlers = LifecycleHandlers(knownServices: knownServices,
                                              webViewController: webViewController!,
                                              homeViewController: self)
        appWebInterface = AppWebInterface(webView: webViewController?.webView)
        
    
        guard apiConfigCallError else {
            self.webViewController?.loadPage(url: pageUrl)
            return
        }
        
    }
    
    func apiCallFailure() {
        guard isOffline() else {
            apiConfigCallError = true
            let error = ErrorMessage(.APICallFailure)
            return showNativeViewContainer(errorMessage: error)
        }
    }
    
    func delayedBiometricsStart(_ timer: Double) {
        if isOnLogin() {
            self.showWebViewContainer()
            if #available(iOS 10.0, *) {
                Timer.scheduledTimer(timeInterval: timer, target: self, selector: #selector(self.attemptBiometricLogin), userInfo: nil, repeats: false)
            }
        }
    }
    
    func isOnLogin() -> Bool {
        let loginURLString = config().HomeUrl + "login" + config().NhsOnlineRequiredQueryString
        
        return self.webViewController?.webView.url?.absoluteString == loginURLString
    }
    
    @objc @available(iOS 10.0, *)
    public func attemptBiometricLogin() {
        do {
            
            if(UserDefaultsManager.getBiometricAvailability() == BiometricState.Registered) {
                biometricService?.authenticate()
            } else if (UserDefaultsManager.getBiometricAvailability() == BiometricState.Invalidated) {
                biometricService?.deRegister()
                showBiometricsAlert(.BiometricsInvalidated)
            }
        }
    }
    
    func showBiometricsAlert(_ alertType: BiometricAlertType) {
        self.present(BiometricStringHandler().getBiometricAlert(type: alertType), animated: true, completion: nil)
    }
    
    func reloadLoginPage() {
        self.webViewController?.loadPage(url: pageUrl)
    }
    
    func setUpControllers() {
        webViewController = self.storyboard?.instantiateViewController(withIdentifier: "WebViewController") as? WebViewController
        webViewController?.loadViewIfNeeded()
        webViewController?.setWebViewDelegate(delegate: webViewDelegate!)
        webViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        
        errorViewController = self.storyboard?.instantiateViewController(withIdentifier: "PageUnavailabilityViewController") as? PageUnavailabilityViewController
        errorViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        errorViewController?.loadViewIfNeeded()
        
        biometricViewController = self.storyboard?.instantiateViewController(withIdentifier: "BiometricsViewController") as? BiometricsViewController
        biometricViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        biometricViewController?.homeViewController = self
        biometricService = BiometricService(homeViewController: self, biometricViewController: biometricViewController!)
        biometricViewController?.biometricService = biometricService
        biometricViewController?.loadViewIfNeeded()
        
        biometricResultController = self.storyboard?.instantiateViewController(withIdentifier: "BiometricsResultViewController") as? BiometricsResultViewController
        biometricResultController?.view.translatesAutoresizingMaskIntoConstraints = false
        biometricResultController?.loadViewIfNeeded()
        
        currentNativeViewController = errorViewController
    }
    
    func addSubview(subView:UIView, toView parentView:UIView) {
        parentView.addSubview(subView)
        var viewBindingsDict = [String: AnyObject]()
        viewBindingsDict["subView"] = subView
        parentView.addConstraints(NSLayoutConstraint.constraints(withVisualFormat: "H:|[subView]|",
                                                                 options: [], metrics: nil, views: viewBindingsDict))
        parentView.addConstraints(NSLayoutConstraint.constraints(withVisualFormat: "V:|[subView]|",
                                                                 options: [], metrics: nil, views: viewBindingsDict))
    }
    
    func postNdopToken(token: String?) {
        if (token != nil) {
            self.webViewController?.postNdopToken(token: token!)
        }
    }
    
    func updateHeaderText(headerText: String?, accessibilityLabel: String? = nil) {
        if (headerText != nil) {
            self.headerBar.headerTitle.text = headerText
            self.headerBar.headerTitle.accessibilityLabel = accessibilityLabel
            self.headerBarSlim.headerTitle.text = headerText
            self.headerBarSlim.headerTitle.accessibilityLabel = accessibilityLabel
        }
        if let a11yLabel = accessibilityLabel {
            UIAccessibilityPostNotification(UIAccessibilityAnnouncementNotification, a11yLabel)
        } else {
            UIAccessibilityPostNotification(UIAccessibilityAnnouncementNotification, headerText)
        }
    }
    
    func setupNhsLogo() {
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(self.goHome))
        self.headerBar.NHSHomeLogo.isUserInteractionEnabled = true
        self.headerBar.NHSHomeLogo.addGestureRecognizer(tapGesture)
    }
    
    func setupBackArrow() {
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(self.checkSymptomsAndCIDBack))
        self.headerBarSlim.backButtonArrow.isUserInteractionEnabled = true
        self.headerBarSlim.backButtonArrow.addGestureRecognizer(tapGesture)
    }
    
    func setupMyAccountIcon() {
        let tapGesture = UITapGestureRecognizer(target: self, action:  #selector(self.selectMyAccount))
        self.headerBar.myAccountIcon.isUserInteractionEnabled = true
        self.headerBar.myAccountIcon.addGestureRecognizer(tapGesture)
    }
    
    func setupHelpIcon() {
        let tapGesture = UITapGestureRecognizer(target: self, action:  #selector(self.selectHelp))
        self.headerBar.helpIcon.isUserInteractionEnabled = true
        self.headerBar.helpIcon.addGestureRecognizer(tapGesture)
    }
    
    func setupAppVersion() {
        let versionNumber = Bundle.main.object(forInfoDictionaryKey: "CFBundleShortVersionString") as! String
        let updateNativeVersionJavascriptCommand = "var result = window.$nuxt.$store.dispatch('appVersion/updateNativeVersion', '\(versionNumber)');"
        
        executeJavascript(scriptToExecute: updateNativeVersionJavascriptCommand)
    }
    
    func executeJavascript(scriptToExecute: String) {
        let completionHandler: (Any?, Error?) -> Void = {
            (data, error) in
            if(error != nil) {
                let description = error.debugDescription
                print(description)
                
                if #available(iOS 10.0, *) {
                    os_log("An error occured setting the app version number.", log: OSLog.default, type: .error)
                } else {
                    NSLog("An error occured setting the app version number.")
                }
            }
        }
        
        self.webViewController?.webView.evaluateJavaScript(scriptToExecute, completionHandler: completionHandler)
    }
    
    func setVisibilityOfHeaderAndMenuBars(visible: Bool, isSlim: Bool) {
        UIView.animate(withDuration: 0.3, animations: {
            let constraintPriority:UILayoutPriority
            
            if visible {
                constraintPriority = self.showConstraintPriority
            } else {
                constraintPriority = self.hideConstraintPriority
            }
            
            if isSlim {
                self.webviewHeaderSlimTopConstraint.priority = constraintPriority
                self.headerBarSlim.isHidden = !visible
            } else {
                self.webviewHeaderTopConstraint.priority = constraintPriority
                self.webviewNavMenuBottomConstraint.priority = constraintPriority
                self.headerBar.isHidden = !visible
                self.tabBar.isHidden = !visible
            }
        })
        
    }
    
    func getBiometricRegistrationErrorStrings() -> BiometricErrorStrings{
        return BiometricStringHandler().getErrorStrings()
    }
    
    func showWebViewContainer() {
        if (!appVersionCheckError) {
            self.cycleFromViewController(oldViewController: self.currentNativeViewController!, toViewController: self.webViewController!)
        }
    }
    
    func showNativeViewContainer() {
        if (!appVersionCheckError) {
            self.cycleFromViewController(oldViewController: self.webViewController!,
                 toViewController: self.currentNativeViewController!)
        }
    }
    
    func showBiometricsRegistrationError() {
        self.errorViewController?.setUnavailabilityError(errorMessage: ErrorMessage(.BiometricRegistrationError))
        self.updateHeaderText(headerText: getBiometricRegistrationErrorStrings().BiometricRegistrationPageHeader)
        cycleFromViewController(oldViewController: biometricViewController!, toViewController: errorViewController!)
        currentNativeViewController = errorViewController
        UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, errorViewController?.errorTextView)
    }
    
    func showNativeViewContainer(errorMessage: ErrorMessage) {
        self.errorViewController?.setUnavailabilityError(errorMessage: errorMessage)
        switch errorMessage.type {
        case .APICallFailure, .ServiceUnavailable:
            self.updateHeaderText(headerText: NSLocalizedString("ServiceUnavailableErrorMessage", comment: ""))
        default:
            self.updateHeaderText(headerText: NSLocalizedString("ConnectionErrorHeader", comment: ""))
        }
       
        showErrorViewContainer()
        applicationState.unBlock()
    }
    
    func showBiometricSessionError () {
        self.webViewDelegate?.failedUrl = URL(string: config().HomeUrl + "login")
        self.errorViewController?.setUnavailabilityError(errorMessage: ErrorMessage(.BiometricLoginSessionError))
        self.errorViewController?.hideTryAgainLabel()
        self.errorViewController?.setTryAgainButtonText(text: NSLocalizedString("BiometricSessionTimeoutButtonText", comment: ""))
        self.updateHeaderText(headerText: NSLocalizedString("BiometricSessionTimeoutHeader", comment: ""))
        showErrorViewContainer()
        self.webViewController?.dismissSafariViewController()
    }
    
    func showErrorViewContainer() {
        cycleFromViewController(oldViewController: webViewController!, toViewController: errorViewController!)
        currentNativeViewController = errorViewController
        UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, errorViewController?.errorTextView)
    }
    
    func showBiometricViewContainer() {
        guard isOffline() else {
            biometricViewController?.homeViewController = self
            cycleFromViewController(oldViewController: webViewController!, toViewController: biometricViewController!)
            currentNativeViewController = biometricViewController
            updateHeaderText(headerText: NSLocalizedString("LoginAndPasswordOptions", comment: ""))
            UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, biometricViewController?.ContentTextView)
            return
        }
    }
    
    func showBiometricResultsContainer(registration: Bool) {
        guard isOffline() else {
            if registration {
                biometricResultController?.registration = true
            } else {
                biometricResultController?.registration = false
            }
            biometricResultController?.viewController = self
            cycleFromViewController(oldViewController: biometricViewController!, toViewController: biometricResultController!)
            currentNativeViewController = biometricResultController
            UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, biometricResultController?.BoxText)
            return
        }
    }
    
    func resetFocusAndAnnouncePageTitle(pageTitle: String?) {
        self.headerBar.setFocusToNhsLogoForA11y()
        if let title = pageTitle {
            UIAccessibilityPostNotification(UIAccessibilityAnnouncementNotification, title)
        }
    }
    
    func isOffline() -> Bool {
        guard Reachability.isConnectedToNetwork() else {
            let error = ErrorMessage(.NoInternetConnection)
            showNativeViewContainer(errorMessage: error)
            return true
        }
        return false
    }
    
    func cycleFromViewController(oldViewController: UIViewController, toViewController newViewController: UIViewController) {
        oldViewController.willMove(toParentViewController: nil)
        self.addChildViewController(newViewController)
        self.addSubview(subView: newViewController.view, toView:self.containerView!)
        oldViewController.view.accessibilityElementsHidden = true
        newViewController.view.accessibilityElementsHidden = false
        oldViewController.view.removeFromSuperview()
        oldViewController.removeFromParentViewController()
        newViewController.didMove(toParentViewController: self)
    }
    
    func createHomeUrlSubRequestWithPath(urlPathToAppend: String) -> String {
        let homeUrl = URL(string: config().HomeUrl)
        let url = URL(string: urlPathToAppend, relativeTo: homeUrl)?.absoluteString
        
        return url!
    }
    
    @objc func selectMyAccount(sender : UITapGestureRecognizer) {
        self.pageUrl = createHomeUrlSubRequestWithPath(urlPathToAppend: config().MyAccountUrlPath)
        webViewController?.loadPage(url: self.pageUrl)
        self.tabBar.selectedItem = nil
        self.selectedTab = nil
    }
    
    @objc func selectHelp(sender : UITapGestureRecognizer) {
        self.pageUrl = config().HelpURL
        webViewController?.webView.loadPage(url: self.pageUrl)
    }
    
    @objc func goHome(sender: UITapGestureRecognizer) {
        self.pageUrl = config().HomeUrl
        self.webViewController?.loadPage(url: self.pageUrl)
        self.tabBar.selectedItem = nil
        self.selectedTab = nil
    }
    func showWhiteScreen() {
        let v = UIView(frame: UIScreen.main.bounds)
        v.backgroundColor = UIColor.white
        v.tag = 2
        self.view.addSubview(v)
    }
    @objc func checkSymptomsAndCIDBack() {
        if let webview = self.webViewController?.webView {
            if(isCheckYourSymptomsPath(webview: webview)
                || hasCidUrlSuffix(webview: webview)){
                self.setVisibilityOfHeaderAndMenuBars(visible: false, isSlim: true)
                self.showWhiteScreen()
                self.webViewController?.loadPage(url: config().HomeUrl)
                self.tabBar.selectedItem = nil
                self.selectedTab = nil
            } else {
                if(webview.canGoBack) {
                    goingBack = true
                    webview.goBack()
                }
            }
        }
    }
    
    func displayExtendSessionDialogue(sessionDuration: Int){
        if (!isPresented) {
            currentSessionDuration = sessionDuration
            extendSessionOverdue = true;
            return
        }
        
        let alert = UIAlertController(title: NSLocalizedString("sessionExpiryWarningHeader", comment: ""), message: String.localizedStringWithFormat(NSLocalizedString("sessionExpiryWarningDurationInformation", comment: ""),sessionDuration), preferredStyle: .alert)
        alert.addAction(UIAlertAction(title: NSLocalizedString("sessionExpiryWarningGetMoreTime", comment: ""), style: .default, handler: { _ in
            self.appWebInterface?.extendSession()
        }))
        alert.addAction(UIAlertAction(title: NSLocalizedString("sessionExpiryWarningLogOut", comment: ""), style: .default, handler: { _ in
            self.appWebInterface?.logout()
        }))
        
        NotificationCenter.default.addObserver(alert, selector: #selector(alert.close), name: CustomNotifications.dismissAllAlerts, object: nil)
        
        self.present(alert, animated: true, completion: { self.resetDisplayExtendSessionDialogueFlags() })
    }
    
    func dimissAlert(){
        NotificationCenter.default.post(name: CustomNotifications.dismissAllAlerts, object: nil)
        resetDisplayExtendSessionDialogueFlags()
    }
    
    private func resetDisplayExtendSessionDialogueFlags(){
        if (extendSessionOverdue) {
            extendSessionOverdue = false
            currentSessionDuration = nil
        }
    }
    
    func getAlert(title: String, message: String) -> UIAlertController{
        
        let alertController = UIAlertController(title: title, message: message, preferredStyle: .alert)
        let cancel = UIAlertAction(title: "Cancel", style: .cancel) { (action) -> Void in
            alertController.dismiss(animated: true, completion: nil)
        }
        alertController.addAction(cancel)
        return alertController
    }
    
    func isCheckYourSymptomsPath(webview: WKWebView!) -> Bool {
        if #available(iOS 10.0, *) {
            os_log("Checking current url for symtoms path", log: OSLog.default, type: .info)
        } else {
            NSLog("Checking current url for symtoms path")
        }
        let baseUrl = URL(string: config().HomeUrl)
        return (webview.url?.host == baseUrl?.host && webview.url?.path == config().CheckSymptomsUrlPath)
    }
    
    func hasCidUrlSuffix(webview: WKWebView!) -> Bool {
        if #available(iOS 10.0, *) {
            os_log("Checking current url for CidSuffix", log: OSLog.default, type: .info)
        } else {
            NSLog("Checking current url for CidSuffix")
        }
        let absoluteUrl = webview.url?.absoluteString
        
        if(absoluteUrl == nil) {
            if #available(iOS 10.0, *) {
                os_log("Current webview url is nil on checkSymptomsAndCIDBack call", log: OSLog.default, type: .info)
            } else {
                NSLog("Current webview url is nil on checkSymptomsAndCIDBack call")
            }
            return false
        } else {
            return absoluteUrl!.contains(config().CidUrlSuffix)
        }
    }
}

