import UIKit
import UserNotifications
import WebKit

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate, UNUserNotificationCenterDelegate {
    var window: UIWindow?
    var rootViewController: UINavigationController?
    var knownServices = KnownServices(config: config())
    
    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplicationLaunchOptionsKey: Any]?) -> Bool {
        clearCaches()
        
        application.ignoreSnapshotOnNextApplicationLaunch()
        let navigationController = UINavigationController(rootViewController: (self.window?.rootViewController as? HomeViewController)!)
        self.window?.rootViewController = navigationController
        rootViewController = navigationController
        
        if #available(iOS 10.0, *) {
            UNUserNotificationCenter.current().delegate = self
        }
        setLocale()
        handleNotifications(launchOptions: launchOptions)
        return true
    }
    
    func application(_ application: UIApplication, continue userActivity: NSUserActivity, restorationHandler: @escaping ([Any]?) -> Void) -> Bool {
        clearCaches()
        
        let webPageUrl = userActivity.webpageURL?.absoluteString
        self.finishLoginToApp(webPageUrl!)
        return true
    }
    
    func application(_ app: UIApplication, open url: URL, options: [UIApplicationOpenURLOptionsKey : Any] = [:]) -> Bool {
        clearCaches()
                
        self.finishLoginToApp(url.absoluteString)
        return true
    }
    
    private func clearCaches() {
        URLCache.shared.removeAllCachedResponses()
        
        WKWebsiteDataStore.default().removeData(ofTypes: Set([WKWebsiteDataTypeDiskCache, WKWebsiteDataTypeMemoryCache]), modifiedSince: Date(timeIntervalSince1970: 0), completionHandler: {})
    }
    
    private func finishLoginToApp(_ url: String) {
        let viewController = getViewController()
        if url.contains(config().FidoLoginErrorPath) {
            viewController.showBiometricSessionError()
        } else {
            loadPageAndShowView(url, viewController)
        }
    }
    
    private func loadPageAndShowView(_ url: String,_ viewController: HomeViewController) {
        viewController.webViewController?.loadPage(url: url)
        viewController.webViewController?.dismissSafariViewController()
        viewController.webViewController?.setRedirectCompleted(redirect: true)
        let theWebview = viewController.webViewController?.webView
        let view = UIView(frame: (theWebview?.frame)!)
        view.backgroundColor = UIColor.white
        view.tag = 2
        theWebview?.addSubview(view);
    }
    
    func setLocale() {
        let locale = NSLocalizedString("locale", comment: "")
        UserDefaults.standard.set([locale], forKey: "AppleLanguages")
    }
    
    func application(
        _ application: UIApplication,
        didRegisterForRemoteNotificationsWithDeviceToken deviceToken: Data
        ) {
        let viewController = getViewController()
        
        viewController.pushNotificationsAuthorised(deviceToken: deviceToken)
    }
    
    func application(
        _ application: UIApplication,
        didFailToRegisterForRemoteNotificationsWithError error: Error) {
        let viewController = getViewController()
        
        viewController.failedToRegisterForNotifications()
    }
    
    @available(iOS 10.0, *)
    func userNotificationCenter(_ center: UNUserNotificationCenter, willPresent notification: UNNotification, withCompletionHandler completionHandler: @escaping (UNNotificationPresentationOptions) -> Void) {
        completionHandler([.alert, .badge, .sound])
    }
    
    func application(_ application: UIApplication, didReceiveRemoteNotification userInfo: [AnyHashable: Any],
      fetchCompletionHandler completionHandler: @escaping (UIBackgroundFetchResult) -> Void) {
        guard let aps = userInfo["aps"] as? [String: AnyObject] else {
            completionHandler(.failed)
            return
        }
        
        guard let url = aps[config().NotificationLinkPropertyName] as? String else {
            return
        }
        
        let convertedUrl = UrlHelper.ensureUrlWithScheme(url: url)
        if knownServices.isSameSchemeAndHostAsHomeUrl(url: convertedUrl)
        {
            getViewController().webViewController?.loadPage(url: convertedUrl!)
        }
    }

    private func getViewController() -> HomeViewController {
        return rootViewController?.childViewControllers.first as! HomeViewController;
    }
    
    private func handleNotifications(launchOptions: [UIApplicationLaunchOptionsKey: Any]?) {
        guard let notificationOption = launchOptions?[.remoteNotification] as? [String: AnyObject] else {
            return
        }
        
        guard let aps = notificationOption["aps"] as? [String: AnyObject] else {
            return
        }
        
        guard let url = aps[config().NotificationLinkPropertyName] as? String else {
            return
        }
        
        let convertedUrl = UrlHelper.ensureUrlWithScheme(url: url)
        if knownServices.isSameSchemeAndHostAsHomeUrl(url: convertedUrl)
        {
            UserDefaults.standard.set(convertedUrl, forKey: config().NotificationLinkPropertyName)
        }
    }
}
