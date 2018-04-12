package com.nhs.online.nhsonline.services

import android.content.Context
import android.net.Uri
import com.nhs.online.nhsonline.R
import java.net.URL


class KnownServices(private val context: Context) {
    private val serviceList = arrayListOf<Service>()

    enum class ServiceName { NHS111, NHS_ONLINE, ORGAN_DONATION, UNKNOWN }

    init {
        buildKnownServices()
    }

    private fun buildKnownServices() {
        serviceList.add(Service(context.resources.getString(
            R.string.baseURL),
            true,
            queryString = context.resources.getString(R.string.nhsOnlineRequiredQueries),
            unavailabilityErrorMessage = context.resources.getString(R.string.connection_error)))
        serviceList.add(Service(context.resources.getString(
            R.string.organDonation),
            true,
            null,
             unavailabilityErrorMessage = context.resources.getString(R.string.organ_donation_connection_error)))
        serviceList.add(Service(context.resources.getString(
            R.string.nhs111),
            true,
            null,
            unavailabilityErrorMessage = context.resources.getString(R.string.nhs111_connection_error)))
    }

    fun findMatchingKnownService(urlString: String): Service? {
        val url = try{ URL(urlString.toLowerCase())} catch (e: java.net.MalformedURLException){return null}
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

    fun isTheService(service: Service?): ServiceName {
        return when (service?.url?.host) {
            getHostOfUrl(context.resources.getString(R.string.nhs111)) -> ServiceName.NHS111
            getHostOfUrl(context.resources.getString(R.string.organDonation)) -> ServiceName.ORGAN_DONATION
            getHostOfUrl(context.resources.getString(R.string.baseURL)) -> ServiceName.NHS_ONLINE
            else -> ServiceName.UNKNOWN
        }
    }

    private fun getHostOfUrl(urlString: String): String {
        val url = URL(urlString)
        return url.host
    }


    class Service(
        urlString: String,
        val shouldHandleUnavailability: Boolean = false,
        queryString: String? = null,
        val unavailabilityErrorMessage: String? = null
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

        private fun matchesThisServiceHost(uri: Uri) : Boolean {
            return url.host.equals(uri.host,ignoreCase = true)
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
}