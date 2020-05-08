import XCTest
import WebKit

@testable import NHSOnline

class WebAppInterfacesTests: XCTestCase {
    var wKWebView: WKWebView?
    var mockHomeViewControllerWebApp: MockHomeViewControllerWebApp?
    var webAppInterface: WebAppInterface?

    override func setUp() {
        super.setUp()
        
        mockHomeViewControllerWebApp = MockHomeViewControllerWebApp()
        webAppInterface = WebAppInterface(controller: mockHomeViewControllerWebApp!)

    }

    override func tearDown() {
        super.tearDown()
    }

    func test_whenOnLogoutIsTriggered_thenTheAlertPresentedIsDismissed(){
        webAppInterface!.onLogout()
        assert(mockHomeViewControllerWebApp?.alertDismissed == true)
    }
}

class MockHomeViewControllerWebApp: HomeViewController {
    var alertDismissed = false
    
    override func dimissAlert() {
        alertDismissed = true;
    }
}
