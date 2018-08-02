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
        vc.webViewController?.webView.loadPage(url: webPageUrl!)
        vc.webViewController?.dismissSafariViewController()
        return true
    }
}
