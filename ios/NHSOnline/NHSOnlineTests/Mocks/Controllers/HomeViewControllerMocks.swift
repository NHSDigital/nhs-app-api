@testable import NHSOnline

class HomeViewControllerMocks: HomeViewController {
     var alertDismissed = false
     var showWebViewContainerCalled = false
     var updateHeaderTextCalled = false
     var startActivityIndicatorWasCalled = false
     var stopActivityIndicatorWasCalled = false
     var goToPageCalled = false
     var goToPageValue = ""
     var capturedCalendarData: CalendarData?
     
     override func showWebViewContainer() {
         showWebViewContainerCalled = true
     }
     
     override func updateHeaderText(headerText: String?, accessibilityLabel: String? = nil) {
         updateHeaderTextCalled = true
     }
     
     override func showProgressBar() {
        startActivityIndicatorWasCalled = true
     }

     override func hideProgressBar() {
        stopActivityIndicatorWasCalled = true
     }
     
     override func viewDidLoad() {
        return
     }
     
     override func setVisibilityOfHeaderAndMenuBars(headerType: HeaderType) {
        return
     }

     override func dimissAlert() {
        alertDismissed = true;
     }
    
    override func handleGoToPage(page: String) {
        goToPageCalled = true
        goToPageValue = page
    }

    override func addEventToCalendar(calendarData: CalendarData) {
        capturedCalendarData = calendarData
    }
}
