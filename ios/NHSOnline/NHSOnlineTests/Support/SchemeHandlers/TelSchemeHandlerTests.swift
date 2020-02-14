import Foundation
import XCTest
@testable import NHSOnline

class TelchemeHandlerTests: XCTestCase {

    func test_handle_returnsTrue_WhenTelToUrl() {
        let url: URL = URL(string: "tel:someonesomewhere")!

        let systemUnderTest = TelSchemeHandler()
        let result = systemUnderTest.handle(url: url)

        XCTAssertTrue(result)
    }

    func test_handle_returnsFalse_WhenNotATelToUrl() {
        let url: URL = URL(string: "mailto:someonesomewhere")!

        let systemUnderTest = TelSchemeHandler()
        let result = systemUnderTest.handle(url: url)

        XCTAssertFalse(result)
    }
}
