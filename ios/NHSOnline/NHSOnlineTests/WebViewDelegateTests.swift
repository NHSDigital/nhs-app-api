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
    var vc: HomeViewController?
    
    override func setUp() {
        super.setUp()        
        
        let storyboard = UIStoryboard(name: "Main", bundle: nil)
        vc = storyboard.instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        let ks: KnownServices = KnownServices.init(nil)
        let wai: WebAppInterface = WebAppInterface(controller: vc!)
        webViewDelegate = MockWebViewDelegate(controller: vc!, knownServices: ks, webAppInterface: wai)
        
        iProovWebViewDelegate = IProovMockWebViewDelegate(controller: vc!, knownServices: ks, webAppInterface: wai)

        wKWebView = WKWebView(frame: .zero)
        mockWKWebView = MockWKWebView()
        wKNavigation = WKNavigation()
        nSURLErrorCancelled = NSError(domain:NSURLErrorDomain,code:-999,userInfo:[NSLocalizedDescriptionKey:""])
        error = NSError(domain:NSURLErrorDomain,code:-1000,userInfo:[NSLocalizedDescriptionKey:""])

    }
    
    func test_WhenPageLoadIsNormal_VerifyStartActivityIndicatorIsCalled() {
        webViewDelegate?.checkPageLoadOriginAndStartActivityIndicator()
        
        assert(webViewDelegate?.startActivityIndicatorWasCalled == true,
               "Expected the startActivityIndicator() Method to be invoked")
    }
    
    func test_WhenPageLoadIsFromBackButton_VerifyStartActivityIndicatorIsNotCalled() {
        webViewDelegate?.viewController.goingBack = true
        
        webViewDelegate?.checkPageLoadOriginAndStartActivityIndicator()
        
        assert(webViewDelegate?.startActivityIndicatorWasCalled == false,
               "startActivityIndicator() Method should not be invoked")
    }
    
    func test_webViewDidFailProvisionalNavigationWithNSURLErrorCancelled_VerifyStopActivityIndicatorIsCalled() {
        wKWebView?.loadPage(url: config().HomeUrl)
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: nSURLErrorCancelled!)
        
        assert(webViewDelegate?.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
        
        assert(vc!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithNSURLErrorCancelled_VerifyStopActivityIndicatorIsNotCalled() {
        wKWebView?.loadPage(url: config().HomeUrl + "auth-return?code=something")
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: nSURLErrorCancelled!)
        
        assert(webViewDelegate?.stopActivityIndicatorWasCalled == false,
               "Expected the stopActivityIndicator() Method to not have been invoked")
        
        assert(vc!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithOtherNSError_VerifyStopActivityIndicatorIsCalled() {
        wKWebView?.loadPage(url: config().HomeUrl)
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: error!)
        
        assert(webViewDelegate?.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
        
        assert(vc!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithOtherNSError_VerifyStopActivityIndicatorIsNotCalled() {
        wKWebView?.loadPage(url: config().HomeUrl + "auth-return?code=something")
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: error!)
        
        assert(webViewDelegate?.stopActivityIndicatorWasCalled == false,
               "Expected the stopActivityIndicator() Method to not have been invoked")
        
        assert(vc!.applicationState.isReady() == true)
    }
    
    func test_pageIsNotResponding_VerifyStopActivityIndicatorIsCalled(){
        webViewDelegate?.pageIsNotResponding()
        
        assert(webViewDelegate?.stopActivityIndicatorWasCalled == true,
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

    var startActivityIndicatorWasCalled = false
    var stopActivityIndicatorWasCalled = false
    
    var attemptedIProovLaunch = false

    override init(controller: HomeViewController, knownServices: KnownServices, webAppInterface: WebAppInterface ) {
        super.init(controller: controller, knownServices: knownServices, webAppInterface: webAppInterface)
    }
    override func startActivityIndicator() {
        startActivityIndicatorWasCalled = true
    }
    
    override func stopActivityIndicator() {
        stopActivityIndicatorWasCalled = true
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
