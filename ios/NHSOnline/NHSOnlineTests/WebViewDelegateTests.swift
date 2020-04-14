import XCTest
import WebKit
@testable import NHSOnline

class WebViewDelegateTests: XCTestCase {
    var webViewDelegate: MockWebViewDelegate?
    
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
        homeViewController = viewController
        wKWebView = WKWebView(frame: .zero)
        mockWKWebView = MockWKWebView()
        wKNavigation = WKNavigation()
        nSURLErrorCancelled = NSError(domain:NSURLErrorDomain,code:-999,userInfo:[NSLocalizedDescriptionKey:""])
        error = NSError(domain:NSURLErrorDomain,code:-1000,userInfo:[NSLocalizedDescriptionKey:""])
    }
    
    func test_WhenPageLoadIsNormal_VerifyStartActivityIndicatorIsCalled() {
        webViewDelegate?.checkPageLoadOriginAndStartProgressSpinner()
        
        assert(homeViewController!.startActivityIndicatorWasCalled == true,
               "Expected the startActivityIndicator() Method to be invoked")
    }
    
    func test_WhenPageLoadIsFromBackButton_VerifyStartActivityIndicatorIsNotCalled() {
        webViewDelegate?.viewController.goingBack = true
        
        webViewDelegate?.checkPageLoadOriginAndStartProgressSpinner()
        
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
}

class MockWebViewDelegate : WebViewDelegate {
    override init(controller: HomeViewController, knownServiceProvider: KnownServicesProtocol,
    configurationServiceProvider: ConfigurationServiceProtocol, webAppInterface: WebAppInterface) {
        super.init(controller: controller,knownServiceProvider: knownServiceProvider, configurationServiceProvider: configurationServiceProvider, webAppInterface: webAppInterface)
    }
}

class MockWKWebView : WKWebView {
    var attemptedEvaluateJavaScript = false
    
    override func evaluateJavaScript(_ javaScriptString: String, completionHandler: ((Any?, Error?) -> Void)? = nil) {
        attemptedEvaluateJavaScript =  true;
    }
}
