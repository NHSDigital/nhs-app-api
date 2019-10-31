import WebKit

class AppWebInterface {
    let webView: WKWebView?
    
    init(webView: WKWebView?) {
        self.webView = webView
    }
    
    func getNotificationsStatus(status: String) {
        dispatchEvent(event: "notifications/settingsStatus", args: "'\(status)'")
    }
    
    func extendSession() {
        dispatchEvent(event: "session/extend")
    }
    
    func logout() {
        dispatchEvent(event: "auth/logout")
    }
    
    func notificationsUnauthorised() {
        dispatchEvent(event: "notifications/unauthorised")
    }
    
    func notificationsAuthorised(devicePns: String, trigger: String) {
        let response = "'{\"devicePns\":\"\(devicePns)\",\"deviceType\":\"ios\",\"trigger\":\"\(trigger)\"}'"
        dispatchEvent(event: "notifications/authorised", args: response)
    }
    
    private func dispatchEvent(event: String, args: String = "") {
        var eventArgs = "'\(event)'"
        
        if !args.isEmpty {
            eventArgs += ", \(args)"
        }
        
        let eventString = "window.$nuxt.$store.dispatch(\(eventArgs));"
        DispatchQueue.main.async {
            self.webView?.evaluateJavaScript(eventString)
        }
    }
}
