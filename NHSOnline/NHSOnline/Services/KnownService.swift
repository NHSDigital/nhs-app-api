import Foundation

class KnownService {
    let url:URLComponents
    let shouldHandleUnavailability:Bool
    private var urlQueryItems = Array<URLQueryItem>()
    
    init(urlString:String, shouldHandleUnavailability:Bool = false, urlQueryString:String? = nil) {
        self.url = URLComponents(string: urlString)!
        self.shouldHandleUnavailability = shouldHandleUnavailability
        self.retrieveQueryKeyValueFrom(queryString: urlQueryString)
    }
    
    private func retrieveQueryKeyValueFrom(queryString:String?) {
        if queryString != nil {
            let queryStringRemovedQuestionMark = queryString?.replacingOccurrences(of: "?", with: "")
            let queryKeyValues = queryStringRemovedQuestionMark?.components(separatedBy: "&")
            queryKeyValues?.forEach { queryParameter in
                let keyValuePair = queryParameter.components(separatedBy: "=")
                
                if keyValuePair.count == 2 {
                    self.urlQueryItems.append(URLQueryItem(name: keyValuePair[0], value: keyValuePair[1]))
                }
            }
        }
    }
    
    func hasMissingQueryString(urlString: String) -> Bool {
        if(self.urlQueryItems.isEmpty) {
            return false
        }
        
        guard var urlComponents = URLComponents(string: urlString) else {
            return false
        }
        
        guard let queryItems  = urlComponents.queryItems else {
            return true
        }
        
        for urlQueryItem in urlQueryItems {
            if !queryItems.contains(urlQueryItem) {
                return true
            }
        }
        
        return false
    }
    
    func addingMissingQueryParameters(urlString:String) -> String {
        if(self.urlQueryItems.isEmpty) {
            return urlString
        }
        
        guard var urlComponents = URLComponents(string: urlString) else {
            return urlString
        }
        
        var queryItems = urlComponents.queryItems ?? [URLQueryItem]()
        
        urlQueryItems.forEach { urlQueryItem in
            if(!queryItems.contains(urlQueryItem)) {
                queryItems.append(urlQueryItem)
            }
        }
        
        urlComponents.queryItems = queryItems
        
        return urlComponents.string ?? urlString
    }
}
