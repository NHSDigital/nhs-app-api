import UIKit
import os.log

class UnsecureViewController : UIViewController {
    private let showConstraintPriority = UILayoutPriority.init(rawValue: 900)
    private let hideConstraintPriority = UILayoutPriority.init(rawValue: 850)
    
    @IBOutlet weak var containerView: UIView!

    
    let knownServices = KnownServices(config: config())
    var lifecycleHandlers: LifecycleHandlers?
    var webViewController: UnsecureWebViewController?
    var nativeViewController: PageUnavailabilityViewController?
    var webViewDelegate: UnsecureWebViewDelegate?

    var pageUrl = config().HomeUrl
    

    
    override func viewDidLoad() {
        super.viewDidLoad()
        webViewDelegate = UnsecureWebViewDelegate(controller: self, knownServices: knownServices)
        webViewController = self.storyboard?.instantiateViewController(withIdentifier: "UnsecureWebViewController") as? UnsecureWebViewController
        webViewController?.loadViewIfNeeded()
        webViewController?.setWebViewDelegate(delegate: webViewDelegate!)
        webViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        
        nativeViewController = self.storyboard?.instantiateViewController(withIdentifier: "PageUnavailabilityViewController") as? PageUnavailabilityViewController
        nativeViewController?.view.translatesAutoresizingMaskIntoConstraints = false
        nativeViewController?.loadViewIfNeeded()
        
        self.addChildViewController(self.webViewController!)
        self.addSubview(subView: (self.webViewController?.view)!, toView: self.containerView)
        lifecycleHandlers = LifecycleHandlers(knownServices: knownServices, webViewController: webViewController!)
    }
    
    override func viewWillAppear(_ animated: Bool) {
        self.configureNavBar()
    }
    
    @objc func checkForPages() {
        if (self.webViewController?.webView.canGoBack)! {
            self.webViewController?.webView.goBack()
        } else if let failedUrl = self.webViewDelegate?.failedUrl {
           if isCheckSymptomsUnsecureURL(failedUrl: failedUrl) {
                    let url = self.createHomeUrlSubRequestWithPath(urlPathToAppend: config().CheckSymptomsUrlPath) + config().NhsOnlineRequiredQueryString
                    self.webViewController?.webView.loadPage(url: url)
           } else {
                closeVC()
            }
        } else {
           closeVC()
        }
    }
    
    func closeVC() {
        self.navigationController?.popViewController(animated: true)
        dismiss(animated: true, completion: nil)
    }
    
    func isCheckSymptomsUnsecureURL(failedUrl: URL) -> Bool {
        let nhs111Title = NSLocalizedString("NHS111Title", comment: "")
        let conditionsTitle = NSLocalizedString("ConditionsTitle", comment: "")
        let unsecureServices = [nhs111Title, conditionsTitle]
        if let foundServiceInfo = self.knownServices.findMatchingKnownServiceInfo(url: failedUrl), let title = foundServiceInfo.title {
            if (unsecureServices.contains(title)) {
                return true
            }
        }
        return false
    }
    
    func configureNavBar() {
        self.title = NSLocalizedString("CheckSymptomsTitle", comment: "")
        let navBar = self.navigationController?.navigationBar
        navBar?.isHidden = false
        navBar?.barTintColor = UIColor.init(red: 0, green: 0.369, blue: 0.722, alpha: 1)
        navBar?.tintColor = UIColor.white
        navBar?.isTranslucent = false
        navBar?.titleTextAttributes = [NSAttributedStringKey.foregroundColor : UIColor.white, NSAttributedStringKey.font : UIFont.systemFont(ofSize: 20)]
        self.navigationItem.hidesBackButton = true
        let newBackButton = UIBarButtonItem(image: UIImage(named: "backArrow"), style: UIBarButtonItemStyle.plain, target: self, action: #selector(checkForPages))
        self.navigationItem.leftBarButtonItem = newBackButton
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
    
    func showWebViewContainer() {
        self.cycleFromViewController(oldViewController: self.nativeViewController!, toViewController: self.webViewController!)
    }
    
    func showNativeViewContainer(errorMessage: ErrorMessage) {
        self.nativeViewController?.setUnavailabilityError(errorMessage: errorMessage)
        self.cycleFromViewController(oldViewController: self.webViewController!, toViewController: self.nativeViewController!)
        UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, self.nativeViewController?.errorTextView)
    }
    
    func createHomeUrlSubRequestWithPath(urlPathToAppend: String) -> String {
        let homeUrl = URL(string: config().HomeUrl)
        let url = URL(string: urlPathToAppend, relativeTo: homeUrl)?.absoluteString
        
        return url!
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
    
}

