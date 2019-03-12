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

    func test_When_KnownServiceIsMissingQueryString_Then_ItsCorrectlyAdded() {
        let urlString = config().HomeUrl
        let webViewUrl = URL(string: urlString);
        let knownService = knownServices?.findMatchingKnownService(url: webViewUrl)
        
        let correctUrl = knownService?.addingMissingQueryParameters(urlString: (webViewUrl?.absoluteString)!)
        
        XCTAssertTrue((correctUrl?.contains(config().NhsOnlineRequiredQueryString))!)
    }
    
    func test_When_KnownServiceHasFragment_Then_HasMissingQueryStringReturnsFalse() {
        let urlString = config().HomeUrl + "#test"
        let webViewUrl = URL(string: urlString);
        let knownService = knownServices?.findMatchingKnownService(url: webViewUrl)
        
        let result = knownService?.hasMissingQueryString(urlString: webViewUrl!.absoluteString)
        
        XCTAssertFalse(result!)
    }
    
    func test_When_KnownServiceHasFragment_Then_QueryStringIsNotAdded() {
        let urlString = config().HomeUrl + "#test"
        let webViewUrl = URL(string: urlString);
        let knownService = knownServices?.findMatchingKnownService(url: webViewUrl)
        
        let resultUrl = knownService?.addingMissingQueryParameters(urlString: (webViewUrl?.absoluteString)!)
        
        XCTAssertFalse((resultUrl?.contains(config().NhsOnlineRequiredQueryString))!)
        XCTAssertEqual(resultUrl, urlString)
    }
    
    func test_When_KnownServiceContainsQueryString_Then_ItsNotAdded() {
        let urlString = config().HomeUrl + config().NhsOnlineRequiredQueryString
        let webViewUrl = URL(string: urlString)
        let knownService = knownServices?.findMatchingKnownService(url: webViewUrl)
        
        let correctUrl = knownService?.addingMissingQueryParameters(urlString: (webViewUrl?.absoluteString)!)
        
        XCTAssertEqual(urlString, correctUrl)
    }
    
    func test_When_KnownServiceIsNotFoundNilIsReturned() {
        let urlString = "http://notknown.service.url/"
        let webViewUrl = URL(string: urlString)
        
        let knownService = knownServices?.findMatchingKnownService(url: webViewUrl)
        
        XCTAssertNil(knownService)
    }
    
    func test_When_KnownServiceHeaderTitleIsRequested_ThenCorrectTitleForPathIsReturned() {
        let serviceUrlTitleDictionary = [
            config().HomeUrl : NSLocalizedString("HomeTitle", comment: ""),
            config().Nhs111Url:NSLocalizedString("NHS111Title", comment: ""),
        ]
        for (urlString, title) in serviceUrlTitleDictionary {
            let url = URL(string:urlString)
            guard let knownServiceInfo = knownServices?.findMatchingKnownServiceInfo(url: url) else {
                assertionFailure("known service not found for \(urlString)")
                return
            }
            XCTAssertEqual( title,knownServiceInfo.title, "expected title \(title ) for url \(urlString) not found")
        }
    }
}
