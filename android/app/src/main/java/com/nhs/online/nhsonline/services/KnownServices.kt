package com.nhs.online.nhsonline.services

import android.content.Context
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.support.Optional
import java.net.MalformedURLException
import java.net.URL


class KnownServices(private val context: Context) {
    private val serviceList = buildKnownServices()
    private val externalSites = buildExternalSites()


    fun shouldURLOpenExternally(url: URL): Boolean {
        return externalSites.contains(url)
    }

    fun isHotJar(url: URL): Boolean {
        val hotJarURL = URL(context.resources.getString(R.string.hotjarLink))
        return url == hotJarURL
    }

    fun findMatchingServiceInfo(
        urlString: String,
        withExactMatchingPath: Boolean = false
    ): KnownService.Info? {
        val matchingService = findMatchingKnownService(urlString) ?: return null
        return matchingService.findMatchingServicePathInfo(urlString, withExactMatchingPath)
    }

    fun findMatchingKnownService(urlString: String): KnownService? {
        val url = try {
            URL(urlString.toLowerCase())
        } catch (e: MalformedURLException) {
            return null
        }
        serviceList.forEach { knownService ->
            if (knownService.getUrl().host.equals(url.host, true)) {
                return knownService
            }
        }
        return null
    }

    fun findKnownServiceAndAddMissingQueryFor(urlString: String): String {
        val matchingKnownService = findMatchingKnownService(urlString) ?: return urlString
        return matchingKnownService.addMissingQueryStrings(urlString)
    }

    fun findNHSAppInternalServiceInfoByPath(path: String): KnownService.Info? {
        val nhsService = findMatchingKnownService(fetchStringResource(R.string.baseURL))
        return nhsService?.findMatchingServicePathInfoByPath(path)
    }

    fun isLoginUrlWithSourceQuery(urlString: String?): Boolean {
        val nonNullUrlString = urlString ?: ""
        val knownService = findMatchingKnownService(nonNullUrlString) ?: return false
        val url = URL(urlString)
        val nhsUrl = URL(fetchStringResource(R.string.baseURL))

        return nhsUrl.host == url.host &&
                knownService.hasOnlyRequiredQueries(nonNullUrlString) &&
                url.path.startsWith("/${fetchStringResource(R.string.loginPath)}")
    }

    fun isCIDRedirectUrl(urlString: String): Boolean = try {
        val nhsUrl = URL(fetchStringResource(R.string.baseURL))
        val url = URL(urlString)
        nhsUrl.host == url.host && url.path == fetchStringResource(R.string.authRedirectPath)
    } catch (e: MalformedURLException) {
        false
    }

    private fun buildKnownServices(): ArrayList<KnownService> {
        val services = arrayListOf<KnownService>()
        val nhsAppService = buildNHSInternalAppService()

        val nhsUK = KnownService(fetchStringResource(R.string.nhsUK))
        nhsUK.addPathInfo(URL(fetchStringResource(R.string.dataSharing)).path,
            false,
            fetchStringResource(R.string.data_preferences_header))

        nhsUK.addPathInfo(URL(fetchStringResource(R.string.conditions)).path,
            false,
            fetchStringResource(R.string.conditions_header))

        val nhs111 = KnownService(fetchStringResource(
            R.string.nhs111),
            fetchStringResource(R.string.nhs_111_header),
            fetchStringResource(R.string.nhs_111_header_description),
            false)
        val dataPref = KnownService(fetchStringResource(R.string.dataPreferencesBaseUrl),
            fetchStringResource(R.string.data_preferences_header),
            fetchStringResource(R.string.data_preferences_header),
            false)

        services.add(nhsAppService)
        services.add(nhsUK)
        services.add(nhs111)
        services.add(dataPref)

        return services
    }

    private fun buildNHSInternalAppService(): KnownService {
        val internalService = KnownService(
                fetchStringResource(R.string.baseURL),
                fetchStringResource(R.string.home_header),
                queryStrings = fetchStringResource(R.string.nhsOnlineRequiredQueries))
        internalService.addPathInfo(fetchStringResource(R.string.symptomsPath),
            true,
            fetchStringResource(R.string.symptoms_header))
        internalService.addPathInfo(fetchStringResource(R.string.checkYourSymptoms),
            false,
            fetchStringResource(R.string.symptoms_header))
        internalService.addPathInfo(fetchStringResource(R.string.appointmentsPath),
            true,
            fetchStringResource(R.string.appointments_header))
        internalService.addPathInfo(fetchStringResource(R.string.prescriptionsPath),
            true,
            fetchStringResource(R.string.prescriptions_header))
        internalService.addPathInfo(fetchStringResource(R.string.myRecordPath),
            true,
            fetchStringResource(R.string.my_record_header))
        internalService.addPathInfo(fetchStringResource(R.string.morePath),
            true,
            fetchStringResource(R.string.more_header))
        internalService.addPathInfo(fetchStringResource(R.string.myAccountPath),
            true,
            fetchStringResource(R.string.my_account_header))
        internalService.addPathInfo(fetchStringResource(R.string.organDonationPath),
            true,
            fetchStringResource(R.string.organ_donation_header))
        internalService.addPathInfo(fetchStringResource(R.string.dataSharingPath),
            true,
            fetchStringResource(R.string.data_sharing_header))
        internalService.addPathInfo(fetchStringResource(R.string.adminHelpPath),
            true,
            fetchStringResource(R.string.admin_help_header))
        return internalService
    }

    fun isUrlHostSameAsHomeUrlHost(urlString: String?): Boolean {
        if (urlString.isNullOrBlank())
            return false
        val homeUrl = URL(context.resources.getString(R.string.baseURL))
        val url = URL(urlString)
        return homeUrl.host == url.host
    }

    private fun fetchStringResource(resourceId: Int): String {
        return context.resources.getString(resourceId)
    }

    fun getPostRequestReloadUrl(url: String): String? {
        return when {
            url.startsWith((fetchStringResource(R.string.dataPreferencesBaseUrl))) -> fetchStringResource(R.string.dataSharingPath)
            else -> null
        }
    }

    private fun buildExternalSites(): List<URL> {
        val eSites = arrayListOf<URL>()
        val sitesFromResource: Array<String>? =
            context.resources.getStringArray(R.array.externalSiteUrls)

        sitesFromResource?.forEach { site -> eSites.add(URL(site)) }
        return eSites
    }

}