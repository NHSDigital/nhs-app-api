import XCTest
@testable import NHSOnline

class WebViewUrlTests: XCTestCase {
    var viewController: MockHomeViewController?
    var webViewDelegate: WebViewDelegate?
    var knownServicesProvider: KnownServicesProtocol?
    var configurationServiceProvider: ConfigurationServiceProtocol?
    
    override func setUp() {
        super.setUp()

        knownServicesProvider = SuccessKnownServiceProtocolMock()
        configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        
        let viewController = MockHomeViewController()
        viewController.knownServicesProvider = SuccessKnownServiceProtocolMock()
        viewController.configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        let webAppInterface = WebAppInterface(controller: viewController)
        webViewDelegate = WebViewDelegate(controller: viewController, knownServiceProvider: knownServicesProvider!,
                                          configurationServiceProvider: configurationServiceProvider!,
                                          webAppInterface: webAppInterface)
    }
    
    override func tearDown() {
        super.tearDown()
    }

    func test_When_NhsAppSchemeUrlIsLoaded_Then_ItIsCorrectedToHttps() {
        let urlString = URL(string: config().AppScheme + "://test.com/test")
        let validatedUrl = webViewDelegate?.ensureSupportedScheme(urlString!);

         XCTAssertTrue(validatedUrl!.scheme == config().BaseScheme)
    }
    
    func test_When_KnownServiceIsNotFound_DefaultServiceIsReturned() {
        let urlString = "http://notknown.service.url/"
        let webViewUrl = URL(string: urlString)

        var result: KnownService? = nil
        switch knownServicesProvider?.getKnownServices() {
        case .success(let knownServices):
            result = knownServices.findMatchingKnownService(webViewUrl)
        default:
            XCTFail()
        }

        XCTAssertNotNil(result)
        XCTAssert(result is RootService)
        XCTAssert((result as! RootService).url == "")
    }
}
