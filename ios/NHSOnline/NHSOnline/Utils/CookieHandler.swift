import Foundation
import WebKit
import SwiftyJSON

class CookieHandler {
    let cookieExpirationIntervalInMillis: Double = 31556926
    
    func setCookieFromNameAndValue(key: String, value: AnyObject) {
        let cookie = createCookie(key: key, value: value)

        addCookie(cookie: cookie)
    }
    
    func createCookie(key: String, value: AnyObject) -> HTTPCookie {
        let domain = config().HomeHost.dropLast()
        
        return HTTPCookie(properties: [
            .domain: String(domain),
            .path: "/",
            .name: key,
            .value: value,
            .secure: "TRUE",
            .expires: NSDate(timeIntervalSinceNow: cookieExpirationIntervalInMillis)
            ])!
    }
    
    func addCookie (cookie: HTTPCookie) {
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
    
    func cookieExists(name: String) -> Bool {
        var exists = false
        
        let cookies = HTTPCookieStorage.shared.cookies
        
        if (cookies?.isEmpty == true) {
            return exists
        }
        
        if (cookies!.first(where: { $0.name == name }) != nil) {
            exists = true
        }
        
        return exists
    }
    
    func createGenericCookieKey(name: String) -> String {
        return "nhso.\(name)"
    }
    
    func createUniqueCookieKey(name: String, uniqueIdentifier: String) -> String {
        return "nhso.\(name)-\(uniqueIdentifier)"
    }
}
