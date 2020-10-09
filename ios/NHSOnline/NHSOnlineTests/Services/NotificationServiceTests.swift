import XCTest
import Foundation
import WebKit
@testable import NHSOnline

class NotifictionServiceTests: XCTestCase {
    
    var notificationService: NotificationsService?
    var cookieHandler: CookieHandlerMocks?
    var appWebInterface: AppWebInterfaceMocks?
    var mockWKWebView: WebViewMocks?
    let nhsLoginId = "loginId12345"
    
    override func setUp() {
        super.setUp()
        appWebInterface = AppWebInterfaceMocks(webView: mockWKWebView)
        cookieHandler = CookieHandlerMocks()
        notificationService = NotificationsService.init(appWebInterface: appWebInterface!, cookieHandler: cookieHandler!)
    }
    
    func test_addNotificationToCookie(){
        notificationService?.addNotificationCookie(nhsLoginId: nhsLoginId)
        
        XCTAssert(cookieHandler?.cookieExists(name: "nhso.notifications-prompt-\(nhsLoginId)") == true)
        XCTAssert(cookieHandler?.addedCookie == true)
    }
    
    func test_checkNotificationsCookieCalled_whenCookieHasNotBeenAdded_returnsFalseToWeb(){
        notificationService?.checkNotificationCookie(nhsLoginId: nhsLoginId)
        
        XCTAssert(cookieHandler?.addedCookie == false)
        XCTAssert(appWebInterface?.notificationCookieFound == false)
    }
    
    func test_checkNotificationsCookieCalled_whenCookieHasBeenAdded_returnsTrueToWeb(){
        notificationService?.addNotificationCookie(nhsLoginId: nhsLoginId)
        notificationService?.checkNotificationCookie(nhsLoginId: nhsLoginId)
        
        XCTAssert(cookieHandler?.addedCookie == true)
        XCTAssert(appWebInterface?.notificationCookieFound == true)
    }
}
