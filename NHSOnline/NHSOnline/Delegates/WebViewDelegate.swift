import UIKit
import WebKit
import os.log

class WebViewDelegate: NSObject, WKNavigationDelegate {
    let viewController: HomeViewController
    let activityIndicator = UIActivityIndicatorView(activityIndicatorStyle: UIActivityIndicatorViewStyle.gray)
    let responseWaitingTime = config().ResponseWaitingTime
    var shouldHandleErrors = false
    var timer: Timer!
    var startDate: Date!
    
    init(controller: HomeViewController) {
        viewController = controller
        activityIndicator.center = viewController.view.center
        viewController.view.addSubview(activityIndicator)
    }
    
    func stopErrorsHandling() {
        shouldHandleErrors = false
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
    
    @objc func pageIsNotResponding() {
        if(self.viewController.webViewController?.webView.isLoading)! {
            os_log("Page is not responding for a long time, loading stoped.", log: OSLog.default, type: .error)
            self.viewController.webViewController?.webView.stopLoading()
            self.showNativeViewContainer()
        }
    }
    
    func showWebViewContainer() {
        self.timer.invalidate()
        self.activityIndicator.stopAnimating()
        self.viewController.webViewContainer.alpha = 1
        self.viewController.nativeViewContainer.alpha = 0
    }
    
    func showNativeViewContainer() {
        self.timer.invalidate()
        self.activityIndicator.stopAnimating()
        self.viewController.webViewContainer.alpha = 0
        self.viewController.nativeViewContainer.alpha = 1
    }
}
