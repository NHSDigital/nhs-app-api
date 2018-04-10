import XCTest
@testable import NHSOnline

class WebViewUrlTests: XCTestCase {
    
    var viewController: HomeViewController?
    var webViewDelegate: WebViewDelegate?
    var knownServices: KnownServices?
    
    override func setUp() {
        super.setUp()
        
        knownServices = KnownServices(config: config())
        viewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        webViewDelegate = WebViewDelegate(controller: viewController!, knownServices: knownServices!)
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    func test_When_KnownServiceIsMissingQueryString_Then_ItsCorrectlyAdded() {
        let urlString = config().HomeUrl
        let webViewUrl = URL(string: urlString);
        let knownService = knownServices?.findMatchingKnownServiceFor(url: webViewUrl!)
        
        let correctUrl = knownService?.addingMissingQueryParameters(urlString: (webViewUrl?.absoluteString)!)
        
        XCTAssertTrue((correctUrl?.contains(config().NhsOnlineRequiredQueryString))!)
    }
    
    func test_When_KnownServiceContainsQueryString_Then_ItsNotAdded() {
        let urlString = config().HomeUrl + config().NhsOnlineRequiredQueryString
        let webViewUrl = URL(string: urlString)
        let knownService = knownServices?.findMatchingKnownServiceFor(url: webViewUrl!)
        
        let correctUrl = knownService?.addingMissingQueryParameters(urlString: (webViewUrl?.absoluteString)!)
        
        XCTAssertTrue((correctUrl?.elementsEqual(urlString))!)
    }
    
    func test_When_KnownServiceIsNotFoundNilIsReturned() {
        let urlString = "http://notknown.service.url/"
        let webViewUrl = URL(string: urlString)
        
        let knownService = knownServices?.findMatchingKnownServiceFor(url: webViewUrl!)
        
        XCTAssertNil(knownService)
    }
}
