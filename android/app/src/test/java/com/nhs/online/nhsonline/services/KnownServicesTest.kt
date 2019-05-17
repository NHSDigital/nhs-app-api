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
    fun findKnownServiceAndAddMissingQuery_returnsValidUrl_forValidServiceNoQueryString() {
        val result = testKnownServices.findKnownServiceAndAddMissingQueryFor("https://111.nhs.uk")

        Assert.assertEquals("https://111.nhs.uk", result)
    }

    @Test
    fun findKnownServiceAndAddMissingQuery_returnsValidUrl_forValidServiceNoQueryStringDifferentCasing() {
        val result = testKnownServices.findKnownServiceAndAddMissingQueryFor("https://111.NHS.UK")

        Assert.assertEquals("https://111.NHS.UK", result)
    }

    @Test
    fun findKnownServiceAndAddMissingQuery_returnsValidUrl_forValidServiceWithQueryString() {
        val result = testKnownServices.findKnownServiceAndAddMissingQueryFor("http://10.0.2.2:3000")

        Assert.assertEquals("http://10.0.2.2:3000?source=android", result)
    }

    @Test
    fun findKnownServiceAndAddMissingQuery_returnsValidUrl_forValidServiceNoQueryStringWithInputQueryString() {
        val result =
            testKnownServices.findKnownServiceAndAddMissingQueryFor("https://111.nhs.uk?param1=param1Value")

        Assert.assertEquals("https://111.nhs.uk?param1=param1Value", result)
    }

    @Test
    fun findKnownServiceAndAddMissingQuery_returnsValidUrl_forValidServiceWithQueryStringWithInputQueryString() {
        val result =
            testKnownServices.findKnownServiceAndAddMissingQueryFor("http://10.0.2.2:3000?source=android")

        Assert.assertEquals("http://10.0.2.2:3000?source=android", result)
    }

    @Test
    fun findKnownServiceAndAddMissingQuery_returnsValidUrl_forValidServiceWithQueryStringWithAdditionalInputQueryString() {
        val result =
            testKnownServices.findKnownServiceAndAddMissingQueryFor("http://10.0.2.2:3000?param1=param1Value")

        Assert.assertEquals("http://10.0.2.2:3000?param1=param1Value&source=android", result)
    }

    @Test
    fun findKnownServiceAndAddMissingQuery_returnsOriginalUrl_forInvalidServiceWithInputQueryString() {
        val result =
            testKnownServices.findKnownServiceAndAddMissingQueryFor("https://www.google.co.uk?param1=param1Value")

        Assert.assertEquals("https://www.google.co.uk?param1=param1Value", result)
    }

    @Test
    fun findKnownServiceAndAddMissingQuery_returnsOriginalUrl_forInvalidServiceNoQueryString() {
        val result =
            testKnownServices.findKnownServiceAndAddMissingQueryFor("https://www.google.co.uk")

        Assert.assertEquals("https://www.google.co.uk", result)
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
                getResourceString(R.string.prescriptionsPath),
                getResourceString(R.string.myRecordPath),
                getResourceString(R.string.myAccountPath),
                getResourceString(R.string.morePath) + "random/path",
                "${getResourceString(R.string.checkYourSymptoms)}/random-path")

        val equivalentNhsAppHeaders = arrayListOf(getResourceString(R.string.symptoms_header),
            getResourceString(R.string.appointments_header),
            getResourceString(R.string.prescriptions_header),
            getResourceString(R.string.my_record_header),
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
    fun isLoginUrlWithSourceQuery_ReturnsFalseWhenLoginUrlHasAdditionalAQuery() {
        val loginUrlWithExtraQuery = "http://10.0.2.2:3000/login?source=android&responseCode=102"
        val result = testKnownServices.isLoginUrlWithSourceQuery(loginUrlWithExtraQuery)
        Assert.assertFalse("Additional query responseCode=102 shouldn't in known Service", result)
    }

    @Test
    fun isLoginUrlWithSourceQuery_ReturnsFalseWhenLoginUrlHasNoQuery() {
        val loginUrl = "http://10.0.2.2:3000/login"
        val result = testKnownServices.isLoginUrlWithSourceQuery(loginUrl)
        Assert.assertFalse(result)
    }

    @Test
    fun isLoginUrlWithSourceQuery_ReturnsTrueWhenLoginUrlHasOnlySourceQuery() {
        val loginUrlWithSourceQuery = "http://10.0.2.2:3000/login?source=android"
        val result = testKnownServices.isLoginUrlWithSourceQuery(loginUrlWithSourceQuery)
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
            testKnownServices.shouldURLOpenExternally(URL("https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/"))

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

        Assert.assertEquals(result, getResourceString(R.string.dataSharingPath))
    }

    private fun getResourceString(resourceId: Int): String {
        return context.resources.getString(resourceId)
    }
}