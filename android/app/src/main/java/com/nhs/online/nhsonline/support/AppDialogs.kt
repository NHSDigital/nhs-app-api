package com.nhs.online.nhsonline.support

import android.app.Activity
import android.support.v7.app.AlertDialog
import android.text.method.LinkMovementMethod
import android.util.Log
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

            val dialog = createDialog(
                    R.string.updateRequiredTitle,
                    R.string.updateRequiredMessage,
                    R.string.close) {
                activity.finishAndRemoveTask()
            }

            dialog.setCancelable(false)
            dialog.setCanceledOnTouchOutside(false)
            upgradeDialog = dialog
            dialog.show()
            enableLinkMovementMethod(dialog)
        }
        return
    }

    fun showExitDialog(onLogoutClicked: () -> Unit) {
        if (!activity.isFinishing) {
            val showing = showDialogIfAvailable(exitDialog)
            if (showing) return

            val dialog = createDialog(
                    null,
                    R.string.logoutWarning,
                    R.string.logout,
                    onLogoutClicked,
                    R.string.cancel,
                    null
            )
            exitDialog = dialog
            dialog.show()
        }
        return
    }

    private fun enableLinkMovementMethod(dialog: AlertDialog) {
        val dialogTextView: TextView? = dialog.findViewById(android.R.id.message)
        dialogTextView?.apply {
            text = Html.fromHtml(dialogTextView.text.toString())
            movementMethod = LinkMovementMethod.getInstance()
        }
    }

    fun showExtendSessionDialogue(
            onExtendClicked: () -> Unit,
            onLogoutClicked: () -> Unit
    ) {
        if (!activity.isFinishing) {
            Log.d("AppDialogs", "Entering showExtendSessionDialogue")
            val showing = showDialogIfAvailable(extendSessionDialogue)
            if (showing) return

            val dialog = createDialog(
                    null,
                    R.string.sessionExpiryWarningMessage,
                    R.string.sessionExpiryWarningStayLoggedIn,
                    onExtendClicked,
                    R.string.sessionExpiryWarningLogOut,
                    onLogoutClicked
            )

            dialog.setCancelable(false)
            dialog.setCanceledOnTouchOutside(false)
            extendSessionDialogue = dialog
            dialog.show()
        }
    }

    fun dismissVersionUpgradeDialog() = dismissDialogSafely(upgradeDialog)

    fun dismissExtendSessionDialog() = dismissDialogSafely(extendSessionDialogue)

    fun isUpgradeDialogActive() = isDialogActive(upgradeDialog)

    private fun createDialog(
            title: Int,
            message: Int,
            negativeButtonText: Int,
            onNegativeButtonClicked: (() -> Unit)? = null
    ): AlertDialog = createDialog(
            title,
            message,
            null,
            null,
            negativeButtonText,
            onNegativeButtonClicked
    )

    private fun createDialog(
            title: Int?,
            message: Int,
            positiveButtonText: Int?,
            onPositiveButtonClicked: (() -> Unit)?,
            negativeButtonText: Int,
            onNegativeButtonClicked: (() -> Unit)?
    ): AlertDialog {
        val builder = AlertDialog.Builder(activity)

        if (title != null) {
            builder.setTitle(title)
        }
        builder.setMessage(message)

        if (positiveButtonText != null) {
            builder.setPositiveButton(positiveButtonText) { dialog, _ ->
                onPositiveButtonClicked?.invoke()
                dismissDialogSafely(dialog as? AlertDialog)
            }
        }

        builder.setNegativeButton(negativeButtonText) { dialog, _ ->
            onNegativeButtonClicked?.invoke()
            dismissDialogSafely(dialog as? AlertDialog)
        }

        return builder.create()
    }

    private fun showDialogIfAvailable(dialog: AlertDialog?): Boolean {
        if (isDialogActive(dialog)) return true
        return if (dialog != null) {
            dialog.show(); true
        } else {
            false
        }
    }

    private fun dismissDialogSafely(alertDialog: AlertDialog?) {
        if (isDialogActive(alertDialog) && !activity.isFinishing) {
            alertDialog?.dismiss()
        }
    }

    private fun isDialogActive(alertDialog: AlertDialog?) = alertDialog?.isShowing ?: false

    private fun getResourceString(resId: Int) = activity.resources.getString(resId)
}