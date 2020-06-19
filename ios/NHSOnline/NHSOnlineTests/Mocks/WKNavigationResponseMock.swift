import WebKit

class WKNavigationResponseMock: WKNavigationResponse {
    private var _response: URLResponse
    
    init(statusCode: Int) {
        self._response = HTTPURLResponse(url: URL(string: "www.example.com")!, statusCode: statusCode, httpVersion: nil, headerFields: nil)!
    }
    
    override var response: URLResponse { get { return _response } }
}
