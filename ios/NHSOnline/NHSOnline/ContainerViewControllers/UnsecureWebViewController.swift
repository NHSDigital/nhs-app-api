import UIKit
import WebKit

class UnsecureWebViewController: UIViewController, WKUIDelegate {
    var webViewDelegate: UnsecureWebViewDelegate?
    public var webView: WKWebView!
    
    override func loadView() {
        super.loadView()
        let webConfiguration = WKWebViewConfiguration()
        webView = WKWebView(frame: .zero, configuration: webConfiguration)
        webView.uiDelegate = self
        view = webView
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        let homeVC = HomeViewController()
        let url = homeVC.createHomeUrlSubRequestWithPath(urlPathToAppend: config().CheckSymptomsUrlPath) + config().NhsOnlineRequiredQueryString
        self.webView.loadPage(url: url)
        webViewDelegate?.failedUrl = nil
    }
    
    func reloadWebView() {
        if  webViewDelegate!.failedUrl != nil {
            webView.load(URLRequest(url: webViewDelegate!.failedUrl!))
        } else {
            webView.load(URLRequest(url: URL(string: config().HomeUrl)!))
        }
    }

    func setWebViewDelegate(delegate: UnsecureWebViewDelegate) {
        webView.navigationDelegate = delegate
        webView.uiDelegate = delegate
        webView.configuration.preferences.javaScriptEnabled = true
        delegate.unsecureWebViewController = self
        webViewDelegate = delegate
        webView.configuration.userContentController.add(delegate, name: "updateHeaderText")
    }
}
