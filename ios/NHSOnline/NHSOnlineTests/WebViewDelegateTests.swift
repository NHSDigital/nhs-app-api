import XCTest
import WebKit
@testable import NHSOnline

class WebViewDelegateTests: XCTestCase {
    var webViewDelegate: MockWebViewDelegate?
    var iProovWebViewDelegate : IProovMockWebViewDelegate?
    
    var wKWebView: WKWebView?
    var mockWKWebView: MockWKWebView?
    var wKNavigation: WKNavigation?
    var error: NSError?
    var nSURLErrorCancelled: NSError?
    var homeViewController: MockHomeViewController?
    
    override func setUp() {
        super.setUp()
        let knownServicesProvider: KnownServicesProtocol = SuccessKnownServiceProtocolMock()
        let configurationServiceProvider: ConfigurationServiceProtocol = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        let viewController = MockHomeViewController()
        viewController.knownServicesProvider = SuccessKnownServiceProtocolMock()
        viewController.configurationServiceProvider = configurationServiceProvider
        let webAppInterface = WebAppInterface(controller: viewController)
        webViewDelegate = MockWebViewDelegate(controller: viewController, knownServiceProvider: knownServicesProvider, configurationServiceProvider: configurationServiceProvider, webAppInterface: webAppInterface)
        iProovWebViewDelegate = IProovMockWebViewDelegate(controller: viewController, knownServiceProvider: knownServicesProvider, configurationServiceProvider: configurationServiceProvider, webAppInterface: webAppInterface)
        homeViewController = viewController
        wKWebView = WKWebView(frame: .zero)
        mockWKWebView = MockWKWebView()
        wKNavigation = WKNavigation()
        nSURLErrorCancelled = NSError(domain:NSURLErrorDomain,code:-999,userInfo:[NSLocalizedDescriptionKey:""])
        error = NSError(domain:NSURLErrorDomain,code:-1000,userInfo:[NSLocalizedDescriptionKey:""])
    }
    
    func test_WhenPageLoadIsNormal_VerifyStartActivityIndicatorIsCalled() {
        webViewDelegate?.checkPageLoadOriginAndStartActivityIndicator()
        
        assert(homeViewController!.startActivityIndicatorWasCalled == true,
               "Expected the startActivityIndicator() Method to be invoked")
    }
    
    func test_WhenPageLoadIsFromBackButton_VerifyStartActivityIndicatorIsNotCalled() {
        webViewDelegate?.viewController.goingBack = true
        
        webViewDelegate?.checkPageLoadOriginAndStartActivityIndicator()
        
        assert(homeViewController!.startActivityIndicatorWasCalled == false,
               "startActivityIndicator() Method should not be invoked")
    }
    
    func test_webViewDidFailProvisionalNavigationWithNSURLErrorCancelled_VerifyStopActivityIndicatorIsCalled() {
        wKWebView?.loadPage(url: config().HomeUrl)
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: nSURLErrorCancelled!)
        
        assert(homeViewController!.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
        
        assert(homeViewController!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithNSURLErrorCancelled_VerifyStopActivityIndicatorIsNotCalled() {
        wKWebView?.loadPage(url: config().HomeUrl + "auth-return?code=something")
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: nSURLErrorCancelled!)
        
        assert(homeViewController!.stopActivityIndicatorWasCalled == false,
               "Expected the stopActivityIndicator() Method to not have been invoked")
        
        assert(homeViewController!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithOtherNSError_VerifyStopActivityIndicatorIsCalled() {
        wKWebView?.loadPage(url: config().HomeUrl)
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: error!)
        
        assert(homeViewController!.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
        
        assert(homeViewController!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithOtherNSError_VerifyStopActivityIndicatorIsNotCalled() {
        wKWebView?.loadPage(url: config().HomeUrl + "auth-return?code=something")
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: error!)
        
        assert(homeViewController!.stopActivityIndicatorWasCalled == false,
               "Expected the stopActivityIndicator() Method to not have been invoked")
        
        assert(homeViewController!.applicationState.isReady() == true)
    }
    
    func test_pageIsNotResponding_VerifyStopActivityIndicatorIsCalled(){
        webViewDelegate?.pageIsNotResponding()
        
        assert(homeViewController!.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
    }

    func test_launchIproovIsCalled() {
        let url = URL(string: "https://iproov.app/testtoken")!
        let iProovUrlRequest = URLRequest(url: url)
        let iProovAction = iProovNavigationAction(testRequest: iProovUrlRequest)

        // act
        iProovWebViewDelegate?.webView(wKWebView!, decidePolicyFor: iProovAction, decisionHandler: { _ in })

        // assert
        assert(iProovWebViewDelegate?.attemptedIProovLaunch == true,
                      "Expected the launchIproov() method to be invoked")
    }

    func test_evaluateJavascriptIsCalled() {
        let url = URL(string: "https://iproov.app/testtoken")
        webViewDelegate?.launchIproov(url: url!, webView: mockWKWebView!)

        assert(mockWKWebView?.attemptedEvaluateJavaScript == true,
               "Expected the evaluateJavaScript() method to be invoked")
    }

    func test_evaluateJavascriptIsNotReachedIfTooFewPathComponents() {
        let url = URL(string: "https://iproov.app")
        webViewDelegate?.launchIproov(url: url!, webView: mockWKWebView!)

        assert(iProovWebViewDelegate?.attemptedIProovLaunch == false,
                      "Expected the function to return before the evaluateJavaScript() method was reached")
    }
}

class MockWebViewDelegate : WebViewDelegate {
    var attemptedIProovLaunch = false

    override init(controller: HomeViewController, knownServiceProvider: KnownServicesProtocol,
    configurationServiceProvider: ConfigurationServiceProtocol, webAppInterface: WebAppInterface) {
        super.init(controller: controller,knownServiceProvider: knownServiceProvider, configurationServiceProvider: configurationServiceProvider, webAppInterface: webAppInterface)
    }
}

class IProovMockWebViewDelegate : WebViewDelegate {
    var attemptedIProovLaunch = false;
    
    override func launchIproov(url: URL, webView: WKWebView) {
        attemptedIProovLaunch = true;
    }
}

class MockWKWebView : WKWebView {
    var attemptedEvaluateJavaScript = false
    
    override func evaluateJavaScript(_ javaScriptString: String, completionHandler: ((Any?, Error?) -> Void)? = nil) {
        attemptedEvaluateJavaScript =  true;
    }
}

class iProovNavigationAction: WKNavigationAction {
    let testRequest: URLRequest
    override var request: URLRequest {
        return testRequest
    }

    init(testRequest: URLRequest) {
        self.testRequest = testRequest
        super.init()
    }
}
