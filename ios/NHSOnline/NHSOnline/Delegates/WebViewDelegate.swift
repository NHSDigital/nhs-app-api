import UIKit
import SafariServices
import WebKit
import os.log

class WebViewDelegate: NSObject, WKNavigationDelegate, WKUIDelegate, WKScriptMessageHandler, SFSafariViewControllerDelegate {
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
    var webAppInterface: WebAppInterface
    var schemeHandlers: SchemeHandlers
    var badResponse: Bool = false
    
    init(controller: HomeViewController, knownServices: KnownServices, webAppInterface: WebAppInterface) {
        self.viewController = controller
        self.knownServices = knownServices
        self.activityIndicator.center = viewController.view.center
        self.viewController.view.addSubview(activityIndicator)
        self.webAppInterface = webAppInterface
        self.schemeHandlers = SchemeHandlers()
        self.schemeHandlers.registerHandler(handler: MailToSchemeHandler())
    }

    func webView(_ webView: WKWebView, decidePolicyFor navigationResponse: WKNavigationResponse, decisionHandler: @escaping (WKNavigationResponsePolicy) -> Void) {
        if let response = navigationResponse.response as? HTTPURLResponse {
            
            let url = self.viewController.webViewController?.webView.url
            let baseUrl = URL(string: config().HomeUrl)
            let statusCode = response.statusCode
            
            if ((url?.host==baseUrl?.host) && statusCode >= 400) {
                badResponse = true
                return decisionHandler(.cancel)
            } else {
                badResponse = false
                return decisionHandler(.allow)
            }
        }
    }
    
