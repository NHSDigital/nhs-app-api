import XCTest
import Foundation
@testable import NHSOnline

class UrlHelperTests: XCTestCase {
    func test_EnsureUrlWithScheme_WhenNotAUrl_ReturnsNil() {
        let nonUrl = "This is not a url"
        
        let result = UrlHelper.ensureUrlWithScheme(url: nonUrl)
        
        XCTAssertNil(result)
    }
    
    func test_EnsureUrlWithScheme_WhenSchemeMissing_ReturnsUrlWithDefaultScheme() {
        let url = "//www.bbc.co.uk"
        
        let result = UrlHelper.ensureUrlWithScheme(url: url)
        
        XCTAssertNotNil(result)
        XCTAssertEqual(result?.scheme, "https")        
    }
    
    func test_EnsureUrlWithScheme_WhenSchemeMissingAndHasPort_ReturnsUrlWithDefaultScheme() {
        let url = "//www.bbc.co.uk:3000"
        
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
        XCTAssertEqual(result?.scheme, config().BaseScheme)
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

    func test_IsSameSchemeAndHostAsHomeUrl_ReturnsTrueWhenUrlHostIsNhsoUrl() {
        let homeUrl = URL(string: config().HomeUrl)
        let homeUrlSameHostAsHome = UrlHelper.isSameSchemeAndHostAsHomeUrl(url: homeUrl)

        XCTAssertTrue(homeUrlSameHostAsHome)
    }

    func test_IsSameSchemeAndHostAsHomeUrl_ReturnsFalseWhenUrlHostIsNhs111Url() {
        let nhs111Url = URL(string: config().Nhs111Url)
        let nhs111UrlSameAsHomeHost = UrlHelper.isSameSchemeAndHostAsHomeUrl(url: nhs111Url)

        XCTAssertFalse(nhs111UrlSameAsHomeHost)
    }

    func test_isValidHomeUrl_ReturnsTrueWhenIsHomeUrlNoQueryString() {
        let homeUrl = URL(string: config().HomeUrl)
        let isValidHomeUrl = UrlHelper.isValidHomeUrl(url: homeUrl)

        XCTAssertTrue(isValidHomeUrl)
    }

    func test_isValidHomeUrl_ReturnsTrueWhenIsHomeUrlWithQueryString() {
        let homeUrl = URL(string: config().HomeUrl + "?param1=abc")
        let isValidHomeUrl = UrlHelper.isValidHomeUrl(url: homeUrl)

        XCTAssertTrue(isValidHomeUrl)
    }

    func test_isValidHomeUrl_ReturnsFalseWhenIsNotHomeUrl() {
        let homeUrl = URL(string: config().HomeUrl + "/appointments")
        let isValidHomeUrl = UrlHelper.isValidHomeUrl(url: homeUrl)

        XCTAssertFalse(isValidHomeUrl)
    }

    func test_isValidHomeUrl_ReturnsFalseWhenExternalUrl() {
        let homeUrl = URL(string: config().Nhs111Url)
        let isValidHomeUrl = UrlHelper.isValidHomeUrl(url: homeUrl)

        XCTAssertFalse(isValidHomeUrl)
    }

    func test_getReloadUrl_ReturnsNilWhenUrlIsNotDataPreferencesURL() {
        let url = URL(string: config().HomeUrl + "/some/path")
        let result = UrlHelper.getReloadUrl(url: url!)
        let expectedResult = url?.absoluteString

        XCTAssertTrue(result.absoluteString == expectedResult, "unexpected URL: " + result.absoluteString)
    }

    func test_getReloadUrl_ReturnsUrlWhenUrlIsDataPreferencesURL() {
        let url = URL(string: config().DataPreferencesURL)
        let result = UrlHelper.getReloadUrl(url: url!)
        let expectedResult = config().HomeUrl + "data-sharing"

        XCTAssertTrue(result.absoluteString == expectedResult, "unexpected URL: " + result.absoluteString)
    }
    
    func test_checkForUrlOverride_ReturnsUrlWhenUserDefaultsStandardUrlIsNil(){
        let url = "https://www.test.com"
        let result = UrlHelper.checkForUrlOverride(url: url)
        
        XCTAssertTrue(result == url, "unexpected URL: " + result)
    }
    
    func test_checkForUrlOverride_ReturnsUrlWhenUserDefaultsStandardUrlIsNotNil(){
        let url = "https://www.test.com"
        let overrideUrl = URL(string: "https://www.overrideurl.com")
        UserDefaults.standard.set(overrideUrl, forKey: config().LinkPropertyName)
        let result = UrlHelper.checkForUrlOverride(url: url)
        
        XCTAssertTrue(result == overrideUrl?.absoluteString, "unexpected URL: " + result)
    }
}
