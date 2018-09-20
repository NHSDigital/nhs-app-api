package com.nhs.online.nhsonline

import android.support.v7.app.AlertDialog
import android.widget.TextView
import com.nhaarman.mockito_kotlin.spy
import com.nhaarman.mockito_kotlin.verify
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.annotation.Config
import org.robolectric.shadows.ShadowDialog


@RunWith(RobolectricTestRunner::class)
@Config(constants = BuildConfig::class)
class AppWebInterfaceTest {

    private val mainActivity = Robolectric.setupActivity(MainActivity::class.java)
    private val spyActivity = spy(mainActivity)
    private val appWebInterface = AppWebInterface(spyActivity)

    @Test
    fun loadSPATest() {
        appWebInterface.loadSpaPage("testpath", "http://test.com")
        verify(spyActivity).evaluateWebviewJavascript("window.\$nuxt.\$router.push('/testpath')")
    }

    @Test
    fun loadDispatchEventTest() {
        appWebInterface.loadDispatchEvent("auth/logout")
        verify(spyActivity).evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('auth/logout')")
    }

    @Test
    fun onDeviceBackButtonPressedLogoutDialogIsShown() {
        spyActivity.loggedIn()
        spyActivity.onBackPressed()
        val logoutAlertDialog = ShadowDialog.getLatestDialog() as AlertDialog
        Assert.assertNotNull(logoutAlertDialog)
        Assert.assertTrue(logoutAlertDialog.isShowing)

        val messageTextView = logoutAlertDialog.findViewById<TextView>(android.R.id.message)
        Assert.assertNotNull(messageTextView)
        messageTextView?.apply { Assert.assertEquals("Are you sure you want to log out?", text) }
    }
}