    func webView(_ webView: WKWebView,
                 decidePolicyFor navigationAction: WKNavigationAction,
                 decisionHandler: @escaping (WKNavigationActionPolicy) -> Void) {
        
        shouldHandleErrors = false
        
        if let initialUrl = navigationAction.request.url {
            
            let url = ensureSupportedScheme(initialUrl)
            
            if(schemeHandlers.handleUrl(url: url)) {
                decisionHandler(.cancel)
                return
            }            
            
            if(url.absoluteString == config().HomeUrl + config().FidoLoginErrorPath) {
                viewController.showBiometricSessionError()
                stopActivityIndicator()
                decisionHandler(.cancel)
                return
            }
            
            guard navigationAction.targetFrame?.isMainFrame != false else {
                decisionHandler(.allow)
                return
            }
            
            if (!knownServices.shouldURLOpenExternally( url)) {
                self.failedUrl = url
            }
            
            if(!Reachability.isConnectedToNetwork()) {
                decisionHandler(.cancel)
                self.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
                return
            }

            if knownServices.shouldURLOpenExternally( url) {
                decisionHandler(.cancel)
                openInSafari(url: url)
                return
            }
            
            let matchingKnownService = knownServices.findMatchingKnownServiceForHostname(hostname: url.host)
            
            if(matchingKnownService != nil) {
                if(matchingKnownService!.hasMissingQueryString(urlString: url.absoluteString)) {
                    let urlString = matchingKnownService!.addingMissingQueryParameters(urlString: url.absoluteString)
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
    
    func webView(_ webView: WKWebView, didStartProvisionalNavigation: WKNavigation!) {
        if timer != nil {
            clearTimer()
            stopActivityIndicator()
        }
        shouldHandleErrors = true
        timer = Timer.scheduledTimer(timeInterval: responseWaitingTime, target: self, selector: #selector(pageIsNotResponding), userInfo: nil, repeats: false)
        
        checkPageLoadOriginAndStartActivityIndicator()
    }
    
    func webView(_ webView: WKWebView, didFinish: WKNavigation!) {
        self.showWebViewContainer()
        if(webView.url?.absoluteString == config().HomeUrl + config().NhsOnlineRequiredQueryString) {
            self.viewController.setVisibilityOfHeaderAndMenuBars(visible: true, isSlim: false)
        }
        
        if(webView.url!.absoluteString.contains(config().CidUrlSuffix)) {
            viewController.updateHeaderText(headerText: "Log in to the NHS App", accessibilityLabel: "Login using Patient ID")
            viewController.setVisibilityOfHeaderAndMenuBars(visible: true, isSlim: true)
        }
        
        UIApplication.shared.keyWindow?.viewWithTag(2)?.removeFromSuperview()
        
        if !self.knownServices.isSameHostAsHomeUrl(url: webView.url) && !self.viewController.headerBar.isHidden {
            self.viewController.resetFocusAndAnnouncePageTitle(pageTitle: webView.title)
        }
    }
    
    func webView(_ webView: WKWebView, didFailProvisionalNavigation: WKNavigation!, withError: Error) {
        if withError._code == NSURLErrorCancelled {
            if #available(iOS 10.0, *) {
                os_log("Page navigation cancelled (user may have double tapped or tapped a different nav menu button while page was still loading): %@", log: OSLog.default, type: .info, withError.localizedDescription)
            } else {
                NSLog("Page navigation cancelled (user may have double tapped or tapped a different nav menu button while page was still loading): %@", withError.localizedDescription)
            }
            stopActivityIndicator()
            viewController.applicationState.unBlock()
            return
        }
        
        if shouldHandleErrors {
            if badResponse {
                return self.showNativeViewContainerWithError(ErrorMessage(.ServiceUnavailable))
            }
            
            if withError._domain == "NSURLErrorDomain" {
                if let info = withError._userInfo as? [String: Any] {
                    if let url = info["NSErrorFailingURLKey"] as? URL {                        self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(url))
                    }
                }
            } else {
                self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(webView.url))
            }
            
            if #available(iOS 10.0, *) {
                os_log("Failed to load the page with error: %@", log: OSLog.default, type: .error, withError.localizedDescription)
            } else {
                NSLog("Failed to load the page with error: %@", withError.localizedDescription)
            }
        }
        stopActivityIndicator()
        viewController.applicationState.unBlock()
    }
    
    func webView(_ webView: WKWebView, createWebViewWith configuration: WKWebViewConfiguration, for navigationAction: WKNavigationAction, windowFeatures: WKWindowFeatures) -> WKWebView? {
        
        if navigationAction.targetFrame == nil {
            webView.load(navigationAction.request)
            self.updateHeaderAndNavigationMenu(url: navigationAction.request.url!)
        }
        
        return nil
    }
    
    func shouldOpenInSafari(url: URL) -> Bool {
        
        let nativeWebViewUrlParts = ["login",
                          config().CidUrlSuffix]

        if(url.absoluteString.containsAnyOf(nativeWebViewUrlParts)) {
            return false
        }
        
        if let _ = knownServices.findMatchingKnownServiceForHostname(hostname: url.host) {
            return false
        }
        
        return true
    }
    
    func openInSafari(url: URL) {
        self.safariViewController = SFSafariViewController(url: url)
        self.safariViewController?.delegate = self
        self.viewController.present(safariViewController!, animated: true, completion: nil)
    }
    
    func safariViewControllerDidFinish(_ controller: SFSafariViewController) {
        self.viewController.delayedBiometricsStart(0.1)
    }
    
    func userContentController(_ userContentController: WKUserContentController, didReceive message: WKScriptMessage) {

        var shouldAllowNativeInteraction = false;
        
        let url = self.viewController.webViewController?.webView.url;
        if(url?.absoluteString.contains("throttling"))!{
            shouldAllowNativeInteraction = true
        }
        
        if  knownServices.shouldAllowNativeInteraction(host: message.frameInfo.securityOrigin.host) || shouldAllowNativeInteraction {
            switch message.name {
            case "areNotificationsEnabled":
                viewController.areNotificationsEnabled()
                break;
            case "attemptBiometricLogin":
                viewController.delayedBiometricsStart(0.3)
                break
            case "clearMenuBarItem":
                clearMenuBarItem()
                break
            case "fetchNativeAppVersion":
                self.viewController.setupAppVersion()
                break
            case "goToLoginOptions":
                goToLoginOptions()
                break
            case "hideHeader":
                viewController.setVisibilityOfHeaderAndMenuBars(visible: false, isSlim: false)
                break
            case "hideWhiteScreen":
                UIApplication.shared.keyWindow?.viewWithTag(2)?.removeFromSuperview()
                break
            case "hideHeaderSlim":
                viewController.setVisibilityOfHeaderAndMenuBars(visible: false, isSlim: true)
                break
            case "hideMenuBar":
                viewController.setVisibilityOfHeaderAndMenuBars(visible: false, isSlim: false)
                break
            case "onLogin":
                WebViewController.Properties.usingAbsoluteUri = false
                break
            case "onLogout":
                WebViewController.Properties.usingAbsoluteUri = true
                webAppInterface.onLogout()
                break
            case "onSessionExpiring":
                var sessionDuration : Int? = message.body as? Int
                if sessionDuration == nil {
                    sessionDuration = 20
                }
                viewController.displayExtendSessionDialogue(sessionDuration: sessionDuration!)
                break
            case "openAppSettings":
                if #available(iOS 10.0, *) {
                    UIApplication.shared.open(URL(string: UIApplicationOpenSettingsURLString)!)
                }
                break
            case "pageLoadComplete":
                viewController.applicationState.unBlock()
                break
            case "requestPnsToken":
                let trigger: String = message.body as! String
                viewController.registerForPushNotifications(trigger: trigger)
                break
            case "resetPageFocus":
                viewController.headerBar.setFocusToNhsLogoForA11y()
                break
            case "setHelpUrl":
                setHelpUrl(url: message.body as? String ?? config().HelpURL)
                break
            case "setRetryPath":
                setRetryUrl(path: message.body as! String)
                break;
            case "setMenuBarItem":
                setMenuBarItem(index: message.body as? Int ?? 0)
                break
            case "showHeader":
                viewController.setVisibilityOfHeaderAndMenuBars(visible: true, isSlim: false)
                break
            case "showHeaderSlim":
                viewController.setVisibilityOfHeaderAndMenuBars(visible: true, isSlim: true)
                break
            case "updateHeaderText":
                if(!Reachability.isConnectedToNetwork()) {
                    self.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
                    return
                }
                viewController.updateHeaderText(headerText: String(describing: message.body))
                break
            case "startDownload":
                viewController.downloadFile(messageBody: String(describing: message.body))
                break
            default:
                break
            }
        }
    }
    
