import UIKit
import WebKit
import os.log

class WebViewController: UIViewController {
    @IBOutlet weak var webView: WKWebView!
    var webViewDelegate: WebViewDelegate?
    
    let knownServices = KnownServices(config: config())
    
    struct Properties {
        static var usingAbsoluteUri: Bool = true
    }
    
    var homeUrl = config().HomeUrl
    
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
    
    private func loadSpaPage(path: String)  {
        
        
        
        var spaPath = path.replacingOccurrences(of: homeUrl, with: "/")
        
        if(!spaPath.starts(with: "/")) {
            spaPath = "/" + spaPath;
        }
        
        let completionHandler: (Any?, Error?) -> Void = {
            (data, error) in
            if(error != nil) {    
                os_log("An error occured when attempting to navigate to the page via Vue Router. Doing a full reload.", log: OSLog.default, type: .error)
                self.webView.loadPage(url: self.homeUrl + path)
            }
        }
        
        webView.evaluateJavaScript("window.$nuxt.$router.push('\(spaPath)');", completionHandler: completionHandler)
        webViewDelegate?.viewController.showWebViewContainer()
    }
    
    func loadPage(url: String) {
        
        if(!Reachability.isConnectedToNetwork()) {
            let errorMessage = knownServices.getUnavailabilityErrorMessageForService(url: self.webView.url!)
            self.webViewDelegate?.showNativeViewContainer(errorMessage: errorMessage!)
            return
        }
        
        var urlToNavigateTo = url
        let webviewUrl = self.webView.url
        
        let urlIsValid = verifyUrl(urlString: urlToNavigateTo)
        
        if(!urlIsValid) {
            urlToNavigateTo = homeUrl + urlToNavigateTo
        }
        
        if(WebViewController.Properties.usingAbsoluteUri) {
            webView.loadPage(url: urlToNavigateTo)
        }
        else if(webviewUrl?.absoluteString.contains(homeUrl))! {
            self.loadSpaPage(path: urlToNavigateTo)
        }
        else {
            webView.loadPage(url: urlToNavigateTo)
        }
    }
    
    func verifyUrl(urlString: String?) -> Bool {
        guard let urlString = urlString,
            let url = URL(string: urlString) else {
                return false
        }
        
        return UIApplication.shared.canOpenURL(url)
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
