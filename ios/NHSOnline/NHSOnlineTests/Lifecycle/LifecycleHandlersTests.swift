import XCTest
@testable import NHSOnline

class LifecycleHandlersTests: XCTestCase {
    
    var lifecycleHandlers: LifecycleHandlers?
    var knownServices: KnownServices?
    var homeController: HomeViewController?
    var webViewController: WebViewController?
    var configurationService: MockConfigurationService?
    
    override func setUp() {
        super.setUp()
        
        knownServices = KnownServices(config: config())
        
        homeController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        
        webViewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "WebViewController") as? WebViewController
        
        configurationService = MockConfigurationService(homeViewController: homeController!)
        
        lifecycleHandlers = LifecycleHandlers(knownServices: knownServices!, webViewController: webViewController!, configurationService: configurationService!)
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    func test_ensureHasCheckedAppVersionSinceAppOpened_isInitializedToFalse() {
        XCTAssertFalse(lifecycleHandlers!.hasCheckedAppVersionSinceAppOpened)
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_isTrueIfUserDeviceIsNotAllowed() {
        self.configurationService?.isValidConfiguration = false
        let expectation = XCTestExpectation(description: "version check complete")
        lifecycleHandlers?.performAppVersionCheck(completionHandler: {
            expectation.fulfill()
        })
        wait(for: [expectation], timeout: TimeInterval(exactly: 10)!)
        XCTAssertTrue(lifecycleHandlers!.hasCheckedAppVersionSinceAppOpened)
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_isFalseIfUserDeviceIsAllowed() {
        self.configurationService?.isValidConfiguration = true
        let expectation = XCTestExpectation(description: "version check complete")
        lifecycleHandlers?.performAppVersionCheck(completionHandler: {
            expectation.fulfill()
        })
        wait(for: [expectation], timeout: TimeInterval(exactly: 10)!)
        XCTAssertFalse(lifecycleHandlers!.hasCheckedAppVersionSinceAppOpened)
    }
    
    class MockConfigurationService: ConfigurationService {
        var isValidConfiguration = false
        override func isUserDeviceAllowed(completionHandler: @escaping (ConfigurationResponse) -> Void) {
            
            let response = ConfigurationResponse(
                isValidConfiguration: isValidConfiguration,
                isThrottlingEnabled: false)
            
            completionHandler(response)
        }
    }
}
