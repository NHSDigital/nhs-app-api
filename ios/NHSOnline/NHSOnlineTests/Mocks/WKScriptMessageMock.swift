import WebKit

class WKScriptMessageMock: WKScriptMessage {
    private let _body: Any
    private let _name: String
    private let _frameInfo: WKFrameInfoMock
    
    init(name: String, body: Any, url: String) {
        self._body = body
        self._name = name
        self._frameInfo = WKFrameInfoMock(url: url)
    }
    
    override var body: Any { get { return _body } }

    override var name: String { get { return _name } }
    
    override var frameInfo: WKFrameInfo { get { return _frameInfo } }
}
