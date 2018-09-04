package com.nhs.online.nhsonline.services

import android.content.Context

import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessage
import java.net.URL


class KnownServices(private val context: Context) {
    private val serviceList = arrayListOf<KnownService>()
    private val unavailabilityErrorMessage =
        ErrorMessage(context.resources.getString(R.string.connection_error_title),
            context.resources.getString(R.string.connection_error_message))
    private val externalSites = arrayListOf<URL>()

    enum class ServiceName { NHS111, NHS_ONLINE, ORGAN_DONATION, UNKNOWN }

    init {
        buildKnownServices()
    }

    private fun buildKnownServices() {
        serviceList.add(KnownService(arrayOf(context.resources.getString(
            R.string.baseURL)),
            queryString = context.resources.getString(R.string.nhsOnlineRequiredQueries),
            unavailabilityErrorMessage = unavailabilityErrorMessage,
            shouldValidateSession = true))
        serviceList.add(KnownService(arrayOf(context.resources.getString(
            R.string.organDonation)),
            unavailabilityErrorMessage,
            nativeHeader = context.resources.getString(R.string.organ_donation_register_header),
            shouldValidateSession = false))
        serviceList.add(KnownService(arrayOf(context.resources.getString(
                R.string.dataSharing)),
                unavailabilityErrorMessage,
                nativeHeader = context.resources.getString(R.string.data_sharing_header),
                shouldValidateSession = false))
        serviceList.add(KnownService(arrayOf(context.resources.getString(
            R.string.nhs111)),
            unavailabilityErrorMessage,
            nativeHeader = context.resources.getString(R.string.nhs_111_header),
            shouldValidateSession = false))
        serviceList.add(KnownService(arrayOf(context.resources.getString(
                R.string.conditions)),
                unavailabilityErrorMessage,
                nativeHeader = context.resources.getString(R.string.conditions_header),
                shouldValidateSession = false))

        val externalStrings = context.resources.getStringArray(R.array.externalSiteUrls)
        for (i in externalStrings) {
            externalSites.add(URL(i))
        }
    }

    fun shouldURLOpenExternally(url: URL) : Boolean {
        if (externalSites.contains(url)) {
            return true
        }
        return false
    }

    fun isHotJar(url: URL): Boolean {
        val hotJarURL = URL(context.resources.getString(R.string.hotjarLink))
        if (url == hotJarURL) {
            return true
        }
        return false
    }

    fun findMatchingKnownService(urlString: String): KnownService? {
        val url = try {
            URL(urlString.toLowerCase())
        } catch (e: java.net.MalformedURLException) {
            return null
        }
        serviceList.forEach { knownService ->
            knownService.urlList.forEach { knownUrl ->
                if (knownUrl.host == url.host &&
                        (knownUrl.path == "" || knownUrl.path == "/" || knownUrl.path == url.path )) {
                return knownService
                }
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

    fun getServiceUnavailabilityError(): ErrorMessage {
        return unavailabilityErrorMessage
    }
}