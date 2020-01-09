package com.nhs.online.nhsonline.services

import android.content.Context
import org.junit.Assert
import org.junit.Test
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import java.net.URL

@RunWith(RobolectricTestRunner::class)
class KnownServicesTest : ResourceMockingClass() {
    private val context: Context = mockContext()
    private val testKnownServices = KnownServices(context)

    @Test
    fun findMatchingKnownService_returnsNull_forNotAUrl() {
        val result = testKnownServices.findMatchingKnownService("Not A Url")

        Assert.assertNull(result)
    }

    @Test
    fun findMatchingKnownService_returnsService_forValidServiceUrl() {
        val result = testKnownServices.findMatchingKnownService("https://111.nhs.uk")

        Assert.assertEquals(URL("https://111.nhs.uk"), result?.getUrl())
    }

    @Test
    fun findMatchingKnownService_returnsService_forValidServiceUrlDifferentCasing() {
        val result = testKnownServices.findMatchingKnownService("https://111.NHS.UK")

        Assert.assertEquals(URL("https://111.nhs.uk"), result?.getUrl())
    }

    @Test
    fun findMatchingKnownService_returnsService_forValidServiceUrlWithQueryString() {
        val result =
            testKnownServices.findMatchingKnownService("http://10.0.2.2:3000?source=android")

        Assert.assertEquals(URL("http://10.0.2.2:3000"), result?.getUrl())
    }

    @Test
    fun findMatchingServiceInfo_resolveToNull_whenUrlIsUnknownServiceUrl() {
        val unknownServices = arrayListOf("https://www.google.co.uk",
            "https://www.yahoo.co.uk", "https://www.jobs.nhs.uk")
        unknownServices.forEach { unknownService ->
            val serviceInfo = testKnownServices.findMatchingServiceInfo(unknownService)
            Assert.assertNull(serviceInfo)
        }
    }

    @Test
    fun findMatchingServiceInfo_resolveToDefault_whenServiceInternalPathLocatorIsUnrelated() {
        val unrelatedLocators =
            arrayListOf("${getResourceString(R.string.baseURL)}/unrelatedPath")
        unrelatedLocators.forEach { locator ->
            val serviceInfo = testKnownServices.findMatchingServiceInfo(locator)
            Assert.assertEquals("Home", serviceInfo?.header)
        }
    }

    @Test
    fun findNHSAppInternalServiceInfoByPath_resolvesToMatchingNHSAppInternalPathOrClosestToThePath() {
        val nhsAppServices =
            arrayListOf(getResourceString(R.string.symptomsPath),
                getResourceString(R.string.appointmentsPath),
                getResourceString(R.string.appointmentsGpAtHandPath),
                getResourceString(R.string.informaticaPath),
                getResourceString(R.string.prescriptionsPath),
                getResourceString(R.string.prescriptionsGpAtHandPath),
                getResourceString(R.string.myRecordPath),
                getResourceString(R.string.myRecordGpAtHandPath),
                getResourceString(R.string.myAccountPath),
                getResourceString(R.string.morePath) + "random/path",
                "${getResourceString(R.string.checkYourSymptoms)}/random-path")

        val equivalentNhsAppHeaders = arrayListOf(getResourceString(R.string.symptoms_header),
                getResourceString(R.string.appointments_header),
                getResourceString(R.string.service_unavailable),
                getResourceString(R.string.service_unavailable),
                getResourceString(R.string.prescriptions_header),
                getResourceString(R.string.service_unavailable),
                getResourceString(R.string.my_record_header),
                getResourceString(R.string.service_unavailable),
                getResourceString(R.string.my_account_header),
                getResourceString(R.string.more_header),
                getResourceString(R.string.symptoms_header))
        for (i in 0 until nhsAppServices.size) {
            val serviceInfo =
                testKnownServices.findNHSAppInternalServiceInfoByPath(nhsAppServices[i])
            Assert.assertEquals(equivalentNhsAppHeaders[i], serviceInfo?.header)
        }
    }

    @Test
    fun isLoginUrl_ReturnsTrueWhenLoginUrlHasAdditionalQueryParameters() {
        val loginUrlWithExtraQuery = "http://10.0.2.2:3000/login?responseCode=102"
        val result = testKnownServices.isLoginUrl(loginUrlWithExtraQuery)
        Assert.assertTrue("Failed to recognise login url with additional parameters", result)
    }

    @Test
    fun isLoginUrl_ReturnsTrueWhenLoginUrlHasNoQuery() {
        val loginUrl = "http://10.0.2.2:3000/login"
        val result = testKnownServices.isLoginUrl(loginUrl)
        Assert.assertTrue(result)
    }
    @Test
    fun findMatchingServiceInfo_resolvesToMatchingInternalPathOrClosestInternalThePath() {
        val nhsAppServices =
            arrayListOf(getResourceString(R.string.baseURL) + getResourceString(R.string.symptomsPath),
                getResourceString(R.string.baseURL) + getResourceString(R.string.appointmentsPath),
                getResourceString(R.string.baseURL) + getResourceString(R.string.prescriptionsPath),
                getResourceString(R.string.baseURL) + getResourceString(R.string.myRecordPath),
                getResourceString(R.string.baseURL) + getResourceString(R.string.myAccountPath),
                getResourceString(R.string.baseURL) + getResourceString(R.string.organDonationPath),
                getResourceString(R.string.baseURL) + getResourceString(R.string.morePath) + "random/path",
                "${getResourceString(R.string.baseURL)}/${getResourceString(R.string.checkYourSymptoms)}/random-path")

        val equivalentNhsAppHeaders = arrayListOf(getResourceString(R.string.symptoms_header),
            getResourceString(R.string.appointments_header),
            getResourceString(R.string.prescriptions_header),
            getResourceString(R.string.my_record_header),
            getResourceString(R.string.my_account_header),
            getResourceString(R.string.organ_donation_header),
            getResourceString(R.string.more_header),
            getResourceString(R.string.symptoms_header))
        for (i in 0 until nhsAppServices.size) {
            val serviceInfo = testKnownServices.findMatchingServiceInfo(nhsAppServices[i])
            Assert.assertEquals(equivalentNhsAppHeaders[i], serviceInfo?.header)
        }
    }

    @Test
    fun isHotJarService_foundService() {
        val result =
            testKnownServices.isHotJar(URL("https://in.hotjar.com/s?siteId=859152&amp;surveyId=95785"))

        Assert.assertEquals(true, result)
    }

    @Test
    fun isHotJarService_notFoundService() {
        val result =
            testKnownServices.isHotJar(URL("https://www.google.co.uk"))

        Assert.assertEquals(false, result)
    }

    @Test
    fun isExternalBrowserService_foundService() {
        val result =
            testKnownServices.shouldURLOpenExternally(URL("https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/"))

        Assert.assertEquals(true, result)
    }

    @Test
    fun isExternalBrowserService_notFoundService() {
        val result =
            testKnownServices.shouldURLOpenExternally(URL("https://www.google.co.uk"))

        Assert.assertEquals(false, result)
    }

    @Test
    fun getPostRequestReloadUrl_postRequestDataPreferencesUrl() {
        val result =
                testKnownServices.getPostRequestReloadUrl("https://ndopapp-int1.thunderbird.service.nhs.uk/")

        Assert.assertEquals(result, getResourceString(R.string.dataSharingURL))
    }

    private fun getResourceString(resourceId: Int): String {
        return context.resources.getString(resourceId)
    }
}