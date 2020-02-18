import XCTest
import Foundation
@testable import NHSOnline

class UrlHelperTests : XCTestCase {
    
    func test_EnsureUrlWithScheme_WhenNotAUrl_ReturnsNil() {
        let nonUrl = "This is not a url"
        
        let result = UrlHelper.ensureUrlWithScheme(url: nonUrl)
        
        XCTAssertNil(result)
    }
    
    func test_EnsureUrlWithScheme_WhenSchemeMissing_ReturnsUrlWithDefaultScheme() {
        let url = "www.bbc.co.uk"
        
        let result = UrlHelper.ensureUrlWithScheme(url: url)
        
        XCTAssertNotNil(result)
        XCTAssertEqual(result?.scheme, "https")        
    }
    
    func test_EnsureUrlWithScheme_WhenSchemePresent_ReturnsUrlWithMatchingSchemeHttps() {
        let url = "https://www.bbc.co.uk"
        
        let result = UrlHelper.ensureUrlWithScheme(url: url)
        
        XCTAssertNotNil(result)
        XCTAssertEqual(result?.scheme, "https")
        XCTAssertEqual(result?.host, "www.bbc.co.uk")
    }
    
    func test_EnsureUrlWithScheme_WhenSchemePresent_ReturnsUrlWithMatchingSchemeHttp() {
        let url = "http://www.bbc.co.uk"
        
        let result = UrlHelper.ensureUrlWithScheme(url: url)
        
        XCTAssertNotNil(result)
        XCTAssertEqual(result?.scheme, "http")
        XCTAssertEqual(result?.host, "www.bbc.co.uk")
    }
    
    func test_EnsureUrlWithScheme_WhenSchemePresent_ReturnsUrlWithMatchingSchemeApp() {
        let url = "nhsapp://www.bbc.co.uk"
        
        let result = UrlHelper.ensureUrlWithScheme(url: url)
        
        XCTAssertNotNil(result)
        XCTAssertEqual(result?.scheme, "https")
        XCTAssertEqual(result?.host, "www.bbc.co.uk")
    }
    
    func test_EnsureUrlWithScheme_WhenSchemePresent_ReturnsUrlWithMatchingSchemeEmail() {
        let url = "mailto:email@inbox.com"
        
        let result = UrlHelper.ensureUrlWithScheme(url: url)
        
        XCTAssertNotNil(result)
        XCTAssertEqual(result?.scheme, "mailto")
    }
    
    func test_EnsureUrlWithScheme_WhenSchemePresent_ReturnsUrlWithMatchingSchemeTel() {
        let url = "tel:01234567"
        
        let result = UrlHelper.ensureUrlWithScheme(url: url)
        
        XCTAssertNotNil(result)
        XCTAssertEqual(result?.scheme, "tel")
    }
}
