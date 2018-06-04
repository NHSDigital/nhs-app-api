import UIKit

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate {

    var window: UIWindow?
    var rootViewController: HomeViewController?

    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplicationLaunchOptionsKey: Any]?) -> Bool {
        UITabBarItem.appearance()
            .setTitleTextAttributes(
                [NSAttributedStringKey.font: UIFont.systemFont(ofSize: 8)],
                for: .normal)
        
        rootViewController = self.window?.rootViewController as? HomeViewController

        return true
    }
    
    func application(_ application: UIApplication, continue userActivity: NSUserActivity, restorationHandler: @escaping ([Any]?) -> Void) -> Bool {
        let webPageUrl = userActivity.webpageURL?.absoluteString
        
        rootViewController?.webViewController?.webView.loadPage(url: webPageUrl!)
        rootViewController?.webViewController?.dismissSafariViewController()
        
        return true
    }
}
