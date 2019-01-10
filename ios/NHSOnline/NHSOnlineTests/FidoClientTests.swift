import XCTest
@testable import NHSOnline

class FidoClientTests: XCTestCase {
    
    var fidoClient: FidoClient?
    var fidoRequestHandler: FidoRequestHandler?
    var webViewDelegate: WebViewDelegate?
    var knownServices: KnownServices?
    
    override func setUp() {
        super.setUp()
        
        fidoClient = FidoClient()
        fidoRequestHandler = FidoRequestHandler()
        
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    
    func test_validateUAFRequest_ItsCorrectlyAdded() {
        let jsonString = "{\"header\": {\"upv\": {\"minor\":1, \"major\": 1}, \"op\":\"Auth\", \"appID\": \"anAppId\"}, \"challenge\": \"aChallenge\", \"username\": \"aUsername\"}"
        let regRequestJSON = JSON(parseJSON: jsonString)
        
        if let regRequest = try? FidoRequest(with: regRequestJSON) {
            let appID = "anAppId"
            regRequest.header.appID = appID
            let facetId: String = "anAppId"
            regRequest.header.appID = fidoClient!.selectFacetIdWhenAppIDisEmpty(request: regRequest, facetId: facetId)
            XCTAssert(regRequest.header.appID == appID)
        }else{
            assertionFailure("Failed to create FidoRequest")
            return
        }
    
    }
    
    func test_validateUAFRequest_IsEmpty() {
        let jsonString = "{\"header\": {\"upv\": {\"minor\":1, \"major\": 1}, \"op\":\"Auth\", \"appID\": \"anAppId\"}, \"challenge\": \"aChallenge\", \"username\": \"aUsername\"}"
        let regRequestJSON = JSON(parseJSON: jsonString)
        
        if let regRequest = try? FidoRequest(with: regRequestJSON) {
            let facetId: String = "anAppId"
            regRequest.header.appID = fidoClient!.selectFacetIdWhenAppIDisEmpty(request: regRequest, facetId: facetId)
            XCTAssert(regRequest.header.appID == facetId)
        }else{
            assertionFailure("Failed to create FidoRequest")
            return
        }

    }
    
    func test_generateDeregistrationRequest(){
        let deregRequest = fidoRequestHandler!.generateDeRegisterRequest(keyId: "keyID", facetId: "facetID")
        XCTAssert(deregRequest.header.appID == "facetID")
        XCTAssert(deregRequest.authenticators[0].keyID == "keyID")
    }
    
}
