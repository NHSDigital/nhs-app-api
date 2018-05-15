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
    
    var webViewController: WebViewController?
    var nativeViewController: PageUnavailabilityViewController?
    var webViewDelegate: WebViewDelegate?
    var tabBarDelegate: TabBarDelegate?
    var pageUrl = config().HomeUrl
    let webViewSegue = "webViewSegue"
    let nativeViewSegue = "nativeViewSegue"
    let knownServices = KnownServices(config: config())
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        webViewDelegate = WebViewDelegate(controller: self, knownServices:knownServices)
        
        tabBarDelegate = TabBarDelegate(controller: self)
        webViewController?.setWebViewDelegate(delegate: webViewDelegate!)
        tabBar.delegate = tabBarDelegate
        webViewController?.webView.loadPage(url: pageUrl)
        let tap = UITapGestureRecognizer(target: self, action: #selector(goHome(tapGestureRecognizer:)))
        headerBar.NHSHomeLogo.addGestureRecognizer(tap)
    }
    @objc func goHome(tapGestureRecognizer: UITapGestureRecognizer) {
        self.pageUrl = config().HomeUrl
        self.tabBar.selectedItem = nil
        self.webViewController?.loadUrl(url: self.pageUrl)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == webViewSegue {
            webViewController = segue.destination as? WebViewController
        } else if segue.identifier == nativeViewSegue {
            nativeViewController = (segue.destination as! PageUnavailabilityViewController)
        }
    }
    
    func updateHeaderText(headerText: String?) {
        if ((headerText) != nil) {
            self.headerBar.headerTitle.text = headerText
        }
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
}

