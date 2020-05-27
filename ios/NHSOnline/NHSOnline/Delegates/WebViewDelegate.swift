import UIKit
import SafariServices
import WebKit
import LocalAuthentication
import os.log
import iProov

class WebViewDelegate: NSObject, WKNavigationDelegate, WKUIDelegate, WKScriptMessageHandler, SFSafariViewControllerDelegate {
    let knownServicesProvider: KnownServicesProtocol
    let configurationServiceProvider: ConfigurationServiceProtocol
    let viewController: HomeViewController
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

    init(controller: HomeViewController,
         knownServiceProvider: KnownServicesProtocol,
         configurationServiceProvider: ConfigurationServiceProtocol,
         webAppInterface: WebAppInterface) {
        self.viewController = controller
        self.knownServicesProvider = knownServiceProvider
        self.configurationServiceProvider = configurationServiceProvider
        self.webAppInterface = webAppInterface
        self.schemeHandlers = SchemeHandlers()
        self.schemeHandlers.registerHandler(handler: MailToSchemeHandler())
        self.schemeHandlers.registerHandler(handler: TelSchemeHandler())
    }

    func webView(_ webView: WKWebView, decidePolicyFor navigationResponse: WKNavigationResponse, decisionHandler: @escaping (WKNavigationResponsePolicy) -> Void) {
        if let response = navigationResponse.response as? HTTPURLResponse {

            let url = self.viewController.webViewController?.webView.url
            let baseUrl = URL(string: config().HomeUrl)
            let statusCode = response.statusCode

            if ((url?.host == baseUrl?.host) && statusCode >= 400) {
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

        if (checkIfIproov(navigationAction: navigationAction)) {
            let url = navigationAction.request.url
            IProov.handle(url: url!, from: webView)
            decisionHandler(.cancel)
            return
        }

        navigate(webView: webView, navigationAction: navigationAction, decisionHandler: decisionHandler);
    }

    func checkIfIproov(navigationAction: WKNavigationAction) -> Bool {
        if (navigationAction.navigationType == .linkActivated && navigationAction.request.url?.host == "iproov.app") {
            return true;
        } else {
            return false;
        }
    }

    private func navigate(webView: WKWebView, navigationAction: WKNavigationAction, decisionHandler: @escaping (WKNavigationActionPolicy) -> Void) {
        shouldHandleErrors = false

        if let initialUrl = navigationAction.request.url {

            let url = ensureSupportedScheme(initialUrl)
            
            if url != initialUrl {
                decisionHandler(.cancel)
                webView.loadPage(url: url.absoluteString)
                return
            }

            if schemeHandlers.handleUrl(url: url) {
                decisionHandler(.cancel)
                return
            }

            if url.absoluteString.contains(config().FidoLoginErrorPath) {
                viewController.appWebInterface?.biometricLoginFailure()
                viewController.hideProgressBar()
                decisionHandler(.cancel)
                return
            }

            guard navigationAction.targetFrame?.isMainFrame != false else {
                decisionHandler(.allow)
                return
            }

            self.failedUrl = url
            
            var knownService: KnownService
            switch knownServicesProvider.getKnownServices() {
            case .success(let knownServicesResponse):
                knownService = knownServicesResponse.findMatchingKnownService(url)
            default:
                decisionHandler(.cancel)
                self.showNativeViewContainerWithError(ErrorMessage(.ServiceUnavailable))
                return
            }

            if (knownService.viewMode == .AppTab) {
                decisionHandler(.cancel)
                openInSafari(url: url)
                return
            }

            self.updateNavigationMenu(knownService: knownService)
        }

        decisionHandler(.allow)
    }

    func webView(_ webView: WKWebView, didStartProvisionalNavigation: WKNavigation!) {
        if timer != nil {
            clearTimer()
        }
        shouldHandleErrors = true
        timer = Timer.scheduledTimer(timeInterval: responseWaitingTime, target: self, selector: #selector(pageIsNotResponding), userInfo: nil, repeats: false)

        if !webView.url!.absoluteString.contains(config().AuthRedirectPath) && !webView.url!.absoluteString.contains(config().CidUrlSuffix) {
            checkPageLoadOriginAndStartProgressSpinner()
        }
    }

    func webView(_ webView: WKWebView, didFinish: WKNavigation!) {
        self.showWebViewContainer()

        if (UrlHelper.isValidHomeUrl(url: webView.url)) {
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: HeaderType.Full)
        }

        if (urlRequiresSlimHeader(webView.url!.absoluteString)) {
            viewController.updateHeaderText(headerText: "Log in to the NHS App", accessibilityLabel: "Login using Patient ID")
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: HeaderType.Slim)
        }

        UIApplication.shared.keyWindow?.viewWithTag(2)?.removeFromSuperview()

        if !UrlHelper.isSameSchemeAndHostAsHomeUrl(url: webView.url) && !self.viewController.headerBar.isHidden {
            self.viewController.resetFocusAndAnnouncePageTitle(pageTitle: webView.title)
        }

        if !webView.url!.absoluteString.contains(config().BiometricAuthResponseParam) {
            viewController.hideProgressBar()
        }
        
        viewController.hideSplashScreen()
    }

    func webView(_ webView: WKWebView, didFailProvisionalNavigation: WKNavigation!, withError: Error) {
        if !webView.url!.absoluteString.contains(config().AuthRedirectPath) {
            viewController.hideProgressBar()
        }
        if withError._code == NSURLErrorCancelled {
            Logger.logInfo(message: "Page navigation cancelled (user may have double tapped or tapped a different nav menu button while page was still loading): %@", withError.localizedDescription)

            viewController.applicationState.unBlock()
            return
        }

        if shouldHandleErrors {
            if badResponse {
                return self.showNativeViewContainerWithError(ErrorMessage(.ServiceUnavailable))
            }

            if withError._domain == "NSURLErrorDomain" {
                if let info = withError._userInfo as? [String: Any] {
                    if let url = info["NSErrorFailingURLKey"] as? URL {
                        switch knownServicesProvider.getKnownServices() {
                        case .success(let knownServices):
                            self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(url))
                        default:
                            self.showNativeViewContainerWithError(ErrorMessage(.ServiceUnavailable))
                        }
                    }
                }
            } else {
                switch knownServicesProvider.getKnownServices() {
                case .success(let knownServices):
                    self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(webView.url))
                default:
                    self.showNativeViewContainerWithError(ErrorMessage(.ServiceUnavailable))
                }
            }

            Logger.logError(message: "Failed to load the page with error: %@", withError.localizedDescription)
        }
        viewController.applicationState.unBlock()
    }

    func webView(_ webView: WKWebView, createWebViewWith configuration: WKWebViewConfiguration, for navigationAction: WKNavigationAction, windowFeatures: WKWindowFeatures) -> WKWebView? {
        if navigationAction.targetFrame == nil {
            webView.load(navigationAction.request)
            switch knownServicesProvider.getKnownServices() {
            case .success(let knownServices):
                let knownService = knownServices.findMatchingKnownService(navigationAction.request.url!)
                self.updateNavigationMenu(knownService: knownService)
            default:
                self.showNativeViewContainerWithError(ErrorMessage(.ServiceUnavailable))
            }
        }
        return nil
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
        var knownServices: KnownServices
        switch knownServicesProvider.getKnownServices() {
        case .success(let knownServicesResponse):
            knownServices = knownServicesResponse
        default:
            self.showNativeViewContainerWithError(ErrorMessage(.ServiceUnavailable))
            return
        }
        
        let knownService = knownServices.findMatchingKnownService(message.frameInfo.request.url)
        
        if (knownService.javaScriptInteractionMode == .SilverThirdParty){
            switch message.name {
            case UserContent.addEventToCalendar.rawValue:
                handleCalendarData(calendarData: message.body as! String)
                break
            case UserContent.goToPage.rawValue:
                viewController.handleGoToPage(page: message.body as! String)
                break;
            default:
                break;
            }
        }

        if (knownService.javaScriptInteractionMode == .NhsApp) {
            switch message.name {
            case UserContent.getNotificationsStatus.rawValue:
                viewController.getNotificationsStatus()
                break;
            case UserContent.attemptBiometricLogin.rawValue:
                viewController.delayedBiometricsStart(0.3)
                break
            case UserContent.clearMenuBarItem.rawValue:
                clearMenuBarItem()
                break
            case UserContent.fetchBiometricSpec.rawValue:
                let biometricState = UserDefaultsManager.getBiometricAvailability()
                self.viewController.handleBiometricSpecRequest(biometricAvailability: biometricState)
                break
            case UserContent.fetchNativeAppVersion.rawValue:
                self.viewController.setupAppVersion()
                break
            case UserContent.hideHeader.rawValue:
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .None)
                break
            case UserContent.hideWhiteScreen.rawValue:
                UIApplication.shared.keyWindow?.viewWithTag(2)?.removeFromSuperview()
                break
            case UserContent.hideHeaderSlim.rawValue:
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .None)
                break
            case UserContent.hideMenuBar.rawValue:
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .None)
                break
            case UserContent.onLogin.rawValue:
                WebViewController.Properties.usingAbsoluteUri = false
                break
            case UserContent.onLogout.rawValue:
                WebViewController.Properties.usingAbsoluteUri = true
                webAppInterface.onLogout()
                break
            case UserContent.onSessionExpiring.rawValue:
                viewController.displayExtendSessionDialogue()
                break
            case UserContent.openAppSettings.rawValue:
                if #available(iOS 10.0, *) {
                    UIApplication.shared.open(URL(string: UIApplicationOpenSettingsURLString)!)
                }
                break
            case UserContent.pageLoadComplete.rawValue:
                viewController.applicationState.unBlock()
                break
            case UserContent.requestPnsToken.rawValue:
                let trigger: String = message.body as! String
                viewController.registerForPushNotifications(trigger: trigger)
                break
            case UserContent.resetPageFocus.rawValue:
                viewController.headerBar.setFocusToNhsLogoForA11y()
                break
            case UserContent.setHelpUrl.rawValue:
                setHelpUrl(url: message.body as? String ?? config().HelpURL)
                break
            case UserContent.setRetryPath.rawValue:
                setRetryUrl(path: message.body as! String)
                break;
            case UserContent.setMenuBarItem.rawValue:
                setMenuBarItem(index: message.body as? Int ?? 0)
                break
            case UserContent.showHeader.rawValue:
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .Full)
                break
            case UserContent.showHeaderSlim.rawValue:
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .Slim)
                break
            case UserContent.startDownload.rawValue:
                viewController.downloadFile(messageBody: String(describing: message.body))
                break
            case UserContent.updateHeaderText.rawValue:
                if (!Reachability.isConnectedToNetwork()) {
                    self.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
                    return
                }
                viewController.updateHeaderText(headerText: String(describing: message.body))
                break
            case UserContent.updateBiometricRegistration.rawValue:
                let biometricState = UserDefaultsManager.getBiometricAvailability()
                viewController.handleBiometricStatusChangeRequest(biometricState: biometricState)
                break
            default:
                break
            }
        }
    }

    func setMenuBarItem(index: Int) {
        if let tabBarDelegate = self.viewController.tabBarDelegate {
            tabBarDelegate.setMenuBarItem(index: index)
        }
    }

    func setHelpUrl(url: String) {
        UserDefaults.standard.set(url, forKey: "HelpUrl")
    }

    func setRetryUrl(path: String) {
        if (!path.isEmpty) {
            var suffix = path
            suffix.remove(at: suffix.startIndex)
            failedUrl = URL(string: config().HomeUrl + suffix)
        }
    }

    func ensureSupportedScheme(_ url: URL) -> URL {
        if (url.scheme == config().AppScheme) {
            self.viewController.setVisibilityOfHeaderAndMenuBars(headerType: HeaderType.None)
        }
        return UrlHelper.ensureUrlWithScheme(url: url.absoluteString)!
    }

    func clearMenuBarItem() {
        self.viewController.clearSelectedTab()
    }

    @objc func pageIsNotResponding() {
        if self.viewController.webViewController?.webView.isLoading == true {
            Logger.logError(message: "Page is not responding for a long time, loading stopped.")

            self.viewController.webViewController?.webView.stopLoading()
            let url = self.viewController.webViewController?.webView.url
            let baseUrl = URL(string: config().HomeUrl)
            if (url?.host == baseUrl?.host) {
                switch knownServicesProvider.getKnownServices() {
                case .success(let knownServices):
                    self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(url))
                default:
                    self.showNativeViewContainerWithError(ErrorMessage(.ServiceUnavailable))
                }
            }
        }
    }

    func updateNavigationMenu(knownService: KnownService) {
        if let tabBarDelegate = self.viewController.tabBarDelegate {
            tabBarDelegate.setMenuBarItem(menuTab: knownService.menuTab)
        }
    }

    private func showWebViewContainer() {
        clearTimer()
        self.viewController.showWebViewContainer()
    }

    func showNativeViewContainerWithError(_ errorMessage: ErrorMessage) {
        clearTimer()
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

    func checkPageLoadOriginAndStartProgressSpinner() {
        if (self.viewController.goingBack) {
            self.viewController.goingBack = false
            Logger.logInfo(message: "Page looks like it came from a goBack - not starting native spinner")
        } else {
            self.viewController.showProgressBar()
        }
    }
    
    func urlRequiresSlimHeader(_ url: String?) -> Bool{
        if url == nil { return false }
        
        switch configurationServiceProvider.getConfigurationResponse() {
        case .success(let configurationResponse):
            for path in configurationResponse.nhsLoginLoggedInPaths {
               if( url!.contains(path)) {
                   return false
               }
           }
        default:
            break
        }
        
        if (url!.contains(config().CidUrlSuffix)) {
            return true
        }
        return false
    }
    
    private func handleCalendarData(calendarData: String) {
        let eventData = Data(String(calendarData).utf8);

         do {
             if let json = try JSONSerialization.jsonObject(with: eventData, options: []) as? [String: Any] {
                 let subject = json["subject"] as? String
                 let body = json["body"] as? String
                 let location = json["location"] as? String
                 let startTimeEpochInSeconds = json["startTimeEpochInSeconds"] as? NSNumber
                 let endTimeEpochInSeconds = json["endTimeEpochInSeconds"] as? NSNumber

                 var start: Int64? = nil
                 if (startTimeEpochInSeconds != nil) {
                     start = startTimeEpochInSeconds?.int64Value
                 }
                 
                 var end: Int64? = nil
                 if (endTimeEpochInSeconds != nil) {
                     end = endTimeEpochInSeconds?.int64Value
                 }
      
                 viewController.addEventToCalendar(
                    calendarData: CalendarData(subject: subject,
                                               body: body,
                                               location: location,
                                               startTimeEpochSeconds: start,
                                               endTimeEpochSeconds: end))
             }
         } catch let error as NSError {
             print("Failed to load: \(error.localizedDescription)")
         }
    }
}
