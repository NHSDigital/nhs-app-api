package com.nhs.online.nhsonline.resources

import android.app.Activity
import android.content.Context
import android.content.pm.PackageManager
import android.content.pm.ResolveInfo
import android.content.res.Resources
import android.net.ConnectivityManager
import android.net.Network
import android.net.NetworkCapabilities
import android.net.NetworkInfo
import com.nhaarman.mockito_kotlin.any
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.eq
import com.nhaarman.mockito_kotlin.mock
import com.nhs.online.nhsonline.R
import org.mockito.ArgumentMatchers.anyInt
import org.mockito.Mockito

open class ResourceMockingClass {


    fun mockContext(): Context {
        val resourceMock: Resources = mock {
            on { getString(R.string.baseURL) } doReturn "http://10.0.2.2:3000"
            on { getString(R.string.dataSharingURL) } doReturn "https://www.nhs.uk/your-nhs-data-matters/"
            on { getString(R.string.connection_error_title) } doReturn "There's an issue with your internet connection"
            on { getString(R.string.connection_error_message) } doReturn "\nCheck your connection and try again." +
                    "\n\nIf the problem continues and you need to book an appointment or get a prescription now, " +
                    "contact your GP surgery directly. For urgent medical advice, call 111."
            on { getString(R.string.server_error_title) } doReturn "We're experiencing technical difficulties"
            on { getString(R.string.Accessible_connection_error_message) } doReturn "Please check your connection and try again.\n" +
                    "        \\n\\nIf the problem persists and you need to book an appointment or get a prescription now,\n" +
                    "        contact your GP surgery directly. For immediate medical advice, call one. one. one.."
            on { getString(R.string.service_unavailable) } doReturn "Service unavailable"
            on { getString(R.string.authRedirectPath) } doReturn "/auth-return"
            on { getString(R.string.dataPreferencesBaseUrl) } doReturn "https://ndopapp-int1.thunderbird.service.nhs.uk/"
            on { getString(R.string.nhs_111_header_description) } doReturn "one one one Online"

            on { getString(R.string.loginPath) } doReturn "login"
            on { getString(R.string.symptomsPath) } doReturn "/symptoms"
            on { getString(R.string.appointmentsPath) } doReturn "/appointments"
            on { getString(R.string.prescriptionsPath) } doReturn "/prescriptions"
            on { getString(R.string.myRecordPath) } doReturn "/my-record-warning"
            on { getString(R.string.myAccountPath) } doReturn "/account"
            on { getString(R.string.morePath) } doReturn "/more"
            on { getString(R.string.checkYourSymptoms) } doReturn "check-your-symptoms"

            on { getString(R.string.my_account_header) } doReturn "My account"
            on { getString(R.string.organ_donation_header) } doReturn "My organ donation decision"
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
            on { getString(R.string.nhs_login_header) } doReturn "Log in to the NHS App"
            on { getString(R.string.nhs_login_accessibility_label) } doReturn "Log in using Patient ID"
            on { getString(R.string.service_unavailable) } doReturn "Service unavailable"
            on { getString(R.string.nhsLoginSuffix) } doReturn "https://ext.signin.nhs.uk"
            on { getString(R.string.admin_help_header) } doReturn "GP help without an appointment"
            on { getString(R.string.browser_unavailable) } doReturn "Browser is disabled"
        }

        return mock {
            on { resources } doReturn resourceMock
            on { getString(R.string.dataPreferencesBaseUrl) } doReturn "https://ndopapp-int1.thunderbird.service.nhs.uk/"
        }
    }

