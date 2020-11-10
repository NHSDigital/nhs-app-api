package com.nhs.online.nhsonline.activities

import android.app.onResume
import android.os.Build
import android.text.SpannableString
import androidx.appcompat.app.AlertDialog
import android.widget.TextView
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.BiometricsInteractor
import com.nhs.online.nhsonline.biometrics.BiometricsInterface
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.support.AppDialogs
import com.nhs.online.nhsonline.text.style.ClickableLink
import com.nhs.online.nhsonline.web.NhsWeb
import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.error_layout.retryButton
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Ignore
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.internal.util.reflection.FieldSetter
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.shadows.ShadowDialog
import org.robolectric.util.ReflectionHelpers

@RunWith(RobolectricTestRunner::class)
class MainActivityTest {

    private lateinit var mainActivity: MainActivity
    private lateinit var spyActivity: MainActivity
    private lateinit var connectionStateMonitorMock: ConnectionStateMonitor

    @Before
    fun setUp() {
        mainActivity = Robolectric.buildActivity(MainActivity::class.java).create().get()
        spyActivity = spy(mainActivity)
    }

    @Test
    fun onResume_whenConfigurationResponseCallUnsuccessful_callsNhsWebReloadLoginUrl() {
        spyActivity.configurationResponse.callSuccessful = true

        val nhsWebMock: NhsWeb = mock()

        FieldSetter.setField(spyActivity,
                spyActivity::class.java.getDeclaredField("nhsWeb"),
                nhsWebMock)

        try {
            spyActivity.onResume()
        } catch (e: Exception) {
            assert(false)
        }
        verify(nhsWebMock).reloadLoginUrl()
    }

