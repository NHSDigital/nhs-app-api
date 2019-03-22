package com.nhs.online.nhsonline.resources

import android.app.Activity
import android.content.Context
import android.content.res.Resources
import android.net.ConnectivityManager
import android.net.NetworkInfo
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhs.online.nhsonline.R
import org.mockito.Mockito

import android.content.pm.PackageManager
import com.nhaarman.mockito_kotlin.eq
import com.nhs.online.nhsonline.activities.SymptomsActivity
import org.mockito.ArgumentMatchers.anyInt

open class ResourceMockingClass {


    fun mockContext(): Context {
        val mockresource: Resources = mock {
            on { getString(R.string.baseURL) } doReturn "http://10.0.2.2:3000"
            on { getString(R.string.nhsUK) } doReturn "https://www.nhs.uk"
            on { getString(R.string.nhs111) } doReturn "https://111.nhs.uk"
            on { getString(R.string.nhs111Location) } doReturn "https://111.service.nhs.uk/"
            on { getString(R.string.organDonation) } doReturn "https://www.organdonation.nhs.uk/"
            on { getString(R.string.organDonationNative) } doReturn "https://www.organdonation.nhs.uk/app/"
            on { getString(R.string.dataSharing) } doReturn "https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing"
            on { getString(R.string.connection_error_title) } doReturn "There's an issue with your internet connection"
            on { getString(R.string.connection_error_message) } doReturn "\nCheck your connection and try again." +
                    "\n\nIf the problem continues and you need to book an appointment or get a prescription now, " +
                    "contact your GP surgery directly. For urgent medical advice, call 111."
            on { getString(R.string.Accessible_connection_error_message) } doReturn "Please check your connection and try again.\n" +
                    "        \\n\\nIf the problem persists and you need to book an appointment or get a prescription now,\n" +
                    "        contact your GP surgery directly. For immediate medical advice, call one. one. one.."
            on { getString(R.string.service_unavailable) } doReturn "Service unavailable"
            on { getString(R.string.nhsOnlineRequiredQueries) } doReturn "?source=android"
            on { getString(R.string.conditions) } doReturn "https://www.nhs.uk/conditions/"
            on { getString(R.string.appIntroPath) } doReturn "file:///android_asset/appintro.html"
            on { getString(R.string.hotjarLink) } doReturn "https://in.hotjar.com/s?siteId=859152&amp;surveyId=95785"
            on { getString(R.string.dataPreferencesBaseUrl) } doReturn "https://ndopapp-int1.thunderbird.service.nhs.uk/"
            on { getStringArray(R.array.externalSiteUrls) } doReturn arrayOf(
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source-licences/",
                    "https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/medical-record-abbreviations/"
            )
            on { getString(R.string.nhs_111_header_description) } doReturn  "one one one Online"

            on { getString(R.string.loginPath) } doReturn "login"
            on { getString(R.string.symptomsPath) } doReturn "/symptoms"
            on { getString(R.string.appointmentsPath) } doReturn "/appointments"
            on { getString(R.string.prescriptionsPath) } doReturn "/prescriptions"
            on { getString(R.string.myRecordPath) } doReturn "/my-record-warning"
            on { getString(R.string.myAccountPath) } doReturn "/account"
            on { getString(R.string.morePath) } doReturn "/more"
            on { getString(R.string.checkYourSymptoms) } doReturn "check-your-symptoms"

            on { getString(R.string.my_account_header) } doReturn "My account"
            on { getString(R.string.organ_donation_register_header) } doReturn "Organ donation register"
            on { getString(R.string.data_sharing_header) } doReturn "Sharing health data preferences"
            on { getString(R.string.data_preferences_header) } doReturn "Sharing health data preferences"
            on { getString(R.string.nhs_111_header) } doReturn "111 Online"
            on { getString(R.string.conditions_header) } doReturn "Health A-Z"
            on { getString(R.string.conditions_header_description) } doReturn "Health A to Z"
            on { getString(R.string.my_record_header) } doReturn "My medical record"
            on { getString(R.string.symptoms_header) } doReturn "Check my symptoms"
            on { getString(R.string.appointments_header) } doReturn "My appointments"
            on { getString(R.string.prescriptions_header) } doReturn "My repeat prescriptions"
            on { getString(R.string.more_header) } doReturn "More"
            on { getString(R.string.home_header) } doReturn "Home"

            on { getStringArray(R.array.nativeAppHosts) } doReturn arrayOf(
                    "https://111.nhs.uk/",
                    "https://111.service.nhs.uk/",
                    "https://www.organdonation.nhs.uk/app/",
                    "https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing/",
                    "https://www.nhs.uk/conditions/",
                    "https://www-preview.dev.nonlive.nhsapp.service.nhs.uk/"
            )
        }

        return mock {
            on { resources } doReturn mockresource
            on { getString(R.string.dataPreferencesBaseUrl) } doReturn "https://ndopapp-int1.thunderbird.service.nhs.uk/"
        }
    }

