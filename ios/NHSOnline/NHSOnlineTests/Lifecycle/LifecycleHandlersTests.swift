import XCTest
@testable import NHSOnline

class LifecycleHandlersTests: XCTestCase {
    var knownServicesProvider: KnownServicesProtocol?
    var homeController: HomeViewController?
    var webViewController: WebViewController?
    let queue = DispatchQueue(label: "MyTestQueue")
    
    override func setUp() {
        super.setUp()
        knownServicesProvider = SuccessKnownServiceProtocolMock()
        homeController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        webViewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "WebViewController") as? WebViewController
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    func test_ensureHasCheckedAppVersionSinceAppOpened_isInitializedToFalse() {
        let configMock = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        let lifecycleHandlers = MockLifecycleHandlers(knownServiceProvider: knownServicesProvider!, webViewController: webViewController!, homeViewController: homeController!, configurationServiceProvider: configMock)
        XCTAssertFalse(lifecycleHandlers.hasCheckedAppVersionSinceAppOpened)
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_verifyDisplayAppVersionOutOfDateIsCalled() {
        let configMock = SuccessConfigurationProtocolMock(configurationResponse: SuccessfulInvalidAppVersionConfigurationResponseMock().instance)
        let lifecycleHandlers = MockLifecycleHandlers(knownServiceProvider: knownServicesProvider!, webViewController: webViewController!, homeViewController: homeController!, configurationServiceProvider: configMock)
        lifecycleHandlers.performAppVersionCheck(onQueue: queue)
        queue.sync {}
        assert(lifecycleHandlers.displayAppVersionOutOfDateWasCalled == true,
               "Expected the displayAppVersionOutOfDate() Method to be invoked")
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_verifyDisplayAppVersionOutOfDateIsNotCalled() {
        let configMock = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        let lifecycleHandlers = MockLifecycleHandlers(knownServiceProvider: knownServicesProvider!, webViewController: webViewController!, homeViewController: homeController!, configurationServiceProvider: configMock)

        lifecycleHandlers.performAppVersionCheck(onQueue: queue)
        queue.sync {}
        assert(lifecycleHandlers.displayAppVersionOutOfDateWasCalled == false,
               "Expected the displayAppVersionOutOfDate() Method not to be invoked")
    }

    class MockLifecycleHandlers : LifecycleHandlers {
        var displayAppVersionOutOfDateWasCalled = false

        override init(knownServiceProvider: KnownServicesProtocol, webViewController: WebViewController, homeViewController: HomeViewController, configurationServiceProvider: ConfigurationServiceProtocol) {
            super.init(knownServiceProvider: knownServiceProvider, webViewController: webViewController, homeViewController: homeViewController, configurationServiceProvider: configurationServiceProvider)
        }
        override func displayAppVersionOutOfDate() {
            displayAppVersionOutOfDateWasCalled = true
        }
    }
}
