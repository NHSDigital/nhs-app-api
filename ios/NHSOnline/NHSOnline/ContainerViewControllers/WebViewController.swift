import UIKit
import WebKit
import os.log

class WebViewController: UIViewController, WKUIDelegate {
    var webViewDelegate: WebViewDelegate?
    public var webView: WKWebView!
    
    override func loadView() {
        super.loadView()
        
        let fileReader = FileReader();
        let webEventsJSLocation = Bundle.main.path(forResource: "WebEvents", ofType: "js")!
        let javascript = fileReader.readContentFromLocation(fileLocation: webEventsJSLocation)
        
        let contentController = WKUserContentController()
        let script = WKUserScript(source: javascript, injectionTime: .atDocumentStart, forMainFrameOnly: true)
        contentController.addUserScript(script)
        
        let config = WKWebViewConfiguration()
        config.userContentController = contentController
        config.suppressesIncrementalRendering = true
        
        webView = WKWebView(frame: .zero, configuration: config)
        webView.uiDelegate = self
        view = webView
    }
    
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
        webView.configuration.userContentController.add(delegate, name: "postNdopToken")
        webView.configuration.userContentController.add(delegate, name: "updateHeaderText")
        webView.configuration.userContentController.add(delegate, name: "clearMenuBarItem")
        webView.configuration.userContentController.add(delegate, name: "setMenuBarItem")
        webView.configuration.userContentController.add(delegate, name: "onLogin")
        webView.configuration.userContentController.add(delegate, name: "onLogout")
        webView.configuration.userContentController.add(delegate, name: "showHeader")
        webView.configuration.userContentController.add(delegate, name: "showHeaderSlim")
        webView.configuration.userContentController.add(delegate, name: "hideHeaderSlim")
        webView.configuration.userContentController.add(delegate, name: "resetPageFocus")
        webView.configuration.userContentController.add(delegate, name: "hideHeader")
        webView.configuration.userContentController.add(delegate, name: "hideMenuBar")
        webView.configuration.userContentController.add(delegate, name: "hideWhiteScreen")
        webView.configuration.userContentController.add(delegate, name: "completeAppIntro")
        webView.configuration.userContentController.add(delegate, name: "onSessionExpiring")
        webView.configuration.userContentController.add(delegate, name: "goToBiometrics")
        webView.configuration.userContentController.add(delegate, name: "focusElement")
        webView.configuration.userContentController.add(delegate, name: "fetchNativeAppVersion")
    }
    
    private func loadSpaPage(path: String)  {
        
        var spaPath = path.replacingOccurrences(of: homeUrl, with: "/")
        
        if(!spaPath.starts(with: "/")) {
            spaPath = "/" + spaPath;
        }
        
        let completionHandler: (Any?, Error?) -> Void = {
            (data, error) in
            if(error != nil) {
                if #available(iOS 10.0, *) {
                    os_log("An error occured when attempting to navigate to the page via Vue Router. Doing a full reload.", log: OSLog.default, type: .error)
                } else {
                    NSLog("An error occured when attempting to navigate to the page via Vue Router. Doing a full reload.")
                }
                let urlIsValid = self.verifyUrl(urlString: path)
                if(urlIsValid){
                    self.webView.loadPage(url: path)
                } else {
                    self.webView.loadPage(url: self.homeUrl + path)
                }
            }
        }
        webView.evaluateJavaScript("window.$nuxt.$router.push('\(spaPath)');", completionHandler: completionHandler)
        webViewDelegate?.viewController.showWebViewContainer()
    }
    
    private func shouldLoadUrlAsSpaPage(urlToNavigateTo:String) -> Bool{
        guard knownServices.isSameHostAsHomeUrl(url: webView.url) else {
            return false
        }
        return knownServices.isSameHostAsHomeUrl(url: URL(string: urlToNavigateTo))
    }
    
    func loadPage(url: String) {
        self.webViewDelegate?.failedUrl = URL(string: url)
        self.webViewDelegate?.clearTimer()
        self.webViewDelegate?.activityIndicator.stopAnimating()
        
        if(!Reachability.isConnectedToNetwork()) {
            webViewDelegate?.showNativeViewContainerWithError(knownServices.getNoInternetConnectionErrorMessage())
            return
        }
        
        var urlToNavigateTo = url        
        let urlIsValid = verifyUrl(urlString: urlToNavigateTo)
        self.webViewDelegate?.updateHeaderAndNavigationMenu(url: URL(string: url))
        
        if(!urlIsValid) {
            urlToNavigateTo = homeUrl + urlToNavigateTo
        }
        
        if(WebViewController.Properties.usingAbsoluteUri || knownServices.isCIDRedirectUrl(urlString: urlToNavigateTo)) {
            webView.loadPage(url: urlToNavigateTo)
        }
        else if(shouldLoadUrlAsSpaPage(urlToNavigateTo: urlToNavigateTo)) {
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
            let vc = webViewDelegate?.viewController
            vc!.tabBar.selectedItem = nil
        }
    }
    
    func postNdopToken(token: String) {
        var urlRequest = URLRequest(url: URL(string: config().DataPreferencesURL)!)
        urlRequest.setValue("application/x-www-form-urlencoded", forHTTPHeaderField: "Content-Type")
        urlRequest.httpMethod = "POST"
        urlRequest.httpBody = ("&token=" + token).data(using: String.Encoding.utf8)
        webView.load(urlRequest)
    }
    
    func dismissSafariViewController() {
        webViewDelegate?.safariViewController?.dismiss(animated: true, completion: nil)
    }
}
