import UIKit
import SafariServices
import WebKit
import os.log
import Foundation

class KnownServices {
    let rootServices: [RootService]?
    let wildCard = "*"

    init() {
        self.rootServices = nil
    }

    init(_ rootServices: [RootService]?) {
        self.rootServices = rootServices
    }

    public func findMatchingKnownService(_ url: URL?) -> KnownService {
        guard let theUrl = url, let rootService = getRootServiceByHostAndScheme(host: (theUrl.host)!, scheme: (theUrl.scheme)!) else {
            return RootService(url: "", javaScriptInteractionMode: .None, menuTab: .None, integrationLevel: .Bronze, validateSession: false, subServices: nil)
        }
        guard let subService = findMatchingSubService(rootService: rootService, path: theUrl.path, query: theUrl.query) else {
            return rootService
        }
        return subService
    }

    private func findMatchingSubService(rootService: RootService, path: String?, query: String?) -> KnownService? {
        guard let subServices = rootService.subServices else {
            return nil
        }
        if (path == nil || path == "" || path == "/") && (query == nil) {
            return nil
        }

        var bestServiceMatch: SubServiceMatch?

        let queryItems = queryStringToArray(query)

        for subService in subServices {
            bestServiceMatch = compareServices(path: path, queryItems: queryItems, bestServiceMatch: bestServiceMatch, subService: subService)
        }
        return bestServiceMatch?.subService
    }

    private func findMatch(path: String?, queryItems: [String], subService: SubService) -> SubServiceMatch {
        let thePath = makePathSafe(path: path)
        let subServicePath = makePathSafe(path: subService.path)

        let pathMatch = subServicePath != "/" ? isPathMatch(subServicePath: subServicePath, path: thePath) : subServicePath == thePath
        let subServiceQueryItems = queryStringToArray(subService.queryString)

        if subServiceQueryItems.isEmpty {
            return SubServiceMatch.init(
                    subService: subService,
                    pathMatch: pathMatch,
                    pathMatchCount: subServicePath.count,
                    queryMatch: queryItems.isEmpty ? true : false,
                    queryMatchCount: 0)
        }

        return SubServiceMatch.init(
                subService: subService,
                pathMatch: pathMatch,
                pathMatchCount: subServicePath.count,
                queryMatch: Set(subServiceQueryItems).isSubset(of: Set(queryItems)),
                queryMatchCount: Set(subServiceQueryItems).intersection(Set(queryItems)).count
        )
    }
    
    private func isPathMatch(subServicePath: String, path: String) -> Bool {
        let pathComponents = path.split(separator: "/", omittingEmptySubsequences: true)
        let subServicePathComponents = subServicePath.split(separator: "/", omittingEmptySubsequences: true)
        
        if (pathComponents.count < subServicePathComponents.count) { return false }
        
        for i in 0 ..< subServicePathComponents.count {
            if((subServicePathComponents[i] != wildCard && subServicePathComponents[i] != pathComponents[i]) ||
                subServicePathComponents[i] == wildCard && pathComponents[i].isEmpty) {
            return false
            }
        }
        
        return true
    }

    private func makePathSafe(path: String?) -> String {
        path != nil && path != "/" ? path! + "/" : "/"
    }

    private func compareServices(path: String?, queryItems: [String], bestServiceMatch: SubServiceMatch?, subService: SubService) -> SubServiceMatch? {
        let subServiceMatch = findMatch(path: path, queryItems: queryItems, subService: subService)

        if !(subServiceMatch.pathMatch || subServiceMatch.queryMatch) {
            return bestServiceMatch
        }

        if bestServiceMatch == nil {
            return subServiceMatch.pathMatch && subServiceMatch.subService.queryString == nil || (subServiceMatch.queryMatch && subServiceMatch.queryMatchCount > 0) ? subServiceMatch : nil
        }

        if !subServiceMatch.pathMatch && bestServiceMatch!.pathMatch {
            return bestServiceMatch
        }

        if subServiceMatch.pathMatch && !bestServiceMatch!.pathMatch {
            return subServiceMatch
        }

        if !subServiceMatch.queryMatch && bestServiceMatch!.queryMatch {
            return bestServiceMatch
        }

        if subServiceMatch.queryMatch && !bestServiceMatch!.queryMatch {
            return subServiceMatch
        }

        if subServiceMatch.queryMatchCount > 0 || bestServiceMatch!.queryMatchCount > 0 {
            return subServiceMatch.queryMatchCount > bestServiceMatch!.queryMatchCount ? subServiceMatch : bestServiceMatch
        }

        return subServiceMatch.pathMatchCount > bestServiceMatch!.pathMatchCount ? subServiceMatch : bestServiceMatch
    }

    private func queryStringToArray(_ query: String?) -> [String] {
        guard let theQuery: String = query else {
            return []
        }
        return theQuery.components(separatedBy: "&")
    }

    public func getRootServiceByHostAndScheme(host: String, scheme: String) -> RootService? {
        if let i = rootServices?.firstIndex(where: {
            URL(string: $0.url)?.host! == host && URL(string: $0.url)?.scheme! == scheme
        }) {
            return rootServices?[i]
        }
        return nil
    }

    struct SubServiceMatch {
        let subService: SubService
        let pathMatch: Bool
        let pathMatchCount: Int
        let queryMatch: Bool
        let queryMatchCount: Int
    }
}
