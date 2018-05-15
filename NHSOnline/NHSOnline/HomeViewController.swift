import UIKit

class HomeViewController : UIViewController {
    private let showConstraintPriority = UILayoutPriority.init(rawValue: 900)
    private let hideConstraintPriority = UILayoutPriority.init(rawValue: 850)
    
    @IBOutlet weak var headerBar: HeaderBar!
    @IBOutlet weak var tabBar: UITabBar!
    @IBOutlet weak var nativeViewContainer: UIView!
    @IBOutlet weak var webViewContainer: UIView!
    
    @IBOutlet weak var webviewHeaderTopConstraint: NSLayoutConstraint!
    @IBOutlet weak var nativeViewHeaderTopConstraint: NSLayoutConstraint!
    @IBOutlet weak var webviewNavMenuBottomConstraint: NSLayoutConstraint!
    @IBOutlet weak var nativeViewNavMenuBottomConstraint: NSLayoutConstraint!
    
    let webViewSegue = "webViewSegue"
    let nativeViewSegue = "nativeViewSegue"
    let knownServices = KnownServices(config: config())
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
        webViewController?.setWebViewDelegate(delegate: webViewDelegate!)
        tabBar.delegate = tabBarDelegate
        webViewController?.webView.loadPage(url: pageUrl)
        let tap = UITapGestureRecognizer(target: self, action: #selector(goHome(tapGestureRecognizer:)))
        headerBar.NHSHomeLogo.addGestureRecognizer(tap)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == webViewSegue {
            webViewController = segue.destination as? WebViewController
        } else if segue.identifier == nativeViewSegue {
            nativeViewController = (segue.destination as! PageUnavailabilityViewController)
        }
    }
    
    func updateHeaderText(headerText: String?) {
        if (headerText != nil) {
            self.headerBar.headerTitle.text = headerText
        }
    }
    
    func setupNhsLogo() {
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(goHome(tapGestureRecognizer:)))
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
            
            if(visible) {
                constraintPriority = self.showConstraintPriority
            } else {
                constraintPriority = self.hideConstraintPriority
            }
            
            self.webviewHeaderTopConstraint.priority = constraintPriority
            self.nativeViewHeaderTopConstraint.priority = constraintPriority
            self.webviewNavMenuBottomConstraint.priority = constraintPriority
            self.nativeViewNavMenuBottomConstraint.priority = constraintPriority
            self.headerBar.isHidden = !visible
            self.tabBar.isHidden = !visible
        })
    }
    
    func createHomeUrlSubRequestWithPath(urlPathToAppend: String) -> String {
        let homeUrl = URL(string: config().HomeUrl)
        let url = URL(string: urlPathToAppend, relativeTo: homeUrl)?.absoluteString
        
        return url!
    }
    
    @objc func selectMyAccount(sender : UITapGestureRecognizer) {
        self.headerBar.headerTitle.text = NSLocalizedString("MyAccountTitle", comment: "")
        self.pageUrl = createHomeUrlSubRequestWithPath(urlPathToAppend: config().MyAccountUrlPath)
        webViewController?.webView.loadPage(url: self.pageUrl)
        self.tabBar.selectedItem = nil
    }
    
    @objc func goHome(tapGestureRecognizer: UITapGestureRecognizer) {
        self.pageUrl = config().HomeUrl
        self.webViewController?.webView.loadPage(url: self.pageUrl)
        self.tabBar.selectedItem = nil
    }
}