    // Returns a context which appears to not have a internet connection
    fun mockDisconnectedContext() : Context {
        val connectivityManager = Mockito.mock(ConnectivityManager::class.java)
        val networkInfo = Mockito.mock( NetworkInfo::class.java )

        Mockito.`when`( connectivityManager.activeNetworkInfo ).thenReturn(networkInfo)
        Mockito.`when`( networkInfo.isConnected).thenReturn( false)
        Mockito.`when`(networkInfo.isAvailable).thenReturn(false)
        Mockito.`when`(networkInfo.isConnectedOrConnecting).thenReturn(false)

        val mockResource: Resources = mock {
            on { getString(R.string.connection_error_header) } doReturn "Internet connection error"
            on { getString(R.string.nhsUK) } doReturn "https://www.nhs.uk"
            on { getString(R.string.nhs111) } doReturn "https://111.nhs.uk"
            on { getString(R.string.connection_error_title) } doReturn "There\\'s an issue with your internet connection"
        }

        return mock {
            on {getSystemService(Context.CONNECTIVITY_SERVICE) } doReturn connectivityManager
            on { resources } doReturn mockResource
        }
    }

    // Returns a context which appears as if it does have internet connection
    fun mockConnectedContext() : Context {
        val connectivityManager = Mockito.mock(ConnectivityManager::class.java)
        val networkInfo = Mockito.mock( NetworkInfo::class.java )

        Mockito.`when`( connectivityManager.activeNetworkInfo ).thenReturn(networkInfo)
        Mockito.`when`( networkInfo.isConnected).thenReturn( true )
        Mockito.`when`(networkInfo.isAvailable).thenReturn(true)
        Mockito.`when`(networkInfo.isConnectedOrConnecting).thenReturn(true)

        val mockresource: Resources = mock {
            on { getString(R.string.connection_error_header) } doReturn "Internet connection error"
            on { getString(R.string.connection_error_title) } doReturn "\"There's an issue with your internet connection\""
            on { getString(R.string.nhsUK) } doReturn "https://www.nhs.uk"
            on { getString(R.string.nhs111) } doReturn "https://111.nhs.uk"
        }

        return mock {
            on {getSystemService(Context.CONNECTIVITY_SERVICE) } doReturn connectivityManager
            on { resources } doReturn mockresource
        }

    }

    // Returns an activity which does returns permission granted when fine location permission
    // are checked
    fun mockGeolocationPermissionsAllow() : Activity {
        return mock {
            on {checkPermission(eq("android.permission.ACCESS_FINE_LOCATION"), anyInt(), anyInt())} doReturn PackageManager.PERMISSION_GRANTED
        }
    }

    fun mockGeolocationPermissionsDenyPermissionPopup() : Activity {

        return mock {
            on {checkPermission(eq("android.permission.ACCESS_FINE_LOCATION"),
                    anyInt(), anyInt())} doReturn PackageManager.PERMISSION_DENIED
            on {
                shouldShowRequestPermissionRationale("android.permission.ACCESS_FINE_LOCATION")
            } doReturn true
        }
    }

    fun mockGeolocationPermissionsDenyRational() : Activity {

        return mock {
            on {checkPermission(eq("android.permission.ACCESS_FINE_LOCATION"),
                    anyInt(), anyInt())} doReturn PackageManager.PERMISSION_DENIED
            on {
                shouldShowRequestPermissionRationale("android.permission.ACCESS_FINE_LOCATION")
            } doReturn false
        }
    }
}