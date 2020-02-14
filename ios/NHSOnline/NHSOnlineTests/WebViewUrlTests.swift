import XCTest
@testable import NHSOnline

class WebViewUrlTests: XCTestCase {
    
    var viewController: HomeViewController?
    var webViewDelegate: WebViewDelegate?
    var knownServices: KnownServices?
    
    override func setUp() {
        super.setUp()
        
        knownServices = KnownServices(nil)
        viewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        let webAppInterface = WebAppInterface(controller: viewController!)
        webViewDelegate = WebViewDelegate(controller: viewController!, knownServices: knownServices!, webAppInterface: webAppInterface)
    }
    
    override func tearDown() {
        super.tearDown()
    }

    func test_When_NhsAppSchemeUrlIsLoaded_Then_ItIsCorrectedToHttps() {
        let urlString = URL(string: config().AppScheme + "://test.com/test")
        let validatedUrl = webViewDelegate?.ensureSupportedScheme(urlString!);

         XCTAssertTrue(validatedUrl!.scheme=="https")
    }
    
    func test_When_KnownServiceIsNotFound_DefaultServiceIsReturned() {
        let urlString = "http://notknown.service.url/"
        let webViewUrl = URL(string: urlString)
        
        let result = knownServices?.findMatchingKnownService(webViewUrl)

        XCTAssertNotNil(result)
        XCTAssert(result is RootService)
        XCTAssert((result as! RootService).url == "")
    }

}
