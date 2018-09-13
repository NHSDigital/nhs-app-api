import UIKit
import SafariServices
import WebKit
import os.log

class WebViewDelegate: NSObject, WKNavigationDelegate, WKUIDelegate, WKScriptMessageHandler {
    let knownServices: KnownServices
    let viewController: HomeViewController
    let activityIndicator = UIActivityIndicatorView(activityIndicatorStyle: UIActivityIndicatorViewStyle.gray)
    let responseWaitingTime = config().ResponseWaitingTime
    var failedUrl: URL? = nil
    var nativeViewController: PageUnavailabilityViewController?
    var safariViewController: SFSafariViewController?
    var shouldHandleErrors = false
    var timer: Timer!
    var startDate: Date!
    var javascript: String!
    
    init(controller: HomeViewController, knownServices: KnownServices) {
        self.viewController = controller
        self.knownServices = knownServices
        self.activityIndicator.center = viewController.view.center
        self.viewController.view.addSubview(activityIndicator)
    }
    

    func webView(_ webView: WKWebView,
                 decidePolicyFor navigationAction: WKNavigationAction,
                 decisionHandler: @escaping (WKNavigationActionPolicy) -> Void) {

        if(!Reachability.isConnectedToNetwork()) {
            decisionHandler(.cancel)
            let urlNavigatingTo = navigationAction.request.url?.absoluteString
            showNoConnectionErrorView(urlNavigatingTo: urlNavigatingTo!, currentWebviewUrl: webView.url!)
            return
        }

        shouldHandleErrors = false

        if let url = navigationAction.request.url {

            guard navigationAction.targetFrame?.isMainFrame != false else {
                decisionHandler(.allow)
                return
            }

            self.failedUrl = url
            if knownServices.shouldURLOpenExternally(url: url) {
                decisionHandler(.cancel)
                openInSafari(url: url)
                return
            }
            if let matchingKnownService = knownServices.findMatchingKnownServiceForHostname(hostname: url.host) {
                if matchingKnownService.hasMissingQueryString(urlString: url.absoluteString) {
                    let urlString = matchingKnownService.addingMissingQueryParameters(urlString: url.absoluteString)
                    decisionHandler(.cancel)
                    webView.loadPage(url: urlString)
                    
                    return
                }
            }
            if shouldOpenInSafari(url: url) {
                decisionHandler(.cancel)
                openInSafari(url: url)
                return;
            }
        }
        
        self.updateHeaderAndNavigationMenu(url: navigationAction.request.url!)
        decisionHandler(.allow)
    }

    func showNoConnectionErrorView(urlNavigatingTo: String, currentWebviewUrl: URL) {

        self.failedUrl = URL(string: urlNavigatingTo)
        var myErrorMessage = knownServices.getServiceUnavailableErrorMessage()

        if let errorMessage = knownServices.getUnavailabilityErrorMessageForService(url: URL(string: urlNavigatingTo)!) {
            myErrorMessage = errorMessage
        }

        self.showNativeViewContainer(errorMessage: myErrorMessage)
        self.viewController.updateHeaderText(headerText: "Internet connection error")

        return
    }
        