    @Test
    fun onBackButtonPressed_OnLoginScreen_ClosesApp() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) + getStringById(
            R.string.loginPath))
        spyActivity.configurationResponse.callSuccessful = true

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        verify(spyActivity).finishAndRemoveTask()
    }

    @Test
    fun onBackButtonPressed_IsLoggedIn_ShowAlertDialog() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) +
                "check-your-symptoms" + "/otherPath")
        spyActivity.configurationResponse.callSuccessful = true

        val nhsWebMock: NhsWeb = mock {
            on { isUserLoggedIn }.thenReturn(true)
        }
        FieldSetter.setField(spyActivity,
            spyActivity::class.java.getDeclaredField("nhsWeb"),
            nhsWebMock)

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        val logoutAlertDialog = ShadowDialog.getLatestDialog() as AlertDialog
        assertNotNull(logoutAlertDialog)
        assertTrue(logoutAlertDialog.isShowing)

        val messageTextView = logoutAlertDialog.findViewById<TextView>(android.R.id.message)
        assertNotNull(messageTextView)
        messageTextView?.apply { assertEquals("Are you sure you want to log out?", text) }
    }

    @Test
    fun onBackButtonPressed_OnCheckYourSystemsScreen_CallsOnbackButtonPressedOnCheckSymptomsUnsecurePage() {
        val url = getStringById(R.string.baseURL) +
                getStringById(R.string.checkYourSymptoms)
        spyActivity.webview.loadUrl(url)

        val nhsWebMock: NhsWeb = mock {
            on { isUserLoggedIn }.thenReturn(false)
        }
        FieldSetter.setField(spyActivity,
            spyActivity::class.java.getDeclaredField("nhsWeb"),
            nhsWebMock)

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }
        verify(nhsWebMock).onbackButtonPressedOnLoggedOutUnsecurePage()
    }

    @Test
    fun onBackButtonPressed_WhenUrlIsUnsecureSymptomsPage_CallsOnbackButtonPressedOnCheckSymptomsUnsecurePage() {
        val url = "http://some-nhs-service-when-not-logged-in/check-your-symptoms"
        spyActivity.webview.loadUrl(url)

        val nhsWebMock: NhsWeb = mock {
            on { isUserLoggedIn }.thenReturn(false)
        }
        FieldSetter.setField(spyActivity, spyActivity::class.java.getDeclaredField("nhsWeb"), nhsWebMock)

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }
        verify(nhsWebMock).onbackButtonPressedOnLoggedOutUnsecurePage()
    }

    @Test
    fun onBackButtonPressed_WhenUrlShouldReloadHomepageOnBackReturn_CallsReloadHomepageOnBackReturn() {
        val url = "http://account.ext.signin.nhs.uk"
        spyActivity.webview.loadUrl(url)

        val nhsWebMock: NhsWeb = mock {
            on { isUserLoggedIn }.thenReturn(false)
            on { reloadUrl }.thenReturn(url)
            on { shouldReloadHomepageOnBackReturn(url) }.thenReturn(true)
        }
        FieldSetter.setField(spyActivity,
            spyActivity::class.java.getDeclaredField("nhsWeb"),
            nhsWebMock)

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        verify(nhsWebMock).reloadHomepageOnBackReturn()
    }

    @Test
    fun onBackButtonPressed_MalFormedUrl_ClosesApp() {
        spyActivity.webview.loadUrl("ghghgh:/&/|sdsdsdiuw")
        spyActivity.configurationResponse.callSuccessful = true

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        verify(spyActivity).finishAndRemoveTask()
    }

    @Test
    fun onBackButtonPressed_NullUrl_ClosesApp() {
        spyActivity.webview.loadUrl(null)
        spyActivity.configurationResponse.callSuccessful = true

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        verify(spyActivity).finishAndRemoveTask()
    }

    @Test
    fun showVersionUpgradeDialog_CheckUpdateUrl_NotInitialised_ShowAlertDialog() {
        try {
            val appDialogs = AppDialogs(spyActivity)
            appDialogs.showVersionUpgradeDialog()
        } catch (e: Exception) {
            assert(false)
        }

        val updateAlertDialog = ShadowDialog.getLatestDialog() as AlertDialog
        assertNotNull(updateAlertDialog)
        assertTrue(updateAlertDialog.isShowing)

        val messageTextView = updateAlertDialog.findViewById<TextView>(android.R.id.message)
        assertNotNull(messageTextView)
        messageTextView?.apply { assertTrue(text.contains("Click here to update")) }

        val dialogUrls = messageTextView?.urls
        val updateUrl = dialogUrls?.firstOrNull()?.url

        messageTextView.apply {
            assertEquals("market://details?id=com.nhs.online.nhsonline",
                updateUrl)
        }
    }

    @Test
    fun showBiometricLoginIfEnabled_SuccessfulConfig_StandardLoginUrl_ReturnsTrue() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) + getStringById(
                R.string.loginPath))
        spyActivity.configurationResponse.callSuccessful = true
        spyActivity.configurationResponse.isSupportedVersion = true

        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { showBiometricLoginIfEnabled() }.thenReturn(true)
        }
        FieldSetter.setField(spyActivity,
            spyActivity::class.java.getDeclaredField("biometricsInterface"),
            biometricsInterfaceMock)

        val result = spyActivity.showBiometricLoginIfEnabled()

        assertTrue(result)
        verify(biometricsInterfaceMock).showBiometricLoginIfEnabled()
    }

    @Test
    fun showBiometricLoginIfEnabled_UnsuccessfulConfig_ReturnsFalse() {
        spyActivity.configurationResponse.callSuccessful = false

        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { showBiometricLoginIfEnabled() }.thenReturn(false)
        } //TODO - we keep this in? only utilised for the verification below

        val result = spyActivity.showBiometricLoginIfEnabled()

        assertFalse(result)
        verify(biometricsInterfaceMock, never()).showBiometricLoginIfEnabled()
    }

    @Test
    fun showBiometricLoginIfEnabled_VersionNotSupported_ReturnsFalse() {
        spyActivity.configurationResponse.isSupportedVersion = false

        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { showBiometricLoginIfEnabled() }.thenReturn(false)
        }

        val result = spyActivity.showBiometricLoginIfEnabled()

        assertFalse(result)
        verify(biometricsInterfaceMock, never()).showBiometricLoginIfEnabled()
    }

    @Test
    fun showBiometricLoginIfEnabled_SuccessfulConfig_FidoAuthLoginUrl_ReturnsFalse() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) + getStringById(
                R.string.loginPath) + "?source=android&" + getStringById(R.string.fidoAuthQueryKey))
        spyActivity.configurationResponse.callSuccessful = true

        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { showBiometricLoginIfEnabled() }.thenReturn(true)
        }
        FieldSetter.setField(spyActivity,
                spyActivity::class.java.getDeclaredField("biometricsInterface"),
                biometricsInterfaceMock)

        val result = spyActivity.showBiometricLoginIfEnabled()

        assertFalse(result)
        verify(biometricsInterfaceMock, never()).showBiometricLoginIfEnabled()
    }

    @Test
    fun showBiometricLoginIfEnabled_SuccessfulConfig_NullUrl_ReturnsFalse() {
        spyActivity.webview.loadUrl(null)
        spyActivity.configurationResponse.callSuccessful = true

        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { showBiometricLoginIfEnabled() }.thenReturn(true)
        }
        FieldSetter.setField(spyActivity,
                spyActivity::class.java.getDeclaredField("biometricsInterface"),
                biometricsInterfaceMock)

        val result = spyActivity.showBiometricLoginIfEnabled()

        assertFalse(result)
        verify(biometricsInterfaceMock, never()).showBiometricLoginIfEnabled()
    }


    @Test
    fun testBiometricsRegistrationNotCalledOnCheckChangedListener() {
        //arrange
        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { showBiometricLoginIfEnabled() }.thenReturn(true)
            on { isFingerprintServiceInitialised() }.thenReturn(false)
            on { isFingerprintRegistered }.thenReturn(false)
        }
        val mockAccessToken = "mockAccessToken"

        val nhsWebMock: NhsWeb = mock { }

        ReflectionHelpers.setField(mainActivity, "biometricsInterface", biometricsInterfaceMock)
        ReflectionHelpers.setField(mainActivity, "nhsWeb", nhsWebMock)

        // act
        mainActivity.configBiometricSetup("")

        //assert
        verify(biometricsInterfaceMock, times(1)).initializeFingerprintService("")
        verify(biometricsInterfaceMock, times(0)).requestBiometricsRegistrationStateChange(mockAccessToken)
        verifyZeroInteractions(nhsWebMock)
    }


    @Test
    fun testUpdateBiometricRegistration() {
        //arrange
        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { isFingerprintServiceInitialised() }.thenReturn(true)
            on { isFingerprintRegistered }.thenReturn(true)
            on { doFingerprintsExist() }.thenReturn(true)
        }

        val mockAccessToken = "mockAccessToken"
        val biometricsInteractorMock: BiometricsInteractor = mock()

        ReflectionHelpers.setField(mainActivity, "biometricsInterface", biometricsInterfaceMock)
        ReflectionHelpers.setField(mainActivity, "biometricsInteractor", biometricsInteractorMock)

        // act
        mainActivity.updateBiometricRegistration(mockAccessToken)

        //assert
        verify(biometricsInteractorMock, times(1)).dismissBiometricNotification()
        verify(biometricsInterfaceMock, times(1)).requestBiometricsRegistrationStateChange(mockAccessToken)
    }

    @Test
    fun showBiometricLoginIfEnabled_SuccessfulConfig_NullQueryUrl_ReturnsTrue() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) + getStringById(
                R.string.loginPath))
        spyActivity.configurationResponse.callSuccessful = true
        spyActivity.configurationResponse.isSupportedVersion = true

        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { showBiometricLoginIfEnabled() }.thenReturn(true)
        }
        FieldSetter.setField(spyActivity,
                spyActivity::class.java.getDeclaredField("biometricsInterface"),
                biometricsInterfaceMock)

        val result = spyActivity.showBiometricLoginIfEnabled()

        assertTrue(result)
        verify(biometricsInterfaceMock).showBiometricLoginIfEnabled()
    }

    @Test
    fun selectingRetryButton_clearsSelectedMenuItem() {
        // arrange
        mainActivity.configurationResponse.callSuccessful = true
        mainActivity.menuBar.switchActiveMenuItemTo(R.id.advice)

        // act
        mainActivity.retryButton.callOnClick()

        // assert
        assertFalse(mainActivity.menuBar.isSelected)
    }

    @Test
    fun showUnavailabilityError_nhs111LinkClickListener_whenConfigurationResponseSuccessfulAndConnectedToNetwork_loadsNhs111Url() {
        val message = "If you need urgent help, go to 111.nhs.uk or call 111."
        spyActivity.configurationResponse.callSuccessful = true
        setNetworkState(true, spyActivity)

        val nhsWebMock: NhsWeb = mock()
        FieldSetter.setField(spyActivity,
                spyActivity::class.java.getDeclaredField("nhsWeb"),
                nhsWebMock)

        spyActivity.showUnavailabilityError(ErrorMessage(mock {
            on { getString(R.string.server_error_title) } doReturn ""
            on { getString(R.string.server_error_message) } doReturn message
            on { getString(R.string.accessible_server_error_message) } doReturn message
        }, ErrorType.ServiceUnavailable))

        val errorTextView = spyActivity.findViewById<TextView>(R.id.errorTextView)
        val nhs111Span = (errorTextView.text as SpannableString).getSpans(31, 41, ClickableLink::class.java)[0]
        nhs111Span.onClick(errorTextView)

        verify(nhsWebMock).loadUrl(getStringById(R.string.nhs111URL))
    }

    @Test
    fun showUnavailabilityError_nhs111LinkClickListener_whenConfigurationResponseUnsuccessfulAndConnectedToNetwork_startsActivityWithIntentWithViewActionAndNhs111Uri() {
        val message = "If you need urgent help, go to 111.nhs.uk or call 111."
        spyActivity.configurationResponse.callSuccessful = false
        setNetworkState(true, spyActivity)

        spyActivity.showUnavailabilityError(ErrorMessage(mock {
            on { getString(R.string.server_error_title) } doReturn ""
            on { getString(R.string.server_error_message) } doReturn message
            on { getString(R.string.accessible_server_error_message) } doReturn message
        }, ErrorType.ServiceUnavailable))

        val errorTextView = spyActivity.findViewById<TextView>(R.id.errorTextView)
        val nhs111Span = (errorTextView.text as SpannableString).getSpans(31, 41, ClickableLink::class.java)[0]
        nhs111Span.onClick(errorTextView)

        val openBrowserUrlCaptor = argumentCaptor<String>()
        verify(spyActivity).openUrlInBrowserActivity(openBrowserUrlCaptor.capture())

        assertEquals(getStringById(R.string.nhs111URL), openBrowserUrlCaptor.firstValue)
    }

    @Test
    fun showUnavailabilityError_nhs111LinkClickListener_whenConfigurationResponseSuccessfulAndNotConnectedToNetwork_startsActivityWithIntentWithViewActionAndNhs111Uri() {
        val message = "If you need urgent help, go to 111.nhs.uk or call 111."
        spyActivity.configurationResponse.callSuccessful = true
        setNetworkState(false, spyActivity)

        spyActivity.showUnavailabilityError(ErrorMessage(mock {
            on { getString(R.string.server_error_title) } doReturn ""
            on { getString(R.string.server_error_message) } doReturn message
            on { getString(R.string.accessible_server_error_message) } doReturn message
        }, ErrorType.ServiceUnavailable))

        val errorTextView = spyActivity.findViewById<TextView>(R.id.errorTextView)
        val nhs111Span = (errorTextView.text as SpannableString).getSpans(31, 41, ClickableLink::class.java)[0]
        nhs111Span.onClick(errorTextView)

        val openBrowserUrlCaptor = argumentCaptor<String>()
        verify(spyActivity).openUrlInBrowserActivity(openBrowserUrlCaptor.capture())

        assertEquals(getStringById(R.string.nhs111URL), openBrowserUrlCaptor.firstValue)
    }

    @Test
    fun openUrlInBrowserActivity_startsOpenUrlInBrowserActivityWithGivenUrl() {
        val url = getStringById(R.string.nhs111URL)
        val openUrlInBrowserActivityMock: OpenUrlInBrowserActivity = mock()
        ReflectionHelpers.setField(spyActivity, "openBrowserActivity", openUrlInBrowserActivityMock)

        spyActivity.openUrlInBrowserActivity(url)

        verify(openUrlInBrowserActivityMock).start(spyActivity, url, spyActivity)
    }

    @Test
    fun buildUserAgentString_returnsFullUserAgentString() {
        val userAgent = "test-user-agent"
        val actualFullUserAgent = spyActivity.buildUserAgentString(userAgent)
        val expectedFullUserAgent = userAgent +
                " nhsapp-android/${com.nhs.online.nhsonline.BuildConfig.VERSION_NAME}" +
                " nhsapp-manufacturer/${Build.MANUFACTURER}" +
                " nhsapp-model/${Build.MODEL}" +
                " nhsapp-os/${Build.VERSION.RELEASE}" +
                " nhsapp-architecture/${Build.SUPPORTED_ABIS.joinToString(",") }"

       assertEquals(expectedFullUserAgent, actualFullUserAgent)
    }

    private fun getStringById(resId: Int): String = mainActivity.resources.getString(resId)

    private fun setNetworkState(isConnected: Boolean, activity: MainActivity) {
        connectionStateMonitorMock = mock {
            on { isConnectedToNetwork } doReturn isConnected
        }

        FieldSetter.setField(activity,
                activity::class.java.getDeclaredField("connectionStateMonitor"),
                connectionStateMonitorMock)
    }
}
