import Foundation

public class CustomNotifications {
    static let pageUnavailabilityOnReloadWebView = Notification.Name("page-unavailability-on-reload-web-view")
    static let dismissAllAlerts = Notification.Name("dismiss-all-alerts")
    static let dismissLeavingPageAlert = Notification.Name("dismiss-leaving-page-alert")
    static let dismissIOSVersionUpdateAlert = Notification.Name("dismiss-ios-version-update-alert")
    static let apiLoadFailure = Notification.Name("api-load-failure")
    static let apiLoadSuccess = Notification.Name("api-load-success")
}
