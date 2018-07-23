import Foundation
import UIKit
import WebKit

class LifecycleHandlers: NSObject {
    var knownServices: KnownServices
    var webViewController: WebViewController
    var blankViewController = BlankViewController()
    
    let validateSessionString: String = "window.validateSession()"
    
    init(knownServices: KnownServices, webViewController: WebViewController) {
        self.knownServices = knownServices
        self.webViewController = webViewController
        blankViewController.view.backgroundColor = UIColor.white
        
        super.init()
        
        NotificationCenter.default.addObserver(self, selector: #selector(self.didFinishLaunchingNotification), name: Notification.Name.UIApplicationDidFinishLaunching, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.didBecomeActive), name: Notification.Name.UIApplicationDidBecomeActive, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.didEnterBackground), name: Notification.Name.UIApplicationDidEnterBackground, object: nil)
    }
    
    @objc func didFinishLaunchingNotification() {
        removeCookies()
        setTabBarFont()
    }
    
    @objc func didBecomeActive() {
        validateSession(knownServices: self.knownServices, webView: self.webViewController.webView)
    }
    
    @objc func didEnterBackground() {
        showWhiteScreen();
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
        let presentedViewController = self.webViewController.presentedViewController;
        
        if presentedViewController == nil || !(presentedViewController is BlankViewController) {
            self.webViewController.present(blankViewController, animated: false, completion: nil)
        }
    }
    
    private func hideWhiteScreen() {
        let presentedViewController = self.webViewController.presentedViewController;
        
        if presentedViewController != nil && presentedViewController is BlankViewController {
            self.webViewController.dismiss(animated: false, completion: nil)
        }
    }
}
