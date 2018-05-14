import UIKit
import WebKit
import os.log

class WebViewController: UIViewController {
    @IBOutlet weak var webView: WKWebView!
    var webViewDelegate: WebViewDelegate?
    
    func setWebViewDelegate(delegate: WebViewDelegate) {
        webView.navigationDelegate = delegate
        webView.configuration.preferences.javaScriptEnabled = true
        webViewDelegate = delegate
        webView.configuration.userContentController.add(delegate, name: "updateHeaderText")
        webView.configuration.userContentController.add(delegate, name: "onLogin")
        webView.configuration.userContentController.add(delegate, name: "onLogout")
    }
    
    func reloadWebView() {
        if((webViewDelegate!.failedUrl) != nil) {
            webView.load(URLRequest(url: webViewDelegate!.failedUrl!))
        } else {
            //potentially insert, an error has occured alert
            webView.load(URLRequest(url: URL(string: config().HomeUrl)!))
        }
    }
    
    func dismissSafariViewController() {
        webViewDelegate?.safariViewController?.dismiss(animated: true, completion: nil)
    }
}
