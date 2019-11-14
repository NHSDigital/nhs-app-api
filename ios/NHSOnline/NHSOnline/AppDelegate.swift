import UIKit
import UserNotifications
import WebKit

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate, UNUserNotificationCenterDelegate {
    var window: UIWindow?
    var rootViewController: UINavigationController?
    
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
        setUserAgent()
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
        
        let webPageUrl = resolveAppScheme(url: url)
        self.finishLoginToApp(webPageUrl)
        return true
    }
    
    private func clearCaches() {
        URLCache.shared.removeAllCachedResponses()
        
        WKWebsiteDataStore.default().removeData(ofTypes: Set([WKWebsiteDataTypeDiskCache, WKWebsiteDataTypeMemoryCache]), modifiedSince: Date(timeIntervalSince1970: 0), completionHandler: {})
    }
    
    private func finishLoginToApp(_ url: String) {
        let viewController = getViewController()
        if(url == config().HomeUrl + config().FidoLoginErrorPath) {
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
    
    func setUserAgent() {
        let versionNumber = Bundle.main.object(forInfoDictionaryKey: "CFBundleShortVersionString") as! String
        let userAgent = UIWebView().stringByEvaluatingJavaScript(from: "navigator.userAgent")! + " nhsapp-ios/" + versionNumber;
        UserDefaults.standard.register(defaults: ["UserAgent" : userAgent])
    }  
    func resolveAppScheme(url: URL) -> String {
        var webPageUrl = url.absoluteString;
        if(url.scheme == config().AppScheme) {
            var comps = URLComponents(url: url, resolvingAgainstBaseURL: false)!
            comps.scheme = config().BaseScheme
            webPageUrl = comps.url!.absoluteString
        }
        
        return webPageUrl
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
    
    private func getViewController() -> HomeViewController {
        return rootViewController?.childViewControllers.first as! HomeViewController;
    }
}
