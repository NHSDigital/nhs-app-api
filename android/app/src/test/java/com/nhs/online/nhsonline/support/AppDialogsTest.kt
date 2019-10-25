package com.nhs.online.nhsonline.support

import android.app.Activity
import android.support.v7.app.AlertDialog
import android.widget.Button
import android.widget.TextView
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.R
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.shadows.ShadowDialog


@RunWith(RobolectricTestRunner::class)
class AppDialogsTest {
    private val activity: Activity = Robolectric.buildActivity(Activity::class.java).get()
    private lateinit var appDialogs: AppDialogs

    @Before
    fun setUp() {
        appDialogs = AppDialogs(activity)
    }

    @Test
    fun showVersionUpgradeDialog() {
        val expectedTitle = "Update required"
        val expectedPartMessageText = "You need to update the NHS App"
        appDialogs.showVersionUpgradeDialog()
        val upgradeDialog = getCurrentActiveAlertDialog()
        val title = extractAlertDialogTitle(upgradeDialog)
        Assert.assertEquals(title, expectedTitle)
        val message = extractAlertDialogMessage(upgradeDialog)
        Assert.assertNotNull(message)
        message?.apply { Assert.assertTrue(message.contains(expectedPartMessageText)) }
    }

    @Test
    fun showVersionUpgradeDialog_ClickingNegativeButton_FinishesActivity() {
        val spyActivity = spy(activity)
        appDialogs = AppDialogs(spyActivity)
        appDialogs.showVersionUpgradeDialog()
        val upgradeDialog = getCurrentActiveAlertDialog()
        val negativeButton: Button = upgradeDialog.getButton(AlertDialog.BUTTON_NEGATIVE)
        negativeButton.callOnClick()
        verify(spyActivity).finishAndRemoveTask()
    }

    @Test
    fun showOverlayDetectedDialog() {
        val expectedTitle = "Screen overlay detected"
        val expectedPartMessageText = "other apps overlaying the screen"
        appDialogs.showOverlayDetectedDialog()
        val overlayDialog = getCurrentActiveAlertDialog()
        val title = extractAlertDialogTitle(overlayDialog)
        Assert.assertEquals(title, expectedTitle)
        val message = extractAlertDialogMessage(overlayDialog)
        Assert.assertNotNull(message)
        message?.apply { Assert.assertTrue(message.contains(expectedPartMessageText)) }
    }

    @Test
    fun sshowOverlayDetectedDialog_ClickingNegativeButton_FinishesActivity() {
        val spyActivity = spy(activity)
        appDialogs = AppDialogs(spyActivity)
        appDialogs.showOverlayDetectedDialog()
        val overlayDialog = getCurrentActiveAlertDialog()
        val negativeButton: Button = overlayDialog.getButton(AlertDialog.BUTTON_NEGATIVE)
        negativeButton.callOnClick()
        verify(spyActivity).finishAndRemoveTask()
    }

    @Test
    fun dismissVersionDialog_NoAction_IfActivityIsFinishing() {
        val spyActivity = spy(activity)
        appDialogs = AppDialogs(spyActivity)
        appDialogs.showVersionUpgradeDialog()
        whenever(spyActivity.isFinishing).thenReturn(true)
        val dialog = ShadowDialog.getLatestDialog()
        appDialogs.dismissVersionUpgradeDialog()
        Assert.assertTrue(dialog.isShowing)
    }

    @Test
    fun showExitDialog() {
        val expectedMessageText = "Are you sure you want to log out?"
        appDialogs.showExitDialog {}
        val exitDialog = getCurrentActiveAlertDialog()
        val message = extractAlertDialogMessage(exitDialog)
        Assert.assertNotNull(message)
        Assert.assertEquals(message, expectedMessageText)
    }

    @Test
    fun showExitDialog_InvokesCallback_OnPositiveButtonClicked() {
        val clickTester:ClickTester = mock()
        appDialogs.showExitDialog{clickTester.clicked()}
        val exitDialog = getCurrentActiveAlertDialog()
        val positiveButton = exitDialog.getButton(AlertDialog.BUTTON_POSITIVE)
        positiveButton.callOnClick()
        verify(clickTester).clicked()
    }

