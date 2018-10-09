import Foundation

class KnownService {
    var url:URL
    let service: KnownServices.Service
    var pathInfoDictionary : [String: Info] = [:]
    private var urlQueryItems = Array<URLQueryItem>()
    let defaultPathInfo: Info
    
    init(serviceUrl:String, service: KnownServices.Service, serviceError: ErrorMessage, title: String? = nil, accessibleTitle:String? = nil,
         validateSession: Bool = true, allowNativeInteraction: Bool = false, urlQueryString:String? = nil) {
        self.service = service
        
        var urlComponents = URLComponents(string: serviceUrl.lowercased())!
        let urlPath = urlComponents.path
        urlComponents.path = ""
        urlComponents.query = nil
        
        self.url = urlComponents.url!
        let baseUrl = self.url.absoluteString
        
        if urlPath.isEmpty || urlPath == "/" {
            self.defaultPathInfo = Info("", service, serviceError, baseUrl , validateSession, allowNativeInteraction, title, accessibleTitle)
        } else {
            self.defaultPathInfo = Info("", service, serviceError, baseUrl , validateSession, allowNativeInteraction)
            addPathInfo(path: urlPath, service: service, validateSession: validateSession, allowNativeInteraction: allowNativeInteraction, title: title, accessibleTitle:accessibleTitle)
        }
        self.retrieveQueryKeyValueFrom(queryString: urlQueryString)
    }
    
    func addPathInfo(path: String, service: KnownServices.Service, validateSession:Bool = true, allowNativeInteraction: Bool = false, title: String?, accessibleTitle:String? = nil, _ serviceError: ErrorMessage? = nil) {
        if path.isEmpty || path == "/" { return }
        let thePath = convertToInfoPathKey(path: path)
        let theError = serviceError ?? defaultPathInfo.serviceMessage
        let baseUrl = url.absoluteString
        
        pathInfoDictionary[thePath] = Info(thePath,service, theError, baseUrl , validateSession, allowNativeInteraction, title, accessibleTitle)
    }
    
    func findMatchingServicePathInfo(urlString: String, exactPathMatch: Bool = false)-> Info? {
        guard let theUrl = URL(string: urlString), theUrl.host == url.host else {
            return nil
        }
        return findMatchingServicePathInfoByPath(path: theUrl.path, exactPathMatch: exactPathMatch)
    }
    
    func findMatchingServicePathInfoByPath(path: String, exactPathMatch: Bool = false)-> Info? {
        if path.isEmpty || path == "/"{ return defaultPathInfo }
        let thePath = convertToInfoPathKey(path: path)
        if let matchingPathInfo = pathInfoDictionary[thePath] {
            return matchingPathInfo
        } else if exactPathMatch {
            return nil
        }
        return retrieveClosestPathInfo(path: thePath)
    }
    
    func hasMissingQueryString(urlString: String) -> Bool {
        if self.urlQueryItems.isEmpty || urlString.isEmpty {
            return false
        }
        
        guard var urlComponents = URLComponents(string: urlString.lowercased()),
            urlComponents.fragment == nil else {
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
        if self.urlQueryItems.isEmpty || urlString.isEmpty {
            return urlString
        }
        
        guard var urlComponents = URLComponents(string: urlString),
            urlComponents.fragment == nil else {
                return urlString
        }
        
        let queryItems = urlComponents.queryItems ?? [URLQueryItem]()
        var queryItemsToAdd = [URLQueryItem]()
        
        urlQueryItems.forEach { urlQueryItem in
            let containsQueryItemAlready : Bool = urlComponents.queryItems?.contains(urlQueryItem) ?? false
            if containsQueryItemAlready == false {
                queryItemsToAdd.append(urlQueryItem)
            }
        }
        
        return appendMissingQueriesToRawUrl(urlString, queryItemsToAdd, queryItems)
    }
    
    private func retrieveClosestPathInfo(path: String)-> Info? {
        var matchingKey = ""
        pathInfoDictionary.keys.forEach {pathKey in
            if (path.hasPrefix(pathKey)) {
                if (matchingKey.isEmpty) {
                    matchingKey = pathKey
                } else {
                    let currentWithoutMatchingPathKey = path.replacingOccurrences(of: matchingKey, with: "")
                    let newWithoutMatchingPathKey = path.replacingOccurrences(of: pathKey, with: "")
                    if (newWithoutMatchingPathKey.count < currentWithoutMatchingPathKey.count){
                        matchingKey = pathKey
                    }
                }
            }
        }
        return pathInfoDictionary[matchingKey]
    }
    
    /*
     Using URLComponents strips encoding from the query string values.
     We don't want to meddle with the encoding (e.g. nhs symptom site is
     quite particular about some urls which are encoded already).
     So find out what extra query strings need added and add them to
     the raw string being requested.
     */
    private func appendMissingQueriesToRawUrl(_ urlString: String, _ queryItemsToAdd: [URLQueryItem], _ queryItems: [URLQueryItem]) -> String {
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
    
    private func convertToInfoPathKey(path: String) -> String {
        var thePath = path
        if let firstChar = thePath.first, firstChar != "/" { thePath = "/\(thePath)" }
        if let lastChar = thePath.last, lastChar == "/" {thePath.removeLast() }
        return thePath.lowercased()
    }
    
    private func retrieveQueryKeyValueFrom(queryString: String?) {
        if let queryStringLowerCase = queryString?.lowercased() {
            let queryStringRemovedQuestionMark = queryStringLowerCase.replacingOccurrences(of: "?", with: "")
            let queryKeyValues = queryStringRemovedQuestionMark.components(separatedBy: "&")
            queryKeyValues.forEach { queryParameter in
                let keyValuePair = queryParameter.components(separatedBy: "=")
                
                if keyValuePair.count == 2 {
                    self.urlQueryItems.append(URLQueryItem(name: keyValuePair[0], value: keyValuePair[1]))
                }
            }
        }
    }
    
    struct Info {
        let path: String
        let serviceMessage: ErrorMessage
        let baseUrl: String
        let validateSession: Bool
        let allowNativeInteraction: Bool
        let serviceName: KnownServices.Service
        let title: String?
        let accessibleTitle: String?
        init(_ path:String, _ service:KnownServices.Service, _ serviceError :ErrorMessage, _ baseUrl:String, _ validateSession:Bool = true, _ allowNativeInteraction: Bool = false, _ title:String? = nil, _ accessibleTitle: String? = nil ) {
            self.path = path
            self.serviceName = service
            self.serviceMessage = serviceError
            self.baseUrl = baseUrl
            self.validateSession = validateSession
            self.allowNativeInteraction = allowNativeInteraction
            self.title = title
            self.accessibleTitle = accessibleTitle
        }
    }
}

