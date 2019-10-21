import XCTest
import WebKit
@testable import NHSOnline

class WebViewDelegateTests: XCTestCase {
    var webViewDelegate: MockWebViewDelegate?
    
    var wKWebView: WKWebView?
    var wKNavigation: WKNavigation?
    var error: NSError?
    var nSURLErrorCancelled: NSError?
    var vc: HomeViewController?
    
    override func setUp() {
        super.setUp()
        
        
        let storyboard = UIStoryboard(name: "Main", bundle: nil)
        vc = storyboard.instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        let ks: KnownServices = KnownServices(config: config())
        let wai: WebAppInterface = WebAppInterface(controller: vc!)
        webViewDelegate = MockWebViewDelegate(controller: vc!, knownServices: ks, webAppInterface: wai)
        
        wKWebView = WKWebView(frame: .zero)
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
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: nSURLErrorCancelled!)
        
        assert(webViewDelegate?.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
        
        assert(vc!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithOtherNSError_VerifyStopActivityIndicatorIsCalled() {
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: error!)
        
        assert(webViewDelegate?.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
        
        assert(vc!.applicationState.isReady() == true)
    }
    
    func test_pageIsNotResponding_VerifyStopActivityIndicatorIsCalled(){
        webViewDelegate?.pageIsNotResponding()
        
        assert(webViewDelegate?.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
    }
}

class MockWebViewDelegate : WebViewDelegate {

    var startActivityIndicatorWasCalled = false
    var stopActivityIndicatorWasCalled = false

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
