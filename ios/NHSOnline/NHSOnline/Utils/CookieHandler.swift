import Foundation
import WebKit
import SwiftyJSON

class CookieHandler {
    let cookieExpirationIntervalInMillis: Double = 31556926
    
    func setCookie(key: String, value: AnyObject) {
        let domain = config().HomeHost.dropLast()
        
        let cookie = HTTPCookie(properties: [
            .domain: String(domain),
            .path: "/",
            .name: key,
            .value: value,
            .secure: "TRUE",
            .expires: NSDate(timeIntervalSinceNow: cookieExpirationIntervalInMillis)
            ])!

        if #available(iOS 11.0, *) {
            setWKCookie(cookie)
        }
        else {
            setHTTPCookie(cookie)
        }
    }
    
    @available(iOS 11.0, *)
    func setWKCookie(_ cookie: HTTPCookie) {
        let cookieStore = WKWebsiteDataStore.default().httpCookieStore
        cookieStore.setCookie(cookie) { }
    }
    
    func setHTTPCookie(_ cookie: HTTPCookie) {
        HTTPCookieStorage.shared.cookieAcceptPolicy = HTTPCookie.AcceptPolicy.always
        HTTPCookieStorage.shared.setCookie(cookie)
    }
}
