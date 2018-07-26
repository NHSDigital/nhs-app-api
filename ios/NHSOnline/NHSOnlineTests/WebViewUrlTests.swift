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
        let knownService = knownServices?.findMatchingKnownServiceForHostname(hostname: webViewUrl?.host)
        
        let correctUrl = knownService?.addingMissingQueryParameters(urlString: (webViewUrl?.absoluteString)!)
        
        XCTAssertTrue((correctUrl?.contains(config().NhsOnlineRequiredQueryString))!)
    }
    
    func test_When_KnownServiceContainsQueryString_Then_ItsNotAdded() {
        let urlString = config().HomeUrl + config().NhsOnlineRequiredQueryString
        let webViewUrl = URL(string: urlString)
        let knownService = knownServices?.findMatchingKnownServiceForHostname(hostname: webViewUrl?.host)
        
        let correctUrl = knownService?.addingMissingQueryParameters(urlString: (webViewUrl?.absoluteString)!)
        
        XCTAssertTrue((correctUrl?.elementsEqual(urlString))!)
    }
    
    func test_When_OrganDonationServiceIsUnavailable_Then_DisplaysCorrectErrorMessage() {
        let urlString = config().OrganDonationUrl
        let webViewUrl = URL(string: urlString)
        let errorMessage = knownServices?.getUnavailabilityErrorMessageForService(url: webViewUrl!)
        
        XCTAssertEqual(errorMessage?.title, NSLocalizedString("ConnectionErrorTitle", comment: ""))
    }

    
    func test_When_KnownServiceIsNotFoundNilIsReturned() {
        let urlString = "http://notknown.service.url/"
        let webViewUrl = URL(string: urlString)
        
        let knownService = knownServices?.findMatchingKnownServiceForHostname(hostname: webViewUrl?.host)
        
        XCTAssertNil(knownService)
    }
    
    func test_When_KnownServiceHeaderTitleIsRequested_ThenCorrectTitleForPathIsReturned() {
        let knownServices = KnownServices(config: config())
        let serviceUrlTitleDictionary = [
            config().HomeUrl : "",
            config().Nhs111Url:NSLocalizedString("NHS111Title", comment: ""),
            config().OrganDonationUrl: NSLocalizedString("OrganDonationTitle", comment: "")
        ]
        for (urlString, title) in serviceUrlTitleDictionary {
            let url = URL(string:urlString)
            guard let knownService = knownServices.findMatchingKnownServiceForHostname(hostname: url?.host) else {
                assertionFailure("known service not found for \(urlString)")
                return
            }
            XCTAssertEqual(knownService.getTitleFor(urlHost: url?.host), title, "expected title \(title ) for url \(urlString) not found")
        }
    }
}
