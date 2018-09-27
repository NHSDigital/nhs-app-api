package com.nhs.online.nhsonline.services

import android.content.Context
import android.content.res.Resources
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import org.junit.Assert
import org.junit.Test
import com.nhs.online.nhsonline.R
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import java.net.URL

@RunWith(RobolectricTestRunner::class)
class KnownServicesTest {

    private fun mockContext(): Context {
        var mockresource: Resources = mock {
            on { getString(R.string.baseURL) } doReturn "http://10.0.2.2:3000"
            on { getString(R.string.nhs111) } doReturn "https://111.nhs.uk"
            on { getString(R.string.nhs111Location) } doReturn "https://111.service.nhs.uk/"
            on { getString(R.string.organDonation) } doReturn "https://www.organdonation.nhs.uk/"
            on { getString(R.string.dataSharing) } doReturn "https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing"
            on { getString(R.string.connection_error_title) } doReturn "There's an issue with your internet connection"
            on { getString(R.string.connection_error_message) } doReturn "\nCheck your connection and try again." +
                    "\n\nIf the problem continues and you need to book an appointment or get a prescription now, " +
                    "contact your GP surgery directly. For urgent medical advice, call 111."
            on { getString(R.string.service_unavailable) } doReturn "Service unavailable"
            on { getString(R.string.nhsOnlineRequiredQueries) } doReturn "?source=android"
            on { getString(R.string.conditions) } doReturn "https://www.nhs.uk/conditions/"
            on { getString(R.string.appIntroPath) } doReturn "file:///android_asset/appintro.html"
            on { getString(R.string.hotjarLink) } doReturn "https://in.hotjar.com/s?siteId=859152&amp;surveyId=95785"
            on { getString(R.string.dataPreferencesBaseUrl) } doReturn "https://ndopapp-int1.thunderbird.service.nhs.uk/"
            on { getStringArray(R.array.externalSiteUrls)} doReturn arrayOf(
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source-licences/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/medical-record-abbreviations/"
            )
            on { getString(R.string.symptomsPath) } doReturn "/symptoms"
            on { getString(R.string.appointmentsPath) } doReturn "/appointments"
            on { getString(R.string.prescriptionsPath) } doReturn "/prescriptions"
            on { getString(R.string.myRecordPath) } doReturn "/my-record-warning"
            on { getString(R.string.myAccountPath) } doReturn "/account"
            on { getString(R.string.morePath) } doReturn "/more"
        }

        return mock { on { resources } doReturn mockresource }
    }

    @Test
    fun findMatchingKnownService_returnsNull_forNotAUrl() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findMatchingKnownService("Not A Url")

        Assert.assertNull(result)
    }

    @Test
    fun findMatchingKnownService_returnsService_forValidServiceUrl() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findMatchingKnownService("https://111.nhs.uk")

        Assert.assertEquals(URL("https://111.nhs.uk"), result?.urlList?.get(0))
    }

    @Test
    fun findMatchingKnownService_returnsService_forValidServiceUrlDifferentCasing() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findMatchingKnownService("https://111.NHS.UK")

        Assert.assertEquals(URL("https://111.nhs.uk"), result?.urlList?.get(0))
    }

    @Test
    fun findMatchingKnownService_returnsService_forValidServiceUrlWithQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result =
            testKnownServices.findMatchingKnownService("http://10.0.2.2:3000?source=android")

        Assert.assertEquals(URL("http://10.0.2.2:3000"), result?.urlList?.get(0))
    }

    @Test
    fun findMatchingInternalService_returnsNull_forNotAUrl() {
        var context: Context = mockContext()
        val testInternalServices = KnownServices(context)

        val result = testInternalServices.findMatchingInternalService("Not A Url")

        Assert.assertNull(result)
    }

    @Test
    fun findMatchingInternalService_returnsService_forValidServiceUrl() {
        var context: Context = mockContext()
        val testInternalServices = KnownServices(context)

        val result = testInternalServices.findMatchingInternalService("http://10.0.2.2:3000/appointments")

        Assert.assertEquals(URL("http://10.0.2.2:3000/appointments"), result?.urlList?.get(0))
    }

    @Test
    fun findMatchingInternalService_returnsService_forValidServiceUrlDifferentCasing() {
        var context: Context = mockContext()
        val testInternalServices = KnownServices(context)

        val result = testInternalServices.findMatchingInternalService("http://10.0.2.2:3000/APPOINTMENTS")

        Assert.assertEquals(URL("http://10.0.2.2:3000/appointments"), result?.urlList?.get(0))
    }

    @Test
    fun findMatchingInternalService_returnsService_forValidServiceUrlWithQueryString() {
        var context: Context = mockContext()
        val testInternalServices = KnownServices(context)

        val result =
                testInternalServices.findMatchingInternalService("http://10.0.2.2:3000/appointments?source=android")

        Assert.assertEquals(URL("http://10.0.2.2:3000/appointments"), result?.urlList?.get(0))
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceNoQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("https://111.nhs.uk")

        Assert.assertEquals("https://111.nhs.uk", result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceNoQueryStringDifferentCasing() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("https://111.NHS.UK")

        Assert.assertEquals("https://111.NHS.UK", result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceWithQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("http://10.0.2.2:3000")

        Assert.assertEquals("http://10.0.2.2:3000?source=android", result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceNoQueryStringWithInputQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result =
            testKnownServices.findKnownServiceAddMissingQueryFor("https://111.nhs.uk?param1=param1Value")

        Assert.assertEquals("https://111.nhs.uk?param1=param1Value", result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceWithQueryStringWithInputQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result =
            testKnownServices.findKnownServiceAddMissingQueryFor("http://10.0.2.2:3000?source=android")

        Assert.assertEquals("http://10.0.2.2:3000?source=android", result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceWithQueryStringWithAdditionalInputQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result =
            testKnownServices.findKnownServiceAddMissingQueryFor("http://10.0.2.2:3000?param1=param1Value")

        Assert.assertEquals("http://10.0.2.2:3000?param1=param1Value&source=android", result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsOriginalUrl_forInvalidServiceWithInputQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result =
            testKnownServices.findKnownServiceAddMissingQueryFor("https://www.google.co.uk?param1=param1Value")

        Assert.assertEquals("https://www.google.co.uk?param1=param1Value", result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsOriginalUrl_forInvalidServiceNoQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result =
            testKnownServices.findKnownServiceAddMissingQueryFor("https://www.google.co.uk")

        Assert.assertEquals("https://www.google.co.uk", result)
    }

    @Test
    fun isHotJarService_foundService() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.isHotJar(URL("https://in.hotjar.com/s?siteId=859152&amp;surveyId=95785"))

        Assert.assertEquals(true, result)
    }

    @Test
    fun isHotJarService_notFoundService() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result =
                testKnownServices.isHotJar(URL("https://www.google.co.uk"))

        Assert.assertEquals(false, result)
    }

    @Test
    fun isExternalBrowserService_foundService() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result =
                testKnownServices.shouldURLOpenExternally(URL("https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/"))

        Assert.assertEquals(true, result)
    }

    @Test
    fun isExternalBrowserService_notFoundService() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result =
                testKnownServices.shouldURLOpenExternally(URL("https://www.google.co.uk"))

        Assert.assertEquals(false, result)
    }
}