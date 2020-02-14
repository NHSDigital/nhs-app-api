import UIKit
import SafariServices
import WebKit
import os.log
import Foundation

class KnownServices {
    let rootServices: [RootService]?

    init() {
        self.rootServices = nil
    }

    init(_ rootServices: [RootService]?) {
        self.rootServices = rootServices
    }

    public func findMatchingKnownService(_ url: URL?) -> KnownService {
        guard let theUrl = url, let rootService = getRootServiceByHostAndScheme(host: (theUrl.host)!, scheme: (theUrl.scheme)!) else {
            return RootService(url: "", javaScriptInteractionMode: .None, menuTab: .None, viewMode: .AppTab, validateSession: false, subServices: nil)
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

        let pathMatch = subServicePath != "/" ? thePath.hasPrefix(subServicePath) : subServicePath == thePath
        let subServiceQueryItems = queryStringToArray(subService.queryString)

        if subServiceQueryItems.isEmpty {
            return SubServiceMatch.init(
                    subService: subService,
                    pathMatch: pathMatch,
                    queryMatch: queryItems.isEmpty ? true : false,
                    queryMatchCount: 0)
        }

        return SubServiceMatch.init(
                subService: subService,
                pathMatch: pathMatch,
                queryMatch: Set(subServiceQueryItems).isSubset(of: Set(queryItems)),
                queryMatchCount: Set(subServiceQueryItems).intersection(Set(queryItems)).count
        )
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

        return subServiceMatch.queryMatchCount > bestServiceMatch!.queryMatchCount ? subServiceMatch : bestServiceMatch
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

    func getUnavailabilityErrorMessageForService(_ url: URL?) -> ErrorMessage {
        guard let theUrl = url, getRootServiceByHostAndScheme(host: (theUrl.host)!, scheme: (theUrl.scheme)!) != nil else {
            return ErrorMessage(.ServiceUnavailable)
        }
        return ErrorMessage(.NoInternetConnection)
    }

    struct SubServiceMatch {
        let subService: SubService
        let pathMatch: Bool
        let queryMatch: Bool
        let queryMatchCount: Int
    }
}