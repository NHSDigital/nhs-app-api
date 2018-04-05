import UIKit

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate {

    var window: UIWindow?

    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplicationLaunchOptionsKey: Any]?) -> Bool {
        UITabBarItem.appearance()
            .setTitleTextAttributes(
                [NSAttributedStringKey.font: UIFont.systemFont(ofSize: 8)],
                for: .normal)
        
        return true
    }
    
    func application(_ application: UIApplication, continue userActivity: NSUserActivity, restorationHandler: @escaping ([Any]?) -> Void) -> Bool {
        let webPageUrl = userActivity.webpageURL?.absoluteString
        let viewController = self.window?.rootViewController as! HomeViewController
        
        viewController.webViewController?.webView.loadPage(url: webPageUrl!)
        viewController.webViewController?.dismissSafariViewController()
        
        return true
    }
}
