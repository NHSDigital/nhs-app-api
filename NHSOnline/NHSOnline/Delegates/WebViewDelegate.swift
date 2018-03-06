import UIKit
import WebKit
import os.log

class WebViewDelegate: NSObject, WKNavigationDelegate {
    let viewController: BaseTabBarViewController
    let activityIndicator = UIActivityIndicatorView(activityIndicatorStyle: UIActivityIndicatorViewStyle.gray)
    let responseWaitingTime = config().ResponseWaitingTime
    var timer: Timer!
    var startDate: Date!
    
    init(controller: BaseTabBarViewController) {
        viewController = controller
        activityIndicator.center = viewController.view.center
        viewController.view.addSubview(activityIndicator)
    }

    func webView(_ webView: WKWebView, didStartProvisionalNavigation: WKNavigation!) {
        timer = Timer.scheduledTimer(timeInterval: responseWaitingTime, target: self, selector: #selector(pageIsNotResponding), userInfo: nil, repeats: false)
        self.activityIndicator.startAnimating()
    }
    
    func webView(_ webView: WKWebView, didFinish: WKNavigation!) {
        self.showWebViewContainer()
    }
    
    func webView(_ webView: WKWebView, didFailProvisionalNavigation: WKNavigation!, withError: Error) {
        os_log("Failed to load the page with error: %@", log: OSLog.default, type: .error, withError.localizedDescription)
        self.showNativeViewContainer()
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
        self.viewController.WebViewContainer.alpha = 1
        self.viewController.NativeViewContainer.alpha = 0
    }
    
    func showNativeViewContainer() {
        self.timer.invalidate()
        self.activityIndicator.stopAnimating()
        self.viewController.WebViewContainer.alpha = 0
        self.viewController.NativeViewContainer.alpha = 1
    }
}
