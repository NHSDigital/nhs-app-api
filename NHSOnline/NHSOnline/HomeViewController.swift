import UIKit

class HomeViewController : UIViewController {
    @IBOutlet weak var tabBar: UITabBar!
    @IBOutlet weak var nativeViewContainer: UIView!
    @IBOutlet weak var webViewContainer: UIView!
    
    var webViewController: WebViewController?
    var nativeViewController: NHS111OnlineUnavailabilityViewController?
    var webViewDelegate: WebViewDelegate?
    var tabBarDelegate: TabBarDelegate?
    var pageUrl = config().BaseUrl
    let nhs111Url = config().Nhs111Url
    let webViewSegue = "webViewSegue"
    let nativeViewSegue = "nativeViewSegue"
    
    override func viewDidLoad() {
        super.viewDidLoad()
        let knownServices = KnownServices(config: config())
        webViewDelegate = WebViewDelegate(controller: self, knownServices:knownServices)
        tabBarDelegate = TabBarDelegate(controller: self)
        webViewController?.setWebViewDelegate(delegate: webViewDelegate!)
        tabBar.delegate = tabBarDelegate
        webViewController?.webView.loadPage(url: pageUrl)
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == webViewSegue {
            webViewController = segue.destination as? WebViewController
        }
    }
}
