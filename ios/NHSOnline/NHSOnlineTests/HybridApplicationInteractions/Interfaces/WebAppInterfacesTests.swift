import XCTest
import WebKit
@testable import NHSOnline

class WebAppInterfacesTests: XCTestCase {
    var wKWebView: WKWebView?
    var mockHomeViewControllerWebApp: HomeViewControllerMocks?
    var webAppInterface: WebAppInterface?

    override func setUp() {
        super.setUp()
        
        mockHomeViewControllerWebApp = HomeViewControllerMocks()
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
