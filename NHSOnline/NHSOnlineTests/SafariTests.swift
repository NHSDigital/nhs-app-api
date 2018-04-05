import XCTest
@testable import NHSOnline

class NHSOnlineTests: XCTestCase {
    
    var viewController: HomeViewController?
    var webViewDelegate: WebViewDelegate?
    
    override func setUp() {
        super.setUp()
        
        let knownServices = KnownServices(config: config())
        viewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        webViewDelegate = WebViewDelegate(controller: viewController!, knownServices: knownServices)
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    func When_HostIsKnown_Then_PageShouldBeOpenedInWebView() {
        let webViewUrl = URL(string: "https://111.nhs.uk/Home/Find");
        let displayedInWebView = webViewDelegate?.shouldOpenInSafari(url: webViewUrl!)
        
        XCTAssertFalse(displayedInWebView!)
    }
    
    func When_HostIsUnknown_Then_PageShouldBeOpenedInSafari() {
        let webViewUrl = URL(string: "https://outside.url.com");
        let displayedInWebView = webViewDelegate?.shouldOpenInSafari(url: webViewUrl!)
        
        XCTAssertTrue(displayedInWebView!)
    }
}
