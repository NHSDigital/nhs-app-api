import Foundation
import XCTest
@testable import NHSOnline

class MailToSchemeHandlerTests: XCTestCase { 
    
    func test_handle_returnsTrue_WhenMailToUrl() {
        let url: URL = URL(string: "mailto:someonesomewhere")!
        
        let systemUnderTest = MailToSchemeHandler()
        let result = systemUnderTest.handle(url: url)
        
        XCTAssertTrue(result)        
    }
    
    func test_handle_returnsFalse_WhenNotAMailToUrl() {
        let url: URL = URL(string: "tel:someonesomewhere")!
        
        let systemUnderTest = MailToSchemeHandler()
        let result = systemUnderTest.handle(url: url)
        
        XCTAssertFalse(result)
    }
}
