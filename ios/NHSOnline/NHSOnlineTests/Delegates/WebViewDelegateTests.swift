import XCTest
import WebKit
@testable import NHSOnline

class WebViewDelegateTests: XCTestCase {
    var webViewDelegate: WebViewDelegateMocks?
    
    var wKWebView: WKWebView?
    var mockWKWebView: WebViewMocks?
    var wKNavigation: WKNavigation?
    var error: NSError?
    var nSURLErrorCancelled: NSError?
    var homeViewController: HomeViewControllerMocks?
    var appWebInterface: AppWebInterfaceMocks?
    var knownServicesProvider: KnownServicesProtocolMocks?
    
    override func setUp() {
        super.setUp()
        self.knownServicesProvider = KnownServicesProtocolMocks.success()
        let configurationServiceProvider: ConfigurationServiceProtocol = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        let viewController = HomeViewControllerMocks()
        viewController.knownServicesProvider = knownServicesProvider
        viewController.configurationServiceProvider = configurationServiceProvider
        
        mockWKWebView = WebViewMocks()
        appWebInterface = AppWebInterfaceMocks(webView: mockWKWebView)
        let webAppInterface = WebAppInterface(controller: viewController)
        webViewDelegate = WebViewDelegateMocks(controller: viewController, knownServiceProvider: knownServicesProvider!, configurationServiceProvider: configurationServiceProvider, webAppInterface: webAppInterface)
        homeViewController = viewController
        wKWebView = WKWebView(frame: .zero)
        mockWKWebView = WebViewMocks()
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
    
    func test_userContentControllerGoToPage_WithJavaScriptInteractionModeSilverThirdParty_CallsControllerGoToPage() {
        // Arrange
        let url = "http://www.example.com"
        let page = "foo"
        let message = WKScriptMessageMock(name: "goToPage", body: page, url: url)
        let knownServices = CompleteKnownServicesMock(url: url,
                                                      javaScriptInteractionMode: JavaScriptInteractionMode.SilverThirdParty)
        
        knownServicesProvider?.knownServicesMock = knownServices
        
        // Act
        webViewDelegate?.userContentController(WKUserContentController(), didReceive: message)
        
        // Assert
        assert(homeViewController?.goToPageCalled == true)
        assert(homeViewController?.goToPageValue == page)
    }
    
    func test_userContentControllerGoToPage_WithJavaScriptInteractionModeNotSilverThirdParty_DoesNotCallControllerGoToPage() {
        // Arrange
        let url = "http://www.example.com"
        let page = "foo"
        let message = WKScriptMessageMock(name: "goToPage", body: page, url: url)
        let knownServices = CompleteKnownServicesMock(url: url,
                                                      javaScriptInteractionMode: JavaScriptInteractionMode.NhsApp)
        
        knownServicesProvider?.knownServicesMock = knownServices
        
        // Act
        webViewDelegate?.userContentController(WKUserContentController(), didReceive: message)
        
        // Assert
        assert(homeViewController?.goToPageCalled == false)
    }
}
