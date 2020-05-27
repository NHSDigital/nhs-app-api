import XCTest
import Foundation
import WebKit
@testable import NHSOnline

class BiometricServiceTests: XCTestCase {
    var viewController: HomeViewControllerMocks?
    var testBiometricServices: BiometricService?
    var mockWKWebView: WebViewMocks?
    var appWebInterface: AppWebInterfaceMocks?
    var fidoClient: FidoClientMocks?
    var laContextMock: LocalAuthenticationMocks?
    var vcHome: HomeViewController!
    var mockApplicationState: ApplicationStateMocks!
    var mockSplashScreen: SplashScreenViewMocks!
    var mockProgressSpinner: ProgressSpinnerViewMocks!
    var mockDocumentInteractionController: DocumentInteractionsControllerMocks!
    var testWebview: WKWebView!

    override func setUp() {
        super.setUp()
        appWebInterface = AppWebInterfaceMocks(webView: mockWKWebView)
        fidoClient =  FidoClientMocks()
        laContextMock = LocalAuthenticationMocks()
        
        UserDefaultsManager.setAccessToken("token")
        
        if let viewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController {
            viewController.knownServicesProvider = SuccessKnownServiceProtocolMock()
            viewController.configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)

            vcHome = viewController
        }

        testWebview = WKWebView()
        testWebview.load(URLRequest(url: URL(string: config().HomeUrl)!))
        _ = vcHome.view

        mockApplicationState = ApplicationStateMocks()
        vcHome?.applicationState = mockApplicationState
        
        mockSplashScreen = SplashScreenViewMocks()
        vcHome?.splashScreen = mockSplashScreen
        
        mockProgressSpinner = ProgressSpinnerViewMocks()
        vcHome?.progressSpinner = mockProgressSpinner
        
        mockDocumentInteractionController = DocumentInteractionsControllerMocks()
        vcHome?.documentInteractionController = mockDocumentInteractionController
        
