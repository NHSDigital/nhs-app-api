import Foundation
import UIKit

class UrlHelper {
    private static let defaultScheme: String = "https"
    
    static func ensureUrlWithScheme(url : String) -> URL? {
        guard var components = URLComponents(string: url) else {
            return nil
        }
        if(components.scheme == nil)
        {
            guard let resolvedUrl = URL(string: "\(defaultScheme)://\(url)") else {
                return nil
            }
            return resolvedUrl
        }
        
        if(components.scheme == config().AppScheme) {
            components.scheme = config().BaseScheme
        }
        
        return components.url
    }
    
    static func resolveAppScheme(url: URL) -> URL {
        if(url.scheme == config().AppScheme) {
            var comps = URLComponents(url: url, resolvingAgainstBaseURL: false)!
            comps.scheme = config().BaseScheme
            return comps.url!
        }
        
        return url
    }
    
    static func resolveAppScheme(url: String) -> URL? {
        return ensureUrlWithScheme(url: url)
    }


    static func getReloadUrl(url:URL) -> URL {
        if url.absoluteString.starts(with: config().DataPreferencesURL) {
            return URL(string: config().DataSharingUrlPath, relativeTo: URL(string: config().HomeUrl))!
        }
        return url
    }

    static func isSameSchemeAndHostAsHomeUrl(url: URL?) -> Bool {
        if let homeUrl = URL(string: config().HomeUrl) {
            if(url?.scheme == config().BaseScheme) {
                return homeUrl.host == url?.host
            }
            return false
        }
        return false
    }
    
    static func isValidHomeUrl(url: URL?) -> Bool {
        if let homeUrl = URL(string: config().HomeUrl) {
            return homeUrl.host == url?.host && homeUrl.path == url?.path
        }
        return false
    }

    static func verifyUrl(urlString: String?) -> Bool {
        guard let urlString = urlString, let url = URL(string: urlString) else {
            return false
        }
        return UIApplication.shared.canOpenURL(url)
    }
    
    static func createRedirectToUrl(url : String) -> URL? {
        let resolvedUrl = UrlHelper.ensureUrlWithScheme(url: "\(config().RedirectorUrl)?redirect_to=\(url)")
        return resolvedUrl
    }
    
    static func createRedirectToPageUrl(page : String) -> URL? {
        return UrlHelper.ensureUrlWithScheme(url: "\(config().RedirectorUrl)?redirect_to_page=\(page)")
    }
    
    static func checkForUrlOverride(url : String) -> String {
        guard let urlToLoad = UserDefaults.standard.url(forKey: config().LinkPropertyName) else {
            return url
        }
        UserDefaults.standard.removeObject(forKey: config().LinkPropertyName)
        return urlToLoad.absoluteString
    }    
}
