import XCTest
@testable import NHSOnline

class NHSOnlineTests: XCTestCase {
    
    var viewController: HomeViewController?
    var webViewDelegate: WebViewDelegate?
    
    override func setUp() {
        super.setUp()
        
        let knownServices = KnownServices(config: config())
        viewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        let webAppInterface = WebAppInterface(controller: viewController!)
        webViewDelegate = WebViewDelegate(controller: viewController!, knownServices: knownServices, webAppInterface: webAppInterface)
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    func test_When_UrlIsForSignIn_Then_PageShouldBeOpenedInWebView() {
        let webViewUrl = URL(string: "https://ext." + config().CidUrlSuffix + "/")
        let displayedInWebView = webViewDelegate?.shouldOpenInSafari(url: webViewUrl!)

        XCTAssertFalse(displayedInWebView!)
    }
    
    func test_When_HostIsKnown_Then_PageShouldBeOpenedInWebView() {
        let webViewUrl = URL(string: config().Nhs111Url);
        let displayedInWebView = webViewDelegate?.shouldOpenInSafari(url: webViewUrl!)
        
        XCTAssertFalse(displayedInWebView!)
    }
    
    func test_When_HostIsUnknown_Then_PageShouldBeOpenedInSafari() {
        let webViewUrl = URL(string: "https://outside.url.com");
        let displayedInWebView = webViewDelegate?.shouldOpenInSafari(url: webViewUrl!)
        
        XCTAssertTrue(displayedInWebView!)
    }
}
