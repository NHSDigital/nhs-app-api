package com.nhs.online.nhsonline.services

import android.content.Context
import android.net.Uri
import com.nhs.online.nhsonline.R
import java.net.URL


class KnownServices(private val context: Context) {
    private val serviceList = arrayListOf<Service>()

    enum class ServiceName { NHS111, NHS_ONLINE, ORGAN_DONATION }

    init {
        buildKnownServices()
    }

    private fun buildKnownServices() {
        serviceList.add(Service(context.resources.getString(
            R.string.baseURL),

            queryString = context.getString(R.string.nhsOnlineRequiredQueries)))
        serviceList.add(Service(context.resources.getString(
            R.string.nhs111),
            true))

    }

    fun findMatchingKnownService(urlString: String): Service? {
        val url = URL(urlString)
        serviceList.forEach { knownService ->
            if (knownService.url.host == url.host) {
                return knownService
            }
        }
        return null
    }

    fun findKnownServiceAddMissingQueryFor(urlString: String): String {
        val matchingKnownService = findMatchingKnownService(urlString)

        return if (matchingKnownService != null && matchingKnownService.hasMissingQueryString(
                urlString)) {
            matchingKnownService.addMissingQueryStrings(urlString)
        } else {
            urlString
        }
    }

    fun isTheService(service: Service, name: ServiceName): Boolean {
        return when (name) {
            ServiceName.NHS111 -> service.url.host == getHostOfUrl(context.resources.getString(R.string.nhs111))
            ServiceName.NHS_ONLINE-> service.url.host == getHostOfUrl(context.resources.getString(R.string.baseURL))
            else -> false
        }
    }

    private fun getHostOfUrl(urlString: String): String {
        val url = URL(urlString)
        return url.host
    }


    class Service(
        urlString: String,
        val shouldHandleUnavailability: Boolean = false,
        queryString: String? = null
    ) {
        private val serviceQueryMap: MutableMap<String, String> = mutableMapOf()
        val url: URL = URL(urlString)

        init {
            retrieveQueryStringKeyValuesInMap(queryString)
        }

        fun addMissingQueryStrings(url: String): String {
            if (serviceQueryMap.isEmpty()) {
                return url
            }

            val uri = Uri.parse(url)
            val uriBuilder = uri.buildUpon()
            serviceQueryMap.keys.forEach {
                if (uri.getQueryParameter(it) == null) {
                    uriBuilder.appendQueryParameter(it, serviceQueryMap[it])
                }
            }

            return uriBuilder.build().toString()
        }

        fun hasMissingQueryString(urlString: String): Boolean {
            if (urlString.isEmpty()) {
                return false
            }
            val uri = Uri.parse(urlString)
            serviceQueryMap.keys.forEach {
                if (uri.getQueryParameter(it) == null) {
                    return true
                }
            }
            return false
        }

        private fun retrieveQueryStringKeyValuesInMap(queryString: String?) {
            val queryStringWithoutQuestionMark = queryString?.replace("?", "")
            val queryStringKeyValues = queryStringWithoutQuestionMark?.split("&")
            queryStringKeyValues?.forEach {
                val queryStringKeyValue = it.split("=")
                if (queryStringKeyValue.size == 2) {
                    serviceQueryMap[queryStringKeyValue[0]] = queryStringKeyValue[1]
                }
            }
        }
    }
}