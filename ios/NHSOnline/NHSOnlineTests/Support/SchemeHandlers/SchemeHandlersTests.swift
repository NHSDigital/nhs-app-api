import Foundation
import XCTest
@testable import NHSOnline

class SchemeHandlersTests: XCTestCase {
    func test_handle_ReturnsTrue_WhenUrlIsHandled() {
        let url: URL = URL(string: "mailto:someonesomewhere")!
        
        let systemUnderTest = SchemeHandlers()
        systemUnderTest.registerHandler(handler: MailToSchemeHandler())
        let result = systemUnderTest.handleUrl(url: url)
        
        XCTAssertTrue(result)
    }
    
    func test_handle_ReturnsFalse_WhenNoHandlersAreRegistered() {
        let url: URL = URL(string: "mailto:someonesomewhere")!
        
        let systemUnderTest = SchemeHandlers()
        let result = systemUnderTest.handleUrl(url: url)
        
        XCTAssertFalse(result)
    }
    
    func test_handle_ReturnsFalse_WhenNoMatchingHandlerIsRegistered() {
        let url: URL = URL(string: "tel:someonesomewhere")!
        
        let systemUnderTest = SchemeHandlers()
        systemUnderTest.registerHandler(handler: MailToSchemeHandler())
        let result = systemUnderTest.handleUrl(url: url)
        
        XCTAssertFalse(result)
    }
    
    func test_handle_ReturnsFalse_WhenUrlIsMalformed() {
        let url: URL = URL(string: "asdfawerqefwefwadfwe")!
        
        let systemUnderTest = SchemeHandlers()
        systemUnderTest.registerHandler(handler: MailToSchemeHandler())
        let result = systemUnderTest.handleUrl(url: url)
        
        XCTAssertFalse(result)
    }
}
