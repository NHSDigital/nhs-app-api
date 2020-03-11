package com.nhs.online.nhsonline.services.knownservices

import com.nhs.online.nhsonline.services.knownservices.enums.ViewMode
import java.net.URL

data class KnownServices(
    var knownServices: List<RootService>? = null
) {
    fun findMatchingKnownService(url: URL?): KnownService? {
        var rootService: RootService?
        var subService: SubService?

        url?.let { theUrl ->
            rootService = getRootServiceByHostAndScheme(theUrl.host, theUrl.protocol)
            rootService?.let { service ->
                subService = findMatchingSubService(service, theUrl.path, theUrl.query)
                subService?.let { return it }

                return service
            }
        }

        return null
    }

    fun getRootServiceByHostAndScheme(host: String, scheme: String): RootService? {
        var i = knownServices?.indexOfFirst { service ->
            val url = URL(service.url)
            url.host == host && url.protocol == scheme
        }

        i?.let { index ->
            if (index >= 0) return knownServices!![index]
        }

        return null
    }

    fun findMatchingSubService(rootService: RootService, path: String?, query: String?): SubService? {
        if (rootService.subServices.isNullOrEmpty()) {
            return null
        }

        if ((path.isNullOrBlank() || path == "/") && query.isNullOrBlank()) {
            return null
        }

        var bestServiceMatch: SubServiceMatch? = null

        var queryItems = queryStringToArray(query)

        rootService.subServices?.forEach { subService ->
            bestServiceMatch = compareServices(path, queryItems, bestServiceMatch, subService)
        }

        return bestServiceMatch?.subService
    }

    data class SubServiceMatch (
            var subService: SubService,
            var pathMatch: Boolean,
            var pathMatchCount: Int,
            var queryMatch: Boolean,
            var queryMatchCount: Int
    )

    fun queryStringToArray(query: String?): List<String> {
        if (query.isNullOrBlank()) {
            return emptyList()
        }

        return query.split("&")
    }

    fun compareServices(path: String?, queryItems: List<String>, bestServiceMatch: SubServiceMatch?, subService: SubService): SubServiceMatch? {
        var subServiceMatch = findMatch(path, queryItems, subService)

        if (!(subServiceMatch.pathMatch || subServiceMatch.queryMatch)) {
            return bestServiceMatch
        }

        if (bestServiceMatch == null) {
            return when((subServiceMatch.pathMatch && subServiceMatch.subService.queryString.isNullOrBlank())
                    || (subServiceMatch.queryMatch && subServiceMatch.queryMatchCount > 0)) {
                true -> subServiceMatch
                false -> null
            }
        }

        if (!subServiceMatch.pathMatch && bestServiceMatch.pathMatch) {
            return bestServiceMatch
        }

        if (subServiceMatch.pathMatch && !bestServiceMatch.pathMatch) {
            return subServiceMatch
        }

        if (!subServiceMatch.queryMatch && bestServiceMatch.queryMatch) {
            return bestServiceMatch
        }

        if (subServiceMatch.queryMatch && !bestServiceMatch.queryMatch) {
            return subServiceMatch
        }

        if (subServiceMatch.queryMatchCount > 0 || bestServiceMatch.queryMatchCount > 0) {
            return when(subServiceMatch.queryMatchCount > bestServiceMatch.queryMatchCount) {
                true -> subServiceMatch
                false -> bestServiceMatch
            }
        }

        return when(subServiceMatch.pathMatchCount > bestServiceMatch.pathMatchCount) {
            true -> subServiceMatch
            false -> bestServiceMatch
        }
    }

    fun findMatch(path: String?, queryItems: List<String>, subService: SubService): SubServiceMatch {
        val thePath = when(path.isNullOrBlank() || path == "/") {
            true -> "/"
            false -> "$path/"
        }

        val subServicePath = when(subService.path.isNullOrBlank() || subService.path == "/") {
            true -> "/"
            false -> "${subService.path}/"
        }

        val pathMatch = when(subServicePath != "/") {
            true -> thePath.startsWith(subServicePath)
            false -> subServicePath == thePath
        }

        val subServiceQueryItems = queryStringToArray(subService.queryString)

        if (subServiceQueryItems.isEmpty()) {
            return SubServiceMatch(
                    subService,
                    pathMatch,
                    subServicePath.length,
                    queryItems.isEmpty(),
                    0
            )
        }

        return SubServiceMatch(
                subService,
                pathMatch,
                subServicePath.length,
                queryItems.containsAll(subServiceQueryItems),
                queryItems.intersect(subServiceQueryItems).size
        )
    }
}