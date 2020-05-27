import WebKit
@testable import NHSOnline

class WebViewMocks : WKWebView {
    var attemptedEvaluateJavaScript = false
    var attemptedJSString: String = ""
    var webViewController: WebViewController?
    
    override open func evaluateJavaScript(_ javaScriptString: String, completionHandler: ((Any?, Error?) -> Void)? = nil) {
        attemptedEvaluateJavaScript =  true;
        attemptedJSString = javaScriptString;
    }
}
