import UIKit
import os.log
import SafariServices

class HomeViewController : UIViewController {
    private let showConstraintPriority = UILayoutPriority.init(rawValue: 900)
    private let hideConstraintPriority = UILayoutPriority.init(rawValue: 850)
    
    @IBOutlet weak var headerBar: HeaderBar!
    @IBOutlet weak var headerBarSlim: HeaderBarSlim!

    @IBOutlet weak var tabBar: UITabBar!
    @IBOutlet weak var containerView: UIView!
    
    @IBOutlet weak var webviewHeaderTopConstraint: NSLayoutConstraint!
    @IBOutlet weak var webviewNavMenuBottomConstraint: NSLayoutConstraint!
    @IBOutlet weak var webviewHeaderSlimTopConstraint: NSLayoutConstraint!
    
    let knownServices = KnownServices(config: config())
    var lifecycleHandlers: LifecycleHandlers?
    var configurationService: ConfigurationService?
    var webViewController: WebViewController?
    var nativeViewController: PageUnavailabilityViewController?
    var webViewDelegate: WebViewDelegate?
    var tabBarDelegate: TabBarDelegate?
    var pageUrl = config().HomeUrl
    var appVersionCheckError: Bool = false
    
    override func viewWillAppear(_ animated: Bool) {
        self.navigationController?.navigationBar.isHidden = true
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()

        setupNhsLogo()
        setupBackArrow()
        setupMyAccountIcon()
        setupHelpIcon()
        
        webViewDelegate = WebViewDelegate(controller: self, knownServices: knownServices)
        tabBarDelegate = TabBarDelegate(controller: self)
        tabBar.delegate = tabBarDelegate
        tabBar.setDefaultTabBarItemsAppearance()
        
        webViewController = self.storyboard?.instantiateViewController(withIdentifier: "WebViewController") as? WebViewController
        webViewController?.loadViewIfNeeded()
        webViewController?.setWebViewDelegate(delegate: webViewDelegate!)
        webViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        
        nativeViewController = self.storyboard?.instantiateViewController(withIdentifier: "PageUnavailabilityViewController") as? PageUnavailabilityViewController
        nativeViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        nativeViewController?.loadViewIfNeeded()
        
        self.addChildViewController(self.webViewController!)
        self.addSubview(subView: (self.webViewController?.view)!, toView: self.containerView)
        self.webViewController?.loadPage(url: pageUrl)
        
        configurationService = ConfigurationService(homeViewController: self)

        lifecycleHandlers = LifecycleHandlers(knownServices: knownServices, webViewController: webViewController!, configurationService: configurationService!)
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
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(self.checkSymptomsBack))
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
        let updatePlatformJavascriptCommand = "var result = window.$nuxt.$store.dispatch('appVersion/updatePlatform', 'iOS');"

        let completionHandler: (Any?, Error?) -> Void = {
            (data, error) in
            if(error != nil) {
                if #available(iOS 10.0, *) {
                    os_log("An error occured setting the app version number.", log: OSLog.default, type: .error)
                } else {
                    NSLog("An error occured setting the app version number.")
                }
            }
        }
        
        self.webViewController?.webView.evaluateJavaScript(updateNativeVersionJavascriptCommand, completionHandler: completionHandler)
        self.webViewController?.webView.evaluateJavaScript(updatePlatformJavascriptCommand, completionHandler: completionHandler)
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
        
        setupAppVersion()
    }
    
    func showWebViewContainer() {
        if (!appVersionCheckError) {
            self.cycleFromViewController(oldViewController: self.nativeViewController!, toViewController: self.webViewController!)
        }
    }
    
    func showNativeViewContainer(errorMessage: ErrorMessage) {
        self.nativeViewController?.setUnavailabilityError(errorMessage: errorMessage)
        self.updateHeaderText(headerText: NSLocalizedString("ConnectionErrorHeader", comment: ""))
        self.cycleFromViewController(oldViewController: self.webViewController!, toViewController: self.nativeViewController!)
        UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, self.nativeViewController?.errorTextView)
    }
    
    func resetFocusAndAnnouncePageTitle(pageTitle: String?) {
        self.headerBar.setFocusToNhsLogoForA11y()
        if let title = pageTitle {
            UIAccessibilityPostNotification(UIAccessibilityAnnouncementNotification, title)
        }
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
    }
    
    @objc func selectHelp(sender : UITapGestureRecognizer) {
        self.pageUrl = config().HelpURL
        webViewController?.webView.loadPage(url: self.pageUrl)
    }
    
    @objc func goHome(sender: UITapGestureRecognizer) {
        self.pageUrl = config().HomeUrl
        self.webViewController?.loadPage(url: self.pageUrl)
        self.tabBar.selectedItem = nil
    }
    func showWhiteScreen() {
        let v = UIView(frame: UIScreen.main.bounds)
        v.backgroundColor = UIColor.white
        v.tag = 2
        self.view.addSubview(v)
    }
    @objc func checkSymptomsBack() {
        let baseUrl = URL(string: config().HomeUrl)
        if let webview = self.webViewController?.webView {
            if(webview.url?.host == baseUrl?.host && webview.url?.path == config().CheckSymptomsUrlPath) {
                self.setVisibilityOfHeaderAndMenuBars(visible: false, isSlim: true)
                self.showWhiteScreen()
                self.webViewController?.loadPage(url: config().HomeUrl)
                
            } else {
                if(webview.canGoBack) {
                    webview.goBack()
                }
            }
        }
    }
}

