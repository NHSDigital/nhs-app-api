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
        self.finishLoginToApp(webPageUrl!)
        return true
    }
    
    func application(_ app: UIApplication, open url: URL, options: [UIApplicationOpenURLOptionsKey : Any] = [:]) -> Bool {
        let webPageUrl = resolveAppScheme(url: url)
        self.finishLoginToApp(webPageUrl)
        return true
    }
    
    private func finishLoginToApp(_ url: String) {
        let viewController = rootViewController?.childViewControllers.first as! HomeViewController
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
