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
        dispatchNativeAppCallback(function: "loginSettingsBiometricCompletion", args: response)
    }
    
    func biometricSpec(biometricTypeReference: String, enabled: Bool) {
        let response = """
        {
            biometricTypeReference: '\(biometricTypeReference)',
            enabled: \(enabled)
        }
        """
        dispatchNativeAppCallback(function: "loginSettingsBiometricSpec", args: response)
    }
    
    func biometricLoginFailure() {
        dispatchNativeAppCallback(function: "loginHandleBiometricLoginFailure")
    }
    
    func getNotificationsStatus(status: String) {
        dispatchNativeAppCallback(function: "notificationsSettingsStatus", args: "'\(status)'")
    }
    
    func extendSession() {
        dispatchNativeAppCallback(function: "sessionExtend")
    }
    
    func leavePage() {
        dispatchNativeAppCallback(function: "pageLeaveWarningLeavePage")
    }
    
    func logout() {
        dispatchNativeAppCallback(function: "authLogout")
    }
    
    func notificationsUnauthorised() {
        dispatchNativeAppCallback(function: "notificationsUnauthorised")
    }
    
    func notificationsAuthorised(devicePns: String, trigger: String) {
        let response = """
            {
                devicePns: '\(devicePns)',
                deviceType: 'ios',
                trigger: '\(trigger)'
            
            }
        """
        dispatchNativeAppCallback(function: "notificationsAuthorised", args: response)
    }
    
    func stayOnPage() {
        dispatchNativeAppCallback(function: "pageLeaveWarningStayOnPage")
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
    
    private func dispatchNativeAppCallback(function: String, args: String = "") {
        var eventArgs = ""
        
        if !args.isEmpty {
            eventArgs += "\(args)"
        }
        
        let eventString = "window.nativeAppCallbacks.\(function)(\(eventArgs));"
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
