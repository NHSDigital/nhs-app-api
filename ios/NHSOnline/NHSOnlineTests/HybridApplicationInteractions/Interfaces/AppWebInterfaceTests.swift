import SwiftyJSON
import WebKit
import XCTest

@testable import NHSOnline

class AppWebInterfacesTests: XCTestCase {
    var wKWebView: WKWebView?
    var mockWKWebView: WebViewMocks?
    var appWebInterface: AppWebInterface?

    override func setUp() {
        super.setUp()

        wKWebView = WKWebView(frame: .zero)
        mockWKWebView = WebViewMocks()
        
        appWebInterface = AppWebInterface.init(webView: mockWKWebView)
    }

    override func tearDown() {
        super.tearDown()
    }

    func test_whenBiometricCompletionIsTriggered_AndRegistrationSuccessfull_thenSuccessBiometricCompletionIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.biometricCompletion(action: "Register", outcome: "Success", errorCode: "")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.loginSettingsBiometricCompletion({\n    action: \'Register\',\n    outcome: \'Success\',\n    errorCode: \'\'\n});")
    }
    
    func test_whenBiometricSpecIsTriggered_AndTypeIsFaceID_thenBiometricSpecIsDispatchedWithFace(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.biometricSpec(biometricTypeReference: "face", enabled: false)
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.loginSettingsBiometricSpec({\n    biometricTypeReference: \'face\',\n    enabled: false\n});")
    }
    
    func test_whenBiometricSpecIsTriggered_AndTypeIsTouchID_thenBiometricSpecIsDispatchedWithTouch(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.biometricSpec(biometricTypeReference: "touch", enabled: false)
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.loginSettingsBiometricSpec({\n    biometricTypeReference: \'touch\',\n    enabled: false\n});" )
    }
    
    func test_whenBiometricLoginFailureIsTriggered_thenHandleBiometricLoginFailureIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.biometricLoginFailure()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.loginHandleBiometricLoginFailure();")
    }
    
    func test_whenGetNotificationsStatusIsTriggered_AndStatusIsAuthorised_thenSettingsStatusIsDispatchedWithAuthorised(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.getNotificationsStatus(status: "authorised")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.notificationsSettingsStatus(\'authorised\');")
    }
    
    func test_whenGetNotificationsStatusIsTriggered_AndStatusIsDenied_thenSettingsStatusIsDispatchedWithDenied(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.getNotificationsStatus(status: "denied")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.notificationsSettingsStatus(\'denied\');")
    }
    
    func test_whenGetNotificationsStatusIsTriggered_AndStatusIsNotDetermined_thenSettingsStatusIsDispatchedWithNotDetermined(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.getNotificationsStatus(status: "notDetermined")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.notificationsSettingsStatus(\'notDetermined\');")
    }

    func test_whenExtendSessionIsTriggered_thenSessionExtendIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.extendSession()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.sessionExtend();")
    }
    
    func test_whenLogoutIsTriggered_thenLogoutIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.logout()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.authLogout();")
    }
    
    func test_whenNotificationsUnauthorisedIsTriggered_thenNotificationsUnauthorisedIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.notificationsUnauthorised()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.notificationsUnauthorised();")
    }
    
    func test_whenNotificationsAuthorisedIsTriggered_AndDevicePnsAndTriggerArePassed_thenNotificationsAuthorisedIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.notificationsAuthorised(devicePns: "pns", trigger: "Test")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.notificationsAuthorised(    {\n        devicePns: \'pns\',\n        deviceType: \'ios\',\n        trigger: \'Test\'\n    \n    });")
    }
    
    func test_whenStayOnPageIsTriggered_thenStayOnPageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.stayOnPage()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.pageLeaveWarningStayOnPage();")
    }
    
    func test_whenLeavePageIsTriggered_thenLeavePageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.leavePage()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.nativeAppCallbacks.pageLeaveWarningLeavePage();")
    }
    
    func test_whenPaycassoSuccessCallbackIsTriggered_thenExpectedMessageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")

        appWebInterface!.paycassoSuccessCallback(isFaceMatched: false, transactionId: "test", transactionType: "DocuSureResponse")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        
        assertPaycassoResponse(
            expectedDict: [
                "isFaceMatched": false,
                "transactionId": "test",
                "transactionType": "DocuSureResponse"
            ],
            expectedMethodCall: "window.authentication.paycassoOnSuccess",
            actualJsString: mockWKWebView?.attemptedJSString
        )
    }
    
    func test_whenPaycassoResponseFailureCallbackCallbackIsTriggered_thenExpectedMessageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")

        appWebInterface!.paycassoResponseFailureCallback(isFaceMatched: false, errorCode: 100, errorMessage: "Test")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        
        assertPaycassoResponse(
            expectedDict: [
                "isFaceMatched": false,
                "error": [
                    "errorCode": 100,
                    "errorMessage": "Test"
                ]
            ],
            expectedMethodCall: "window.authentication.paycassoOnFailure",
            actualJsString: mockWKWebView?.attemptedJSString
        )
    }
    
    func test_whenPaycassoCustomFailureCallbackCallbackIsTriggered_thenExpectedMessageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")

        appWebInterface!.paycassoCustomFailureCallback(isFaceMatched: false, errorMessage: "Test")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }

        wait(for: [expectation], timeout: 10.0)

        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        
        assertPaycassoResponse(
            expectedDict: [
                "isFaceMatched": false,
                "error": [
                    "errorMessage": "Test"
                ]
            ],
            expectedMethodCall: "window.authentication.paycassoOnFailure",
            actualJsString: mockWKWebView?.attemptedJSString
        )
    }
    
    private func assertPaycassoResponse(expectedDict: [String: Any?], expectedMethodCall: String, actualJsString: String?) {
        let attemptedJsString = actualJsString!
        
        assert(attemptedJsString.starts(with: "\(expectedMethodCall)("))
            
        let jsonParam = attemptedJsString.replacingOccurrences(of: "\(expectedMethodCall)(", with: "")
            .replacingOccurrences(of: ");", with: "")
        
        if let dataFromString = jsonParam.data(using: .utf8, allowLossyConversion: false) {
            assertJsonDict(
                expectedDict: expectedDict,
                actualJsonDict: try! JSON(data: dataFromString)
            )
        }
    }
    
    private func assertJsonDict(expectedDict: [String: Any?], actualJsonDict: JSON) {
        for (key, val) in expectedDict {
            if let objVal = val as? [String: Any?] {
                assertJsonDict(expectedDict: objVal, actualJsonDict: actualJsonDict[key])
            }
            else if let intVal = val as? Int {
                assert(actualJsonDict[key].int! as Int == intVal)
            }
            else if let boolVal = val as? Bool {
                assert(actualJsonDict[key].bool! as Bool == boolVal)
            }
            else if let strVal = val as? String {
                assert(actualJsonDict[key].string! as String == strVal)
            }
        }
    }
}
