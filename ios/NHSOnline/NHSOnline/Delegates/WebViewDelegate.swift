import UIKit
import SafariServices
import WebKit
import os.log
import iProov

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
            let url = navigationAction.request.url;

            launchIproov(url: url!, webView: webView);
            decisionHandler(.cancel)
            return;

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
                viewController.showBiometricSessionError()
                stopActivityIndicator()
                decisionHandler(.cancel)
                return
            }

            guard navigationAction.targetFrame?.isMainFrame != false else {
                decisionHandler(.allow)
                return
            }

            if !Reachability.isConnectedToNetwork() {
                decisionHandler(.cancel)
                self.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
                return
            }

            let knownService = knownServices.findMatchingKnownService(url)

            if (knownService.viewMode == .AppTab) {
                decisionHandler(.cancel)
                openInSafari(url: url)
                return
            }

            self.failedUrl = url

            self.updateNavigationMenu(knownService: knownService)
        }

        decisionHandler(.allow)
    }

    func launchIproov(url: URL, webView: WKWebView) {
        if (url.pathComponents.count < 2) {
            Logger.logInfo(message: "Too few path components")
            return;
        }

        let token = url.pathComponents[1]

        Logger.logInfo(message: "Launching IProov")
        let options = Options()
        options.ui.title = NSLocalizedString("NHSLoginTitle", comment: "")

        let completionHandler: (Any?, Error?) -> Void = {
            (data, error) in
            if (error != nil || data == nil) {
                Logger.logError(message: "An error occurred when attempting to navigate to the iProov page.")
                return;
            }
            // The if statement below has been added to mitigate a bug in iProov and should be removed when this has been resolved.
            // This is covered in further detail in the Jira ticket NHSO-8203.
            if (!Reachability.isConnectedToNetwork()) {
                self.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
                return
            }
            IProov.launch(streamingURL: data as! String, token: token, options: options) { (status) in
                self.checkIproovStatus(status: status);
            }
        }
        webView.evaluateJavaScript("window.getIproovEndpoint()", completionHandler: completionHandler)
    }


    private func checkIproovStatus(status: Status) {
        switch status {
        case let .processing(progress, message):
            // The SDK will update your app with the progress of streaming to the server and authenticating
            // the user. This will be called multiple time as the progress updates.
            Logger.logInfo(message: "IProov - Processing: progress = %@, message = %@", progress, message)

        case .success(_):
            // The user was successfully verified/enrolled and the token has been validated.
            // The token passed back will be the same as the one passed in to the original call.
            Logger.logInfo(message: "IProov - Success")

        case let .failure(reason, feedbackCode):
            // The user was not successfully verified/enrolled, as their identity could not be verified,
            // or there was another issue with their verification/enrollment. A reason (as a string)
            // is provided as to why the claim failed, along with a feedback code from the back-end.
            Logger.logInfo(message: "IProov - Failure: reason = %@, feedbackCode = %@", reason, feedbackCode)

        case .cancelled:
            // The user cancelled iProov, either by pressing the close button at the top right, or sending
            // the app to the background.
            Logger.logInfo(message: "IProov - Cancelled")

        case let .error(error):
            // The user was not successfully verified/enrolled due to an error (e.g. lost internet connection)
            // along with an `iProovError` with more information about the error (NSError in Objective-C).
            // It will be called once, or never.
            Logger.logError(message: "IProov - Error: error = %@", "\(error)")
        }
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

        if (UrlHelper.isValidHomeUrl(url: webView.url)) {
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: HeaderType.Full)
        }

        if (webView.url!.absoluteString.contains(config().CidUrlSuffix)) {
            viewController.updateHeaderText(headerText: "Log in to the NHS App", accessibilityLabel: "Login using Patient ID")
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: HeaderType.Slim)
        }

        UIApplication.shared.keyWindow?.viewWithTag(2)?.removeFromSuperview()

        if !UrlHelper.isSameSchemeAndHostAsHomeUrl(url: webView.url) && !self.viewController.headerBar.isHidden {
            self.viewController.resetFocusAndAnnouncePageTitle(pageTitle: webView.title)
        }
    }

    func webView(_ webView: WKWebView, didFailProvisionalNavigation: WKNavigation!, withError: Error) {
        if withError._code == NSURLErrorCancelled {
            Logger.logInfo(message: "Page navigation cancelled (user may have double tapped or tapped a different nav menu button while page was still loading): %@", withError.localizedDescription)

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
                    if let url = info["NSErrorFailingURLKey"] as? URL {
                        self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(url))
                    }
                }
            } else {
                self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(webView.url))
            }

            Logger.logError(message: "Failed to load the page with error: %@", withError.localizedDescription)
        }
        stopActivityIndicator()
        viewController.applicationState.unBlock()
    }

    func webView(_ webView: WKWebView, createWebViewWith configuration: WKWebViewConfiguration, for navigationAction: WKNavigationAction, windowFeatures: WKWindowFeatures) -> WKWebView? {
        if navigationAction.targetFrame == nil {
            webView.load(navigationAction.request)
            let knownService = knownServices.findMatchingKnownService(navigationAction.request.url!)
            self.updateNavigationMenu(knownService: knownService)
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
        guard let rootService = knownServices.getRootServiceByHostAndScheme(
                host: message.frameInfo.securityOrigin.host, scheme: message.frameInfo.securityOrigin.protocol
        ) else {
            return
        }

        if (rootService.javaScriptInteractionMode == .NhsApp) {
            switch message.name {
            case "getNotificationsStatus":
                viewController.getNotificationsStatus()
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
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .None)
                break
            case "hideWhiteScreen":
                UIApplication.shared.keyWindow?.viewWithTag(2)?.removeFromSuperview()
                break
            case "hideHeaderSlim":
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .None)
                break
            case "hideMenuBar":
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .None)
                break
            case "onLogin":
                WebViewController.Properties.usingAbsoluteUri = false
                break
            case "onLogout":
                WebViewController.Properties.usingAbsoluteUri = true
                webAppInterface.onLogout()
                break
            case "onSessionExpiring":
                viewController.displayExtendSessionDialogue()
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
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .Full)
                break
            case "showHeaderSlim":
                viewController.setVisibilityOfHeaderAndMenuBars(headerType: .Slim)
                break
            case "startDownload":
                viewController.downloadFile(messageBody: String(describing: message.body))
                break
            case "updateHeaderText":
                if (!Reachability.isConnectedToNetwork()) {
                    self.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
                    return
                }
                viewController.updateHeaderText(headerText: String(describing: message.body))
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
            return URL(string: url.absoluteString.replacingOccurrences(of: config().AppScheme + ":", with: config().BaseScheme + ":"))!
        }
        return url
    }

    func clearMenuBarItem() {
        self.viewController.tabBar.selectedItem = nil
        self.viewController.selectedTab = nil
    }

    @objc func pageIsNotResponding() {
        if (self.viewController.webViewController?.webView.isLoading)! {
            Logger.logError(message: "Page is not responding for a long time, loading stopped.")

            self.viewController.webViewController?.webView.stopLoading()
            let url = self.viewController.webViewController?.webView.url
            let baseUrl = URL(string: config().HomeUrl)
            if (url?.host == baseUrl?.host) {
                self.showNativeViewContainerWithError(knownServices.getUnavailabilityErrorMessageForService(url))
            }
        }
        stopActivityIndicator()
    }

    func updateNavigationMenu(knownService: KnownService) {
        if let tabBarDelegate = self.viewController.tabBarDelegate {
            tabBarDelegate.setMenuBarItem(menuTab: knownService.menuTab)
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
        if (self.viewController.goingBack) {
            self.viewController.goingBack = false
            Logger.logInfo(message: "Page looks like it came from a goBack - not starting native spinner")
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
