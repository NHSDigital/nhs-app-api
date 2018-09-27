package com.nhs.online.nhsonline.services

import android.content.Context

import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessage
import java.net.URL

class KnownServices(private val context: Context) {
    private val serviceList = arrayListOf<KnownService>()
    private val internalServiceList = arrayListOf<KnownService>()
    private val unavailabilityErrorMessage =
        ErrorMessage(context.resources.getString(R.string.connection_error_title),
            context.resources.getString(R.string.connection_error_message), context.resources.getString(R.string.Accessible_connection_error_message))
    private val externalSites = arrayListOf<URL>()

    enum class ServiceName { NHS111, NHS_ONLINE, ORGAN_DONATION, UNKNOWN }

    init {
        buildKnownServices()
        buildInternalServices()
    }

    private fun buildKnownServices() {
        serviceList.add(KnownService(arrayOf(context.resources.getString(
                R.string.baseURL)),
            queryString = context.resources.getString(R.string.nhsOnlineRequiredQueries),
            unavailabilityErrorMessage = unavailabilityErrorMessage,
            nativeHeader = context.resources.getString(R.string.home_header),
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
                nativeHeaderDescription = context.resources.getString(R.string.conditions_header_description),
                shouldValidateSession = false))
        serviceList.add(KnownService(arrayOf(context.resources.getString(
                R.string.dataPreferencesBaseUrl)),
                unavailabilityErrorMessage,
                nativeHeader = context.resources.getString(R.string.data_preferences_header),
                shouldValidateSession = false))


        val externalStrings = context.resources.getStringArray(R.array.externalSiteUrls)
        for (i in externalStrings) {
            externalSites.add(URL(i))
        }
    }

    private fun buildInternalServices() {
        internalServiceList.add(KnownService(arrayOf(getFullInternalUrl(context.resources.getString(R.string.symptomsPath))),
            unavailabilityErrorMessage,
            nativeHeader = context.resources.getString(R.string.symptoms_header),
            shouldValidateSession = true))
        internalServiceList.add(KnownService(arrayOf(getFullInternalUrl(context.resources.getString(R.string.appointmentsPath))),
            unavailabilityErrorMessage,
            nativeHeader = context.resources.getString(R.string.appointments_header),
            shouldValidateSession = true))
        internalServiceList.add(KnownService(arrayOf(getFullInternalUrl(context.resources.getString(R.string.prescriptionsPath))),
            unavailabilityErrorMessage,
            nativeHeader = context.resources.getString(R.string.prescriptions_header),
            shouldValidateSession = true))
        internalServiceList.add(KnownService(arrayOf(getFullInternalUrl(context.resources.getString(R.string.myRecordPath))),
            unavailabilityErrorMessage,
            nativeHeader = context.resources.getString(R.string.my_record_header),
            shouldValidateSession = true))
        internalServiceList.add(KnownService(arrayOf(getFullInternalUrl(context.resources.getString(R.string.myAccountPath))),
            unavailabilityErrorMessage,
            nativeHeader = context.resources.getString(R.string.my_account_header),
            shouldValidateSession = true))
        internalServiceList.add(KnownService(arrayOf(getFullInternalUrl(context.resources.getString(R.string.morePath))),
            unavailabilityErrorMessage,
            nativeHeader = context.resources.getString(R.string.more),
            shouldValidateSession = true))
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
        return findMatchingService(urlString, serviceList)
    }

    fun findMatchingInternalService(urlString: String): KnownService? {
        return findMatchingService(urlString, internalServiceList)
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

    private fun getFullInternalUrl(urlPath: String): String {
        val baseUrl = context.resources.getString(R.string.baseURL)
        return(baseUrl + urlPath)
    }

    private fun findMatchingService(urlString: String, services: ArrayList<KnownService>): KnownService? {
        val url = try {
            URL(urlString.toLowerCase())
        } catch (e: java.net.MalformedURLException) {
            return null
        }
        services.forEach { knownService ->
            knownService.urlList.forEach { knownUrl ->
                if (knownUrl.host == url.host &&
                        (knownUrl.path == "" || knownUrl.path == "/" || knownUrl.path == url.path )) {
                    return knownService
                }
            }
        }
        return null
    }
}