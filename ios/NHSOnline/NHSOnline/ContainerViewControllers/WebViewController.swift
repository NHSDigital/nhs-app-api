import UIKit
import WebKit
import os.log

class WebViewController: UIViewController, WKUIDelegate {
    var webViewDelegate: WebViewDelegate?
    public var webView: WKWebView!
    var redirect = false
    var knownServiceProvider: KnownServicesProtocol?
    var configurationServiceProvider: ConfigurationServiceProtocol?

    override func loadView() {
        super.loadView()

        let fileReader = FileReader();
        let webEventsJSLocation = Bundle.main.path(forResource: "WebEvents", ofType: "js")!
        let javascript = fileReader.readContentFromLocation(fileLocation: webEventsJSLocation)

        let contentController = WKUserContentController()
        let script = WKUserScript(source: javascript, injectionTime: .atDocumentStart, forMainFrameOnly: true)
        let versionNumber = Bundle.main.object(forInfoDictionaryKey: "CFBundleShortVersionString") as! String
        contentController.addUserScript(script)

        let config = WKWebViewConfiguration()
        config.userContentController = contentController
        config.suppressesIncrementalRendering = true
        config.applicationNameForUserAgent = " nhsapp-ios/" + versionNumber

        webView = WKWebView(frame: .zero, configuration: config)
        webView.uiDelegate = self
        webView.allowsLinkPreview = false
        view = webView
    }

    override func viewDidLoad() {
        self.setNeedsStatusBarAppearanceUpdate()
        super.viewDidLoad()
    }

    override var preferredStatusBarStyle: UIStatusBarStyle {
        .lightContent
    }

    struct Properties {
        static var usingAbsoluteUri: Bool = true
    }

    var homeUrl = config().HomeUrl

    func setWebViewDelegate(delegate: WebViewDelegate) {
        webView.navigationDelegate = delegate
        webView.uiDelegate = delegate
        webView.configuration.preferences.javaScriptEnabled = true
        webViewDelegate = delegate
        webView.configuration.userContentController.add(delegate, name: "getNotificationsStatus")
        webView.configuration.userContentController.add(delegate, name: "attemptBiometricLogin")
        webView.configuration.userContentController.add(delegate, name: "clearMenuBarItem")
        webView.configuration.userContentController.add(delegate, name: "fetchNativeAppVersion")
        webView.configuration.userContentController.add(delegate, name: "goToLoginOptions")
        webView.configuration.userContentController.add(delegate, name: "hideHeaderSlim")
        webView.configuration.userContentController.add(delegate, name: "hideHeader")
        webView.configuration.userContentController.add(delegate, name: "hideMenuBar")
        webView.configuration.userContentController.add(delegate, name: "hideWhiteScreen")
        webView.configuration.userContentController.add(delegate, name: "onLogin")
        webView.configuration.userContentController.add(delegate, name: "onLogout")
        webView.configuration.userContentController.add(delegate, name: "onSessionExpiring")
        webView.configuration.userContentController.add(delegate, name: "openAppSettings")
        webView.configuration.userContentController.add(delegate, name: "pageLoadComplete")
        webView.configuration.userContentController.add(delegate, name: "requestPnsToken")
        webView.configuration.userContentController.add(delegate, name: "resetPageFocus")
        webView.configuration.userContentController.add(delegate, name: "setMenuBarItem")
        webView.configuration.userContentController.add(delegate, name: "setHelpUrl")
        webView.configuration.userContentController.add(delegate, name: "setRetryPath")
        webView.configuration.userContentController.add(delegate, name: "showHeader")
        webView.configuration.userContentController.add(delegate, name: "showHeaderSlim")
        webView.configuration.userContentController.add(delegate, name: "updateHeaderText")
        webView.configuration.userContentController.add(delegate, name: "startDownload")
    }

    private func loadSpaPage(path: String) {
        var spaPath = path.replacingOccurrences(of: homeUrl, with: "/")

        if (!spaPath.starts(with: "/")) {
            spaPath = "/" + spaPath;
        }

        let completionHandler: (Any?, Error?) -> Void = {
            (data, error) in
            if (error != nil) {
                Logger.logError(message: "An error occurred when attempting to navigate to the page via Vue Router. Doing a full reload.")
                if UrlHelper.verifyUrl(urlString: path) {
                    self.webView.loadPage(url: path)
                } else {
                    self.webView.loadPage(url: self.homeUrl + path)
                }
            }
        }
        webView.evaluateJavaScript("window.$nuxt.$store.dispatch('navigation/goTo', '\(spaPath)') && undefined;", 
                completionHandler: completionHandler)
        webViewDelegate?.viewController.showWebViewContainer()
    }

    func loadPage(url: URL) {
        loadPage(url: url.absoluteString, isConnectedToNetwork: Reachability.isConnectedToNetwork())
    }
    
    func loadPage(url: String) {
        loadPage(url: url, isConnectedToNetwork: Reachability.isConnectedToNetwork())
    }

    func loadPage(url: String, isConnectedToNetwork: Bool) {
        self.webViewDelegate?.clearTimer()
        
        var knownServices: KnownServices
        
        switch self.knownServiceProvider?.getKnownServices() {
        case .success(let knownServicesResponse):
            knownServices = knownServicesResponse
        default:
            webViewDelegate?.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
            return
        }

        if (!isConnectedToNetwork) {
            webViewDelegate?.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
            return
        }

        guard var resolvedUrl = UrlHelper.resolveAppScheme(url: url) else {
            return
        }

        if (!UIApplication.shared.canOpenURL(resolvedUrl)) {
            resolvedUrl = URL(string: homeUrl)!
        }
        
        self.webViewDelegate?.failedUrl = resolvedUrl

        let knownService = knownServices.findMatchingKnownService(resolvedUrl)

        self.webViewDelegate?.updateNavigationMenu(knownService: knownService)

        if knownService.viewMode == .AppTab || shouldOpenInWebView(resolvedUrl.absoluteString) {
            webView.loadPage(url: resolvedUrl.absoluteString)
        } else {
            self.loadSpaPage(path: resolvedUrl.absoluteString)
            if (webView.url!.absoluteString == resolvedUrl.absoluteString) {
                webViewDelegate?.viewController.applicationState.unBlock()
            }
        }
    }

    private func shouldOpenInWebView(_ url: String) -> Bool {
        WebViewController.Properties.usingAbsoluteUri || !shouldLoadUrlAsSpaPage(urlToNavigateTo: url)
    }

    private func shouldLoadUrlAsSpaPage(urlToNavigateTo: String) -> Bool {
        guard UrlHelper.isSameSchemeAndHostAsHomeUrl(url: webView.url) else {
            return false
        }
        return UrlHelper.isSameSchemeAndHostAsHomeUrl(url: URL(string: urlToNavigateTo))
    }

    func reloadWebView() {
        let viewController = webViewDelegate?.viewController
        viewController!.clearSelectedTab()
        if let failedUrl = webViewDelegate!.failedUrl {
            let urlToReload = UrlHelper.getReloadUrl(url: failedUrl)
            webView.load(URLRequest(url: urlToReload))
        } else {
            webView.load(URLRequest(url: URL(string: config().HomeUrl)!))
        }
    }

    func dismissSafariViewController() {
        webViewDelegate?.safariViewController?.dismiss(animated: true, completion: nil)
    }

    func setRedirectCompleted(redirect: Bool?) {
        self.redirect = redirect!
    }

    func getRedirectCompleted() -> Bool {
        self.redirect
    }
}
