import UIKit

class HomeViewController : UIViewController {
    private var defaultHeaderBarHeight:CGFloat = 94
    private var defaultTabBarHeight:CGFloat = 49
    
    @IBOutlet weak var headerBar: HeaderBar!
    @IBOutlet weak var tabBar: UITabBar!
    @IBOutlet weak var nativeViewContainer: UIView!
    @IBOutlet weak var webViewContainer: UIView!
    
    @IBOutlet weak var headerHeightConstraint: NSLayoutConstraint!
    @IBOutlet weak var tabBarHeightConstraint: NSLayoutConstraint!
    
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
        setVisibilityOfHeaderAndMenuBars(visible: false)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == webViewSegue {
            webViewController = segue.destination as? WebViewController
        }
    }
    
    func updateHeaderText(headerText: String?) {
        if ((headerText) != nil) {
            self.headerBar.headerTitle.text = headerText
        }
    }
    
    func setVisibilityOfHeaderAndMenuBars(visible:Bool) {
        UIView.animate(withDuration: 0.3, animations: {
            if(visible) {
                self.headerHeightConstraint.constant = self.defaultHeaderBarHeight
                self.tabBarHeightConstraint.constant = self.defaultTabBarHeight
            } else {
                self.headerHeightConstraint.constant = 0
                self.tabBarHeightConstraint.constant = 0
            }
            self.headerBar.isHidden = !visible
            self.tabBar.isHidden = !visible
            self.tabBar.layoutIfNeeded()
            self.headerBar.layoutIfNeeded()
        })
    }
}

