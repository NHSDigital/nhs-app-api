import XCTest
@testable import NHSOnline

class LifecycleHandlersTests: XCTestCase {
    var lifecycleHandlers: MockLifecycleHandlers?
    var knownServices: KnownServices?
    var homeController: HomeViewController?
    var webViewController: WebViewController?
    let queue = DispatchQueue(label: "MyTestQueue")
    
    override func setUp() {
        super.setUp()
        knownServices = KnownServices.init(nil)
        homeController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        webViewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "WebViewController") as? WebViewController

        let configurationResponse = ConfigurationResponse(false, "", false)
        lifecycleHandlers = MockLifecycleHandlers(knownServices: knownServices!, webViewController: webViewController!, homeViewController: homeController!, configurationResponse: configurationResponse)
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    func test_ensureHasCheckedAppVersionSinceAppOpened_isInitializedToFalse() {
        XCTAssertFalse(lifecycleHandlers!.hasCheckedAppVersionSinceAppOpened)
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_verifyDisplayAppVersionOutOfDateIsCalled() {
        lifecycleHandlers?.performAppVersionCheck(onQueue: queue)
        queue.sync {}
        assert(lifecycleHandlers?.displayAppVersionOutOfDateWasCalled == true,
               "Expected the displayAppVersionOutOfDate() Method to be invoked")
    }
    
    func test_ensureValueForHasCheckedAppVersionSinceAppOpened_verifyDisplayAppVersionOutOfDateIsNotCalled() {
        let configurationResponse = ConfigurationResponse(true, "", true)
        lifecycleHandlers = MockLifecycleHandlers(knownServices: knownServices!, webViewController: webViewController!, homeViewController: homeController!, configurationResponse: configurationResponse)

        lifecycleHandlers?.performAppVersionCheck(onQueue: queue)
        queue.sync {}
        assert(lifecycleHandlers?.displayAppVersionOutOfDateWasCalled == false,
               "Expected the displayAppVersionOutOfDate() Method not to be invoked")
    }

    class MockLifecycleHandlers : LifecycleHandlers {
        var displayAppVersionOutOfDateWasCalled = false

        override init(knownServices: KnownServices, webViewController: WebViewController, homeViewController: HomeViewController, configurationResponse: ConfigurationResponse) {
            super.init(knownServices: knownServices, webViewController: webViewController, homeViewController: homeViewController, configurationResponse: configurationResponse)
        }
        override func displayAppVersionOutOfDate() {
            displayAppVersionOutOfDateWasCalled = true
        }
    }
}
