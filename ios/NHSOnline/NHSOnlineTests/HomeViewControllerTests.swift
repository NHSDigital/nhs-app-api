import XCTest
import WebKit
@testable import NHSOnline

class HomeViewControllerTests: XCTestCase {
    var vcHome: HomeViewController!
    var testWebview: WKWebView!

    override func setUp() {
        super.setUp()
        
        let storyboard = UIStoryboard(name: "Main", bundle: nil)
        let vc: HomeViewController = storyboard.instantiateViewController(withIdentifier: "HomeViewController") as! HomeViewController
        testWebview = WKWebView()
        vcHome = vc
        _ = vcHome.view
    }
    
    func test_hasCidUrlSuffix_nilUrl_Returns_False() {
        let testResult = vcHome.hasCidUrlSuffix(webview: testWebview)
       
        XCTAssertFalse(testResult)
    }

    func test_hasCidUrlSuffix_NormalUrl_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: "https://www.google.com/")!))
        
        let testResult = vcHome.hasCidUrlSuffix(webview: testWebview)
        
        XCTAssertFalse(testResult)
    }
    
    func test_hasCidUrlSuffix_CidSuffixUrl_Returns_True() {
        testWebview.load(URLRequest(url: (URL(string: "https://www.google.com/")?.appendingPathComponent(config().CidUrlSuffix))!))
        
        let testResult = vcHome.hasCidUrlSuffix(webview: testWebview)
        
        XCTAssertTrue(testResult)
    }
    
    func test_isCheckYourSymptomsPath_RandomUrl_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: "https://www.google.com/")!))
        
        let testResult = vcHome.isCheckYourSymptomsPath(webview: testWebview)
        
        XCTAssertFalse(testResult)
    }
    
    func test_isCheckYourSymptomsPath_BaseHostNoPath_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: config().HomeUrl)!))
        
        let testResult = vcHome.isCheckYourSymptomsPath(webview: testWebview)
        
        XCTAssertFalse(testResult)
    }
    
    func test_isCheckYourSymptomsPath_NilUrl_Returns_False() {
        let testResult = vcHome.isCheckYourSymptomsPath(webview: testWebview)
        
        XCTAssertFalse(testResult)
    }
    
    func test_isCheckYourSymptomsPath_CorrectBaseHost_AndSymptomsPath_Returns_True() {
        let symptomsPath = config().CheckSymptomsUrlPath

        testWebview.load(URLRequest(url: ((URL(string: config().HomeUrl)?.appendingPathComponent(String(symptomsPath.dropFirst())))!)))
        
        let testResult = vcHome.isCheckYourSymptomsPath(webview: testWebview)
        
        XCTAssertTrue(testResult)
    }
}
