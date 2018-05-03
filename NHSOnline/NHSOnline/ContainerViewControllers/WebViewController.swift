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
        webView.configuration.userContentController.add(delegate, name: "loggedIn")
    }

    func loadUrl(url: String) {
        webViewDelegate?.stopErrorsHandling()
        webView.loadPage(url: url)
    }
    
    func reloadWebView()
    {
        webView.load(URLRequest(url: webViewDelegate!.failedUrl!))
    }
    
    func dismissSafariViewController() {
        webViewDelegate?.safariViewController?.dismiss(animated: true, completion: nil)
    }
}

