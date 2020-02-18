import Foundation

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
}
