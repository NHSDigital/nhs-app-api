import XCTest
import WebKit

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
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'loginSettings/biometricCompletion\', {\n    action: \'Register\',\n    outcome: \'Success\',\n    errorCode: \'\'\n});")
    }
    
    func test_whenBiometricSpecIsTriggered_AndTypeIsFaceID_thenBiometricSpecIsDispatchedWithFace(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.biometricSpec(biometricTypeReference: "face", enabled: false)
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'loginSettings/biometricSpec\', {\n    biometricTypeReference: \'face\',\n    enabled: false\n});")
    }
    
    func test_whenBiometricSpecIsTriggered_AndTypeIsTouchID_thenBiometricSpecIsDispatchedWithTouch(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.biometricSpec(biometricTypeReference: "touch", enabled: false)
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'loginSettings/biometricSpec\', {\n    biometricTypeReference: \'touch\',\n    enabled: false\n});" )
    }
    
    func test_whenBiometricLoginFailureIsTriggered_thenHandleBiometricLoginFailureIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.biometricLoginFailure()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'login/handleBiometricLoginFailure\');")
    }
    
    func test_whenGetNotificationsStatusIsTriggered_AndStatusIsEnabled_thenSettingsStatusIsDispatchedWithEnabled(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.getNotificationsStatus(status: "Enabled")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'notifications/settingsStatus\', \'Enabled\');")
    }
    
    func test_whenGetNotificationsStatusIsTriggered_AndStatusIsDisabled_thenSettingsStatusIsDispatchedWithDisabled(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.getNotificationsStatus(status: "Disabled")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'notifications/settingsStatus\', \'Disabled\');")
    }
    
    func test_whenExtendSessionIsTriggered_thenSessionExtendIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.extendSession()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'session/extend\');")
    }
    
    func test_whenLogoutIsTriggered_thenLogoutIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.logout()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'auth/logout\');")
    }
    
    func test_whenNotificationsUnauthorisedIsTriggered_thenNotificationsUnauthorisedIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.notificationsUnauthorised()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'notifications/unauthorised\');")
    }
    
    func test_whenNotificationsAuthorisedIsTriggered_AndDevicePnsAndTriggerArePassed_thenNotificationsAuthorisedIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.notificationsAuthorised(devicePns: "pns", trigger: "Test")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'notifications/authorised\',     {\n        devicePns:\'pns\',\n        deviceType:\'ios\',\n        trigger:\'Test\'\n    \n    });"    )
    }
    
    func test_whenStayOnPageIsTriggered_thenStayOnPageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.stayOnPage()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'pageLeaveWarning/stayOnPage\');")
    }
    
    func test_whenLeavePageIsTriggered_thenLeavePageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        appWebInterface!.leavePage()
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == "window.$nuxt.$store.dispatch(\'pageLeaveWarning/leavePage\');")
    }
    
    func test_whenPaycassoSuccessCallbackIsTriggered_thenExpectedMessageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        let expectedString = "window.authentication.paycassoOnSuccess(`{\n    \"isFaceMatched\": false,\n    \"transactionId\": \"test\",\n    \"transactionType\": \"DocuSureResponse\"\n}`);"
        
        appWebInterface!.paycassoSuccessCallback(isFaceMatched: false, transactionId: "test", transactionType: "DocuSureResponse")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == expectedString)
    }
    
    func test_whenPaycassoResponseFailureCallbackCallbackIsTriggered_thenExpectedMessageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        let expectedString = "window.authentication.paycassoOnFailure(`{\n    \"isFaceMatched\": false,\n    \"error\": {\n        \"errorCode\": 100,\n        \"errorMessage\": \"Test\"\n    }\n}`);"
        
        appWebInterface!.paycassoResponseFailureCallback(isFaceMatched: false, errorCode: 100, errorMessage: "Test")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == expectedString)
    }
    
    func test_whenPaycassoCustomFailureCallbackCallbackIsTriggered_thenExpectedMessageIsDispatched(){
        let expectation = self.expectation(description: "dispatched to main queue")
        let expectedString = "window.authentication.paycassoOnFailure(`{\n    \"isFaceMatched\": false,\n    \"error\": {\n        \"errorMessage\": \"Test\"\n    }\n}`);"
        
        appWebInterface!.paycassoCustomFailureCallback(isFaceMatched: false, errorMessage: "Test")
        
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        assert(mockWKWebView?.attemptedEvaluateJavaScript == true)
        assert(mockWKWebView?.attemptedJSString == expectedString)
    }
}
