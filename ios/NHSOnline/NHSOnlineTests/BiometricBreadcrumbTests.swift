import XCTest
@testable import NHSOnline

class BiometricBreadcrumTests: XCTestCase {
    let biometricBreadCrumb: BiometricBreadcrumb = BiometricBreadcrumb()
    
    func test_BiometricBreadcrumb_shows_MyAccountText() {
        let accountLinktext = "Back"
        biometricBreadCrumb.setupBreadcrumb()
        XCTAssertEqual(accountLinktext, biometricBreadCrumb.biometricLabel.attributedText?.string)
    }
}
