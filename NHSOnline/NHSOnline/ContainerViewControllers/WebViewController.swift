import UIKit
import WebKit
import os.log

class WebViewController: UIViewController {
    @IBOutlet weak var webView: WKWebView!
    var webViewDelegate: WebViewDelegate?

    func setWebViewDelegate(delegate: WebViewDelegate) {
        webView.navigationDelegate = delegate
        webViewDelegate = delegate
    }

    func loadUrl(url: String) {
        webViewDelegate?.stopErrorsHandling()
        webView.loadPage(url: url)
    }
    
    func dismissSafariViewController() {
        webViewDelegate?.safariViewController?.dismiss(animated: true, completion: nil)
    }
}
