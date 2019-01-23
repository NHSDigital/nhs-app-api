import XCTest
@testable import NHSOnline

class LifecycleHandlersTests: XCTestCase {
    
    var lifecycleHandlers: LifecycleHandlers?
    var knownServices: KnownServices?
    var homeController: HomeViewController?
    var webViewController: WebViewController?
    var configurationService: MockConfigurationService?
    let queue = DispatchQueue(label: "MyTestQueue")
    
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
        lifecycleHandlers?.performAppVersionCheck(onQueue: queue)
        queue.sync {}
        XCTAssertTrue(lifecycleHandlers!.hasCheckedAppVersionSinceAppOpened)
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_isFalseIfUserDeviceIsAllowed() {
        self.configurationService?.isValidConfiguration = true
        lifecycleHandlers?.performAppVersionCheck(onQueue: queue)
        queue.sync {}
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
