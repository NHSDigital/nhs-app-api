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
    var webAppInterface: WebAppInterface
    
    init(controller: HomeViewController, knownServices: KnownServices, webAppInterface: WebAppInterface) {
        self.viewController = controller
        self.knownServices = knownServices
        self.activityIndicator.center = viewController.view.center
        self.viewController.view.addSubview(activityIndicator)
        self.webAppInterface = webAppInterface
    }
    
    func webView(_ webView: WKWebView,
                 decidePolicyFor navigationAction: WKNavigationAction,
                 decisionHandler: @escaping (WKNavigationActionPolicy) -> Void) {
        
        shouldHandleErrors = false
        
        if let url = navigationAction.request.url {
            
            guard navigationAction.targetFrame?.isMainFrame != false else {
                decisionHandler(.allow)
                return
            }
            
            if (!knownServices.shouldURLOpenExternally( url)) {
                self.failedUrl = url
            }
            
            if(!Reachability.isConnectedToNetwork()) {
                decisionHandler(.cancel)
                self.showNativeViewContainerWithError(knownServices.getNoInternetConnectionErrorMessage())
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
                } else if (url.absoluteString == config().OrganDonationUrl) {
                    decisionHandler(.cancel)
                    webView.loadPage(url: config().OrganDonationUrlNative)
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
    
    func showNoConnectionErrorView(urlNavigatingTo: String) {
        self.failedUrl = URL(string: urlNavigatingTo)
        self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(self.failedUrl))
        return
    }
    
    func webView(_ webView: WKWebView, didStartProvisionalNavigation: WKNavigation!) {
        if timer != nil {
            clearTimer()
            self.activityIndicator.stopAnimating()
        }
        shouldHandleErrors = true
        timer = Timer.scheduledTimer(timeInterval: responseWaitingTime, target: self, selector: #selector(pageIsNotResponding), userInfo: nil, repeats: false)
        self.activityIndicator.startAnimating()
        
    }
    
    func webView(_ webView: WKWebView, didFinish: WKNavigation!) {
            if(webView.url?.absoluteString == config().HomeUrl + config().NhsOnlineRequiredQueryString) {
                self.viewController.setVisibilityOfHeaderAndMenuBars(visible: true, isSlim: false)
            }

            self.showWebViewContainer()
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
            return
        }
        
        if shouldHandleErrors {
            if withError._domain == "NSURLErrorDomain" {
                if let info = withError._userInfo as? [String: Any] {
                    if let url = info["NSErrorFailingURLKey"] as? URL {
                        self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(url))
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
    }
    
    func webView(_ webView: WKWebView, createWebViewWith configuration: WKWebViewConfiguration, for navigationAction: WKNavigationAction, windowFeatures: WKWindowFeatures) -> WKWebView? {
        
        if navigationAction.targetFrame == nil {
            webView.load(navigationAction.request)
            self.updateHeaderAndNavigationMenu(url: navigationAction.request.url!)
        }
        
        return nil
    }
    
    func shouldOpenInSafari(url: URL) -> Bool {
        if(url.absoluteString.contains(config().IntroCarouselFileName)) {
            return false
        }
        
        if(url.absoluteString.contains(config().ThrottlingCarouselFileName)) {
            return false
        }
        
        if let _ = knownServices.findMatchingKnownServiceForHostname(hostname: url.host) {
            return false
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
        if(url?.absoluteString.contains("throttling"))!{
            shouldAllowNativeInteraction = true
        }
        
        if  knownServices.shouldAllowNativeInteraction(host: message.frameInfo.securityOrigin.host) || shouldAllowNativeInteraction {
            if (message.name == "onLogin") {
                WebViewController.Properties.usingAbsoluteUri = false
            }
            
            if (message.name == "onLogout") {
                WebViewController.Properties.usingAbsoluteUri = true
                webAppInterface.onLogout()
            }
            
            if (message.name == "hideHeader") {
                viewController.setVisibilityOfHeaderAndMenuBars(visible: false, isSlim: false)
            }
            
            if (message.name == "hideWhiteScreen") {
                UIApplication.shared.keyWindow?.viewWithTag(2)?.removeFromSuperview()
            }
            
            if (message.name == "showHeader") {
                viewController.setVisibilityOfHeaderAndMenuBars(visible: true, isSlim: false)
            }
            
            if (message.name == "showHeaderSlim") {
                viewController.setVisibilityOfHeaderAndMenuBars(visible: true, isSlim: true)
            }
            
            if (message.name == "hideHeaderSlim") {
                viewController.setVisibilityOfHeaderAndMenuBars(visible: false, isSlim: true)
            }
            
            if (message.name == "hideMenuBar") {
                viewController.setVisibilityOfHeaderAndMenuBars(visible: false, isSlim: false)
            }
            
            if (message.name == "resetPageFocus") {
                viewController.headerBar.setFocusToNhsLogoForA11y()
            }
            
            if (message.name == "updateHeaderText") {
                if(!Reachability.isConnectedToNetwork()) {
                    self.showNativeViewContainerWithError(knownServices.getNoInternetConnectionErrorMessage())
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
            if (message.name == "completeAppIntro") {
                
                let defaults = UserDefaults.standard
                defaults.set(false, forKey: config().IsFirstTimeOpened)
                defaults.set(true, forKey: config().HaveShownThrottlingCarouselBefore)
                
                self.viewController.webViewController?.webView.load(URLRequest(url: URL(string: config().HomeUrl)!))
            }
            if (message.name == "onSessionExpiring") {
                var sessionDuration : Int? = message.body as? Int
                if sessionDuration == nil {
                    sessionDuration = 20
                }
                
                viewController.displayExtendSessionDialogue(sessionDuration: sessionDuration!)
            }
        }
    }
    
    func callPostNdopToken(token: String?) {
        self.viewController.webViewController?.postNdopToken(token: token!)
    }
    
    func clearMenuBarItem() {
        self.viewController.tabBar.selectedItem = nil
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
            self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(url))
        }
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
        self.activityIndicator.stopAnimating()
    }
    
    func showNativeViewContainerWithError(_ errorMessage: ErrorMessage) {
        clearTimer()
        self.activityIndicator.stopAnimating()
        self.viewController.showNativeViewContainer(errorMessage: errorMessage)
        UIApplication.shared.keyWindow?.viewWithTag(2)?.removeFromSuperview()
    }
    
    func clearTimer() {
        if self.timer != nil {
            self.timer.invalidate()
            self.timer = nil
        }
    }
}
