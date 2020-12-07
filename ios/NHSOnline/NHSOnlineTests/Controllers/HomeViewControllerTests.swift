import XCTest
import WebKit
@testable import NHSOnline

class HomeViewControllerTests: XCTestCase {
    var testWebview: WKWebView!
    var vcHome: HomeViewController!
    var mockApplicationState: ApplicationStateMocks!
    var mockSplashScreen: SplashScreenViewMocks!
    var mockProgressSpinner: ProgressSpinnerViewMocks!
    var mockBiometricService: BiometricServiceMocks!
    var appWebInterface: AppWebInterfaceMocks?
    var fidoClient: FidoClientMocks?
    var laContextMock: LocalAuthenticationMocks?
    var fileDownloadHelperMock: FileDownloadHelperMock?
    var dataDownloadAlertStringHandlerMock: DataDownloadAlertStringHandlerMock?

    let app = XCUIApplication.self

    override func setUp() {
        super.setUp()

        if let viewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController {
            viewController.knownServicesProvider = KnownServicesProtocolMocks.success()
            viewController.configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)

            vcHome = viewController
        }

        testWebview = WKWebView()
        _ = vcHome.view
        
        appWebInterface = AppWebInterfaceMocks(webView: testWebview)
        
        fidoClient =  FidoClientMocks()
        
        laContextMock = LocalAuthenticationMocks()

        mockApplicationState = ApplicationStateMocks()
        vcHome?.applicationState = mockApplicationState
        
        mockSplashScreen = SplashScreenViewMocks()
        vcHome?.splashScreen = mockSplashScreen
        
        mockProgressSpinner = ProgressSpinnerViewMocks()
        vcHome?.progressSpinner = mockProgressSpinner
                
        vcHome?.appWebInterface = appWebInterface
        
        fileDownloadHelperMock = FileDownloadHelperMock()
        vcHome?.fileDownloader = fileDownloadHelperMock
        
        dataDownloadAlertStringHandlerMock = DataDownloadAlertStringHandlerMock()
        
