import XCTest
import WebKit
@testable import NHSOnline

class WebViewDelegateTests: XCTestCase {
    var webViewDelegate: MockWebViewDelegate?
    
    override func setUp() {
        super.setUp()
        
        let storyboard = UIStoryboard(name: "Main", bundle: nil)
        let vc: HomeViewController = storyboard.instantiateViewController(withIdentifier: "HomeViewController") as! HomeViewController
        let ks: KnownServices = KnownServices(config: config())
        let wai: WebAppInterface = WebAppInterface(controller: vc)
        webViewDelegate = MockWebViewDelegate(controller: vc, knownServices: ks, webAppInterface: wai)
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
}

class MockWebViewDelegate : WebViewDelegate {

    var startActivityIndicatorWasCalled = false

    override init(controller: HomeViewController, knownServices: KnownServices, webAppInterface: WebAppInterface ) {
        super.init(controller: controller, knownServices: knownServices, webAppInterface: webAppInterface)
    }
    override func startActivityIndicator() {
        startActivityIndicatorWasCalled = true
    }
}
