import UIKit
import WebKit

class UnsecureWebViewController: UIViewController {

    @IBOutlet weak var webView: WKWebView!
    var webViewDelegate: WebViewDelegate?
    
    override func viewDidLoad() {
        super.viewDidLoad()
        let homeVC = HomeViewController()
        let url = homeVC.createHomeUrlSubRequestWithPath(urlPathToAppend: config().CheckSymptomsUrlPath) + config().NhsOnlineRequiredQueryString
        self.webView.loadPage(url: url)
    }
    
    func reloadWebView() {
        if  webViewDelegate!.failedUrl != nil {
            webView.load(URLRequest(url: webViewDelegate!.failedUrl!))
        } else {
            webView.load(URLRequest(url: URL(string: config().HomeUrl)!))
        }
    }

    func setWebViewDelegate(delegate: WebViewDelegate) {
        webView.navigationDelegate = delegate
        webView.uiDelegate = delegate
        webView.configuration.preferences.javaScriptEnabled = true
        delegate.unsecureWebViewController = self
        webViewDelegate = delegate
        webView.configuration.userContentController.add(delegate, name: "updateHeaderText")
    }
}
