package com.nhs.online.nhsonline.browseractivities

import android.content.Context
import android.content.res.Resources
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.services.KnownServices
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class OpenUrlInBrowserActivityTest {
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
            on { getString(R.string.dataPreferencesRedirect) } doReturn "https://ndopapp-int1.thunderbird.service.nhs.uk/createsession"
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
    fun canStart_returnsFalse_forSupportedHosts() {
        val openUrlInBrowserActivity = OpenUrlInBrowserActivity(arrayOf("https://111.nhs.uk/"))
        val context: Context = mockContext()

        val urls = listOf("https://111.nhs.uk/", "https://111.nhs.uk/Help/Terms")

        urls.forEach{url ->
            val result = openUrlInBrowserActivity.canStart(context, url)
            Assert.assertFalse(result)
        }
    }

    @Test
    fun canStart_returnsTrue_forNotSupportedHosts() {
        val openUrlInBrowserActivity = OpenUrlInBrowserActivity(arrayOf("https://111.nhs.uk/"))
        val context: Context = mockContext()

        val urls = listOf("https://www.google.co.uk/", "https://www.nhs.uk")

        urls.forEach{url ->
             val result = openUrlInBrowserActivity.canStart(context, url)
            Assert.assertTrue(result)
        }
    }

    @Test
    fun start_throwsException_supportedHosts() {
        val openUrlInBrowserActivity = OpenUrlInBrowserActivity(arrayOf("https://111.nhs.uk/"))
        val context: Context = mockContext()

        val urls = listOf("https://111.nhs.uk/", "https://111.nhs.uk/Help/Terms")

        var message = ""
        try {
            urls.forEach{url ->
                openUrlInBrowserActivity.start(context, url)
            }
        } catch (exception: RuntimeException) {
            message =  exception?.message ?:  ""
        }

        Assert.assertEquals("Cannot open url in browser", message)
    }
}