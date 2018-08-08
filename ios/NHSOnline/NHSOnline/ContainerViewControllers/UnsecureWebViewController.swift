import UIKit
import WebKit

class UnsecureWebViewController: UIViewController {

    @IBOutlet weak var webView: WKWebView!
    var webViewDelegate: WebViewDelegate?

    override func viewWillAppear(_ animated: Bool) {
        self.configureNavBar()
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        let homeVC = HomeViewController()
        let url = homeVC.createHomeUrlSubRequestWithPath(urlPathToAppend: config().CheckSymptomsUrlPath) + config().NhsOnlineRequiredQueryString
        self.webView.loadPage(url: url)
    }
    
    @objc func checkForPages() {
        if (self.webView.canGoBack) {
            self.webView.goBack()
        } else {
            self.navigationController?.popViewController(animated: true)
        }
    }
    func setWebViewDelegate(delegate: WebViewDelegate) {
        webView.navigationDelegate = delegate
        webView.uiDelegate = delegate
        webView.configuration.preferences.javaScriptEnabled = true
        delegate.unsecureViewController = self
        webViewDelegate = delegate
        webView.configuration.userContentController.add(delegate, name: "updateHeaderText")

    }
    
    func configureNavBar() {
        self.title = NSLocalizedString("CheckSymptomsTitle", comment: "")
        let navBar = self.navigationController?.navigationBar
        navBar?.isHidden = false
        navBar?.barTintColor = UIColor.init(red: 0, green: 0.369, blue: 0.722, alpha: 1)
        navBar?.tintColor = UIColor.white
        navBar?.isTranslucent = false
        navBar?.titleTextAttributes = [NSAttributedStringKey.foregroundColor : UIColor.white]
        self.navigationItem.hidesBackButton = true
        let newBackButton = UIBarButtonItem(image: UIImage(named: "backArrow"), style: UIBarButtonItemStyle.plain, target: self, action: #selector(checkForPages))
        self.navigationItem.leftBarButtonItem = newBackButton
    }
}
