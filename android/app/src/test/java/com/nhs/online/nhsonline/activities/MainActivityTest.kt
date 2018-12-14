package com.nhs.online.nhsonline.activities

import kotlinx.android.synthetic.main.activity_main.*
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import android.app.onResume
import android.support.v7.app.AlertDialog
import android.widget.TextView
import com.nhaarman.mockito_kotlin.spy
import com.nhaarman.mockito_kotlin.times
import com.nhaarman.mockito_kotlin.verify
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import junit.framework.Assert.assertEquals
import org.junit.Assert
import org.junit.Before
import org.mockito.internal.util.reflection.FieldSetter
import org.robolectric.Robolectric
import org.robolectric.shadows.ShadowDialog


@RunWith(RobolectricTestRunner::class)
class MainActivityTest {

    lateinit var mainActivity: MainActivity
    lateinit var spyActivity: MainActivity

    @Before
    fun setUp() {
        mainActivity = Robolectric.setupActivity(MainActivity::class.java)
        spyActivity = spy(mainActivity)
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
        val loginUrl = mainActivity.resources.getString(R.string.baseURL) + mainActivity.resources.getString(R.string.loginPath)
        assertEquals(mainActivity.webview.url, loginUrl)
    }

    @Test
    fun onBackButtonPressed_OnLoginScreen_ClosesApp() {
        spyActivity.webview.loadUrl(mainActivity.resources.getString(R.string.baseURL) + mainActivity.resources.getString(R.string.loginPath))
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
        spyActivity.webview.loadUrl(mainActivity.resources.getString(R.string.baseURL) +
                mainActivity.resources.getString(R.string.gpFinderPath))
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
        spyActivity.webview.loadUrl(mainActivity.resources.getString(R.string.baseURL) +
                mainActivity.resources.getString(R.string.gpFinderPath) + "/otherPath")
        spyActivity.isSuccessfulConfigCheck = true

        val appWebInterface = AppWebInterface(spyActivity)
        val spyAppWebInterface = spy(appWebInterface)

        FieldSetter.setField(spyActivity, spyActivity::class.java.getDeclaredField("appWebInterface"), spyAppWebInterface)

        try {
            spyActivity.onBackPressed()
        } catch (e: Exception) {
            assert(false)
        }

        verify(spyAppWebInterface, times(1)).resetGPFinderFlow()
    }


    @Test
    fun onBackButtonPressed_IsLoggedIn_ShowAlertDialog() {
        spyActivity.webview.loadUrl(mainActivity.resources.getString(R.string.baseURL) +
                mainActivity.resources.getString(R.string.gpFinderPath) + "/otherPath")
        spyActivity.isSuccessfulConfigCheck = true

        FieldSetter.setField(spyActivity, spyActivity::class.java.getDeclaredField("isLoggedIn"), true)

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
}