        vcHome.webViewController?.loadPage(url: config().HomeUrl)
    
    }

    override func tearDown() {
        super.tearDown()
    }
    
    func test_whenRegisterIsCalled_andBiometricsIsAvailable_thenRegistrationIsSuccessful(){
        laContextMock?.updateBiometricsCapability(capable: true)
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.register()
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == true)
        XCTAssert(appWebInterface?.biometricAction == "Register")
        XCTAssert(appWebInterface?.biometricOutcome == "Success")
        XCTAssert(appWebInterface?.biometricErrorCode == "")
        
    }
    
    func test_whenRegisterIsCalled_andBiometricsAreInvalid_thenRegistrationFailsAndSendsErrorToWeb(){
        fidoClient?.shouldThrowFidoError = true
        
        laContextMock?.updateBiometricsCapability(capable: true)
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.register()
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == true)
        XCTAssert(appWebInterface?.biometricAction == "Register")
        XCTAssert(appWebInterface?.biometricOutcome == "Failed")
        XCTAssert(appWebInterface?.biometricErrorCode == "10005")
        
    }

    func test_whenRegisterIsCalled_andUserHasFaceID_andActionIsCancelled_thenRegistrationFailsAndSendsErrorToWeb(){
        fidoClient?.userCancelled = true
        
        laContextMock?.updateBiometricsCapability(capable: true)
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.register()
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == true)
        XCTAssert(appWebInterface?.biometricAction == "Register")
        XCTAssert(appWebInterface?.biometricOutcome == "Failed")
        XCTAssert(appWebInterface?.biometricErrorCode == "10005")
        
    }
    
    func test_whenRegisterIsCalled_andUserHasTouchID_andActionIsCancelled_thenRegistrationEndsAndSendsCancelToWeb(){
        fidoClient?.userCancelled = true
        
        laContextMock?.updateBiometricsCapability(capable: true)
        laContextMock?.updateShouldUseFaceId(shouldUse: false)
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.register()
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == true)
        XCTAssert(appWebInterface?.biometricAction == "Register")
        XCTAssert(appWebInterface?.biometricOutcome == "Cancelled")
        XCTAssert(appWebInterface?.biometricErrorCode == "")
        
    }
    
    func test_whenDeregisterIsCalled_andBiometricsIsAvailable_thenDeregistrationIsSuccessful(){
        
        laContextMock?.updateBiometricsCapability(capable: true)
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.deRegister(deregisterFidoCredentials: true)
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == true)
        XCTAssert(appWebInterface?.biometricAction == "Deregister")
        XCTAssert(appWebInterface?.biometricOutcome == "Success")
        XCTAssert(appWebInterface?.biometricErrorCode == "")
        
    }
    
    func test_whenDeregisterIsCalled_andBiometricsAreInvalid_thenDeregistrationFailsAndSendsErrorToWeb(){
        
        laContextMock?.updateBiometricsCapability(capable: true)
        
        fidoClient?.shouldThrowFidoError = true
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.deRegister(deregisterFidoCredentials: true)
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == true)
        XCTAssert(appWebInterface?.biometricAction == "Deregister")
        XCTAssert(appWebInterface?.biometricOutcome == "Failed")
        XCTAssert(appWebInterface?.biometricErrorCode == "10005")
        
    }
    
    func test_whenAuthenticateIsCalled_andBiometricsIsAvailable_thenAuthenticationIsSuccessful(){
        laContextMock?.updateBiometricsCapability(capable: true)
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.authenticate()
        
        XCTAssert(vcHome.webViewController?.webView.url == URL(string: config().HomeUrl+"?fidoAuthResponse="))
        
    }
    
    func test_whenAuthenticateIsCalled_andFidoTokenIsUnavailable_thenAuthenticationFailsSendingAppropriateErrorToWeb(){
        
        laContextMock?.updateBiometricsCapability(capable: true)
        fidoClient?.updateFidoTokenErrorStatus(showThrow: true)
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.authenticate()
        
        XCTAssert(appWebInterface?.biometricLoginFailureCalled == true)
        
    }
    
    func test_whenAuthenticateIsCalled_andUserHasTouchID_andActionIsCancelled_thenAuthenticateEndsAndWontCallErrorPageInWeb(){
        fidoClient?.userCancelled = true
        
        laContextMock?.updateBiometricsCapability(capable: true)
        laContextMock?.updateShouldUseFaceId(shouldUse: false)
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.authenticate()
        
        XCTAssert(appWebInterface?.biometricLoginFailureCalled == false)
        
    }
    
    func test_whenAuthenticateIsCalled_andUserHasFaceID_andActionIsCancelled_thenAuthenticateEndsAndTriggersErrorInWeb(){
        fidoClient?.userCancelled = true
        
        laContextMock?.updateBiometricsCapability(capable: true)
        
        testBiometricServices = createBiometricService()
        
        testBiometricServices?.authenticate()
        
        XCTAssert(appWebInterface?.biometricLoginFailureCalled == true)
        
    }
    
    func test_whenCheckAvailabilityIsCalled_AndBiometricsAreAvailable_willReturnAvailable(){
        
        laContextMock?.updateBiometricsCapability(capable: true)
        fidoClient?.fidoTokenErrorThrown = true
        
        testBiometricServices = createBiometricService()
        
        let biometricsAvailable = testBiometricServices?.checkBiometricCapability()
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == false)
        XCTAssert(biometricsAvailable == true)
        
    }
    
    func test_whenCheckAvailabilityIsCalled_AndBiometricsAreLocked_willReturnBiometricLockout(){
        
        laContextMock?.updateBiometricsCapability(capable: true)
        laContextMock?.updateShouldReturnLockoutError(shouldThrow: true)
        fidoClient?.fidoTokenErrorThrown = true
        
        testBiometricServices = createBiometricService()
        
        let biometricsAvailable = testBiometricServices?.checkBiometricCapability()
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == true)
        XCTAssert(appWebInterface?.biometricAction == "Register")
        XCTAssert(appWebInterface?.biometricOutcome == "Failed")
        XCTAssert(appWebInterface?.biometricErrorCode == "10004")
        XCTAssert(biometricsAvailable == false)
        
    }
    
    func test_whenCheckAvailabilityIsCalled_AndBiometricsAreNotEnrolled_willReturnBiometricNotEnrolled(){
        
        laContextMock?.updateBiometricsCapability(capable: true)
        laContextMock?.updateShouldReturnNotEnrolled(shouldThrow: true)
        fidoClient?.fidoTokenErrorThrown = true
        
        testBiometricServices = createBiometricService()
        
        let biometricsAvailable = testBiometricServices?.checkBiometricCapability()
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == true)
        XCTAssert(appWebInterface?.biometricAction == "Register")
        XCTAssert(appWebInterface?.biometricOutcome == "Failed")
        XCTAssert(appWebInterface?.biometricErrorCode == "10004")
        XCTAssert(biometricsAvailable == false)
        
    }
    
    func test_whenCheckAvailabilityIsCalled_AndBiometricsAreNotAvailable_willReturnBiometricNotAvailable(){
        
        laContextMock?.updateBiometricsCapability(capable: true)
        laContextMock?.updateShouldReturnNotAvailableError(shouldThrow: true)
        fidoClient?.fidoTokenErrorThrown = true
        
        testBiometricServices = createBiometricService()
        
        let biometricsAvailable = testBiometricServices?.checkBiometricCapability()
        
        XCTAssert(appWebInterface?.biometricCompletionCalled == true)
        XCTAssert(appWebInterface?.biometricAction == "Register")
        XCTAssert(appWebInterface?.biometricOutcome == "Failed")
        XCTAssert(appWebInterface?.biometricErrorCode == "10004")
        XCTAssert(biometricsAvailable == false)
        
    }
    
    func createBiometricService() -> BiometricService {
        let configurationServiceProvider = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        
        return BiometricService.init(homeViewController: vcHome,
        configurationServiceProvider: configurationServiceProvider,
        appWebInterface: appWebInterface!,
        fidoClient: fidoClient,
        laContext: laContextMock!)
    }
}
