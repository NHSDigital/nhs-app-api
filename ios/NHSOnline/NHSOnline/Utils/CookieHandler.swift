import Foundation
import WebKit
import SwiftyJSON

class CookieHandler {
    func getAccessTokenFromCookie(completion : @escaping (_: String?) -> ())  {
        if #available(iOS 11.0, *) {
            self.getAccessToken(completion: { accessToken in
                if let token = accessToken {
                    completion(token)
                }else{
                    completion(nil)
                }
            })
        } else {
            completion(getAccessTokeniOS10OrBelow())
        }
    }
    
    @objc func retryReadAccessToken() {
        struct Holder {
            static var cookieRetryCount = 0
        }
        
        if Holder.cookieRetryCount < 3 {
            Holder.cookieRetryCount += 1
            self.readAccessTokenFromCookie()
        }
    }
    
    func readAccessTokenFromCookie(){
        let timer = Timer.scheduledTimer(timeInterval: 3, target: self,
                             selector: #selector(retryReadAccessToken), userInfo: nil, repeats: false)
        self.getAccessTokenFromCookie(completion: { accessToken in
            if let token = accessToken {
                UserDefaultsManager.setAccessToken(token)
                timer.invalidate()
            }else{
                timer.invalidate()
                NSLog("Could not retrieve access token")
            }
        })
    }
    
    func sortCookie(jsonString: String) -> String? {
        if let jsonData = jsonString.data(using: .utf8) {
             let json = JSON(jsonData)
            if let accessToken = json["accessToken"].string {
                return accessToken
            }
        }
        return nil
    }
    
    func getAccessTokeniOS10OrBelow() -> String? {
        if let cookies = HTTPCookieStorage.shared.cookies {
            if (cookies.count > 0) {
                for cookie in cookies {
                    if (cookie.name == config().SessionCookieName) {
                        if let token = cookie.value.decodeUrl() {
                            return self.sortCookie(jsonString: token)
                        }
                    }
                }
            }
        }
        return nil
    }
    
    @available(iOS 11.0, *)
    func getAccessToken(completion : @escaping (_: String?) -> ()) {
        let httpCookieStore = WKWebsiteDataStore.default().httpCookieStore
        httpCookieStore.getAllCookies { (cookies) in
            if (cookies.count > 0) {
                for cookie in cookies {
                    if (cookie.name == config().SessionCookieName) {
                        if let token = cookie.value.decodeUrl() {
                            return completion(self.sortCookie(jsonString: token))
                        }
                    }
                }

            }
        completion(nil)
        }
    }
}
