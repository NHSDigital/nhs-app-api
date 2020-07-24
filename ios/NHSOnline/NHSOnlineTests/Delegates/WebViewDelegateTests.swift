import XCTest
import WebKit
@testable import NHSOnline

class WebViewDelegateTests: XCTestCase {
    var webViewDelegate: WebViewDelegate?
    
    var wKWebView: WKWebView?
    var mockWKWebView: WebViewMocks?
    var wKNavigation: WKNavigation?
    var error: NSError?
    var nSURLErrorCancelled: NSError?
    var homeViewController: HomeViewControllerMocks?
    var appWebInterface: AppWebInterfaceMocks?
    var knownServicesProvider: KnownServicesProtocolMocks?
    var loggingService: LoggingServiceMocks?
    
    override func setUp() {
        super.setUp()
        self.knownServicesProvider = KnownServicesProtocolMocks.success()
        let configurationServiceProvider: ConfigurationServiceProtocol = SuccessConfigurationProtocolMock(configurationResponse: SuccessConfigurationResponseMock().instance)
        let viewController = HomeViewControllerMocks()
        viewController.knownServicesProvider = knownServicesProvider
        viewController.configurationServiceProvider = configurationServiceProvider
        
        loggingService = LoggingServiceMocks()
        mockWKWebView = WebViewMocks()
        appWebInterface = AppWebInterfaceMocks(webView: mockWKWebView)
        let webAppInterface = WebAppInterface(controller: viewController)
        webViewDelegate = WebViewDelegate(controller: viewController, knownServiceProvider: knownServicesProvider!, configurationServiceProvider: configurationServiceProvider, webAppInterface: webAppInterface, loggingService: loggingService!)
        homeViewController = viewController
        wKWebView = WKWebView(frame: .zero)
        mockWKWebView = WebViewMocks()
        wKNavigation = WKNavigation()
        nSURLErrorCancelled = NSError(domain:NSURLErrorDomain,code:-999,userInfo:[NSLocalizedDescriptionKey:""])
        error = NSError(domain:NSURLErrorDomain,code:-1000,userInfo:[NSLocalizedDescriptionKey:""])
    }
    
    func test_WhenPageLoadIsNormal_VerifyStartActivityIndicatorIsCalled() {
        webViewDelegate?.checkPageLoadOriginAndStartProgressSpinner()
        
        assert(homeViewController!.startActivityIndicatorWasCalled == true,
               "Expected the startActivityIndicator() Method to be invoked")
    }
    
    func test_WhenPageLoadIsFromBackButton_VerifyStartActivityIndicatorIsNotCalled() {
        webViewDelegate?.viewController.goingBack = true
        
        webViewDelegate?.checkPageLoadOriginAndStartProgressSpinner()
        
        assert(homeViewController!.startActivityIndicatorWasCalled == false,
               "startActivityIndicator() Method should not be invoked")
    }
    
    func test_webViewNavigationFailWithStatusCodeGreaterOrEqualTo400_LogsAnApiError() {
        wKWebView?.loadPage(url: config().HomeUrl)
        let decisionHandler: (WKNavigationResponsePolicy) -> Void = {
            (policy) in return;
        }
        
        let navigationResponse = WKNavigationResponseMock(statusCode: 404)
        webViewDelegate?.webView(wKWebView!, decidePolicyFor: navigationResponse, decisionHandler: decisionHandler)
        
        assert(loggingService?.calledLogError == true)
    }
    
    func test_webViewNavigationSuccess_DoesNotLogAnApiError() {
        wKWebView?.loadPage(url: config().HomeUrl)
        let decisionHandler: (WKNavigationResponsePolicy) -> Void = {
            (policy) in return;
        }
        
        let navigationResponse = WKNavigationResponseMock(statusCode: 204)
        webViewDelegate?.webView(wKWebView!, decidePolicyFor: navigationResponse, decisionHandler: decisionHandler)
        
        assert(loggingService?.calledLogError == false)
    }
    
    func test_webViewDidFailProvisionalNavigationWithNSURLErrorCancelled_VerifyStopActivityIndicatorIsCalled() {
        wKWebView?.loadPage(url: config().HomeUrl)
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: nSURLErrorCancelled!)
        
