import XCTest
import Foundation
@testable import NHSOnline

class NotificationServicesTests: XCTestCase {
    var notificationsService: NotificationsService?
    var mockWKWebView: WebViewMocks?
    var cookieHandler: CookieHandlerMocks?
    var appWebInterface: AppWebInterfaceMocks?
    
    override func setUp() {
        super.setUp()
        appWebInterface = AppWebInterfaceMocks(webView: mockWKWebView)
        cookieHandler = CookieHandlerMocks()
        notificationsService = NotificationsService(appWebInterface: appWebInterface!, cookieHandler: cookieHandler!)
    }
    
    override func tearDown() {
       super.tearDown()
   }

    @available(iOS 12, *)
    func test_getNotificationsStatus_iOS12_ResolveToAuthorised_WhenNotificationSettingsIsAuthorized(){
        let status: UNAuthorizationStatus = .authorized
        let result = notificationsService!.getNotificationsStatus(with: compatibility.iOS12(), authorizationStatus: status)
        
        XCTAssertNotNil(result)
        XCTAssert(result == "authorised")
    }
    
    @available(iOS 12, *)
    func test_getNotificationsStatus_iOS12_ResolveToAuthorised_WhenNotificationSettingsIsDenied(){
        let status: UNAuthorizationStatus = .denied
        let result = notificationsService!.getNotificationsStatus(with: compatibility.iOS12(), authorizationStatus: status)
        
        XCTAssertNotNil(result)
        XCTAssert(result == "denied")
    }
    
    @available(iOS 12, *)
    func test_getNotificationsStatus_iOS12_ResolveToAuthorised_WhenNotificationSettingsIsNotDetermined(){
        let status: UNAuthorizationStatus = .notDetermined
        let result = notificationsService!.getNotificationsStatus(with: compatibility.iOS12(), authorizationStatus: status)
        
        XCTAssertNotNil(result)
        XCTAssert(result == "notDetermined")
    }
    
    @available(iOS 12, *)
    func test_getNotificationsStatus_iOS12_ResolveToAuthorised_WhenNotificationSettingsIsNotProvisional(){
        let status: UNAuthorizationStatus = .provisional
        let result = notificationsService!.getNotificationsStatus(with: compatibility.iOS12(), authorizationStatus: status)
        
        XCTAssertNotNil(result)
        XCTAssert(result == "authorised")
    }
    
    @available(iOS 10, *)
    func test_getNotificationsStatus_iOS10_ResolveToAuthorised_WhenNotificationSettingsIsAuthorized(){
        let status: UNAuthorizationStatus = .authorized
        let result = notificationsService!.getNotificationsStatus(with: compatibility.iOS10(), authorizationStatus: status)
       
        XCTAssertNotNil(result)
        XCTAssert(result == "authorised")
    }
    
    @available(iOS 10, *)
    func test_getNotificationsStatus_iOS10_ResolveToAuthorised_WhenNotificationSettingsIsNotDetrmined(){
        let status: UNAuthorizationStatus = .notDetermined
        let result = notificationsService!.getNotificationsStatus(with: compatibility.iOS10(), authorizationStatus: status)
       
        XCTAssertNotNil(result)
        XCTAssert(result == "notDetermined")
    }
    
    @available(iOS 10, *)
    func test_getNotificationsStatus_iOS10_ResolveToAuthorised_WhenNotificationSettingsIsDenied(){
        let status: UNAuthorizationStatus = .denied
        let result = notificationsService!.getNotificationsStatus(with: compatibility.iOS10(), authorizationStatus: status)
       
        XCTAssertNotNil(result)
        XCTAssert(result == "denied")
    }
}
