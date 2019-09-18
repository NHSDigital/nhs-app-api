package com.nhs.online.nhsonline.support

import android.annotation.SuppressLint
import android.app.Activity
import android.support.v7.app.AlertDialog
import android.text.method.LinkMovementMethod
import android.util.Log
import android.widget.Button
import android.widget.TextView
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.utils.Html


class AppDialogs(private val activity: Activity) {
    private var upgradeDialog: AlertDialog? = null
    private var extendSessionDialogue: AlertDialog? = null
    private var exitDialog: AlertDialog? = null

    fun showVersionUpgradeDialog() {
        if (!activity.isFinishing) {
            val showing = showDialogIfAvailable(upgradeDialog)
            if (showing) return

            val content = getResourceString(R.string.UpdateHeader) +
                    "<br/><br/>" +
                    getResourceString(R.string.UpdateNativeLink) +
                    "<br/><br/>" +
                    getResourceString(R.string.UpdateDesc)
            val title = getResourceString(R.string.UpdateRequiredHeader)
            upgradeDialog = showNonCancellableDialog(title, content)
        }
        return
    }

    private fun showNonCancellableDialog(
        title: String, message: String
    ): AlertDialog {
        val builder =
            createDialogBuilder(title, message, activity.resources.getString(R.string.Close)) {
                activity.finishAndRemoveTask()
            }

        val dialog = builder.create()
        dialog.setCancelable(false)
        dialog.setCanceledOnTouchOutside(false)
        dialog.show()
        enableLinkMovementMethod(message, dialog)
        return dialog
    }

    fun showExitDialog(onLogoutClicked: () -> Unit) {
        if (!activity.isFinishing) {
            val showing = showDialogIfAvailable(exitDialog)
            if (showing) return

            val builder: AlertDialog.Builder = AlertDialog.Builder(activity)
            builder.setMessage(getResourceString(R.string.logoutWarning))
                    .setPositiveButton(getResourceString(R.string.logout)) { _, _ -> onLogoutClicked.invoke() }
                    .setNegativeButton(getResourceString(R.string.cancel)) { _, _ -> }

            val dialog: AlertDialog = builder.create()
            exitDialog = dialog
            dialog.show()
        }
        return
    }

    private fun createDialogBuilder(
        title: String, message: String,
        negativeButtonText: String, onNegativeButtonClicked: (() -> Unit)? = null
    ): AlertDialog.Builder {
        val builder = AlertDialog.Builder(activity)
        builder.setTitle(title)
        builder.setMessage(message)
        builder.setNegativeButton(negativeButtonText) { _, _ ->
            onNegativeButtonClicked?.invoke()
        }
        return builder
    }

    private fun enableLinkMovementMethod(message: String, dialog: AlertDialog) {
        val dialogTextView: TextView? = dialog.findViewById(android.R.id.message)
        dialogTextView?.apply {
            text = Html.fromHtml(message)
            movementMethod = LinkMovementMethod.getInstance()
        }
    }

    fun showExtendSessionDialogue(
        sessionDuration: Int,
        onExtendClicked: () -> Unit,
        onLogoutClicked: () -> Unit
    ) {
        Log.d("AppDialogs", "Entering showExtendSessionDialogue")
        val showing = showDialogIfAvailable(extendSessionDialogue)
        if (showing) return

        val dialog =
            initialiseExtendSessionDialogue(sessionDuration, onExtendClicked, onLogoutClicked)
        extendSessionDialogue = dialog
        dialog.show()
    }

    @SuppressLint("InflateParams")
    private fun initialiseExtendSessionDialogue(
        sessionDuration: Int,
        onExtendClicked: () -> Unit,
        onLogoutClicked: () -> Unit
    ): AlertDialog {
        val inflater = activity.layoutInflater
        val dialogView = inflater.inflate(R.layout.session_expiry_warning_dialogue, null)
        val textView =
            dialogView.findViewById(R.id.sessionExpiryWarningDurationInformation) as TextView
        val sessionExpiryMessage =
            activity.resources.getString(R.string.sessionExpiryWarningDurationInformation)
                .format(sessionDuration)
        textView.text = sessionExpiryMessage
        textView.contentDescription = sessionExpiryMessage
        val extendSession = dialogView.findViewById(R.id.extendSession) as Button
        val logOut = dialogView.findViewById(R.id.logOut) as Button

        val builder: AlertDialog.Builder = AlertDialog.Builder(activity)
        builder.setView(dialogView)
        builder.setCancelable(false)

        val dialog: AlertDialog = builder.create()
        extendSession.setOnClickListener {
            onExtendClicked.invoke()
            dismissDialogSafely(dialog)
        }
        logOut.setOnClickListener {
            onLogoutClicked.invoke()
            dismissDialogSafely(dialog)
        }
        dialog.setCanceledOnTouchOutside(false)

        return dialog
    }

    fun dismissVersionUpgradeDialog() = dismissDialogSafely(upgradeDialog)

    fun dismissExtendSessionDialog() = dismissDialogSafely(extendSessionDialogue)

    fun isUpgradeDialogActive() = isDialogActive(upgradeDialog)

    private fun showDialogIfAvailable(dialog: AlertDialog?): Boolean {
        if (isDialogActive(dialog)) return true
        return if (dialog != null) {
            dialog.show(); true
        } else {
            false
        }
    }

    private fun dismissDialogSafely(alertDialog: AlertDialog?){
        if(isDialogActive(alertDialog) && !activity.isFinishing){
            alertDialog?.dismiss()
        }
    }

    private fun isDialogActive(alertDialog: AlertDialog?) = alertDialog?.isShowing ?: false

    private fun getResourceString(resId: Int) = activity.resources.getString(resId)
}