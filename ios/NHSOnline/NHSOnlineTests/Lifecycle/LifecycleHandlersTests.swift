import XCTest
@testable import NHSOnline

class LifecycleHandlersTests: XCTestCase {
    
    var lifecycleHandlers: MockLifecycleHandlers?
    var knownServices: KnownServices?
    var homeController: HomeViewController?
    var mockConfigurationService: MockConfigurationService?
    var webViewController: WebViewController?
    let queue = DispatchQueue(label: "MyTestQueue")
    
    override func setUp() {
        super.setUp()
        
        knownServices = KnownServices(config: config())
        
        homeController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        
        webViewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "WebViewController") as? WebViewController
        
        mockConfigurationService = MockConfigurationService()
        
        lifecycleHandlers = MockLifecycleHandlers(knownServices: knownServices!, webViewController: webViewController!, homeViewController: homeController!)
        lifecycleHandlers?.configurationService = mockConfigurationService
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    func test_ensureHasCheckedAppVersionSinceAppOpened_isInitializedToFalse() {
        XCTAssertFalse(lifecycleHandlers!.hasCheckedAppVersionSinceAppOpened)
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_verifyDisplayAppVersionOutOfDateIsCalled() {
        self.mockConfigurationService?.isValidConfiguration = false
        lifecycleHandlers?.performAppVersionCheck(onQueue: queue)
        queue.sync {}
        assert(lifecycleHandlers?.displayAppVersionOutOfDateWasCalled == true,
               "Expected the displayAppVersionOutOfDate() Method to be invoked")
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_verifyDisplayAppVersionOutOfDateIsNotCalled() {
        self.mockConfigurationService?.isValidConfiguration = true
        lifecycleHandlers?.performAppVersionCheck(onQueue: queue)
        queue.sync {}
        assert(lifecycleHandlers?.displayAppVersionOutOfDateWasCalled == false,
               "Expected the displayAppVersionOutOfDate() Method not to be invoked")
    }
    
    class MockConfigurationService: ConfigurationServiceProtocol {
        var isValidConfiguration = false
        
        func isUserDeviceAllowed(homeViewController: HomeViewController, completionHandler: @escaping (ConfigurationResponse?) -> Void) {
            
            let response = ConfigurationResponse(isValidConfiguration, "", false)
            
            completionHandler(response)
        }
    }
    
    class MockLifecycleHandlers : LifecycleHandlers {
        var displayAppVersionOutOfDateWasCalled = false

        override init(knownServices: KnownServices, webViewController: WebViewController, homeViewController: HomeViewController) {
            super.init(knownServices: knownServices, webViewController: webViewController, homeViewController: homeViewController)
        }
        override func displayAppVersionOutOfDate() {
            displayAppVersionOutOfDateWasCalled = true
        }
    }
}
