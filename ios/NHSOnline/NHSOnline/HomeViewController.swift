import Foundation
import os.log
import SafariServices
import UIKit
import WebKit

class HomeViewController : UIViewController {
    private let showConstraintPriority = UILayoutPriority.init(rawValue: 900)
    private let hideConstraintPriority = UILayoutPriority.init(rawValue: 850)
    
    var applicationState = ApplicationState()
    var documentInteractionController = UIDocumentInteractionController()
    
    @IBOutlet weak var headerBar: HeaderBar!
    @IBOutlet weak var headerBarSlim: HeaderBarSlim!
    
    @IBOutlet weak var Notch: UIView!
    @IBOutlet weak var tabBarSpacer: UIView!
    @IBOutlet weak var tabBar: UITabBar!
    @IBOutlet weak var containerView: UIView!
    
    @IBOutlet weak var webviewHeaderTopConstraint: NSLayoutConstraint!
    @IBOutlet weak var webviewNavMenuBottomConstraint: NSLayoutConstraint!
    @IBOutlet weak var webviewHeaderSlimTopConstraint: NSLayoutConstraint!

    var lifecycleHandlers: LifecycleHandlers?
    var webViewController: WebViewController?
    var errorViewController: PageUnavailabilityViewController?
    var biometricViewController: BiometricsViewController?
    var biometricResultController: BiometricsResultViewController?
    var currentNativeViewController: UIViewController?
    var webViewDelegate: WebViewDelegate?
    var tabBarDelegate: TabBarDelegate?
    var myAccountUrl = config().MyAccountUrlPath
    var appVersionCheckError: Bool = false
    var apiConfigCallError: Bool = false
    var biometricsHasBeenAttempted: Bool = false
    var appWebInterface: AppWebInterface?
    var webAppInterface: WebAppInterface?
    var extendSessionOverdue: Bool = false
    var isPresented: Bool = false
    var goingBack: Bool = false
    var biometricService: BiometricService?
    var notificationsService: NotificationsService?
    var knownServicesProvider: KnownServicesProtocol?
    var configurationServiceProvider: ConfigurationServiceProtocol?
    public var selectedTab: Int?
    
    override func viewWillAppear(_ animated: Bool) {
        self.navigationController?.navigationBar.isHidden = true
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        isPresented = true
        
        if (extendSessionOverdue) {
            displayExtendSessionDialogue()
        }
    }
    
    override var preferredStatusBarStyle: UIStatusBarStyle {
        return .lightContent
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        isPresented = false
    }
    
