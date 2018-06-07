import UIKit

class HomeViewController : UIViewController {
    private let showConstraintPriority = UILayoutPriority.init(rawValue: 900)
    private let hideConstraintPriority = UILayoutPriority.init(rawValue: 850)
    
    @IBOutlet weak var headerBar: HeaderBar!
    @IBOutlet weak var tabBar: UITabBar!
    @IBOutlet weak var containerView: UIView!
    
    @IBOutlet weak var webviewHeaderTopConstraint: NSLayoutConstraint!
    @IBOutlet weak var webviewNavMenuBottomConstraint: NSLayoutConstraint!
    
    let knownServices = KnownServices(config: config())
    var lifecycleHandlers: LifecycleHandlers?
    var webViewController: WebViewController?
    var nativeViewController: PageUnavailabilityViewController?
    var webViewDelegate: WebViewDelegate?
    var tabBarDelegate: TabBarDelegate?
    var pageUrl = config().HomeUrl
    
    override func viewDidLoad() {
        super.viewDidLoad()
        setupNhsLogo()
        setupMyAccountIcon()
        
        webViewDelegate = WebViewDelegate(controller: self, knownServices:knownServices)
        tabBarDelegate = TabBarDelegate(controller: self)
        tabBar.delegate = tabBarDelegate
        
        webViewController = self.storyboard?.instantiateViewController(withIdentifier: "WebViewController") as? WebViewController
        webViewController?.loadViewIfNeeded()
        webViewController?.setWebViewDelegate(delegate: webViewDelegate!)
        webViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        
        nativeViewController = self.storyboard?.instantiateViewController(withIdentifier: "PageUnavailabilityViewController") as? PageUnavailabilityViewController
        nativeViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        nativeViewController?.loadViewIfNeeded()
        
        self.addChildViewController(self.webViewController!)
        self.addSubview(subView: (self.webViewController?.view)!, toView: self.containerView)
        self.webViewController?.webView.loadPage(url: pageUrl)
        
        lifecycleHandlers = LifecycleHandlers(knownServices: knownServices, webView: webViewController!.webView)
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
    
    func updateHeaderText(headerText: String?) {
        if (headerText != nil) {
            self.headerBar.headerTitle.text = headerText
        }
    }
    
    func setupNhsLogo() {
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(self.goHome))
        self.headerBar.NHSHomeLogo.isUserInteractionEnabled = true
        self.headerBar.NHSHomeLogo.addGestureRecognizer(tapGesture)
    }
    
    func setupMyAccountIcon() {
        let tapGesture = UITapGestureRecognizer(target: self, action:  #selector(self.selectMyAccount))
        self.headerBar.myAccountIcon.isUserInteractionEnabled = true
        self.headerBar.myAccountIcon.addGestureRecognizer(tapGesture)
    }
    
    func setVisibilityOfHeaderAndMenuBars(visible:Bool) {
        UIView.animate(withDuration: 0.3, animations: {
            let constraintPriority:UILayoutPriority
            
            if visible {
                constraintPriority = self.showConstraintPriority
            } else {
                constraintPriority = self.hideConstraintPriority
            }
            
            self.webviewHeaderTopConstraint.priority = constraintPriority
            self.webviewNavMenuBottomConstraint.priority = constraintPriority
            self.headerBar.isHidden = !visible
            self.tabBar.isHidden = !visible
        })
    }
    
    func showWebViewContainer() {
        self.cycleFromViewController(oldViewController: self.nativeViewController!, toViewController: self.webViewController!)
    }
    
    func showNativeViewContainer(errorMessage: ErrorMessage) {
        self.nativeViewController?.setUnavailabilityError(errorMessage: errorMessage)
        self.cycleFromViewController(oldViewController: self.webViewController!, toViewController: self.nativeViewController!)
        UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, self.nativeViewController?.errorTextView)
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
        webViewController?.webView.loadPage(url: self.pageUrl)
        self.tabBar.selectedItem = nil
        updateHeaderText(headerText: NSLocalizedString("MyAccountTitle", comment: ""))

    }
    
    @objc func goHome(sender: UITapGestureRecognizer) {
        self.pageUrl = config().HomeUrl
        self.webViewController?.webView.loadPage(url: self.pageUrl)
        self.tabBar.selectedItem = nil
        updateHeaderText(headerText: NSLocalizedString("HomeTitle", comment: ""))

    }
}

