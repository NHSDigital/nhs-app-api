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
}
