import Foundation
import UIKit
import WebKit
import os.log

class LifecycleHandlers: NSObject {
    var validateSessionString: String = "window.validateSession()"
    var knownServices: KnownServices
    var webView: WKWebView
    
    init(knownServices: KnownServices, webView: WKWebView) {
        self.knownServices = knownServices
        self.webView = webView
        super.init()
        
        NotificationCenter.default.addObserver(self, selector: #selector(self.didBecomeActive), name: Notification.Name.UIApplicationDidBecomeActive, object: nil)
    }
    
    @objc func didBecomeActive() {
        validateSession(knownServices: self.knownServices, webView: self.webView)
    }
    
    private func validateSession(knownServices: KnownServices, webView: WKWebView) {
        if knownServices.shouldValidateSession(host: webView.url?.host) {
            webView.evaluateJavaScript(validateSessionString, completionHandler: nil)
        }
    }
}
