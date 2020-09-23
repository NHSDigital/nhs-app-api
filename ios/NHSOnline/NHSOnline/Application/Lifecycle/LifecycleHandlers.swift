import Foundation
import os.log
import DeviceKit
import UIKit
import WebKit

class LifecycleHandlers: NSObject {
    var knownServicesProvider: KnownServicesProtocol
    var webViewController: WebViewController?
    var hasCheckedAppVersionSinceAppOpened = false
    var homeViewController: HomeViewController
    var configurationServiceProvider: ConfigurationServiceProtocol
    let validateSessionString: String = "window.validateSession()"

    init(knownServiceProvider: KnownServicesProtocol, webViewController: WebViewController, homeViewController: HomeViewController,
         configurationServiceProvider: ConfigurationServiceProtocol) {
        self.knownServicesProvider = knownServiceProvider
        self.webViewController = webViewController
        self.homeViewController = homeViewController
        self.configurationServiceProvider = configurationServiceProvider
        super.init()
        createLifecycleObservers()
    }

    func createLifecycleObservers() {
        NotificationCenter.default.addObserver(self, selector: #selector(self.didFinishLaunchingNotification),
                name: Notification.Name.UIApplicationDidFinishLaunching, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.appEnteredForeground),
                name: Notification.Name.UIApplicationWillEnterForeground, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.didBecomeActive),
                name: Notification.Name.UIApplicationDidBecomeActive, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.didEnterBackground),
                name: Notification.Name.UIApplicationDidEnterBackground, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.performAppVersionCheck),
                name: CustomNotifications.pageUnavailabilityOnReloadWebView, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.showAPIUnavailableError),
                name: CustomNotifications.apiLoadFailure, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(self.apiCallSuccessful),
                name: CustomNotifications.apiLoadSuccess, object: nil)
    }

    @objc func performAppVersionCheck(onQueue queue: DispatchQueue = DispatchQueue.main) {
        if (hasCheckedAppVersionSinceAppOpened == false) {
            switch configurationServiceProvider.getConfigurationResponse() {
            case .success(let configurationResponse):
                if (!configurationResponse.isSupportedVersion) {
                    queue.async {
                        self.displayAppVersionOutOfDate()
                    }
                }
            default:
                return
            }
        }
    }

    @objc func showAPIUnavailableError() {
        self.homeViewController.apiCallFailure()
    }

    @objc func apiCallSuccessful() {
        self.homeViewController.apiConfigCallError = false
    }
    
    @objc func appEnteredForeground() {
        switch knownServicesProvider.getKnownServices() {
        case .success(let knownServices):
            if knownServices.findMatchingKnownService(webViewController?.webView.url).validateSession {
                validateSession(webView: (self.webViewController?.webView)!)
            } else {
                hideWhiteScreen()
            }

            self.homeViewController.delayedBiometricsStart(0.5)
        default:
            hideWhiteScreen()
            self.homeViewController.delayedBiometricsStart(0.5)
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

        alert.show()
        self.hasCheckedAppVersionSinceAppOpened = true
    }

    @objc func didFinishLaunchingNotification() {
        removeCookies()
        setTabBarFont()
    }

    @objc func didBecomeActive() {
        performAppVersionCheck()
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

    private func validateSession(webView: WKWebView) {
        let completionHandler: (Any?, Error?) -> Void = {
            (data, error) in
            self.hideWhiteScreen()
        }
        webView.evaluateJavaScript(validateSessionString, completionHandler: completionHandler)
    }

    private func showWhiteScreen() {
        guard let _ = UIApplication.shared.keyWindow?.viewWithTag(1) else {
            let window = UIApplication.shared.keyWindow!
            let v = UIView(frame: window.frame)
            v.backgroundColor = UIColor.white
            v.tag = 1
            window.addSubview(v);
            return
        }
    }

    private func hideWhiteScreen() {
        UIApplication.shared.keyWindow?.viewWithTag(1)?.removeFromSuperview()
        homeViewController.resumeProgressBar()
    }
}
