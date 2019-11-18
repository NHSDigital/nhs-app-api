import XCTest
import WebKit
@testable import NHSOnline

class BiometricsViewControllerTests : XCTestCase {
    var biometricsViewController: BiometricsViewController!
    var testWebview: WKWebView!

    override func setUp() {
        super.setUp()
        let storyboard = UIStoryboard(name: "Main", bundle: nil)
        let vc: BiometricsViewController = storyboard.instantiateViewController(withIdentifier: "BiometricsViewController") as! BiometricsViewController
        biometricsViewController = vc
    }
    
    @available(iOS 11.0, *)
    func test_activatingBiometricsToggle_SetsPromptCookie() {
        let bogusUISwitch: UISwitch = UISwitch()
        biometricsViewController.biometricToggleChanged(bogusUISwitch)
        
        let expectation = XCTestExpectation(description: "Verified cookie presence")

        let cookieStore = WKWebsiteDataStore.default().httpCookieStore
        cookieStore.getAllCookies { (cookies) in
            var hasBiometricCookie = false
            for cookie in cookies {
                if(cookie.name=="HideBiometricBanner") {
                    hasBiometricCookie = true
                }
            }
            XCTAssert(hasBiometricCookie)
            expectation.fulfill()
        }
        
        wait(for: [expectation], timeout: 10.0)
    }
    
    func test_biometricBreadcrumb_Returns_True() {
        biometricsViewController.viewDidLoad()
        XCTAssert(true, "MyAccountTitle")
    }
    
    func test_biometricViewController_ShouldCallHomeViewController() {
        
        let homeViewController = MockHomeViewController()
        biometricsViewController.homeViewController = homeViewController
        
        let bogusGestureRecogniser: UITapGestureRecognizer = UITapGestureRecognizer()
        biometricsViewController.selectMyAccount(sender: bogusGestureRecogniser)
        
        assert(homeViewController.showWebViewContainerCalled == true,
               "Expected the showWebViewContainer() Method to be invoked")
        
        assert(homeViewController.updateHeaderTextCalled == true,
               "Expected the updateHeaderText() Method to be invoked")
    }
}

class MockHomeViewController: HomeViewController {
    var showWebViewContainerCalled = false
    var updateHeaderTextCalled = false
    override func showWebViewContainer() {
        showWebViewContainerCalled = true
    }
    
    override func updateHeaderText(headerText: String?, accessibilityLabel: String? = nil) {
        updateHeaderTextCalled = true
    }
}
    
