import SwiftyJSON
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
    
    func leavePage() {
        dispatchEvent(event: "pageLeaveWarning/leavePage")
    }
    
    func logout() {
        dispatchEvent(event: "auth/logout")
    }
    
    func notificationsUnauthorised() {
        dispatchEvent(event: "notifications/unauthorised")
    }
    
    func notificationsAuthorised(devicePns: String, trigger: String) {
        let response = """
            {
                devicePns: '\(devicePns)',
                deviceType: 'ios',
                trigger: '\(trigger)'
            
            }
        """
        dispatchEvent(event: "notifications/authorised", args: response)
    }
    
    func paycassoResponseFailureCallback(isFaceMatched: Bool, errorCode: Int, errorMessage: String) {
        let responseJson = dictToJson(dict: [
            "isFaceMatched": isFaceMatched,
            "error": [
                "errorCode": errorCode,
                "errorMessage": errorMessage
            ] as [String: Any?]
        ])

        dispatchNHSLoginEvent(functionName: "paycassoOnFailure", functionArg: responseJson)
    }
    
    func paycassoCustomFailureCallback(isFaceMatched: Bool, errorMessage: String) {
        let responseJson = dictToJson(dict: [
            "isFaceMatched": isFaceMatched,
            "error": [
                "errorMessage": errorMessage
            ] as [String: Any?]
        ])

        dispatchNHSLoginEvent(functionName: "paycassoOnFailure", functionArg: responseJson)
    }
    
    func paycassoSuccessCallback(isFaceMatched: Bool, transactionId: String, transactionType: String) {
        let responseJson = dictToJson(dict: [
            "isFaceMatched": isFaceMatched,
            "transactionId": transactionId,
            "transactionType": transactionType
        ])

        dispatchNHSLoginEvent(functionName: "paycassoOnSuccess", functionArg: responseJson)
    }
    
    private func dictToJson(dict: [String: Any?]) -> String {
        return JSON(dict).rawString()!
    }

    func stayOnPage() {
        dispatchEvent(event: "pageLeaveWarning/stayOnPage")
    }
    
    private func dispatchEvent(event: String, args: String = "") {
        var eventArgs = "'\(event)'"
        
        if !args.isEmpty {
            eventArgs += ", \(args)"
        }
        
        let eventString = "window.$nuxt.$store.dispatch(\(eventArgs));"
        evaluateWebviewJavascript(javascriptText: eventString)
    }
    
    private func dispatchNHSLoginEvent(functionName: String, functionArg: String) {
        let eventString = "window.authentication.\(functionName)(\(functionArg));"
        evaluateWebviewJavascript(javascriptText: eventString)
    }
    
    private func evaluateWebviewJavascript(javascriptText: String) {
        DispatchQueue.main.async {
            self.webView?.evaluateJavaScript(javascriptText)
        }
    }
}