        assert(homeViewController!.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
        
        assert(homeViewController!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithNSURLErrorCancelled_VerifyStopActivityIndicatorIsNotCalled() {
        wKWebView?.loadPage(url: config().HomeUrl + "auth-return?code=something")
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: nSURLErrorCancelled!)
        
        assert(homeViewController!.stopActivityIndicatorWasCalled == false,
               "Expected the stopActivityIndicator() Method to not have been invoked")
        
        assert(homeViewController!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithOtherNSError_VerifyStopActivityIndicatorIsCalled() {
        wKWebView?.loadPage(url: config().HomeUrl)
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: error!)
        
        assert(homeViewController!.stopActivityIndicatorWasCalled == true,
               "Expected the stopActivityIndicator() Method to be invoked")
        
        assert(homeViewController!.applicationState.isReady() == true)
    }
    
    func test_webViewDidFailProvisionalNavigationWithOtherNSError_VerifyStopActivityIndicatorIsNotCalled() {
        wKWebView?.loadPage(url: config().HomeUrl + "auth-return?code=something")
        webViewDelegate?.webView(wKWebView!, didFailProvisionalNavigation: wKNavigation, withError: error!)
        
        assert(homeViewController!.stopActivityIndicatorWasCalled == false,
               "Expected the stopActivityIndicator() Method to not have been invoked")
        
        assert(homeViewController!.applicationState.isReady() == true)
    }
    
    func test_userContentControllerGoToPage_WithJavaScriptInteractionModeSilverThirdParty_CallsControllerGoToPage() {
        // Arrange
        let url = "http://www.example.com"
        let page = "foo"
        let message = WKScriptMessageMock(name: "goToPage", body: page, url: url)
        let knownServices = CompleteKnownServicesMock(url: url,
                                                      javaScriptInteractionMode: JavaScriptInteractionMode.SilverThirdParty)
        
        knownServicesProvider?.knownServicesMock = knownServices
        
        // Act
        webViewDelegate?.userContentController(WKUserContentController(), didReceive: message)
        
        // Assert
        assert(homeViewController?.goToPageCalled == true)
        assert(homeViewController?.goToPageValue == page)
    }
    
    func test_userContentControllerGoToPage_WithJavaScriptInteractionModeNotSilverThirdParty_DoesNotCallControllerGoToPage() {
        // Arrange
        let url = "http://www.example.com"
        let page = "foo"
        let message = WKScriptMessageMock(name: "goToPage", body: page, url: url)
        let knownServices = CompleteKnownServicesMock(url: url,
                                                      javaScriptInteractionMode: JavaScriptInteractionMode.NhsApp)
        
        knownServicesProvider?.knownServicesMock = knownServices
        
        // Act
        webViewDelegate?.userContentController(WKUserContentController(), didReceive: message)
        
        // Assert
        assert(homeViewController?.goToPageCalled == false)
    }
    
    func test_addEventToCalendar_deserializesJsonAndCallsViewControllerWithCorrectData_SilverThirdParty() {
        addEventToCalendar_deserializesJsonAndCallsViewControllerWithCorrectData(javascriptInteractionMode: JavaScriptInteractionMode.SilverThirdParty)
    }
    
    func test_addEventToCalendar_deserializesJsonAndCallsViewControllerWithCorrectData_NhsApp() {
        addEventToCalendar_deserializesJsonAndCallsViewControllerWithCorrectData(javascriptInteractionMode: JavaScriptInteractionMode.NhsApp)
    }
    
    private func addEventToCalendar_deserializesJsonAndCallsViewControllerWithCorrectData(javascriptInteractionMode: JavaScriptInteractionMode) {
        let url = "http://www.example.com"
        let message = AddEventToCalendarWKScriptMessageMock(
            subject: "appt checkup", messageBody: "please attend", location: "clinic room 1", startTimeEpochSeconds: 1591788300, endTimeEpochSeconds: 1591789860)

        let knownServices = CompleteKnownServicesMock(url: url, javaScriptInteractionMode: javascriptInteractionMode)
        knownServicesProvider?.knownServicesMock = knownServices

        webViewDelegate?.userContentController(WKUserContentController(), didReceive: message)
        
        // assert
        assert(homeViewController?.capturedCalendarData != nil)
        assert(homeViewController?.capturedCalendarData!.subject == message.subject)
        assert(homeViewController?.capturedCalendarData!.body == message.messageBody)
        assert(homeViewController?.capturedCalendarData!.location == message.location)
        assert(homeViewController?.capturedCalendarData!.startTimeEpochSeconds == message.startTimeEpochSeconds!.int64Value)
        assert(homeViewController?.capturedCalendarData!.endTimeEpochSeconds == message.endTimeEpochSeconds!.int64Value)
        assert(homeViewController?.capturedCalendarData!.source == javascriptInteractionMode)
     }
     
    func test_addEventToCalendar_deserializesJsonAndCanHandleDecmialPointsInTimeFields_SilverThirdParty() {
        addEventToCalendar_deserializesJsonAndCanHandleDecmialPointsInTimeFields(javascriptInteractionMode: JavaScriptInteractionMode.SilverThirdParty)
    }
    
    func test_addEventToCalendar_deserializesJsonAndCanHandleDecmialPointsInTimeFields_NhsApp() {
        addEventToCalendar_deserializesJsonAndCanHandleDecmialPointsInTimeFields(javascriptInteractionMode: JavaScriptInteractionMode.NhsApp)
    }
    
    private func addEventToCalendar_deserializesJsonAndCanHandleDecmialPointsInTimeFields(javascriptInteractionMode: JavaScriptInteractionMode) {
        let url = "http://www.example.com"
        let message = AddEventToCalendarWKScriptMessageMock(
             subject: "appt checkup", messageBody: "please attend", location: "clinic room 1", startTimeEpochSeconds: 1591788300.5, endTimeEpochSeconds: 1591789860.5)

        let knownServices = CompleteKnownServicesMock(url: url, javaScriptInteractionMode: javascriptInteractionMode)
        knownServicesProvider?.knownServicesMock = knownServices

        webViewDelegate?.userContentController(WKUserContentController(), didReceive: message)
         
        // assert
        assert(homeViewController?.capturedCalendarData != nil)
        assert(homeViewController?.capturedCalendarData!.subject == message.subject)
        assert(homeViewController?.capturedCalendarData!.body == message.messageBody)
        assert(homeViewController?.capturedCalendarData!.location == message.location)
        assert(homeViewController?.capturedCalendarData!.startTimeEpochSeconds == message.startTimeEpochSeconds!.int64Value)
        assert(homeViewController?.capturedCalendarData!.endTimeEpochSeconds == message.endTimeEpochSeconds!.int64Value)
        assert(homeViewController?.capturedCalendarData!.source == javascriptInteractionMode)
    }

    func test_addEventToCalendar_deserializesNullValues_SilverThirdParty() {
        addEventToCalendar_deserializesNullValues(javascriptInteractionMode: JavaScriptInteractionMode.SilverThirdParty)
    }
    
    func test_addEventToCalendar_deserializesNullValues_NhsApp() {
        addEventToCalendar_deserializesNullValues(javascriptInteractionMode: JavaScriptInteractionMode.NhsApp)
    }
    
    private func addEventToCalendar_deserializesNullValues(javascriptInteractionMode: JavaScriptInteractionMode) {
        let url = "http://www.example.com"
        let message = AddEventToCalendarWKScriptMessageMock(
            subject: nil, messageBody: nil, location: nil, startTimeEpochSeconds: nil, endTimeEpochSeconds: nil)

        let knownServices = CompleteKnownServicesMock(url: url, javaScriptInteractionMode: javascriptInteractionMode)
        knownServicesProvider?.knownServicesMock = knownServices

        webViewDelegate?.userContentController(WKUserContentController(), didReceive: message)
        
        // assert
        assert(homeViewController?.capturedCalendarData != nil)
        assert(homeViewController?.capturedCalendarData!.subject == nil)
        assert(homeViewController?.capturedCalendarData!.body == nil)
        assert(homeViewController?.capturedCalendarData!.location == nil)
        assert(homeViewController?.capturedCalendarData!.startTimeEpochSeconds == nil)
        assert(homeViewController?.capturedCalendarData!.endTimeEpochSeconds == nil)
        assert(homeViewController?.capturedCalendarData!.source == javascriptInteractionMode)
     }

}
