import WebKit

class WKFrameInfoMock: WKFrameInfo {
    private let _request: URLRequest
    
    init(url: String) {
        self._request = URLRequest(url: URL(string: url)!)
    }
    
    override var request: URLRequest { get { return _request } }
}
