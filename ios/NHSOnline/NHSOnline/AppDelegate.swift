import UIKit

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate {
    var window: UIWindow?
    var rootViewController: UINavigationController?

    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplicationLaunchOptionsKey: Any]?) -> Bool {
        application.ignoreSnapshotOnNextApplicationLaunch()
        let navigationController = UINavigationController(rootViewController: (self.window?.rootViewController as? HomeViewController)!)
        self.window?.rootViewController = navigationController
        rootViewController = navigationController

        setLocale()

        return true
    }

    func application(_ application: UIApplication, continue userActivity: NSUserActivity, restorationHandler: @escaping ([Any]?) -> Void) -> Bool {
        let webPageUrl = userActivity.webpageURL?.absoluteString
        let vc = rootViewController?.childViewControllers.first as! HomeViewController
        vc.webViewController?.loadPage(url: webPageUrl!)
        vc.webViewController?.dismissSafariViewController()

        return true
    }
    
    func application(_ app: UIApplication, open url: URL, options: [UIApplicationOpenURLOptionsKey : Any] = [:]) -> Bool {
        let webPageUrl = resolveAppScheme(url: url)
        let vc = rootViewController?.childViewControllers.first as! HomeViewController
        vc.webViewController?.loadPage(url: webPageUrl)
        vc.webViewController?.dismissSafariViewController()

        return true
    }
    
    func setLocale() {
        let locale = NSLocalizedString("locale", comment: "")
        UserDefaults.standard.set([locale], forKey: "AppleLanguages")
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

    func applicationWillEnterForeground(_ application: UIApplication) {
        LogoutIfSessionExpired()
    }

    private func LogoutIfSessionExpired() {
        let defaults = UserDefaults()

        var sessionExpired = false

        if(defaults.object(forKey: "TimeEnterBackground") != nil) {
            let timeEnterBackground = UserDefaults().integer(forKey: "TimeEnterBackground")
            let currentTime = Date().ticks

            let timePassed = currentTime.advanced(by: -timeEnterBackground)
            let date = Date(ticks: timePassed)

            let minutesPassed = Calendar.current.component(.minute, from: date)

            let sessionTimeout = config().SessionTimeout

            if(minutesPassed >= sessionTimeout) {
                sessionExpired = true
            }
        }

        let vc = rootViewController?.childViewControllers.first as! HomeViewController

        if(sessionExpired) {
            vc.setVisibilityOfHeaderAndMenuBars(visible: false)
            WebViewController.Properties.usingAbsoluteUri = true
        }

        vc.webViewController?.reloadWebView()
    }

    func applicationWillResignActive(_ application: UIApplication) {
        let currentDateTime = Date().ticks
        UserDefaults.standard.set(currentDateTime, forKey: "TimeEnterBackground")
    }
}
