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
    
    func resolveAppScheme(url: URL) -> String {
        var webPageUrl = url.absoluteString;
        if(url.scheme == config().AppScheme) {
            var comps = URLComponents(url: url, resolvingAgainstBaseURL: false)!
            comps.scheme = config().BaseScheme
            webPageUrl = comps.url!.absoluteString
        }
        return webPageUrl
    }
}
