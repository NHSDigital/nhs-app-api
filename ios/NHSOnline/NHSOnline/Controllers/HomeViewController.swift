import Foundation
import os.log
import SafariServices
import LocalAuthentication
import UIKit
import WebKit
import DeviceKit
import FidoClientIOS
import EventKit
import EventKitUI

class HomeViewController : UIViewController, EKEventEditViewDelegate, PaycassoFlowDelegate {
    private let showConstraintPriority = UILayoutPriority.init(rawValue: 900)
    private let hideConstraintPriority = UILayoutPriority.init(rawValue: 850)
    var progressSpinner: ProgressSpinner?
    var splashScreen: SplashScreen?
    
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
    var paycassoService: PaycassoService?
    var notificationsService: NotificationsService?
    var knownServicesProvider: KnownServicesProtocol?
    var configurationServiceProvider: ConfigurationServiceProtocol?
    var laContext: LAContext = LAContext()
    var deviceService: DeviceService?
    var compatibilityService: CompatibilityService?
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
        showSplashScreen()
        showProgressBar()
        self.setNeedsStatusBarAppearanceUpdate()
        super.viewDidLoad()

        setupNhsLogo()
        setupBackArrow()
        setupCloseIcon()
        setupMyAccountIcon()
        setupHelpIcon()
    
        webAppInterface = WebAppInterface(controller: self)
        webViewDelegate = WebViewDelegate(controller: self,
                                          knownServiceProvider: self.knownServicesProvider!,
                                          configurationServiceProvider: self.configurationServiceProvider!,
                                          webAppInterface: webAppInterface!,
                                          loggingService: LoggingService())
        
        tabBarDelegate = TabBarDelegate(controller: self)
        tabBar.delegate = tabBarDelegate
        tabBar.setDefaultTabBarItemsAppearance()
        
        setUpControllers()
        
        self.addChildViewController(self.webViewController!)
        self.addSubview(subView: (self.webViewController?.view)!, toView: self.containerView)

        appWebInterface = AppWebInterface(webView: webViewController?.webView)
        
        let cookieHandler = CookieHandler()
        
        notificationsService = NotificationsService(appWebInterface: appWebInterface!, cookieHandler: cookieHandler)
        lifecycleHandlers = LifecycleHandlers(knownServiceProvider: self.knownServicesProvider!,
                webViewController: webViewController!,
                homeViewController: self,
                configurationServiceProvider: self.configurationServiceProvider!)
        
        var fidoClient: FidoClientProtocol? = nil
        
