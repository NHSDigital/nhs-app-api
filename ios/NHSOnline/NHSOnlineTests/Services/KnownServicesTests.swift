import XCTest
import Foundation
@testable import NHSOnline

class KnownServicesTests: XCTestCase {
    var testSubServices: [SubService] = []
    var testRootServices: [RootService] = []
    var testKnownServices: KnownServices?

    override func setUp() {
        super.setUp()

        testSubServices.append(SubService.init(path: "/path", queryString: nil, javaScriptInteractionMode: .Unknown,
                menuTab: .Unknown, viewMode: .Unknown, validateSession: false))
        testSubServices.append(SubService.init(path: "/path/valid-subpath", queryString: nil, javaScriptInteractionMode: .Unknown,
                menuTab: .Unknown, viewMode: .Unknown, validateSession: false))
        testSubServices.append(SubService.init(path: nil, queryString: "foo=bar", javaScriptInteractionMode: .Unknown,
                menuTab: .Unknown, viewMode: .Unknown, validateSession: false))
        testSubServices.append(SubService.init(path: "/path", queryString: "foo=bar", javaScriptInteractionMode: .Unknown,
                menuTab: .Unknown, viewMode: .Unknown, validateSession: false))
        testSubServices.append(SubService.init(path: nil, queryString: "foo=bar&bar=ram", javaScriptInteractionMode: .Unknown, 
                menuTab: .Unknown, viewMode: .Unknown, validateSession: false))

        testRootServices.append(RootService.init(url: "https://test.com", javaScriptInteractionMode: .NhsApp, 
                menuTab: .Symptoms, viewMode: .AppTab, validateSession: false, subServices: testSubServices))

        testKnownServices = KnownServices.init(testRootServices)
    }

    override func tearDown() {
        super.tearDown()
    }

    func test_findMatchingSubService_ResolveToDefaultService_WhenUrlIsInvalid(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://invalidurl.com"))

        XCTAssertNotNil(result)
        XCTAssert(result is RootService)
        XCTAssert((result as! RootService).url == "")
    }

    func test_findMatchingSubService_ResolveToRootService_WhenUrlHasNoPathAndQuery(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com"))

        XCTAssertNotNil(result)
        XCTAssert(result is RootService)
        XCTAssert((result as! RootService).url == "https://test.com")
    }

    func test_findMatchingSubService_ResolveToRootService_WhenUrlHasInvalidPath(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/pathnotvalid/"))

        XCTAssertNotNil(result)
        XCTAssert(result is RootService)
        XCTAssert((result as! RootService).url == "https://test.com")
    }

    func test_findMatchingSubService_ResolveToRootService_WhenUrlHasInvalidQueryString(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/?query=invalid"))

        XCTAssertNotNil(result)
        XCTAssert(result is RootService)
        XCTAssert((result as! RootService).url == "https://test.com")
    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasValidPath(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/path"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == "/path")
        XCTAssert((result as! SubService).queryString == nil)
    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasPartialValidPath(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/path/subpath"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == "/path")
        XCTAssert((result as! SubService).queryString == nil)
    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasValidTwoLevelPath(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/path/valid-subpath"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == "/path/valid-subpath")
        XCTAssert((result as! SubService).queryString == nil)
    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasValidQueryString(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/?foo=bar"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == nil)
        XCTAssert((result as! SubService).queryString == "foo=bar")
    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasValidQueryStringUnmatchedPath(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/dave?foo=bar"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == nil)
        XCTAssert((result as! SubService).queryString == "foo=bar")
    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasValidPathAndQueryString(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/path?foo=bar"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == "/path")
        XCTAssert((result as! SubService).queryString == "foo=bar")
    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasPartialValidPathAndQueryString(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/path/nestedpath?foo=bar&ram=you"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == "/path")
        XCTAssert((result as! SubService).queryString == "foo=bar")
    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasPartialValidPathAndInvalidQueryString(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/path/nestedpath?aaa=bbb"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == "/path")
        XCTAssert((result as! SubService).queryString == nil)
    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasValidMultipleQueryStringParamaters(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/?foo=bar&bar=ram"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == nil)
        XCTAssert((result as! SubService).queryString == "foo=bar&bar=ram")

    }

    func test_findMatchingSubService_ResolveToSubService_WhenUrlHasValidMultipleQueryStringParametersInDifferentOrder(){
        let result = testKnownServices!.findMatchingKnownService(URL(string: "https://test.com/?bar=ram&foo=bar"))

        XCTAssertNotNil(result)
        XCTAssert(result is SubService)
        XCTAssert((result as! SubService).path == nil)
        XCTAssert((result as! SubService).queryString == "foo=bar&bar=ram")
    }

    func test_getRootServiceByHostAndScheme_ResolveToRootService_WhenUrlIsValid(){
        let result = testKnownServices!.getRootServiceByHostAndScheme(host: "test.com", scheme: "https")

        XCTAssertNotNil(result)
        XCTAssert(result!.url == "https://test.com")
    }

    func test_getRootServiceByHostAndScheme_ResolveToNil_WhenUrlIsNotValid(){
        let result = testKnownServices!.getRootServiceByHostAndScheme(host: "test.com", scheme: "http")

        XCTAssertNil(result)
    }
}
