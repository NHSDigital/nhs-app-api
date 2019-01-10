import XCTest
@testable import NHSOnline

class LifecycleHandlersTests: XCTestCase {
    
    var lifecycleHandlers: LifecycleHandlers?
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
        
        lifecycleHandlers = LifecycleHandlers(knownServices: knownServices!, webViewController: webViewController!, homeViewController: homeController!, configurationService: mockConfigurationService! as ConfigurationServiceProtocol)
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    func test_ensureHasCheckedAppVersionSinceAppOpened_isInitializedToFalse() {
        XCTAssertFalse(lifecycleHandlers!.hasCheckedAppVersionSinceAppOpened)
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_isTrueIfUserDeviceIsNotAllowed() {
        self.mockConfigurationService?.isValidConfiguration = false
        lifecycleHandlers?.performAppVersionCheck(onQueue: queue)
        queue.sync {}
        XCTAssertTrue(lifecycleHandlers!.hasCheckedAppVersionSinceAppOpened)
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_isFalseIfUserDeviceIsAllowed() {
        self.mockConfigurationService?.isValidConfiguration = true
        lifecycleHandlers?.performAppVersionCheck(onQueue: queue)
        queue.sync {}
        XCTAssertFalse(lifecycleHandlers!.hasCheckedAppVersionSinceAppOpened)
    }
    
    class MockConfigurationService: ConfigurationServiceProtocol {
        
        var isValidConfiguration = false
        
        func isUserDeviceAllowed(homeViewController: HomeViewController, completionHandler: @escaping (ConfigurationResponse?) -> Void) {
            
            let response = ConfigurationResponse(isValidConfiguration, false, "", false)
            
            completionHandler(response)
        }
    }
}
