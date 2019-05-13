import XCTest
import WebKit
@testable import NHSOnline

class BiometricsViewControllerTests : XCTestCase {
    var biometricsViewController: BiometricsViewController!
    
    override func setUp() {
        super.setUp()
        
        let storyboard = UIStoryboard(name: "Main", bundle: nil)
        let vc: BiometricsViewController = storyboard.instantiateViewController(withIdentifier: "BiometricsViewController") as! BiometricsViewController
        
        biometricsViewController = vc
    }
    
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
}
