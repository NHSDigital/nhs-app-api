import UIKit
import WebKit
import os.log

class WebViewController: UIViewController, WKUIDelegate {
    var webViewDelegate: WebViewDelegate?
    public var webView: WKWebView!
    var redirect = false
    var knownServiceProvider: KnownServicesProtocol?
    var configurationServiceProvider: ConfigurationServiceProtocol?
    public var headerStrategy: HeaderStrategyProtocol?
    
    override func loadView() {
        super.loadView()
        
        let contentController = WKUserContentController()
        LoadUserScript(name: "WebEventsThirdParty", contentController: contentController)
        LoadUserScript(name: "WebEventsPrivate", contentController: contentController)
        LoadUserScript(name: "WebEventsNHSLogin", contentController: contentController)
        
        let versionNumber = Bundle.main.object(forInfoDictionaryKey: "CFBundleShortVersionString") as! String
        let config = WKWebViewConfiguration()
        config.userContentController = contentController
        config.suppressesIncrementalRendering = true
        config.applicationNameForUserAgent = " nhsapp-ios/" + versionNumber

        webView = WKWebView(frame: .zero, configuration: config)
        webView.uiDelegate = self
        webView.allowsLinkPreview = false
        
        webView.addObserver(self, forKeyPath: "URL", options: .new, context: nil)
        
        view = webView
    }
    
    override func observeValue(forKeyPath keyPath: String?, of object: Any?, change: [NSKeyValueChangeKey : Any]?, context: UnsafeMutableRawPointer?) {
        if !Reachability.isConnectedToNetwork() {
            webViewDelegate?.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
        }
        if keyPath == "URL" {
            applyHeaderStrategy()
        }
    }

    func LoadUserScript(name: String, contentController: WKUserContentController){
      let fileReader = FileReader();
      let webEventsJSLocation = Bundle.main.path(forResource: name, ofType: "js")!
      let javascript = fileReader.readContentFromLocation(fileLocation: webEventsJSLocation)
      
      let script = WKUserScript(source: javascript, injectionTime: .atDocumentStart, forMainFrameOnly: true)
      contentController.addUserScript(script)
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
        UserContent.allCases.forEach { value in
            webView.configuration.userContentController.add(delegate, name: "\(value.rawValue)")
        }

        webView.addObserver(self, forKeyPath: "URL", options: .new, context: nil)
    }
    
    deinit {
        webView.removeObserver(self, forKeyPath: "URL")
    }
    
    func applyHeaderStrategy() {
        var knownService: KnownService
        switch knownServiceProvider!.getKnownServices() {
        case .success(let knownServicesResponse):
            knownService = knownServicesResponse.findMatchingKnownService(webView.url)
        default:
            return
        }
        
        self.headerStrategy!.apply(integrationLevel: knownService.integrationLevel)
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
    
    func loadPage(url: String, getKnownServices: Bool = true) {
        loadPage(url: url, isConnectedToNetwork: Reachability.isConnectedToNetwork(), getKnownServices: getKnownServices)
    }

    func loadPage(url: String, isConnectedToNetwork: Bool, getKnownServices: Bool = true) {
        self.webViewDelegate?.clearTimer()
        
        guard var resolvedUrl = UrlHelper.resolveAppScheme(url: url) else {
            return
        }
        
        if (!getKnownServices) {
            webView.loadPage(url: resolvedUrl.absoluteString)
            return
        }
        
        var knownServices: KnownServices
        
        switch self.knownServiceProvider?.getKnownServices() {
        case .success(let knownServicesResponse):
            knownServices = knownServicesResponse
        default:
            webViewDelegate?.showNativeViewContainerWithError(ErrorMessage(.NoInternetConnection))
            return
        }

        if (!UIApplication.shared.canOpenURL(resolvedUrl)) {
            resolvedUrl = URL(string: homeUrl)!
        }
        
        self.webViewDelegate?.failedUrl = resolvedUrl

        let knownService = knownServices.findMatchingKnownService(resolvedUrl)

        self.webViewDelegate?.updateNavigationMenu(knownService: knownService)

        if knownService.integrationLevel == .Bronze || shouldOpenInWebView(resolvedUrl.absoluteString) {
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
        
        if (config().CompatibilityCheckEnabled) {
            let compatibilityService = viewController!.compatibilityService!
            compatibilityService.check(isCheckEnabled: config().CompatibilityCheckEnabled)

            if (compatibilityService.hasShownIncompatibleScreen || compatibilityService.hasShownUpdateDialog) {
                return
            }
        }
        
        viewController!.clearSelectedTab()
        if let failedUrl = webViewDelegate!.failedUrl {
            let urlToReload = UrlHelper.getReloadUrl(url: failedUrl)
            webView.load(URLRequest(url: urlToReload))
        } else {
            webView.load(URLRequest(url: URL(string: self.homeUrl)!))
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
