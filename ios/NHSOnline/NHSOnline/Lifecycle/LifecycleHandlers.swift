import Foundation
import UIKit
import WebKit

class LifecycleHandlers: NSObject {
    var knownServices: KnownServices
    var webViewController: WebViewController?
    var hasCheckedAppVersionSinceAppOpened = false
    let configurationService: ConfigurationService
    let validateSessionString: String = "window.validateSession()"
    
    init(knownServices: KnownServices, webViewController: WebViewController, configurationService: ConfigurationService) {
        self.knownServices = knownServices
        self.webViewController = webViewController
        self.configurationService = configurationService
        super.init()
        createLifecycleObservers()
    }
    
    func createLifecycleObservers() {
        NotificationCenter.default.addObserver(self, selector: #selector(self.didFinishLaunchingNotification), name: Notification.Name.UIApplicationDidFinishLaunching, object: nil)
        
        NotificationCenter.default.addObserver(self, selector: #selector(self.didBecomeActive), name: Notification.Name.UIApplicationDidBecomeActive, object: nil)
        
        NotificationCenter.default.addObserver(self, selector: #selector(self.didEnterBackground), name: Notification.Name.UIApplicationDidEnterBackground, object: nil)
        
        NotificationCenter.default.addObserver(self, selector: #selector(self.performAppVersionCheck), name: CustomNotifications.pageUnavailabilityOnReloadWebView, object: nil)
    }
    
    @objc
    func performAppVersionCheck(completionHandler: (() -> Void)? = nil) {
        if (hasCheckedAppVersionSinceAppOpened == false) {
            configurationService.isUserDeviceAllowed { (result) in
                if (result.isValidConfiguration == false) {
                    DispatchQueue.main.async {
                        self.displayAppVersionOutOfDate()
                        completionHandler?()
                    }
                } else {
                    completionHandler?()
                }
            }
        }
    }
    
    func displayAppVersionOutOfDate() {
        let appUpdateRequiredTitle = NSLocalizedString("AppUpdateRequiredTitle", comment: "")
        let appUpdateRequiredMessage = NSLocalizedString("AppUpdateRequiredMessage", comment: "")
        let appUpdateRequiredCloseButtonText = NSLocalizedString("AppUpdateRequiredCloseButtonText", comment: "")
        let appUpdateRequiredGoToUpdateButtonText = NSLocalizedString("AppUpdateRequiredGoToUpdateButtonText", comment: "")
        
        let alert = UIAlertController(title: appUpdateRequiredTitle, message: appUpdateRequiredMessage, preferredStyle: UIAlertControllerStyle.alert)
        
        let appStoreUrl = config().AppStoreUrl
        if (!appStoreUrl.isEmpty) {
            alert.addAction(UIAlertAction(title: appUpdateRequiredGoToUpdateButtonText, style: .default, handler: { action in
                self.hasCheckedAppVersionSinceAppOpened = false
                
                if let url = URL(string: appStoreUrl), UIApplication.shared.canOpenURL(url) {
                    if #available(iOS 10.0, *) {
                        UIApplication.shared.open(url, options: [:], completionHandler: nil)
                    } else {
                        UIApplication.shared.openURL(url)
                    }
                }
            }))
        }

        alert.addAction(UIAlertAction(title: appUpdateRequiredCloseButtonText, style: .cancel, handler: { action in
            self.hasCheckedAppVersionSinceAppOpened = false
            UIControl().sendAction(#selector(URLSessionTask.suspend), to: UIApplication.shared, for: nil)
        }))
        let win = UIWindow(frame: UIScreen.main.bounds)
        let vc = UIViewController()
        vc.view.backgroundColor = .clear
        win.rootViewController = vc
        win.windowLevel = UIWindowLevelAlert + 1
        win.makeKeyAndVisible()
        vc.present(alert, animated: true, completion: nil)
        self.hasCheckedAppVersionSinceAppOpened = true
    }
    
    @objc func didFinishLaunchingNotification() {
        removeCookies()
        setTabBarFont()
    }
    
    @objc func didBecomeActive() {
        
        performAppVersionCheck()
        
        if knownServices.shouldValidateSession(host: webViewController?.webView.url?.host) {
            validateSession(knownServices: self.knownServices, webView: (self.webViewController?.webView)!)
        } else {
            hideWhiteScreen()
        }
    }
    
    @objc func didEnterBackground() {
        showWhiteScreen()
    }
    
    private func removeCookies() {
        let cookieJar = HTTPCookieStorage.shared
        
        for cookie in cookieJar.cookies! {
            cookieJar.deleteCookie(cookie)
        }
    }
    
    private func setTabBarFont() {
        UITabBarItem.appearance().setTitleTextAttributes(
            [NSAttributedStringKey.font: UIFont.systemFont(ofSize: 8)],
            for: .normal)
    }
    
    private func validateSession(knownServices: KnownServices, webView: WKWebView) {
        if knownServices.shouldValidateSession(host: webView.url?.host) {
            let completionHandler: (Any?, Error?) -> Void = {
                (data, error) in
                self.hideWhiteScreen()
            }
            
            webView.evaluateJavaScript(validateSessionString, completionHandler: completionHandler)
        }
    }
    
    private func showWhiteScreen() {
        let window = UIApplication.shared.keyWindow!
        let v = UIView(frame: window.frame)
        v.backgroundColor = UIColor.white
        v.tag = 1
        window.addSubview(v);
    }
    
    private func hideWhiteScreen() {
        UIApplication.shared.keyWindow?.viewWithTag(1)?.removeFromSuperview()
    }
}
