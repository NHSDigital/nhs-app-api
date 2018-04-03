import UIKit
import WebKit
import os.log

class WebViewDelegate: NSObject, WKNavigationDelegate {
    
    let viewController: HomeViewController
    let safari: Safari
    let webViewHosts =  [URL(string: config().BaseUrl)?.host,
                         URL(string: config().Nhs111Url)?.host,
                         URL(string: config().OrganDonationUrl)?.host]
    let activityIndicator = UIActivityIndicatorView(activityIndicatorStyle: UIActivityIndicatorViewStyle.gray)
    let responseWaitingTime = config().ResponseWaitingTime
    var shouldHandleErrors = false
    var timer: Timer!
    var startDate: Date!
    
    init(controller: HomeViewController) {
        viewController = controller
        safari = Safari()
        activityIndicator.center = viewController.view.center
        viewController.view.addSubview(activityIndicator)
    }
    
    func stopErrorsHandling() {
        shouldHandleErrors = false
    }
    
    func webView(_ webView: WKWebView,
                          decidePolicyFor navigationAction: WKNavigationAction,
                          decisionHandler: @escaping (WKNavigationActionPolicy) -> Void) {
        
        if let url = navigationAction.request.url {
            if shouldOpenInSafari(url: url) {
                decisionHandler(.cancel)
                safari.open(url: url)
                return;
            }
        }
        
        decisionHandler(.allow)
    }
    
    func webView(_ webView: WKWebView, didStartProvisionalNavigation: WKNavigation!) {
        shouldHandleErrors = true
        timer = Timer.scheduledTimer(timeInterval: responseWaitingTime, target: self, selector: #selector(pageIsNotResponding), userInfo: nil, repeats: false)
        self.activityIndicator.startAnimating()
    }
    
    func webView(_ webView: WKWebView, didFinish: WKNavigation!) {
        self.showWebViewContainer()
    }
    
    func webView(_ webView: WKWebView, didFailProvisionalNavigation: WKNavigation!, withError: Error) {
        if(shouldHandleErrors) {
            os_log("Failed to load the page with error: %@", log: OSLog.default, type: .error, withError.localizedDescription)
            self.showNativeViewContainer()
        }
    }
    
    func shouldOpenInSafari(url: URL) -> Bool {
        let currentHost = url.host
        
        for host in webViewHosts {
            if (host == currentHost) {
                return false;
            }
        }
        
        return true;
    }
    
    @objc func pageIsNotResponding() {
        if(self.viewController.webViewController?.webView.isLoading)! {
            os_log("Page is not responding for a long time, loading stoped.", log: OSLog.default, type: .error)
            self.viewController.webViewController?.webView.stopLoading()
            self.showNativeViewContainer()
        }
    }
    
    private func showWebViewContainer() {
        self.timer.invalidate()
        self.activityIndicator.stopAnimating()
        self.viewController.webViewContainer.alpha = 1
        self.viewController.nativeViewContainer.alpha = 0
    }
    
    private func showNativeViewContainer() {
        self.timer.invalidate()
        self.activityIndicator.stopAnimating()
        self.viewController.webViewContainer.alpha = 0
        self.viewController.nativeViewContainer.alpha = 1
    }
}
