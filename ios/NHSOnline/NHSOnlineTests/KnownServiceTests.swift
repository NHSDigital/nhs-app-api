import XCTest
@testable import NHSOnline

class KnownServiceTests: XCTestCase {
    let serviceError = ErrorMessage(title: "error title")
    let otherService = KnownServices.Service.OTHERS
    
    func test_AddPathInfo_GeneratesPathInfoForSpecifiedPath() {
        let paths = ["pathOne", "pathTwo", "pathThree"]
        let headers = ["HeaderOne", "HeaderTwo", "HeaderThree"]
        let testService = KnownService(serviceUrl: "http://localhost:3000", service: otherService, serviceError: serviceError)
        
        testService.addPathInfo(path: paths[0], service: otherService, title: headers[0])
        testService.addPathInfo(path: paths[1], service: otherService, title: headers[1])
        testService.addPathInfo(path: paths[2], service: otherService, title: headers[2])
        
        for index in 0..<paths.count {
            let pathInfo = testService.findMatchingServicePathInfoByPath(path: paths[index])
            XCTAssertNotNil(pathInfo)
            XCTAssertEqual(headers[index], pathInfo?.title)
        }
    }
    
    func test_AddPathInfo_WithOrWithoutStartingForwardSlashHasNoEffect() {
        let path1 = "pathOne"
        let path1WithSlash = "/\(path1)"
        let path2 = "pathTwo"
        let header1 = "HeaderOne"
        let header2 = "HeaderTwo"
        
        let testService = KnownService(serviceUrl: "http://localhost:3000", service: otherService, serviceError: serviceError)
        testService.addPathInfo(path: path1WithSlash, service: otherService, title: header1)
        testService.addPathInfo(path: path2, service: otherService, title: header2)
        
        let pathInfoOne = testService.findMatchingServicePathInfoByPath(path: path1)
        let pathInfoTwo = testService.findMatchingServicePathInfoByPath(path: path2)
        XCTAssertEqual(header1, pathInfoOne?.title)
        XCTAssertEqual(header2, pathInfoTwo?.title)
    }
    
    func test_AddPathInfo_EmptyPathOrPathWithJustSlashDoesNotOverrideDefaultPathInfo() {
        let emptyPath = ""
        let pathWithSlash = "/"
        let header1 = "HeaderOne"
        let header2 = "HeaderTwo"
        let defaultHeader = "DefaultHeader"
        
        let testService = KnownService(serviceUrl: "http://localhost:3000", service: otherService, serviceError: serviceError, title: defaultHeader , validateSession: false, allowNativeInteraction: false)
        
        testService.addPathInfo(path: emptyPath, service: otherService, title: header1)
        testService.addPathInfo(path: pathWithSlash, service: otherService, title: header2)
        
        let emptyPathInfo = testService.findMatchingServicePathInfo(urlString: "http://localhost:3000\(emptyPath)")
        let slashPathInfo = testService.findMatchingServicePathInfo(urlString: "http://localhost:3000\(pathWithSlash)")
        XCTAssertNotNil(emptyPathInfo)
        XCTAssertNotNil(slashPathInfo)
        XCTAssertNotEqual(header1, emptyPathInfo?.title)
        XCTAssertNotEqual(header2, slashPathInfo?.title)
        XCTAssertEqual(defaultHeader, emptyPathInfo?.title)
        XCTAssertEqual(defaultHeader, slashPathInfo?.title)
    }
    
    func test_KnownServiceContructor_UrlParamWithPathGeneratesPathInfoForThePathAndDefaultPathInfoWithoutTitle() {
        let url = "http://localhost:3000"
        let path = "url-path"
        let header = "Header"
        let testService = KnownService(serviceUrl: "\(url)/\(path)", service: otherService, serviceError: serviceError, title: header, validateSession: false, allowNativeInteraction: false)
        
        let defaultPathInfo = testService.findMatchingServicePathInfo(urlString: url)
        XCTAssertNotNil(defaultPathInfo)
        XCTAssertNil(defaultPathInfo?.title)
        let pathInfo = testService.findMatchingServicePathInfoByPath(path: path)
        XCTAssertNotNil(pathInfo)
        XCTAssertEqual(header, pathInfo?.title)
    }
    
    func test_FindMatchingServicePathInfo_ResolveToPathInfoMatchingToUrlString() {
        let path = "pathOne"
        let header = "Header"
        let testService = KnownService(serviceUrl: "http://localhost:3000/", service: otherService, serviceError: serviceError)
        testService.addPathInfo(path: path, service: otherService, title: header)
        let pathInfo = testService.findMatchingServicePathInfo(urlString: "http://localhost:3000/\(path)")
        XCTAssertNotNil(pathInfo)
        XCTAssertEqual(header, pathInfo?.title)
    }
    
