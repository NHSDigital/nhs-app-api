import Foundation
@testable import NHSOnline

class CookieHandlerMocks : CookieHandler {
    var addedCookie = false
    var cookieName = ""
    
    override func setCookieFromNameAndValue(key: String, value: AnyObject) {
        addedCookie = true
        cookieName = key
    }
    
    override func cookieExists(name: String) -> Bool {
        if (cookieName != name) {
            return false
        }
        
        return addedCookie
    }
    
    override func createGenericCookieKey(name: String) -> String {
        return "nhso.\(name)"
    }
    
    override func createUniqueCookieKey(name: String, uniqueIdentifier: String) -> String {
        return "nhso.\(name)-\(uniqueIdentifier)"
    }
}