    override func viewDidLoad() {
        self.setNeedsStatusBarAppearanceUpdate()
        super.viewDidLoad()

        setupNhsLogo()
        setupBackArrow()
        setupMyAccountIcon()
        setupHelpIcon()

        webAppInterface = WebAppInterface(controller: self)
        webViewDelegate = WebViewDelegate(controller: self, knownServiceProvider: self.knownServicesProvider!, webAppInterface: webAppInterface!)
        tabBarDelegate = TabBarDelegate(controller: self)
        tabBar.delegate = tabBarDelegate
        tabBar.setDefaultTabBarItemsAppearance()
        
        setUpControllers()
        
        self.addChildViewController(self.webViewController!)
        self.addSubview(subView: (self.webViewController?.view)!, toView: self.containerView)

        appWebInterface = AppWebInterface(webView: webViewController?.webView)
        notificationsService = NotificationsService(appWebInterface: appWebInterface!)
        lifecycleHandlers = LifecycleHandlers(knownServiceProvider: self.knownServicesProvider!,
                webViewController: webViewController!,
                homeViewController: self,
                configurationServiceProvider: self.configurationServiceProvider!)

        switch self.configurationServiceProvider!.getConfigurationResponse() {
        case .success(_):
            guard let urlToLoad = UserDefaults.standard.url(forKey: config().NotificationLinkPropertyName) else {
                self.webViewController?.loadPage(url: config().HomeUrl)
                return
            }
            
            self.webViewController?.loadPage(url: urlToLoad)
            UserDefaults.standard.removeObject(forKey: config().NotificationLinkPropertyName)
            return
        default:
            apiCallFailure()
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
        let webView = self.webViewController?.webView
        checkForLoginPageAndTriggerBiometricTimer(webView!, timer)
    }
    
    func checkForLoginPageAndTriggerBiometricTimer(_ webView: WKWebView, _ timer: Double) {
        if isOnBiometricsLogin(webView: webView) {
            clearSelectedTab()
            self.showWebViewContainer()
            if #available(iOS 10.0, *) {
                Timer.scheduledTimer(timeInterval: timer, target: self, selector: #selector(self.attemptBiometricLoginIfAppVersionValid), userInfo: nil, repeats: false)
            }
        }
    }
    
    func clearSelectedTab() {
        tabBar.selectedItem = nil
        selectedTab = nil
    }
    
    func isOnBiometricsLogin(webView: WKWebView) -> Bool {
        let loginURLString = config().HomeUrl + "login"
        guard let fidoUrl = webView.url?.absoluteString.contains("fidoAuthResponse") else { return false }
        return !fidoUrl && (webView.url?.absoluteString.contains(loginURLString))!
    }
    
    @objc @available(iOS 10.0, *)
    public func attemptBiometricLoginIfAppVersionValid() {
        do {
            switch self.configurationServiceProvider!.getConfigurationResponse() {
            case .success(let configurationResponse):
                if (configurationResponse.isSupportedVersion) {
                    self.attemptBiometricLogin()
                    self.biometricsHasBeenAttempted = true
                }
            default:
                return
            }
        }
    }
    
    @objc @available(iOS 10.0, *)
    func attemptBiometricLogin() {
        do {
            if(UserDefaultsManager.getBiometricAvailability() == BiometricState.Registered) {
                biometricService?.authenticate()
            } else if (UserDefaultsManager.getBiometricAvailability() == BiometricState.Invalidated) {
                biometricService?.deRegister(deregisterFidoCredentials: false)
                showBiometricsAlert(.BiometricsInvalidated)
            }
        }
    }

    func showBiometricsAlert(_ alertType: BiometricAlertType) {
        let alert = BiometricStringHandler().getBiometricAlert(type: alertType)
        alert.show()
    }
    
    func reloadLoginPage() {
        self.webViewController?.loadPage(url: config().HomeUrl)
    }
    
    func backToAccountPage() {
        self.webViewController?.loadPage(url: myAccountUrl)
    }
    
    func setUpControllers() {
        webViewController = self.storyboard?.instantiateViewController(withIdentifier: "WebViewController") as? WebViewController
        webViewController?.knownServiceProvider = self.knownServicesProvider
        webViewController?.configurationServiceProvider = self.configurationServiceProvider
        webViewController?.loadViewIfNeeded()
        webViewController?.setWebViewDelegate(delegate: webViewDelegate!)
        webViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        
        errorViewController = self.storyboard?.instantiateViewController(withIdentifier: "PageUnavailabilityViewController") as? PageUnavailabilityViewController
        errorViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        errorViewController?.loadViewIfNeeded()
        
        biometricViewController = self.storyboard?.instantiateViewController(withIdentifier: "BiometricsViewController") as? BiometricsViewController
        biometricViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        biometricViewController?.homeViewController = self
        biometricService = BiometricService(homeViewController: self, biometricViewController: biometricViewController!, configurationServiceProvider: self.configurationServiceProvider!)
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
    
    func updateHeaderText(headerText: String?, accessibilityLabel: String? = nil) {
        if let a11yLabel = accessibilityLabel {
            UIAccessibilityPostNotification(UIAccessibilityAnnouncementNotification, a11yLabel)
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
                Logger.logInfo(message: "An error occurred setting the app version number")
            }
        }
        
        self.webViewController?.webView.evaluateJavaScript(scriptToExecute, completionHandler: completionHandler)
    }
    
    func setVisibilityOfHeaderAndMenuBars(headerType: HeaderType) {
        UIView.animate(withDuration: 0.3, animations: {
            self.Notch.isHidden = false
            
            switch headerType {
            case HeaderType.Full:
                self.setHeaderVisibility(visible: true)
                self.setSlimHeaderVisibility(visible: false)
            case HeaderType.Slim:
                self.setHeaderVisibility(visible: false)
                self.setSlimHeaderVisibility(visible: true)
            default:
                self.setHeaderVisibility(visible: false)
                self.setSlimHeaderVisibility(visible: false)
            }
        })
    }
    
    private func setHeaderVisibility(visible: Bool) {
        let contraintPriority: UILayoutPriority
        if visible {
            contraintPriority = showConstraintPriority
        } else {
            contraintPriority = hideConstraintPriority
        }
        
        self.webviewHeaderTopConstraint.priority = contraintPriority
        self.webviewNavMenuBottomConstraint.priority = contraintPriority
        self.headerBar.isHidden = !visible
        self.tabBar.isHidden = !visible
        self.tabBarSpacer.isHidden = !visible
    }
    
    private func setSlimHeaderVisibility(visible: Bool) {
        let contraintPriority: UILayoutPriority
        if visible {
            contraintPriority = showConstraintPriority
        } else {
            contraintPriority = hideConstraintPriority
        }
        
        self.webviewHeaderSlimTopConstraint.priority = contraintPriority
        self.headerBarSlim.isHidden = !visible
    }
    
    func getBiometricRegistrationErrorStrings() -> BiometricErrorStrings{
        return BiometricStringHandler().getErrorStrings()
    }
    
    func getDownloadErrorStrings() -> DataDownloadErrorStrings{
        return DataDownloadHandler().getErrorStrings()
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
        let urlToLoad = createHomeUrlSubRequestWithPath(urlPathToAppend: config().MyAccountUrlPath)
        webViewController?.loadPage(url: urlToLoad)
        self.tabBar.selectedItem = nil
        self.selectedTab = nil
    }
    
    @objc func selectHelp(sender : UITapGestureRecognizer) {
        if (UserDefaults.standard.string(forKey: "HelpUrl") == nil) {
            UserDefaults.standard.set(config().HelpURL, forKey: "HelpUrl")
        }
        
        let urlToLoad = UserDefaults.standard.string(forKey: "HelpUrl") == config().HelpLoginURL
            ? config().HelpLoginURL
            : UserDefaults.standard.string(forKey: "HelpUrl") ?? config().HelpURL

        webViewController?.webView.loadPage(url: urlToLoad)
    }
    
    @objc func goHome(sender: UITapGestureRecognizer) {
        self.webViewController?.loadPage(url: config().HomeUrl)
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
                self.setVisibilityOfHeaderAndMenuBars(headerType: HeaderType.None)
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
    
    func displayExtendSessionDialogue(){
        if (!isPresented) {
            extendSessionOverdue = true;
            return
        }
        
        let alert = UIAlertController(title: NSLocalizedString("sessionExpiryWarningMessage", comment: ""), message: nil, preferredStyle: .alert)
        alert.addAction(UIAlertAction(title: NSLocalizedString("sessionExpiryWarningLogOut", comment: ""), style: .default, handler: { _ in
            self.appWebInterface?.logout()
        }))
        alert.addAction(UIAlertAction(title: NSLocalizedString("sessionExpiryWarningStayLoggedIn", comment: ""), style: .default, handler: { _ in
            self.appWebInterface?.extendSession()
        }))
        
        NotificationCenter.default.addObserver(alert, selector: #selector(alert.close), name: CustomNotifications.dismissAllAlerts, object: nil)
        
        alert.show()
        self.resetDisplayExtendSessionDialogueFlags()
    }
    
    func dimissAlert(){
        NotificationCenter.default.post(name: CustomNotifications.dismissAllAlerts, object: nil)
        resetDisplayExtendSessionDialogueFlags()
    }
    
    private func resetDisplayExtendSessionDialogueFlags(){
        if (extendSessionOverdue) {
            extendSessionOverdue = false
        }
    }
    
    func isCheckYourSymptomsPath(webview: WKWebView!) -> Bool {
        Logger.logInfo(message: "Checking current url for symptoms path")

        let baseUrl = URL(string: config().HomeUrl)
        return (webview.url?.host == baseUrl?.host && webview.url?.path == config().CheckSymptomsUrlPath)
    }
    
    func hasCidUrlSuffix(webview: WKWebView!) -> Bool {
        Logger.logInfo(message: "Checking current url for CidSuffix")

        let absoluteUrl = webview.url?.absoluteString
        
        if(absoluteUrl == nil) {
            Logger.logInfo(message: "Current webview url is nil on checkSymptomsAndCIDBack call")
            return false
        } else {
            return absoluteUrl!.contains(config().CidUrlSuffix)
        }
    }
    
    func registerForPushNotifications(trigger: String) {
        notificationsService?.registerForPushNotifications(trigger: trigger)
    }
    
    func pushNotificationsAuthorised(deviceToken: Data) {
        notificationsService?.authorised(deviceToken: deviceToken)
    }
    
    func failedToRegisterForNotifications() {
        notificationsService?.failedToRegister()
    }
    
    func getNotificationsStatus() {
        notificationsService?.getNotificationsStatus()
    }
    
    func showDownloadError() {
        self.errorViewController?.setUnavailabilityError(errorMessage: ErrorMessage(.DownloadError))
        self.updateHeaderText(headerText: getDownloadErrorStrings().DownloadPageHeader)
        cycleFromViewController(oldViewController: self.webViewController!, toViewController: errorViewController!)
        currentNativeViewController = errorViewController
        UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, errorViewController?.errorTextView)
    }
    
    func showDataDownloadAlert(alertType: DataDownloadAlertType) {
        let alert = DataDownloadAlertHandler().getDownloadAlert(type: alertType)
        alert.show()
    }
    
    func downloadFile(messageBody: String) {
         if #available(iOS 10.0, *) {
             let splitMessage = messageBody.components(separatedBy: "|split|")
             let base64Data = splitMessage[0]
             let fileName = splitMessage[1]
             let mimeType = splitMessage[2]

             let dataWithoutStart = base64Data.split(separator: ",", maxSplits: 1, omittingEmptySubsequences: true)[1]
             let convertedData = Data(base64Encoded: String(describing: dataWithoutStart), options: .ignoreUnknownCharacters)

             if (mimeType.containsAnyOf(["image"])){
                 
                 let tmpURL = FileManager.default.temporaryDirectory
                     .appendingPathComponent(fileName)
                 do {
                     try convertedData!.write(to: tmpURL)
                 } catch {
                     self.showDownloadError()
                     return
                 }
                 self.documentInteractionController.url = tmpURL
                 self.documentInteractionController.uti = "public.image, public.content"
                 self.documentInteractionController.name = tmpURL.lastPathComponent
                 if UIDevice.current.userInterfaceIdiom == .pad {
                     var squareFrame: CGRect {
                         let midX = self.view.bounds.midX
                         let midY = self.view.bounds.midY
                         let size: CGFloat = 64
                         return CGRect(x: midX-size/2, y: midY-size/2, width: size, height: size)
                     }
                     
                     self.documentInteractionController.presentOptionsMenu(
                         from: squareFrame,
                         in: self.view,
                         animated: true
                     )
                 } else {
                     self.documentInteractionController.presentOptionsMenu(
                         from: self.view.frame,
                         in: self.view,
                         animated: true
                     )
                 }
             } else {
                 let tmpURL = FileManager.default.temporaryDirectory
                     .appendingPathComponent(fileName)
                 do {
                     try convertedData!.write(to: tmpURL)
                 } catch {
                     self.showDownloadError()
                     return
                 }
                     
                 self.documentInteractionController.url = tmpURL
                 self.documentInteractionController.uti = "public.data, public.content"
                 self.documentInteractionController.name = tmpURL.lastPathComponent
                 if UIDevice.current.userInterfaceIdiom == .pad {
                     var squareFrame: CGRect {
                         let midX = self.view.bounds.midX
                         let midY = self.view.bounds.midY
                         let size: CGFloat = 64
                         return CGRect(x: midX-size/2, y: midY-size/2, width: size, height: size)
                     }
                     
                     self.documentInteractionController.presentOpenInMenu(
                         from: squareFrame,
                         in: self.view,
                         animated: true
                     )
                 } else {
                 self.documentInteractionController.presentOpenInMenu(
                     from: self.view.frame,
                     in: self.view,
                     animated: true
                 )
                 }
             }
             
         } else {
             self.showDataDownloadAlert(alertType: .OSNotSupported)
             return
         }
    }
}