    func webView(_ webView: WKWebView, didStartProvisionalNavigation: WKNavigation!) {
        if timer != nil {
            clearTimer()
        }
        shouldHandleErrors = true
        timer = Timer.scheduledTimer(timeInterval: responseWaitingTime, target: self, selector: #selector(pageIsNotResponding), userInfo: nil, repeats: false)
        self.activityIndicator.startAnimating()
        
    }
    
    func webView(_ webView: WKWebView, didFinish: WKNavigation!) {
        
        var isIntroPage = false
        
        let url = webView.url
        if(url?.absoluteString.contains(config().CarouselFileName))! {
            isIntroPage = true
        }

        if knownServices.shouldAllowNativeInteraction(host: webView.url?.host) || isIntroPage {
            let fileReader = FileReader();
            let webEventsJSLocation = Bundle.main.path(forResource: "WebEvents", ofType: "js")!
            javascript = fileReader.readContentFromLocation(fileLocation: webEventsJSLocation)
            webView.evaluateJavaScript(javascript!, completionHandler: nil)
        }
        self.failedUrl = nil
        self.showWebViewContainer()
    }
    
    func webView(_ webView: WKWebView, didFailProvisionalNavigation: WKNavigation!, withError: Error) {
        
        if withError._code == NSURLErrorCancelled {
            if #available(iOS 10.0, *) {
                os_log("Page navigation cancelled (user may have double tapped or tapped a different nav menu button while page was still loading): %@", log: OSLog.default, type: .info, withError.localizedDescription)
            } else {
                NSLog("Page navigation cancelled (user may have double tapped or tapped a different nav menu button while page was still loading): %@", withError.localizedDescription)
            }
            return
        }
        
        if shouldHandleErrors {
            var errorMessage: ErrorMessage? = nil
            
            if withError._domain == "NSURLErrorDomain" {
                if let info = withError._userInfo as? [String: Any] {
                    if let url = info["NSErrorFailingURLKey"] as? URL {
                        errorMessage = knownServices.getUnavailabilityErrorMessageForService(url: url)
                        self.showNativeViewContainer(errorMessage: errorMessage!)
                    }
                }
            } else {
                errorMessage = knownServices.getUnavailabilityErrorMessageForService(url: webView.url!)
                self.showNativeViewContainer(errorMessage: errorMessage!)
            }
            if #available(iOS 10.0, *) {
                os_log("Failed to load the page with error: %@", log: OSLog.default, type: .error, withError.localizedDescription)
            } else {
                NSLog("Failed to load the page with error: %@", withError.localizedDescription)
            }
        }
    }
    
    func webView(_ webView: WKWebView, createWebViewWith configuration: WKWebViewConfiguration, for navigationAction: WKNavigationAction, windowFeatures: WKWindowFeatures) -> WKWebView? {
        
        if navigationAction.targetFrame == nil {
            webView.load(navigationAction.request)
            self.updateHeaderAndNavigationMenu(url: navigationAction.request.url!)
        }
        
        return nil
    }
    
    func shouldOpenInSafari(url: URL) -> Bool {
        let currentHost = url.host
        let knownHosts = self.knownServices.getAllKnownHosts()
       
        
        if(url.absoluteString.contains(config().CarouselFileName)) {
            return false
        }
        
        for host in knownHosts {
            if (host == currentHost) {
                return false
            }
        }
        
        return true
    }
    
    func openInSafari(url: URL) {
        self.safariViewController = SFSafariViewController(url: url)
        self.viewController.present(safariViewController!, animated: true, completion: nil)
    }
    
    func userContentController(_ userContentController: WKUserContentController, didReceive message: WKScriptMessage) {
        
        var shouldAllowNativeInteraction = false;
        
        let url = self.viewController.webViewController?.webView.url;
        if(url?.absoluteString.contains("appintro"))!{
            shouldAllowNativeInteraction = true
        }
        
        if  knownServices.shouldAllowNativeInteraction(host: message.frameInfo.securityOrigin.host) || shouldAllowNativeInteraction {
            if (message.name == "onLogin") {
                viewController.setVisibilityOfHeaderAndMenuBars(visible: true)
                WebViewController.Properties.usingAbsoluteUri = false
            }
            
            if (message.name == "onLogout") {
                viewController.setVisibilityOfHeaderAndMenuBars(visible: false)
                WebViewController.Properties.usingAbsoluteUri = true
            }
            
            if (message.name == "updateHeaderText") {

                if(!Reachability.isConnectedToNetwork()) {
                    showNoConnectionErrorView(urlNavigatingTo: (url?.absoluteString)!, currentWebviewUrl: url!)
                    return
                }

                viewController.updateHeaderText(headerText: String(describing: message.body))
            }
            if (message.name == "postNdopToken") {
                callPostNdopToken(token: String(describing: message.body))
            }
            if (message.name == "clearMenuBarItem") {
                clearMenuBarItem()
            }
            if (message.name == "checkSymptoms") {
                checkSymptoms()
            }
            if (message.name == "completeAppIntro") {
                
                let defaults = UserDefaults.standard
                defaults.set(false, forKey: config().IsFirstTimeOpened)
                
                self.viewController.webViewController?.webView.load(URLRequest(url: URL(string: config().HomeUrl)!))
            }
        }
    }
    
    func callPostNdopToken(token: String?) {
        self.viewController.webViewController?.postNdopToken(token: token!)
    }
    
    func clearMenuBarItem() {
        self.viewController.tabBar.selectedItem = nil
    }
    
    func checkSymptoms() {
        self.viewController.callCheckSymptoms()
    }
    
    @objc func pageIsNotResponding() {
        if(self.viewController.webViewController?.webView.isLoading)! {
            if #available(iOS 10.0, *) {
                os_log("Page is not responding for a long time, loading stoped.", log: OSLog.default, type: .error)
            } else {
                NSLog("Page is not responding for a long time, loading stoped.")
            }
            self.viewController.webViewController?.webView.stopLoading()
            let url = self.viewController.webViewController?.webView.url
            if let knownService = knownServices.findMatchingKnownServiceForHostname(hostname: url?.host){
                self.showNativeViewContainer(errorMessage: knownService.serviceErrorMessage)
            } else {
                self.showNativeViewContainer(errorMessage: knownServices.getServiceUnavailableErrorMessage())
            }
        }
    }
    
    private func updateHeaderAndNavigationMenu(url: URL?) {
        let service: KnownService
        if let tabBarDelegate = self.viewController.tabBarDelegate {
                if let internalService = knownServices.findMatchingInternalServiceForURL(url: url) {
                    service = internalService
                } else if let knownService = knownServices.findMatchingKnownServiceForURL(url: url) {
                    service = knownService
                } else {
                    return
                }
                switch service.service {
                case .NHS_111, .SYMPTOMS:
                    tabBarDelegate.selectMenu(menu: .Symptoms)
                    break
                case .APPOINTMENTS:
                    tabBarDelegate.selectMenu(menu: .Appointments)
                    break
                case .PRESCRIPTIONS:
                    tabBarDelegate.selectMenu(menu: .Prescriptions)
                    break
                case .MY_RECORD:
                    tabBarDelegate.selectMenu(menu: .MyRecord)
                    break
                case .ORGAN_DONATION:
                    tabBarDelegate.selectMenu(menu: .More)
                default : break
                }

                if (service.serviceTitle != "") {
                    viewController.updateHeaderText(headerText: service.serviceTitle)
                }
            }
    }
    
    private func showWebViewContainer() {
        clearTimer()
        self.activityIndicator.stopAnimating()
        self.viewController.showWebViewContainer()
    }
    
    func showNativeViewContainer(errorMessage: ErrorMessage) {
        clearTimer()
        self.activityIndicator.stopAnimating()
        self.viewController.showNativeViewContainer(errorMessage: errorMessage)
        
    }
    
    private func clearTimer() {
        if self.timer != nil {
            self.timer.invalidate()
            self.timer = nil
        }
    }
}
