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
import com.nhs.online.nhsonline.utils.Html
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
        mainActivity = Robolectric.buildActivity(MainActivity::class.java).create().get()
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

    @Test
    fun showVersionUpgradeDialog_CheckUpdateUrl_NotInitialised_ShowAlertDialog() {
        try {
            spyActivity.showVersionUpgradeDialog()
        } catch (e: Exception) {
            assert(false)
        }

        val updateAlertDialog = ShadowDialog.getLatestDialog() as AlertDialog
        Assert.assertNotNull(updateAlertDialog)
        Assert.assertTrue(updateAlertDialog.isShowing)

        val messageTextView = updateAlertDialog.findViewById<TextView>(android.R.id.message)
        Assert.assertNotNull(messageTextView)
        messageTextView?.apply { Assert.assertTrue(text.contains("Click here to update")) }

        var dialogUrls = messageTextView?.urls
        val updateUrl = dialogUrls!![0].getURL()

        messageTextView?.apply { Assert.assertEquals("market://details?id=com.nhs.online.nhsonline", updateUrl) }
    }

    @Test
    fun showVersionUpgradeDialog_IsInitialised_AndNotCurrentlyShowing_ShowAlertDialog() {
        lateinit var upgradeDialog: AlertDialog

        val builder: AlertDialog.Builder = AlertDialog.Builder(spyActivity)
                .setTitle("Test Header")

        builder.setCancelable(false)
        upgradeDialog = builder.create()
        upgradeDialog.setCanceledOnTouchOutside(false)
        upgradeDialog.setCancelable(false)

        FieldSetter.setField(spyActivity, spyActivity::class.java.getDeclaredField("upgradeDialog"), upgradeDialog)

        try {
            spyActivity.showVersionUpgradeDialog()
        } catch (e: Exception) {
            assert(false)
        }

        val updateAlertDialog = ShadowDialog.getLatestDialog() as AlertDialog
        Assert.assertNotNull(updateAlertDialog)
        Assert.assertTrue(updateAlertDialog.isShowing)

        val messageTextView = updateAlertDialog.findViewById<TextView>(android.R.id.message)
        Assert.assertNotNull(messageTextView)
        messageTextView?.apply { Assert.assertTrue(text.contains("Click here to update")) }
    }


    @Test
    fun showVersionUpgradeDialog_IsInitialised_CurrentlyShowing_CurrentAlertShouldntBeOverridden() {
        lateinit var upgradeDialog: AlertDialog

        val builder: AlertDialog.Builder = AlertDialog.Builder(spyActivity)
                .setMessage("Test Message")

        builder.setCancelable(false)
        upgradeDialog = builder.create()
        upgradeDialog.setCanceledOnTouchOutside(false)
        upgradeDialog.setCancelable(false)
        upgradeDialog.show()

        FieldSetter.setField(spyActivity, spyActivity::class.java.getDeclaredField("upgradeDialog"), upgradeDialog)
        try {
            spyActivity.showVersionUpgradeDialog()
        } catch (e: Exception) {
            assert(false)
        }

        val updateAlertDialog = ShadowDialog.getLatestDialog() as AlertDialog
        Assert.assertNotNull(updateAlertDialog)
        Assert.assertTrue(updateAlertDialog.isShowing)
        val messageTextView = updateAlertDialog.findViewById<TextView>(android.R.id.message)
        Assert.assertNotNull(messageTextView)
        messageTextView?.apply { Assert.assertTrue(text.contains("Test Message")) }
    }
}