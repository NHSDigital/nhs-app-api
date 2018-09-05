import Foundation

 class KnownService {
    var urls = [URLComponents]()
    let service: KnownServices.Service
    let shouldAllowNativeInteraction: Bool
    let shouldValidateSession: Bool
    let serviceTitle: String?
    let serviceErrorMessage: ErrorMessage
    private var urlQueryItems = Array<URLQueryItem>()
    
    init(urlStrings:[String], serviceTitle: String? = "", service: KnownServices.Service, serviceErrorMessage: ErrorMessage,
         shouldAllowNativeInteraction: Bool = false,shouldValidateSession: Bool = true, urlQueryString:String? = nil) {
        self.urls = urlStrings.map { URLComponents(string: $0)! }
        self.service = service
        self.shouldAllowNativeInteraction = shouldAllowNativeInteraction
        self.shouldValidateSession = shouldValidateSession
        self.serviceTitle = serviceTitle
        self.serviceErrorMessage = serviceErrorMessage
        self.retrieveQueryKeyValueFrom(queryString: urlQueryString)
    }
    
    func getTitleFor(urlHost:String?)-> String? {
        return serviceTitle
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
        if self.urlQueryItems.isEmpty {
            return false
        }
        
        guard var urlComponents = URLComponents(string: urlString) else {
            return false
        }
        
        if (urlComponents.fragment != nil) {
            return false
        }
        
        guard let queryItems = urlComponents.queryItems else {
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
        if self.urlQueryItems.isEmpty {
            return urlString
        }
        
        guard var urlComponents = URLComponents(string: urlString) else {
            return urlString
        }
        
        if (urlComponents.fragment != nil) {
            return urlString
        }
        
        /*
         Using URLComponents strips encoding from the query string values.
         We don't want to meddle with the encoding (e.g. nhs symptom site is
         quite particular about some urls which are encoded already).
         So find out what extra query strings need added and add them to
         the raw string being requested.
         */
        let queryItems = urlComponents.queryItems ?? [URLQueryItem]()
        var queryItemsToAdd = [URLQueryItem]()
        
        urlQueryItems.forEach { urlQueryItem in
            let containsQueryItemAlready : Bool = urlComponents.queryItems?.contains(urlQueryItem) ?? false
            if containsQueryItemAlready == false {
                queryItemsToAdd.append(urlQueryItem)
            }
        }
        
        var stringToReturn = urlString
        
        if (queryItemsToAdd.count > 0) {
            if (queryItems.count == 0) {
                stringToReturn += "?"
            } else {
                stringToReturn += "&"
            }
            
            for (index, queryItem) in queryItemsToAdd.enumerated() {
                stringToReturn += "\(queryItem.name)=\(queryItem.value ?? "")"
                if (index != queryItemsToAdd.count - 1) {
                    stringToReturn += "&"
                }
            }
        }
        
        return stringToReturn
    }
}