    // Returns a context which appears to not have a internet connection
    fun mockDisconnectedContext(): Context {
        val connectivityManager = Mockito.mock(ConnectivityManager::class.java)
        val networkInfo = Mockito.mock(NetworkInfo::class.java)
        val network = Mockito.mock(Network::class.java)
        val capabilities = Mockito.mock(NetworkCapabilities::class.java)

        //api < 23
        Mockito.`when`(connectivityManager.activeNetworkInfo).thenReturn(networkInfo)
        Mockito.`when`(networkInfo.isConnected).thenReturn(false)

        //api >= 23
        Mockito.`when`(connectivityManager.activeNetwork).thenReturn(network)
        Mockito.`when`(connectivityManager.getNetworkCapabilities(network)).thenReturn(capabilities)
        Mockito.`when`(capabilities.hasTransport(NetworkCapabilities.TRANSPORT_CELLULAR)).thenReturn(false)
        Mockito.`when`(capabilities.hasTransport(NetworkCapabilities.TRANSPORT_WIFI)).thenReturn(false)
        Mockito.`when`(capabilities.hasTransport(NetworkCapabilities.TRANSPORT_VPN)).thenReturn(false)

        val mockResource: Resources = mock {
            on { getString(R.string.connection_error_header) } doReturn "Internet connection error"
            on { getString(R.string.service_unavailable) } doReturn "Service unavailable"
            on { getString(R.string.connection_error_title) } doReturn "There\\'s an issue with your internet connection"
            on { getString(R.string.server_error_title) } doReturn "We're experiencing technical difficulties"
        }

        return mock {
            on { getSystemService(Context.CONNECTIVITY_SERVICE) } doReturn connectivityManager
            on { resources } doReturn mockResource
        }
    }

    // Returns a context which appears as if it does have internet connection
    fun mockConnectedContext(): Context {
        val connectivityManager = Mockito.mock(ConnectivityManager::class.java)
        val networkInfo = Mockito.mock(NetworkInfo::class.java)

        Mockito.`when`(connectivityManager.activeNetworkInfo).thenReturn(networkInfo)
        Mockito.`when`(networkInfo.isConnected).thenReturn(true)

        val resourceMock: Resources = mock {
            on { getString(R.string.service_unavailable) } doReturn "Service unavailable"
            on { getString(R.string.apiUnavailableErrorMessage) } doReturn "You currently cannot " +
                    "use this service. \\n\\nIf the problem continues and you need to book an\n " +
                    "appointment or get a prescription now, contact your GP surgery directly. For " +
                    "urgent medical advice, call 111."
            on { getString(R.string.accessible_apiUnavailableErrorMessage) } doReturn "You currently " +
                    "cannot use this service. If the problem continues and you need to book an " +
                    "appointment or get a prescription now, contact your GP surgery directly. For " +
                    "urgent medical advice, call one one one."
            on { getString(R.string.connection_error_header) } doReturn "Internet connection error"
            on { getString(R.string.connection_error_title) } doReturn "\"There's an issue with your internet connection\""
            on { getString(R.string.server_error_title) } doReturn "We're experiencing technical difficulties"
        }

        return mock {
            on { getSystemService(Context.CONNECTIVITY_SERVICE) } doReturn connectivityManager
            on { resources } doReturn resourceMock
        }

    }

    fun mockNullConnectionContext(): Context {
        val connectivityManager = Mockito.mock(ConnectivityManager::class.java)

        //api < 23
        Mockito.`when`(connectivityManager.activeNetworkInfo).thenReturn(null)

        //api >= 23
        Mockito.`when`(connectivityManager.activeNetwork).thenReturn(null)

        return mock {
            on { getSystemService(Context.CONNECTIVITY_SERVICE) } doReturn connectivityManager
        }
    }

    // Returns an activity which does returns permission granted when fine location permission
    // are checked
    fun mockGeolocationPermissionsAllow(): Activity {
        return mock {
            on {
                checkPermission(eq("android.permission.ACCESS_FINE_LOCATION"),
                        anyInt(),
                        anyInt())
            } doReturn PackageManager.PERMISSION_GRANTED
        }
    }

    fun mockGeolocationPermissionsDenyPermissionPopup(): Activity {

        return mock {
            on {
                checkPermission(eq("android.permission.ACCESS_FINE_LOCATION"),
                        anyInt(), anyInt())
            } doReturn PackageManager.PERMISSION_DENIED
            on {
                shouldShowRequestPermissionRationale("android.permission.ACCESS_FINE_LOCATION")
            } doReturn true
        }
    }

    fun mockGeolocationPermissionsDenyRational(): Activity {

        return mock {
            on {
                checkPermission(eq("android.permission.ACCESS_FINE_LOCATION"),
                        anyInt(), anyInt())
            } doReturn PackageManager.PERMISSION_DENIED
            on {
                shouldShowRequestPermissionRationale("android.permission.ACCESS_FINE_LOCATION")
            } doReturn false
        }
    }

    fun mockFileUpload(): Activity {
        val resolveInfo: ResolveInfo? = null
        val packageManagerMock: PackageManager =
                mock { on { resolveActivity(any(), any()) } doReturn resolveInfo }

        return mock { on { packageManager } doReturn packageManagerMock }
    }
}