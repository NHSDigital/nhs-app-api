package com.nhs.online.nhsonline.services

import android.net.Uri
import java.net.URL

 class KnownService(
        urlString: Array<String>,
        val shouldHandleUnavailability: Boolean = false,
        queryString: String? = null,
        val unavailabilityErrorMessage: String? = null,
        var nativeHeader: String? = null,
        var shouldValidateSession: Boolean = true
) {
    private val serviceQueryMap: MutableMap<String, String> = mutableMapOf()
    val urlList: Array<URL> = urlString.map { URL(it) }.toTypedArray()

    init {
        retrieveQueryStringKeyValuesInMap(queryString)
    }

    fun addMissingQueryStrings(url: String): String {
        if (serviceQueryMap.isEmpty()) {
            return url
        }
        val uri = Uri.parse(url)
        if(matchesThisServiceHost(uri)) {
            val uriBuilder = uri.buildUpon()
            serviceQueryMap.keys.forEach {
                if (uri.getQueryParameter(it) == null) {
                    uriBuilder.appendQueryParameter(it, serviceQueryMap[it])
                }
            }
            return uriBuilder.build().toString()
        }

        return url
    }

    fun hasMissingQueryString(urlString: String): Boolean {
        if (urlString.isEmpty()) {
            return false
        }
        val uri = Uri.parse(urlString.toLowerCase())
        if(matchesThisServiceHost(uri)) {
            serviceQueryMap.keys.forEach {
                if (uri.getQueryParameter(it) == null) {
                    return true
                }
            }
        }
        return false
    }

    fun hasNativeHeader() : Boolean {
        return !nativeHeader.isNullOrEmpty()
    }
     private fun matchesThisServiceHost(uri: Uri) : Boolean {
         for (url in urlList) {
             if (url.host.equals(uri.host,ignoreCase = true)) {
                 return true
             }
         }
         return false
     }

    private fun retrieveQueryStringKeyValuesInMap(queryString: String?) {
        val queryStringWithoutQuestionMark = queryString?.toLowerCase()?.replace("?", "")
        val queryStringKeyValues = queryStringWithoutQuestionMark?.split("&")
        queryStringKeyValues?.forEach {
            val queryStringKeyValue = it.split("=")
            if (queryStringKeyValue.size == 2) {
                serviceQueryMap[queryStringKeyValue[0]] = queryStringKeyValue[1]
            }
        }
    }
}