    func goToLoginOptions() {
        CookieHandler().readAccessTokenFromCookie()
        viewController.showBiometricViewContainer()
    }
    
    func setMenuBarItem(index: Int){
        let tabBar = self.viewController.tabBar!
        if(index >= 0 && index < tabBar.items!.count){
            tabBar.selectedItem = tabBar.items![index]
            viewController.selectedTab = index
        }
    }
    
    func setHelpUrl(url: String){
        UserDefaults.standard.set(url, forKey: "HelpUrl" )
    }
    
    func setRetryUrl(path: String){
        if(!path.isEmpty){
            var suffix = path
            suffix.remove(at: suffix.startIndex)
            failedUrl = URL(string: config().HomeUrl + suffix)
        }
    }
    
    func ensureSupportedScheme(_ url: URL) -> URL {
        if(url.scheme==config().AppScheme) {
            self.viewController.setVisibilityOfHeaderAndMenuBars(visible: false, isSlim: true)
            return URL(string: url.absoluteString.replacingOccurrences(of: config().AppScheme + ":", with: config().BaseScheme + ":"))!
        }
        return url
    }
    
    func clearMenuBarItem() {
        self.viewController.tabBar.selectedItem = nil
        self.viewController.selectedTab = nil
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
            let baseUrl = URL(string: config().HomeUrl)
            if (url?.host==baseUrl?.host){ self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(url))
            }
        }
        stopActivityIndicator()
    }
    
    func updateHeaderAndNavigationMenu(url: URL?) {
        
        if let tabBarDelegate = self.viewController.tabBarDelegate {
            guard let serviceInfo = knownServices.findMatchingKnownServiceInfo(url: url) else {
                return
            }
            switch serviceInfo.serviceName {
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
                break;
            default : break
            }
            
            if let serviceTitle = serviceInfo.title, !serviceTitle.isEmpty {
                viewController.updateHeaderText(headerText: serviceTitle, accessibilityLabel: serviceInfo.accessibleTitle)
            }
        }
    }
    
    private func showWebViewContainer() {
        clearTimer()
        self.viewController.showWebViewContainer()
        stopActivityIndicator()
    }
    
    func showNativeViewContainerWithError(_ errorMessage: ErrorMessage) {
        clearTimer()
        stopActivityIndicator()
        if !Reachability.isConnectedToNetwork() {
            self.viewController.showNativeViewContainer(errorMessage: ErrorMessage(.NoInternetConnection))
        } else {
            self.viewController.showNativeViewContainer(errorMessage: errorMessage)
        }
        UIApplication.shared.keyWindow?.viewWithTag(2)?.removeFromSuperview()
    }
    
    func clearTimer() {
        if self.timer != nil {
            self.timer.invalidate()
            self.timer = nil
        }
    }

    func checkPageLoadOriginAndStartActivityIndicator() {
        if(self.viewController.goingBack) {
            self.viewController.goingBack=false
            if #available(iOS 10.0, *) {
                os_log("Page looks like it came from a goBack - not starting native spinner", log: OSLog.default, type: .info)
            } else {
                NSLog("Page looks like it came from a goBack - not starting native spinner")
            }
        } else {
            startActivityIndicator()
        }
    }
    
    func startActivityIndicator() {
        self.activityIndicator.startAnimating()
        UIApplication.shared.beginIgnoringInteractionEvents()
    }

    func stopActivityIndicator() {
        UIApplication.shared.endIgnoringInteractionEvents()
        self.activityIndicator.stopAnimating()
    }
}