        vcHome?.dataDownloadAlertHandler = dataDownloadAlertStringHandlerMock
    }

    func test_hasCidUrlSuffix_nilUrl_Returns_False() {
        let testResult = vcHome.hasCidUrlSuffix(webview: testWebview)

        XCTAssertFalse(testResult)
    }

    func test_hasCidUrlSuffix_NormalUrl_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: "https://www.google.com/")!))

        let testResult = vcHome.hasCidUrlSuffix(webview: testWebview)

        XCTAssertFalse(testResult)
    }

    func test_hasCidUrlSuffix_CidSuffixUrl_Returns_True() {
        testWebview.load(URLRequest(url: (URL(string: "https://www.google.com/")?.appendingPathComponent(config().CidUrlSuffix))!))

        let testResult = vcHome.hasCidUrlSuffix(webview: testWebview)

        XCTAssertTrue(testResult)
    }

    func test_isCheckYourSymptomsPath_RandomUrl_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: "https://www.google.com/")!))

        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().CheckSymptomsUrlPath)

        XCTAssertFalse(testResult)
    }

    func test_isCheckYourSymptomsPath_BaseHostNoPath_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: config().HomeUrl)!))

        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().CheckSymptomsUrlPath)

        XCTAssertFalse(testResult)
    }

    func test_isCheckYourSymptomsPath_NilUrl_Returns_False() {
        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().CheckSymptomsUrlPath)

        XCTAssertFalse(testResult)
    }

    func test_isCheckYourSymptomsPath_CorrectBaseHost_AndSymptomsPath_Returns_True() {
        let symptomsPath = config().CheckSymptomsUrlPath

        testWebview.load(URLRequest(url: ((URL(string: config().HomeUrl)?.appendingPathComponent(String(symptomsPath.dropFirst())))!)))

        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().CheckSymptomsUrlPath)

        XCTAssertTrue(testResult)
    }
    
    func test_isPreRegistrationPath_RandomUrl_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: "https://www.google.com/")!))

        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().PreRegistrationInstructionsPath)

        XCTAssertFalse(testResult)
    }

    func test_isPreRegistrationPath_BaseHostNoPath_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: config().HomeUrl)!))

        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().PreRegistrationInstructionsPath)

        XCTAssertFalse(testResult)
    }

    func test_isPreRegistrationPath_NilUrl_Returns_False() {
        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().PreRegistrationInstructionsPath)

        XCTAssertFalse(testResult)
    }

    func test_isPreRegistrationPath_CorrectBaseHost_AndSymptomsPath_Returns_True() {
        let preRegPath = config().PreRegistrationInstructionsPath

        testWebview.load(URLRequest(url: ((URL(string: config().HomeUrl)?.appendingPathComponent(String(preRegPath.dropFirst())))!)))

        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().PreRegistrationInstructionsPath)

        XCTAssertTrue(testResult)
    }
    
    func test_isBiometricErrorPage_RandomUrl_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: "https://www.google.com/")!))

        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().BiometricLoginErrorPath)

        XCTAssertFalse(testResult)
    }

    func test_isBiometricErrorPage_BaseHostNoPath_Returns_False() {
        testWebview.load(URLRequest(url: URL(string: config().HomeUrl)!))

        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().BiometricLoginErrorPath)

        XCTAssertFalse(testResult)
    }

    func test_isBiometricErrorPage_NilUrl_Returns_False() {
        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().BiometricLoginErrorPath)

        XCTAssertFalse(testResult)
    }

    func test_isBiometricErrorPage_CorrectBaseHost_AndBiometricErrorPath_Returns_True() {
        let biometricPath = config().BiometricLoginErrorPath

        testWebview.load(URLRequest(url: ((URL(string: config().HomeUrl)?.appendingPathComponent(String(biometricPath.dropFirst())))!)))

        let testResult = vcHome.checkCurrentUrlForPath(webview: testWebview, urlPath: config().BiometricLoginErrorPath)

        XCTAssertTrue(testResult)
    }

    func test_delayedBiometricsStart_shouldClearMenuBar() {
        testWebview.load(URLRequest(url: ((URL(string:
            config().HomeUrl + "login")!))))
        vcHome.selectedTab = 1

        vcHome.checkForLoginPageAndTriggerBiometricTimer(testWebview, 10)

        XCTAssert(vcHome?.selectedTab == nil)
    }

    func test_biometricsShownIfAppVersionIsValidAndBiometricsIsAvailable() {
        vcHome.knownServicesProvider = KnownServicesProtocolMocks.success()
        vcHome.configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)

        vcHome.attemptBiometricLoginIfAppVersionValid()

        assert(vcHome.biometricsHasBeenAttempted == true)
    }

    func test_biometricsNotShownIfAppVersionIsInvalid() {
        vcHome.knownServicesProvider = KnownServicesProtocolMocks.success()
        vcHome.configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessfulInvalidAppVersionConfigurationResponseMock().instance)

        vcHome.attemptBiometricLoginIfAppVersionValid()

        assert(vcHome.biometricsHasBeenAttempted == false)
    }
    
    func test_downloadFile_error() {
        fileDownloadHelperMock?.setDownloadFileResponse(outcome: DownloadOutcome.ERROR)
        
        vcHome.downloadFile(messageBody:"test message")
        let child = vcHome?.children[0]
        let length = vcHome?.children.count ?? 0

        XCTAssert( length > 0, "DownloadFile Error Test No Child View Added" )
        XCTAssert( child as? PageUnavailabilityViewController != nil, "DownloadFile Error Test Child is wrong Type")
    }
    
    func test_downloadFile_notSupported() {
        
        XCTAssert(dataDownloadAlertStringHandlerMock?.getDownloadAlertWasCalled == false)
        
        fileDownloadHelperMock?.setDownloadFileResponse(outcome: DownloadOutcome.NOT_SUPPORTED)
        vcHome.downloadFile(messageBody:"test message")
        
        XCTAssert(dataDownloadAlertStringHandlerMock?.getDownloadAlertWasCalled == true)
    }


    func test_showWebViewContainer_isBlockedisFalse() {
        vcHome.showWebViewContainer()
        assert(self.mockApplicationState.isBlocked == false)
    }
    
    func test_showErrorViewContainer_hidesProgressAndSplashScreen() {
        vcHome.showErrorViewContainer()
        
        DispatchQueue.main.asyncAfter(deadline: .now() + 0.2, execute: {
            assert(self.mockProgressSpinner.isVisible == false)
            assert(self.mockSplashScreen.isVisible == false)
        })
    }
    
    func test_whenHandleBiometricRegistrationIsCalled_AndBiometricStateIsNotRegistered_willCallRegisterSuccessfully() {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        laContextMock?.biometricsCapable = true
        
        mockBiometricService = BiometricServiceMocks(homeViewController: vcHome,
                                                     configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
        vcHome?.biometricService = mockBiometricService
        
        let expectation = self.expectation(description: "dispatched to main queue")
        
        vcHome.handleBiometricStatusChangeRequest(biometricState: BiometricState.Not_Registered)
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        
        XCTAssert(mockBiometricService.calledRegister == true)
    }
    
    func test_whenHandleBiometricRegistrationIsCalled_AndBiometricStateIsRegistered_willCallDeregisterSuccessfully() {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        laContextMock?.biometricsCapable = true
        
        mockBiometricService = BiometricServiceMocks(homeViewController: vcHome,
                                                     configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
        vcHome?.biometricService = mockBiometricService
        
        let expectation = self.expectation(description: "dispatched to main queue")
        
        vcHome.handleBiometricStatusChangeRequest(biometricState: BiometricState.Registered)
        
        vcHome.handleBiometricStatusChangeRequest(biometricState: BiometricState.Not_Registered)
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        
        XCTAssert(mockBiometricService.calledDeRegister == true)
    }
    
    func test_whenHandleBiometricRegistrationIsCalled_AndBiometricStateIsUnavailable_willNotMakeAnyCallsToWeb() {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        laContextMock?.biometricsCapable = false
        
        mockBiometricService = BiometricServiceMocks(homeViewController: vcHome,
                                                     configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
        vcHome?.biometricService = mockBiometricService
        
        mockBiometricService.biometricsAvailable = false
        
        let expectation = self.expectation(description: "dispatched to main queue")
        
        vcHome.handleBiometricStatusChangeRequest(biometricState: BiometricState.Registered)
        
        vcHome.handleBiometricStatusChangeRequest(biometricState: BiometricState.Not_Registered)
        DispatchQueue.main.async{
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: 10.0)
        
        XCTAssert(mockBiometricService.calledDeRegister == false)
        XCTAssert(mockBiometricService.calledRegister == false)
    }
    
    func test_whenHandleBiometricSpecRequestIsCalled_AndBiometricStateIsNotRegistered_AndBiometricTypeIsFace_willReturnBiometricSpecWithFaceAndNotEnabled() {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        laContextMock?.biometricsCapable = true
        laContextMock?.updateShouldUseFaceId(shouldUse: true)
        
        
        mockBiometricService = BiometricServiceMocks(homeViewController: vcHome,
                                                     configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
        vcHome?.biometricService = mockBiometricService
        
        vcHome?.handleBiometricSpecRequest(biometricAvailability: BiometricState.Not_Registered)
        
        XCTAssert(appWebInterface?.biometricSpecRequestCalled == true)
        XCTAssert(appWebInterface?.biometricEnabled == false)
        XCTAssert(appWebInterface?.biometricTypeRef == "face")
    }
    
    func test_whenHandleBiometricSpecRequestIsCalled_AndBiometricStateIsRegistered_AndBiometricTypeIsFace_willReturnBiometricSpecWithFaceAndEnabled() {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        laContextMock?.biometricsCapable = true
        
        
        mockBiometricService = BiometricServiceMocks(homeViewController: vcHome,
                                                     configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
        vcHome?.biometricService = mockBiometricService
        
        vcHome?.handleBiometricSpecRequest(biometricAvailability: BiometricState.Registered)
        
        XCTAssert(appWebInterface?.biometricSpecRequestCalled == true)
        XCTAssert(appWebInterface?.biometricEnabled == true)
        XCTAssert(appWebInterface?.biometricTypeRef == "face")
    }

    func test_whenHandleBiometricSpecRequestIsCalled_AndBiometricStateIsNotRegistered_AndBiometricTypeIsTouch_willReturnBiometricSpecWithTouchAndNotEnabled() {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        laContextMock?.biometricsCapable = true
        laContextMock?.updateShouldUseFaceId(shouldUse: false)
        
        
        mockBiometricService = BiometricServiceMocks(homeViewController: vcHome,
                                                     configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
        vcHome?.biometricService = mockBiometricService
        
        vcHome?.handleBiometricSpecRequest(biometricAvailability: BiometricState.Not_Registered)
        
        XCTAssert(appWebInterface?.biometricSpecRequestCalled == true)
        XCTAssert(appWebInterface?.biometricEnabled == false)
        XCTAssert(appWebInterface?.biometricTypeRef == "touch")
    }
    
    func test_whenHandleBiometricSpecRequestIsCalled_AndBiometricStateIsRegistered_AndBiometricTypeIsTouch_willReturnBiometricSpecWithTouchAndEnabled() {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        laContextMock?.biometricsCapable = true
        laContextMock?.updateShouldUseFaceId(shouldUse: false)
        
        
        mockBiometricService = BiometricServiceMocks(homeViewController: vcHome,
                                                     configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
        vcHome?.biometricService = mockBiometricService
        
        vcHome?.handleBiometricSpecRequest(biometricAvailability: BiometricState.Registered)
        
        XCTAssert(appWebInterface?.biometricSpecRequestCalled == true)
        XCTAssert(appWebInterface?.biometricEnabled == true)
        XCTAssert(appWebInterface?.biometricTypeRef == "touch")
    }
    
    func test_whenHandleBiometricSpecRequestIsCalled_AndBiometricStateIsInvalidated_AndBiometricTypeIsTouch_willReturnBiometricSpecWithTouchAndNotEnabled() {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        laContextMock?.biometricsCapable = true
        laContextMock?.updateShouldUseFaceId(shouldUse: false)
        
        
        mockBiometricService = BiometricServiceMocks(homeViewController: vcHome,
                                                     configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
        vcHome?.biometricService = mockBiometricService
        
        vcHome?.handleBiometricSpecRequest(biometricAvailability: BiometricState.Invalidated)
        
        XCTAssert(appWebInterface?.biometricSpecRequestCalled == true)
        XCTAssert(appWebInterface?.biometricEnabled == false)
        XCTAssert(appWebInterface?.biometricTypeRef == "touch")
    }
    
    func test_whenHandleBiometricSpecRequestIsCalled_AndBiometricStateIsInvalidated_AndBiometricTypeIsFace_willReturnBiometricSpecWithFaceAndNotEnabled() {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        laContextMock?.biometricsCapable = true
        
        
        mockBiometricService = BiometricServiceMocks(homeViewController: vcHome,
                                                     configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
        vcHome?.biometricService = mockBiometricService
        
        vcHome?.handleBiometricSpecRequest(biometricAvailability: BiometricState.Invalidated)
        
        XCTAssert(appWebInterface?.biometricSpecRequestCalled == true)
        XCTAssert(appWebInterface?.biometricEnabled == false)
        XCTAssert(appWebInterface?.biometricTypeRef == "face")
    }
    
    func test_whenPaycassoOnSuccessIsCalled_andResponseIsInstaSureResponse_willSendExpectedMessagethroughAppWebInterface() {
        let response: PCSFlowResponse = PCSInstaSureFlowResponse()
        
        vcHome.onSuccess(response)
        
        XCTAssert(appWebInterface?.paycassoCallbackCalled == true)
        XCTAssert(appWebInterface?.paycassoTransactionType == "InstaSureFlowResponse")
        XCTAssert(appWebInterface?.paycassoTransactionId == response.transactionId)
        XCTAssert(appWebInterface?.paycassoFaceMatched == true)
        
    }
    
    func test_whenPaycassoOnSuccessIsCalled_andResponseIsDocuSureResponse_willSendExpectedMessagethroughAppWebInterface() {
        let response: PCSFlowResponse = PCSDocuSureFlowResponse()
        
        vcHome.onSuccess(response)
        
        XCTAssert(appWebInterface?.paycassoCallbackCalled == true)
        XCTAssert(appWebInterface?.paycassoTransactionType == "DocuSureFlowResponse")
        XCTAssert(appWebInterface?.paycassoTransactionId == response.transactionId)
        XCTAssert(appWebInterface?.paycassoFaceMatched == false)
    }
    
    func test_whenPaycassoOnSuccessIsCalled_andResponseIsVeriSureResponse_willSendExpectedMessagethroughAppWebInterface() {
        let response: PCSFlowResponse = PCSVeriSureFlowResponse()
        
        vcHome.onSuccess(response)
        
        XCTAssert(appWebInterface?.paycassoCallbackCalled == true)
        XCTAssert(appWebInterface?.paycassoTransactionType == "VeriSureFlowResponse")
        XCTAssert(appWebInterface?.paycassoTransactionId == response.transactionId)
        XCTAssert(appWebInterface?.paycassoFaceMatched == false)
    }
    
    func test_whenPaycassoOnFailureIsCalled_andResponseIsInstaSureResponse_willSendExpectedMessagethroughAppWebInterface() {
        
        let response = PCSFlowFailureResponse()
        
        vcHome.onFailure(response)
        
        XCTAssert(appWebInterface?.paycassoCallbackCalled == true)
        XCTAssert(appWebInterface?.paycassoErrorcode == response.failureCode)
        XCTAssert(appWebInterface?.paycassoErrorMessage == response.failureMessage)
        XCTAssert(appWebInterface?.paycassoFaceMatched == false)
        
    }
    
    func test_whenHandleShowPaycassoCalled_andConfigurationStringIsEmpty_willSendExpectedMessageThroughAppWebInterface() {
        
        vcHome.handleShowPaycasso(paycassoConfiguration: "")
        
        XCTAssert(appWebInterface?.paycassoCallbackCalled == true)
        XCTAssert(appWebInterface?.paycassoCallbackCalled == true)
        XCTAssert(appWebInterface?.paycassoErrorMessage == "Paycasso configuration passed is empty/invalid")
        XCTAssert(appWebInterface?.paycassoFaceMatched == false)
    }
}
