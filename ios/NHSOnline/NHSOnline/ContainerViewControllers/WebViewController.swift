import UIKit
import WebKit
import os.log

class WebViewController: UIViewController {
    @IBOutlet weak var webView: WKWebView!
    var webViewDelegate: WebViewDelegate?
    
    func setWebViewDelegate(delegate: WebViewDelegate) {
        webView.navigationDelegate = delegate
        webView.uiDelegate = delegate
        webView.configuration.preferences.javaScriptEnabled = true
        webViewDelegate = delegate
        webView.configuration.userContentController.add(delegate, name: "updateHeaderText")
        webView.configuration.userContentController.add(delegate, name: "clearMenuBarItem")
        webView.configuration.userContentController.add(delegate, name: "onLogin")
        webView.configuration.userContentController.add(delegate, name: "onLogout")
        webView.configuration.userContentController.add(delegate, name: "checkSymptoms")
        webView.configuration.userContentController.add(delegate, name: "completeAppIntro")
    }
    
    func reloadWebView() {
        if let failedUrl = webViewDelegate!.failedUrl {
            webView.load(URLRequest(url: failedUrl))
        } else {
            webView.load(URLRequest(url: URL(string: config().HomeUrl)!))
        }
    }
    
    func dismissSafariViewController() {
        webViewDelegate?.safariViewController?.dismiss(animated: true, completion: nil)
    }
}
