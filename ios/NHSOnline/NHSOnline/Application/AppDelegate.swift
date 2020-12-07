import UIKit
import UserNotifications
import WebKit

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate, UNUserNotificationCenterDelegate {
    var window: UIWindow?
    var rootViewController: UINavigationController?
    
    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {
        clearCaches()

        application.ignoreSnapshotOnNextApplicationLaunch()
        if let homeViewController = self.window?.rootViewController as? HomeViewController {
            homeViewController.knownServicesProvider = ConfigurationServiceManager.getKnownServiceProvider()
            homeViewController.configurationServiceProvider = ConfigurationServiceManager.getConfigurationServiceProvider()
            homeViewController.splashScreen = SplashScreen()
            homeViewController.progressSpinner = ProgressSpinner()
            homeViewController.fileDownloader = FileDownloadHelper()
            homeViewController.dataDownloadAlertHandler = DataDownloadAlertHandler()
            
            let navigationController = UINavigationController(rootViewController: homeViewController)
            self.window?.rootViewController = navigationController
            self.rootViewController = navigationController
        }
        
        if #available(iOS 10.0, *) {
            UNUserNotificationCenter.current().delegate = self
        }
        
        let userActivities = launchOptions?[.userActivityDictionary] as? [String: AnyObject]

        let activity = userActivities?["UIApplicationLaunchOptionsUserActivityTypeKey"] as? String
        
        if (activity == "NSUserActivityTypeBrowsingWeb"){
            UserDefaults.standard.set(true, forKey: config().DeepLinkAppClosed)
        }
        
        setLocale()
        handleNotifications(launchOptions: launchOptions)
        return true
    }
    
    func application(_ application: UIApplication, continue userActivity: NSUserActivity, restorationHandler: @escaping ([UIUserActivityRestoring]?) -> Void) -> Bool {
        clearCaches()
        
        let webPageUrl = userActivity.webpageURL?.absoluteString
        let convertedUrl = UrlHelper.createRedirectToUrl(url: webPageUrl!)
        if UrlHelper.isSameSchemeAndHostAsHomeUrl(url: convertedUrl)
        {
            UserDefaults.standard.set(convertedUrl, forKey: config().LinkPropertyName)
        }

        if #available(iOS 12.0, *) {
            self.finishLoginToApp(convertedUrl!.absoluteString)
            return true
        } else {
            let appClosed = UserDefaults.standard.bool(forKey: config().DeepLinkAppClosed)
            if (appClosed) {
                UserDefaults.standard.removeObject(forKey: config().DeepLinkAppClosed)
                return false;
            }
            self.finishLoginToApp(convertedUrl!.absoluteString)
            return true
        }
    }
    
    func application(_ app: UIApplication, open url: URL, options: [UIApplication.OpenURLOptionsKey : Any] = [:]) -> Bool {
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
            viewController.appWebInterface?.biometricLoginFailure()
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
        
        guard let url = aps[config().LinkPropertyName] as? String else {
            return
        }
        
        let convertedUrl = UrlHelper.createRedirectToUrl(url: url)
        getViewController().webViewController?.loadPage(url: convertedUrl!)
    }

    private func getViewController() -> HomeViewController {
        return rootViewController?.children.first as! HomeViewController;
    }
    
    private func handleNotifications(launchOptions: [UIApplication.LaunchOptionsKey: Any]?) {
        guard let notificationOption = launchOptions?[.remoteNotification] as? [String: AnyObject] else {
            return
        }
        
        guard let aps = notificationOption["aps"] as? [String: AnyObject] else {
            return
        }
        
        guard let url = aps[config().LinkPropertyName] as? String else {
            return
        }
        
        let convertedUrl = UrlHelper.createRedirectToUrl(url: url)
        UserDefaults.standard.set(convertedUrl, forKey: config().LinkPropertyName)
    }
}
