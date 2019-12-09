package com.nhs.online.nhsonline.activities

import kotlinx.android.synthetic.main.activity_main.*
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import android.app.onResume
import android.support.v7.app.AlertDialog
import android.widget.TextView
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.BiometricsInterface
import com.nhs.online.nhsonline.network.MockConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.support.AppDialogs
import com.nhs.online.nhsonline.web.NhsWeb
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import kotlinx.android.synthetic.main.error_layout.retryButton
import org.junit.Assert
import org.junit.Assert.assertFalse
import org.junit.Before
import org.mockito.internal.util.reflection.FieldSetter
import org.robolectric.Robolectric
import org.robolectric.shadows.ShadowDialog


@RunWith(RobolectricTestRunner::class)
class MainActivityTest {

    private lateinit var mainActivity: MainActivity
    private lateinit var spyActivity: MainActivity

    @Before
    fun setUp() {
        mainActivity = Robolectric.buildActivity(MainActivity::class.java).create().get()
        spyActivity = spy(mainActivity)
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
    }

    @Test
    fun onResume_nullWebViewUrl_noException_resetUrlToLogin() {
        mainActivity.webview.loadUrl(null)
        mainActivity.isSuccessfulConfigCheck = true

        try {
            mainActivity.onResume()
        } catch (e: Exception) {
            assert(false)
        }
        val loginUrl =
            getStringById(R.string.baseURL) + getStringById(R.string.loginPath) + getStringById(R.string.nhsOnlineRequiredQueries)
        Assert.assertEquals(mainActivity.webview.url, loginUrl)
    }

