import WebKit

class AppWebInterface {
    let webView: WKWebView?
    
    init(webView: WKWebView?) {
        self.webView = webView
    }
    
    func logout() {
        dispatchEvent(event: "auth/logout")
    }
    
    func extendSession() {
        dispatchEvent(event: "session/extend")
    }
    
    private func dispatchEvent(event: String) {
        let eventString = "window.$nuxt.$store.dispatch('\(event)');"
        
        webView?.evaluateJavaScript(eventString)
    }
}
