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
    
    func notificationsUnauthorised() {
        dispatchEvent(event: "notifications/unAuthorised")
    }

    func notificationsAuthorised(devicePns: String) {
        let response = "{\"devicePns\":\"\(devicePns)\",\"deviceType\":\"ios\"}"
        dispatchEvent(event: "notifications/authorised", args: response)
    }

    private func dispatchEvent(event: String, args: Any = "") {
        let eventString = "window.$nuxt.$store.dispatch('\(event)', '\(args)');"
        DispatchQueue.main.async {
            self.webView?.evaluateJavaScript(eventString)
        }
    }
}