    @Test
    fun onResume_LoginPath_StartBiometrics() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) + getStringById(
            R.string.loginPath))
        spyActivity.isSuccessfulConfigCheck = true

        try{
            spyActivity.onResume()
        } catch(e: Exception) {
            assert(false)
        }

        verify(spyActivity, times(1)).showBiometricLoginIfEnabled(true)
    }

    @Test
    fun onResume_NotLoginPath_DoNotStartBiometrics() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) + getStringById(
            R.string.gpFinderPath))
        spyActivity.isSuccessfulConfigCheck = true

        try{
            spyActivity.onResume()
        } catch(e: Exception) {
            assert(false)
        }

        verify(spyActivity, times(0)).showBiometricLoginIfEnabled(true)
    }

    @Test
    fun onBackButtonPressed_OnLoginScreen_ClosesApp() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) + getStringById(
            R.string.loginPath))
        spyActivity.isSuccessfulConfigCheck = true

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        verify(spyActivity, times(1)).finishAndRemoveTask()
    }

    @Test
    fun onBackButtonPressed_OnGpFinderScreen_ClosesApp() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) +
                getStringById(R.string.gpFinderPath))
        spyActivity.isSuccessfulConfigCheck = true

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        verify(spyActivity, times(1)).finishAndRemoveTask()
    }

    @Test
    fun onBackButtonPressed_OnGpFinderOtherScreen_ResetGpFlow() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) +
                getStringById(R.string.gpFinderPath) + "/otherPath")
        spyActivity.isSuccessfulConfigCheck = true

        val appWebInterface = AppWebInterface(spyActivity.webview)
        val spyAppWebInterface = spy(appWebInterface)

        FieldSetter.setField(spyActivity,
            spyActivity::class.java.getDeclaredField("appWebInterface"),
            spyAppWebInterface)

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }
        verify(spyAppWebInterface, times(1)).resetGPFinderFlow(any())
    }

    @Test
    fun onBackButtonPressed_IsLoggedIn_ShowAlertDialog() {
        spyActivity.webview.loadUrl(getStringById(R.string.baseURL) +
                getStringById(R.string.gpFinderPath) + "/otherPath")
        spyActivity.isSuccessfulConfigCheck = true

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
        Assert.assertNotNull(logoutAlertDialog)
        Assert.assertTrue(logoutAlertDialog.isShowing)

        val messageTextView = logoutAlertDialog.findViewById<TextView>(android.R.id.message)
        Assert.assertNotNull(messageTextView)
        messageTextView?.apply { Assert.assertEquals("Are you sure you want to log out?", text) }
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
        verify(nhsWebMock, times(1)).onbackButtonPressedOnCheckSymptomsUnsecurePage()
    }

    @Test
    fun onBackButtonPressed_WhenUrlIsUnsecureSymptomsPage_CallsOnbackButtonPressedOnCheckSymptomsUnsecurePage() {
        val url = "http://some-nhs-service-when-not-logged-in"
        spyActivity.webview.loadUrl(url)

        val nhsWebMock: NhsWeb = mock {
            on { isUserLoggedIn }.thenReturn(false)
            on { isCheckSymptomsUnsecureURL(url) }.thenReturn(true)
        }
        FieldSetter.setField(spyActivity,
            spyActivity::class.java.getDeclaredField("nhsWeb"),
            nhsWebMock)

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }
        verify(nhsWebMock, times(1)).onbackButtonPressedOnCheckSymptomsUnsecurePage()
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

        verify(nhsWebMock, times(1)).reloadHomepageOnBackReturn()
    }

    @Test
    fun onBackButtonPressed_MalFormedUrl_ClosesApp() {
        spyActivity.webview.loadUrl("ghghgh:/&/|sdsdsdiuw")
        spyActivity.isSuccessfulConfigCheck = true

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        verify(spyActivity, times(1)).finishAndRemoveTask()
    }

    @Test
    fun onBackButtonPressed_NullUrl_ClosesApp() {
        spyActivity.webview.loadUrl(null)
        spyActivity.isSuccessfulConfigCheck = true

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        verify(spyActivity, times(1)).finishAndRemoveTask()
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
        Assert.assertNotNull(updateAlertDialog)
        Assert.assertTrue(updateAlertDialog.isShowing)

        val messageTextView = updateAlertDialog.findViewById<TextView>(android.R.id.message)
        Assert.assertNotNull(messageTextView)
        messageTextView?.apply { Assert.assertTrue(text.contains("Click here to update")) }

        val dialogUrls = messageTextView?.urls
        val updateUrl = dialogUrls?.firstOrNull()?.url

        messageTextView.apply {
            Assert.assertEquals("market://details?id=com.nhs.online.nhsonline",
                updateUrl)
        }
    }

    @Test
    fun showBiometricLoginIfEnabled_SuccessfulConfig_ReturnsTrue() {
        spyActivity.isSuccessfulConfigCheck = true

        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { showBiometricLoginIfEnabled() }.thenReturn(true)
        }
        FieldSetter.setField(spyActivity,
            spyActivity::class.java.getDeclaredField("biometricsInterface"),
            biometricsInterfaceMock)

        val result = spyActivity.showBiometricLoginIfEnabled()

        Assert.assertTrue(result)
        verify(biometricsInterfaceMock, times(1)).showBiometricLoginIfEnabled()
    }

    @Test
    fun showBiometricLoginIfEnabled_UnsuccessfulConfig_ReturnsFalse() {
        spyActivity.isSuccessfulConfigCheck = false

        val biometricsInterfaceMock: BiometricsInterface = mock {
            on { showBiometricLoginIfEnabled() }.thenReturn(false)
        }
        FieldSetter.setField(spyActivity,
                spyActivity::class.java.getDeclaredField("biometricsInterface"),
                biometricsInterfaceMock)

        val result = spyActivity.showBiometricLoginIfEnabled()

        Assert.assertFalse(result)
        verify(biometricsInterfaceMock, times(0)).showBiometricLoginIfEnabled()
    }

    @Test
    fun selectingRetryButton_clearsSelectedMenuItem() {
        // arrange
        mainActivity.isSuccessfulConfigCheck = true
        mainActivity.menuBar.switchActiveMenuItemTo(R.id.symptoms)

        // act
        mainActivity.retryButton.callOnClick()

        // assert
        assertFalse(mainActivity.menuBar.isSelected)
    }

    private fun getStringById(resId: Int): String = mainActivity.resources.getString(resId)
}