    func test_FindMatchingServicePathInfoByPath_ResolveToPathInfoMatchingPath() {
        let path = "pathOne"
        let header = "Header"
        let testService = KnownService(serviceUrl: "http://localhost:3000/", service: otherService, serviceError: serviceError)
        testService.addPathInfo(path: path, service: otherService, title: header)
        let pathInfo = testService.findMatchingServicePathInfoByPath(path: path)
        XCTAssertNotNil(pathInfo)
        XCTAssertEqual(header, pathInfo?.title)
    }
    
    func test_FindMatchingServicePathInfoByPath_ResolveToPathInfoMatchingExactPath_ButNilWhenClosest_WhenExactPathParamSetToTrue() {
        let path1 = "pathOne"
        let closestPath = "\(path1)/extra-path"
        let header = "Header"
        let testService = KnownService(serviceUrl: "http://localhost:3000/", service: otherService, serviceError: serviceError)
        testService.addPathInfo(path: path1, service: otherService, title: header)
        let pathInfo1 = testService.findMatchingServicePathInfoByPath(path: path1, exactPathMatch: true)
        XCTAssertNotNil(pathInfo1)
        XCTAssertEqual(header, pathInfo1?.title)
        
        let pathInfo2 = testService.findMatchingServicePathInfoByPath(path: closestPath, exactPathMatch: true)
        XCTAssertNil(pathInfo2)
    }
    
    func test_FindMatchingServicePathInfo_ReturnsPathInfoWithMatchingPathOrClosestToThePath() {
        let path1 = "pathOne"
        let path2 = "\(path1)/extra/path"
        let header = "Header"
        let testService = KnownService(serviceUrl: "http://localhost:3000/", service: otherService, serviceError: serviceError)
        testService.addPathInfo(path: path1, service: otherService, title: header)
        
        let pathInfoOne = testService.findMatchingServicePathInfoByPath(path: path1)
        let pathInfoTwo = testService.findMatchingServicePathInfoByPath(path: path2)
        XCTAssertNotNil(pathInfoOne)
        XCTAssertNotNil(pathInfoTwo)
        XCTAssertEqual(pathInfoOne?.title, pathInfoTwo?.title)
    }
    
    func test_HasMissingQueryString_ReturnsFalse_ForValidQueryStringDifferentCase() {
        let testService = KnownService(serviceUrl: "http://localhost:3000", service: otherService, serviceError: serviceError, validateSession: false, allowNativeInteraction: false, urlQueryString: "?source=iOS")
        let hasMissing = testService.hasMissingQueryString(urlString: "http://localhost:3000?source=IOS")
        XCTAssertFalse(hasMissing)
    }
    
    func test_HasMissingQueryString_ReturnsTrue_ForValidQueryStringWithMismatchValue() {
        let testService = KnownService(serviceUrl: "http://localhost:3000", service: otherService, serviceError: serviceError, validateSession: false, allowNativeInteraction: false, urlQueryString: "?source=iOS")
        let hasMissing = testService.hasMissingQueryString(urlString: "http://localhost:3000?source=")
        XCTAssertTrue(hasMissing)
    }
    
    func test_HasMissingQueryString_ReturnsFalse_ForEmptyUrl() {
        let testService = KnownService(serviceUrl: "http://localhost:3000", service: otherService, serviceError: serviceError, validateSession: false, allowNativeInteraction: false, urlQueryString: "?source=iOS")
        let hasMissing = testService.hasMissingQueryString(urlString: "")
        XCTAssertFalse(hasMissing)
    }
    
    func test_AddingMissingQueryParameters_ReturnsFullUrl_ForServiceWithProvideQuery() {
        let testService = KnownService(serviceUrl: "http://localhost:3000", service: otherService, serviceError: serviceError, validateSession: false, allowNativeInteraction: false, urlQueryString: "?source=iOS")
        let fullUrl = testService.addingMissingQueryParameters(urlString: "http://localhost:3000")
        let expectedUlr = "http://localhost:3000?source=ios"
        XCTAssertEqual(expectedUlr, fullUrl)
    }
    
    func test_AddingMissingQueryParameters_ReturnsSameUrlString_ForServiceWithNoQuery() {
        let testService = KnownService(serviceUrl: "http://localhost:3000", service: otherService, serviceError: serviceError)
        let fullUrl = testService.addingMissingQueryParameters(urlString: "http://localhost:3000")
        XCTAssertEqual("http://localhost:3000", fullUrl)
    }
    
    func test_AddingMissingQueryParameters_ReturnsOriginalUrlWithMissingQuery_ForServiceWithAQuery() {
        let testService = KnownService(serviceUrl: "http://localhost:3000", service: otherService, serviceError: serviceError, urlQueryString:"?source=ios")
        let fullUrl = testService.addingMissingQueryParameters(urlString: "http://localhost:3000?param1=param1Value")
        XCTAssertEqual("http://localhost:3000?param1=param1Value&source=ios", fullUrl)
    }
}
