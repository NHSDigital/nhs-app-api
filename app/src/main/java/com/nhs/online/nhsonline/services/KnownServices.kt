package com.nhs.online.nhsonline.services

import android.content.Context

import com.nhs.online.nhsonline.R
import java.net.URL


class KnownServices(private val context: Context) {
    private val serviceList = arrayListOf<KnownService>()

    enum class ServiceName { NHS111, NHS_ONLINE, ORGAN_DONATION, UNKNOWN }

    init {
        buildKnownServices()
    }

    private fun buildKnownServices() {

        serviceList.add(KnownService(arrayOf(context.resources.getString(
            R.string.baseURL)),
            true,
            queryString = context.resources.getString(R.string.nhsOnlineRequiredQueries),
            unavailabilityErrorMessage = context.resources.getString(R.string.service_unavailable)))

        serviceList.add(KnownService(arrayOf(context.resources.getString(
            R.string.organDonation)),
            true,
            null,
             unavailabilityErrorMessage = context.resources.getString(R.string.organ_donation_connection_error),
             nativeHeader = context.resources.getString(R.string.organ_donation_register_header)))

        serviceList.add(KnownService(arrayOf(context.resources.getString(
            R.string.nhs111), context.resources.getString(
                R.string.nhs111Location)),
            true,
            null,
            unavailabilityErrorMessage = context.resources.getString(R.string.nhs111_connection_error),
            nativeHeader = context.resources.getString(R.string.nhs_111_header)))
    }

    fun findMatchingKnownService(urlString: String): KnownService? {
        val url = try{ URL(urlString.toLowerCase())} catch (e: java.net.MalformedURLException){return null}
        serviceList.forEach { knownService ->
            knownService.urlList.forEach { knownUrl ->
                if (knownUrl.host == url.host) {
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

}