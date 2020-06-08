import WebKit

class AppWebInterface {
    let webView: WKWebView?
    
    init(webView: WKWebView?) {
        self.webView = webView
    }
    
    func biometricCompletion(action: String, outcome: String, errorCode: String) {
        let response = """
        {
            action: '\(action)',
            outcome: '\(outcome)',
            errorCode: '\(errorCode)'
        }
        """
        dispatchEvent(event: "loginSettings/biometricCompletion", args: response)
    }
    
    func biometricSpec(biometricTypeReference: String, enabled: Bool) {
        let response = """
        {
            biometricTypeReference: '\(biometricTypeReference)',
            enabled: \(enabled)
        }
        """
        dispatchEvent(event: "loginSettings/biometricSpec", args: response)
    }
    
    func biometricLoginFailure() {
        dispatchEvent(event: "login/handleBiometricLoginFailure")
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
    
    func goToPage(page: String) {
        dispatchEvent(event: "navigation/goToPage", args: "'\(page)'")
    }
    
    func notificationsAuthorised(devicePns: String, trigger: String) {
        let response = """
            {
                devicePns:'\(devicePns)',
                deviceType:'ios',
                trigger:'\(trigger)'
            
            }
        """
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
