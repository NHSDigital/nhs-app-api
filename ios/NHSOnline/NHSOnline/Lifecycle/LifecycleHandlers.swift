import Foundation
import UIKit
import WebKit

class LifecycleHandlers: NSObject {
    var knownServices: KnownServices
    var webViewController: WebViewController?
    let validateSessionString: String = "window.validateSession()"
    
    init(knownServices: KnownServices, webViewController: WebViewController) {
        self.knownServices = knownServices
        self.webViewController = webViewController
        super.init()
        createLifecycleObservers()
    }
    
    func createLifecycleObservers() {
        NotificationCenter.default.addObserver(self, selector: #selector(self.didFinishLaunchingNotification), name: Notification.Name.UIApplicationDidFinishLaunching, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.didBecomeActive), name: Notification.Name.UIApplicationDidBecomeActive, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.didEnterBackground), name: Notification.Name.UIApplicationDidEnterBackground, object: nil)
    }
    
    @objc func didFinishLaunchingNotification() {
        removeCookies()
        setTabBarFont()
    }
    
    @objc func didBecomeActive() {
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