    @Test
    fun showExitDialog_NoAlertDialogue_IfActivityIsFinishing() {
        val spyActivity = spy(activity)
        whenever(spyActivity.isFinishing).thenReturn(true)
        appDialogs = AppDialogs(spyActivity)
        appDialogs.showExitDialog {}
        Assert.assertNull(ShadowDialog.getLatestDialog())
    }

    @Test
    fun showExtendSessionDialogue() {
        val sessionDuration = 10
        val expectedMessageText =
            "Every $sessionDuration minutes, we log you out for security purposes."
        appDialogs.showExtendSessionDialogue(sessionDuration, {}, {})
        val extendSessionDialog = getCurrentActiveAlertDialog()
        val messageTextView: TextView? =
            extendSessionDialog.findViewById(R.id.sessionExpiryWarningDurationInformation)
        Assert.assertNotNull(messageTextView)
        val message = messageTextView?.text
        Assert.assertNotNull(message)
        Assert.assertEquals(message, expectedMessageText)
    }

    @Test
    fun showExtendSessionDialogue_InvokesLogoutCallback_OnLogoutButtonClicked() {
        val clickTester:ClickTester = mock()
        appDialogs.showExtendSessionDialogue(10, {}, {clickTester.clicked()})
        val extendSessionDialog = getCurrentActiveAlertDialog()
        val logoutButton: Button? = extendSessionDialog.findViewById(R.id.logOut)
        Assert.assertNotNull(logoutButton)
        logoutButton?.callOnClick()
        verify(clickTester).clicked()
    }

    @Test
    fun showExtendSessionDialogue_InvokeExtendCallback_OnExtendButtonClicked() {
        val clickTester:ClickTester = mock()
        appDialogs.showExtendSessionDialogue(10, {clickTester.clicked()}, {})
        val extendSessionDialog = getCurrentActiveAlertDialog()
        val extendButton: Button? = extendSessionDialog.findViewById(R.id.extendSession)
        Assert.assertNotNull(extendButton)
        extendButton?.callOnClick()
        verify(clickTester).clicked()
    }

    @Test
    fun dismissExtendSessionDialog_NoAction_IfActivityIsFinishing() {
        val spyActivity = spy(activity)
        appDialogs = AppDialogs(spyActivity)
        appDialogs.showExtendSessionDialogue(10, {}, {})
        whenever(spyActivity.isFinishing).thenReturn(true)
        val dialog = ShadowDialog.getLatestDialog()
        appDialogs.dismissExtendSessionDialog()
        Assert.assertTrue(dialog.isShowing)
    }

    @Test
    fun showExtendSessionDialogue_NoAlertDialog_IfActivityIsFinishing() {
        val sessionDuration = 10
        val spyActivity = spy(activity)
        whenever(spyActivity.isFinishing).thenReturn(true)
        appDialogs = AppDialogs(spyActivity)
        appDialogs.showExtendSessionDialogue(sessionDuration, {}, {})
        Assert.assertNull(ShadowDialog.getLatestDialog())
    }

    private fun extractAlertDialogMessage(alertDialog: AlertDialog): String? {
        val messageTextView: TextView? = alertDialog.findViewById(android.R.id.message)
        Assert.assertNotNull(messageTextView)
        return messageTextView?.text.toString()
    }

    private fun extractAlertDialogTitle(alertDialog: AlertDialog): String? {
        val titleTextView: TextView? =
            alertDialog.findViewById(android.support.v7.appcompat.R.id.alertTitle)
        Assert.assertNotNull(titleTextView)
        return titleTextView?.text.toString()
    }

    private fun getCurrentActiveAlertDialog(): AlertDialog {
        val alertDialog = ShadowDialog.getLatestDialog() as AlertDialog
        Assert.assertNotNull(alertDialog)
        Assert.assertTrue(alertDialog.isShowing)
        return alertDialog
    }

    private interface ClickTester {
        fun clicked()
    }
}