        if #available(iOS 11.0, *) {
            fidoClient = FidoClient()
        }
        
        laContext.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: nil)
        
        biometricService = BiometricService(homeViewController: self,
                                            configurationServiceProvider: self.configurationServiceProvider!,
                                            appWebInterface: appWebInterface!,
                                            fidoClient: fidoClient,
                                            laContext: laContext)
        let deviceInfoService = DeviceInfoService()
        deviceService = DeviceService(viewController: self,
                                      deviceInfoProtocol: deviceInfoService)
        compatibilityService = CompatibilityService(viewController: self)
        paycassoService = PaycassoService(homeViewController: self, appWebInterface: appWebInterface!, loggingService: LoggingService())
        

        DispatchQueue.main.async {
            switch self.configurationServiceProvider!.getConfigurationResponse() {
            case .success(_):
                self.compatibilityService!.check(isCheckEnabled: config().CompatibilityCheckEnabled)
                if (self.compatibilityService!.hasShownCompatibilityScreen) {
                    return
                }

                self.webViewController?.loadPage(url: UrlHelper.checkForUrlOverride(url: config().HomeUrl))
                return
            default:
                self.apiCallFailure()
                return
            }
        }
    }
    
    func apiCallFailure() {
        guard isOffline() else {
            apiConfigCallError = true
            let error = ErrorMessage(.APICallFailure)
            return showNativeViewContainer(errorMessage: error)
        }
    }
    
    func loadCompatibilityScreen(isCompatible: Bool) {
       let url = URL(string: config().CompatibilityScreenUrl,
                     relativeTo: URL(string: config().HomeUrl))?.absoluteString
    
        if (url == nil) {
            self.webViewController?.loadPage(url: UrlHelper.checkForUrlOverride(url: config().HomeUrl))
            return
        }
    
        var urlComponents = URLComponents(string: url!)!
    
        urlComponents.queryItems = [URLQueryItem(name: "incompatible", value: "\(!isCompatible)")]

        self.webViewController?.loadPage(url:urlComponents.url!.absoluteString, getKnownServices: false)
        LoggingService().logError(message: "iOS Compatibility Check Failure: Device is incompatible and cannot be upgraded")
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
        self.tabBar.selectedItem = nil
        self.selectedTab = nil
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
        webViewController?.headerStrategy = LoggedOutHeaderStrategy(controller: self)
        
        errorViewController = self.storyboard?.instantiateViewController(withIdentifier: "PageUnavailabilityViewController") as? PageUnavailabilityViewController
        errorViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        errorViewController?.loadViewIfNeeded()
        
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
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(self.loggedOutPagesAndCIDBack))
        self.headerBarSlim.backButtonArrow.isUserInteractionEnabled = true
        self.headerBarSlim.backButtonArrow.addGestureRecognizer(tapGesture)
    }
    
    func setupCloseIcon(){
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(self.loggedOutPagesAndCIDBack))
        self.headerBarSlim.closeIcon.isUserInteractionEnabled = true
        self.headerBarSlim.closeIcon.addGestureRecognizer(tapGesture)
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
        let updateNativeVersionJavascriptCommand = "var result = window.nativeAppCallbacks.appVersionUpdateNativeVersion('\(versionNumber)');"
        
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
            case HeaderType.SlimBack:
                self.headerBarSlim.backButtonArrow.isHidden = false
                self.headerBarSlim.closeIcon.isHidden = true
                self.setHeaderVisibility(visible: false)
                self.setSlimHeaderVisibility(visible: true)
            case HeaderType.SlimClose:
                self.headerBarSlim.backButtonArrow.isHidden = true
                self.headerBarSlim.closeIcon.isHidden = false
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
        self.updateHeaderText(headerText: NSLocalizedString("BiometricSessionTimeoutHeader", comment: ""))
        showErrorViewContainer()
        self.webViewController?.dismissSafariViewController()
    }
    
    func showErrorViewContainer() {
        hideSplashScreen()
        hideProgressBar()
        cycleFromViewController(oldViewController: webViewController!, toViewController: errorViewController!)
        currentNativeViewController = errorViewController
        UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, errorViewController?.errorTextView)
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
        self.clearSelectedTab()
    }
    
    open func handleBiometricSpecRequest(biometricAvailability: BiometricState) {
        if #available(iOS 10.0, *) {
            switch (biometricAvailability) {
            case .Registered:
                biometricService?.sendBiometricSpec(enabled: true)
            case .Not_Registered:
                biometricService?.sendBiometricSpec(enabled: false)
            case .Invalidated:
                biometricService?.sendBiometricSpec(enabled: false)
            }
        }
    }
    
    open func handleShowPaycasso(paycassoConfiguration: String){
        
        let configData = Data(String(paycassoConfiguration).utf8);
        
        if (configData.isEmpty) {
            LoggingService().logError(message: "Invalid configuration received. Returning to login with failure")
            
            PaycassoErrors.init(errorMessage: "Paycasso configuration passed is empty/invalid", appWebInterface: appWebInterface!).callbackCustomResponseToWeb()
            
        } else {
            self.paycassoService?.startTransaction(configData: configData)
        }
    }
    
    func onSuccess(_ response: PCSFlowResponse) {
        
        var transactionType = ""
        var isFaceMatched = false
        
        switch (response) {
            case is PCSInstaSureFlowResponse:
                isFaceMatched = true
                transactionType = "InstaSureFlowResponse"
            
            case is PCSDocuSureFlowResponse:
                transactionType = "DocuSureFlowResponse"
            
            case is PCSVeriSureFlowResponse:
                transactionType = "VeriSureFlowResponse"
            default:
                LoggingService().logError(message: "The returned response document type is not recognisedn")
                PaycassoErrors.init(errorMessage: "The returned response document type is not recognised", appWebInterface: appWebInterface!).callbackCustomResponseToWeb()
        }
        
        if (transactionType == "") {
            return
        }
        
        LoggingService().logInfo(message: "Success response received from Paycasso with transaction type of \(transactionType), sending response to login")
        self.appWebInterface?.paycassoSuccessCallback(isFaceMatched: isFaceMatched, transactionId: response.transactionId, transactionType: transactionType)
    }

    func onFailure(_ response: PCSFlowFailureResponse) {
        LoggingService().logError(message: "Failure response returned from Paycasso with failure code \(response.failureCode), returning response to login")
        PaycassoErrors.init(errorMessage: response.failureMessage, appWebInterface: appWebInterface!).callbackPaycassoResponseToWeb(code: response.failureCode)
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
        self.clearSelectedTab()
    }
    
    func showWhiteScreen() {
        let v = UIView(frame: UIScreen.main.bounds)
        v.backgroundColor = UIColor.white
        v.tag = 2
        self.view.addSubview(v)
    }
    @objc func loggedOutPagesAndCIDBack() {
        if let webview = self.webViewController?.webView {
            let backForwardList = webview.backForwardList
            
            if(checkCurrentUrlForPath(webview: webview, urlPath: config().CheckSymptomsUrlPath)
                || hasCidUrlSuffix(webview: webview)
                || checkCurrentUrlForPath(webview: webview, urlPath: config().PreRegistrationInstructionsPath)
                || checkCurrentUrlForPath(webview: webview, urlPath: config().BiometricLoginErrorPath)){
                self.showWhiteScreen()
                self.webViewController?.loadPage(url: config().HomeUrl)
                self.clearSelectedTab()
            } else if (backForwardList.backItem?.url.path == config().PreRegistrationInstructionsPath
                || backForwardList.backItem?.url.path == config().CheckSymptomsUrlPath){
                self.webViewController?.loadPage(url: backForwardList.backItem?.url.absoluteString ?? config().HomeUrl)
            } else {
                if(webview.canGoBack) {
                    goingBack = true
                    webview.goBack()
                }
            }
        }
    }
    
    func displayExtendSessionDialogue(){
        self.dismissIOSVersionUpdateWarningDialog()
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
    
    func displayLeavingPageDialogue(){
        let alert = UIAlertController(title: NSLocalizedString("leavingPageWarningTitle", comment: ""), message: NSLocalizedString("leavingPageWarningMessage", comment: ""), preferredStyle: .alert)
        alert.addAction(UIAlertAction(title: NSLocalizedString("leavingPageWarningStayOnPage", comment: ""), style: .default, handler: { _ in
            self.appWebInterface?.stayOnPage()
        }))
        alert.addAction(UIAlertAction(title: NSLocalizedString("leavingPageWarningLeavePage", comment: ""), style: .default, handler: { _ in
            self.appWebInterface?.leavePage()
        }))

        NotificationCenter.default.addObserver(alert, selector: #selector(alert.close), name: CustomNotifications.dismissAllAlerts, object: nil)
        NotificationCenter.default.addObserver(alert, selector: #selector(alert.close), name: CustomNotifications.dismissLeavingPageAlert, object: nil)
        
        alert.show()
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
    
    func checkCurrentUrlForPath(webview: WKWebView!, urlPath: String) -> Bool {
        Logger.logInfo(message: "Checking current url for %@", urlPath)

        let baseUrl = URL(string: config().HomeUrl)
        return (webview.url?.host == baseUrl?.host && webview.url?.path == urlPath)
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
    
    func addNotificationCookie(nhsLoginId: String) {
        notificationsService?.addNotificationCookie(nhsLoginId: nhsLoginId)
    }
    
    func checkNotificationCookie(nhsLoginId: String) {
        notificationsService?.checkNotificationCookie(nhsLoginId: nhsLoginId)
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
    
    func handleBiometricStatusChangeRequest(biometricState: BiometricState) {
        if #available(iOS 10.0, *) {
            if (!(biometricService?.checkBiometricCapability())!) {
                return
            }
            
            // adding this to the main queue which shows prompt instantly and allows spinner
            DispatchQueue.main.asyncAfter(deadline: .now()) {
               if(biometricState == BiometricState.Registered) {
                    self.biometricService?.deRegister(deregisterFidoCredentials: true)
                } else {
                    self.biometricService?.register()
                }
            }
        }
    }
    
    func showProgressBar() {
        self.progressSpinner?.show(uiView: self.view)
    }
    
    func hideProgressBar() {
        self.progressSpinner?.hide(uiView: self.view)
    }
    
    func resumeProgressBar() {
        self.progressSpinner?.resume(uiView: self.view)
    }
    
    func showSplashScreen() {
        self.splashScreen?.show(uiView: self.view)
    }
    
    func hideSplashScreen() {
        DispatchQueue.main.asyncAfter(deadline: .now() + 0.1, execute: {
            self.splashScreen?.hide()
        })
    }
    
    func handleGoToPage(page: String) {
        let redirectUrl = UrlHelper.createRedirectToPageUrl(page: page)
        webViewController?.loadPage(url: redirectUrl!)
    }
   
    func eventEditViewController(_ controller: EKEventEditViewController, didCompleteWith action: EKEventEditViewAction) {
        self.dismiss(animated: true, completion: nil)
    }
    
    func addEventToCalendar(calendarData: CalendarData) {

        if (!isCalendarDataValid(subject: calendarData.subject,
                                 startTimeEpochInSeconds: calendarData.startTimeEpochSeconds,
                                 endTimeEpochInSeconds: calendarData.endTimeEpochSeconds)) {
            
            showErrorDialog()
            
            let failureMessage = "Add to calendar failure from " + buildLoggingMessageSource(source: calendarData.source);
            self.webViewDelegate?.loggingService.logError(message: failureMessage)
            return
        }
        
        let eventStore = EKEventStore()
        eventStore.requestAccess( to: EKEntityType.event, completion:{(granted, error) in
            DispatchQueue.main.async {
                if (error == nil) {
                    if (granted) {
                        self.addCalendarEvent(eventStore: eventStore, calendarData: calendarData)
                        
                        let successMessage = "Add to calendar success from " + self.buildLoggingMessageSource(source: calendarData.source);
                        self.webViewDelegate?.loggingService.logInfo(message: successMessage)
                    } else {
                        self.showOpenSettingsAlert()
                    }
                }
            }
        })
    }
    
    func dismissIOSVersionUpdateWarningDialog() {
        NotificationCenter.default.post(name: CustomNotifications.dismissIOSVersionUpdateAlert, object: nil)
    }

    func dismissPageLeaveWarningDialogue() {
        NotificationCenter.default.post(name: CustomNotifications.dismissLeavingPageAlert, object: nil)
    }

    private func addCalendarEvent(eventStore: EKEventStore, calendarData: CalendarData) {
        let event = EKEvent(eventStore: eventStore)
            
        event.title = calendarData.subject
        event.startDate = Date(timeIntervalSince1970: TimeInterval(calendarData.startTimeEpochSeconds!))
        event.endDate = Date(timeIntervalSince1970: TimeInterval(calendarData.endTimeEpochSeconds!))
        event.location = calendarData.location
        event.notes = calendarData.body

        let eventController = EKEventEditViewController()
        eventController.event = event
        eventController.eventStore = eventStore

        eventController.editViewDelegate = self
        self.present(eventController, animated: true, completion: nil)
    }
    
    private func buildLoggingMessageSource(source: JavaScriptInteractionMode) -> String {
        switch (source) {
        case JavaScriptInteractionMode.NhsApp:
            return "the NhsApp"
        case JavaScriptInteractionMode.SilverThirdParty:
            return "the a third party"
        default:
            return "an unknown source"
        }
    }

    private func showOpenSettingsAlert() {
        let showSettingTitle = NSLocalizedString("AllowCalendarAccessTitle", comment: "")
        let showSettingsMessage = NSLocalizedString("AllowCalendarAccessMessage", comment: "")
        let showSettingsDismissButtonText = NSLocalizedString("AllowCalendarAccessDismissText", comment: "")
        let showSettingGoToSettingsButtonText = NSLocalizedString("AllowCalendarAccessGoToSettingsText", comment: "")

        let alertController = UIAlertController(title: showSettingTitle, message: showSettingsMessage, preferredStyle: .alert)

        let cancelAction = UIAlertAction(title: showSettingsDismissButtonText, style: .default, handler: nil)
        alertController.addAction(cancelAction)
        
        if #available(iOS 10.0, *) {
            let settingsAction = UIAlertAction(title: showSettingGoToSettingsButtonText, style: .default) { (_) -> Void in
                guard let settingsUrl = URL(string: UIApplicationOpenSettingsURLString) else {
                    return
                }
                if UIApplication.shared.canOpenURL(settingsUrl) {
                    UIApplication.shared.open(settingsUrl, completionHandler: { (success) in })
                }
            }
            alertController.addAction(settingsAction)
        }

        self.present(alertController, animated: true, completion: nil)
    }
    
    private func isCalendarDataValid(subject: String?, startTimeEpochInSeconds: Int64?, endTimeEpochInSeconds: Int64?) -> Bool {
        if (subject == nil || subject!.isEmpty || startTimeEpochInSeconds == nil || endTimeEpochInSeconds == nil ||
            startTimeEpochInSeconds! > endTimeEpochInSeconds!) {
            
            return false
        }
        return true
    }
    
    private func showErrorDialog()  {
        let addToCalErrorDialogTitle = NSLocalizedString("AddToCalendarErrorTitle", comment: "")
        let addToCalErrorDialogMessage = NSLocalizedString("AddToCalendarErrorMessage", comment: "")
        let addToCalErrorDialogDismissButtonText = NSLocalizedString("AddToCalendarErrorDismissButtonText", comment: "")
        let addToCalErrorDialogAddEventButtonText = NSLocalizedString("AddToCalendarErrorAddEventButtonText", comment: "")
        
        let okAction = UIAlertAction(title: addToCalErrorDialogDismissButtonText, style: .cancel, handler: nil)
        let goToCalendarAction = UIAlertAction(title: addToCalErrorDialogAddEventButtonText, style: .default) {
            UIAlertAction in
            let eventStore = EKEventStore()
            eventStore.requestAccess( to: EKEntityType.event, completion:{(granted, error) in
                DispatchQueue.main.async {
                    if (granted) && (error == nil) {
                        
                        let event = EKEvent(eventStore: eventStore)

                        let eventController = EKEventEditViewController()
                        eventController.event = event
                        eventController.eventStore = eventStore

                        eventController.editViewDelegate = self
                        self.present(eventController, animated: true, completion: nil)
                    }
                }
            })
        }

        let alert = UIAlertController(title: addToCalErrorDialogTitle, message: addToCalErrorDialogMessage, preferredStyle: .alert)

        alert.addAction(okAction)
        alert.addAction(goToCalendarAction)

        self.present(alert, animated: true)
    }
}

