import Foundation
import UIKit
import WebKit

class LifecycleHandlers: NSObject {
    var knownServices: KnownServices
    var webView: WKWebView
    
    let validateSessionString: String = "window.validateSession()"
    
    init(knownServices: KnownServices, webView: WKWebView) {
        self.knownServices = knownServices
        self.webView = webView
        super.init()
        
        NotificationCenter.default.addObserver(self, selector: #selector(self.didBecomeActive), name: Notification.Name.UIApplicationDidBecomeActive, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.didFinishLaunchingNotification), name: Notification.Name.UIApplicationDidFinishLaunching, object: nil)
    }
    
    @objc func didFinishLaunchingNotification() {
        removeCookies()
        setTabBarFont()
    }
    
    @objc func didBecomeActive() {
        validateSession(knownServices: self.knownServices, webView: self.webView)
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
            webView.evaluateJavaScript(validateSessionString, completionHandler: nil)
        }
    }
}
