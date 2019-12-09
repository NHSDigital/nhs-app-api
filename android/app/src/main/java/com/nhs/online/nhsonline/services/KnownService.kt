package com.nhs.online.nhsonline.services

import android.net.Uri
import java.net.URI
import java.net.URL

class KnownService(
    serviceUrl: String,
    header: String? = null,
    nativeHeaderDescription: String? = null,
    shouldValidateSession: Boolean = true
) {
    private val url: URL
    private val pathInfos = mutableMapOf<String, Info>()
    private val default: Info

    init {
        val uri = URI(serviceUrl)
        url = URI(uri.scheme, uri.authority, null, null, null).toURL()

        default = Info("", url.toString(),
            shouldValidateSession, header, nativeHeaderDescription)

        generatePathInfoFrom(uri.path)
    }

    fun addPathInfo(
        path: String, shouldValidateSession: Boolean,
        header: String, nativeHeaderDescription: String? = null
    ) {
        if (path.isBlank() || path == "/")
            return
        var thePath = if (path.first() != '/') "/$path" else path
        thePath = if (thePath.last() == '/') thePath.substring(0, thePath.length - 1) else thePath
        pathInfos[thePath] = Info(thePath, default.baseUrl,
            shouldValidateSession, header, nativeHeaderDescription)
    }

    fun getUrl(): URL {
        return url
    }

    fun hasDefaultNativeHeader(): Boolean {
        return default.header != null
    }

    fun findMatchingServicePathInfo(urlString: String, exactPathMatch: Boolean = false): Info? {
        val theUrl = URL(urlString)
        if (!theUrl.host.equals(url.host, true))
            return null
        return findMatchingServicePathInfoByPath(theUrl.path, exactPathMatch)
    }

    fun findMatchingServicePathInfoByPath(path: String, exactPathMatch: Boolean = false): Info? {
        if (path.isEmpty() || path == "/")
            return default

        var thePath = if (path.first() != '/') "/$path" else path
        thePath = if (thePath.last() == '/') thePath.substring(0, thePath.length - 1) else thePath
        val matchingPathInfo = pathInfos[thePath]
        if (matchingPathInfo != null)
            return matchingPathInfo
        else if (exactPathMatch)
            return null
        return retrieveClosestPathInfo(thePath)
    }

    private fun retrieveClosestPathInfo(path: String): Info? {
        var matchingKey = ""
        pathInfos.keys.forEach { pathKey ->
            if (path.startsWith(pathKey, true)) {
                if (matchingKey.isEmpty()) {
                    matchingKey = pathKey
                } else {
                    val currentWithoutMatchingPathKey = path.replace(matchingKey, "")
                    val newWithoutMatchingPathKey = path.replace(pathKey, "")
                    if (newWithoutMatchingPathKey.length < currentWithoutMatchingPathKey.length)
                        matchingKey = pathKey
                }
            }
        }
        return if (matchingKey.isEmpty()) default else pathInfos[matchingKey]
    }

    private fun buildQueryStringMap(queryStrings: String?): Map<String, String>? {
        val queries = queryStrings ?: ""
        if (queries.isEmpty())
            return null

        val queriesWithNoQMark = queries.toLowerCase().replace("?", "")
        val queryKeyValues = queriesWithNoQMark.split("&")
        val queryMap = mutableMapOf<String, String>()

        queryKeyValues.forEach {
            val queryKV = it.split("=")
            if (queryKV.size == 2) {
                queryMap[queryKV[0]] = queryKV[1]
            }
        }
        return queryMap
    }

    private fun generatePathInfoFrom(urlPath: String) {
        if (urlPath.isBlank() || (urlPath.length == 1 && urlPath.last() == '/'))
            return

        val path: String =
            if (urlPath.last() == '/') urlPath.substring(urlPath.length - 1) else urlPath

        pathInfos[path] = default.copy(path = path)
    }

    data class Info(
        val path: String,
        val baseUrl: String,
        val shouldValidateSession: Boolean = true,
        val header: String? = null,
        val nativeHeaderDescription: String? = null
    )
}