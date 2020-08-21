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
    private var leavingPageWarningDialogue: AlertDialog? = null
    private var exitDialog: AlertDialog? = null

    fun showVersionUpgradeDialog() {
        if (!activity.isFinishing) {
            showDialogIfPresent(upgradeDialog)

            if (!isDialogActive(upgradeDialog)) {

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
        }
        return
    }

    fun showExitDialog(onLogoutClicked: () -> Unit) {
        if (!activity.isFinishing) {
            showDialogIfPresent(exitDialog)

            if (!isDialogActive(exitDialog)) {

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
            showDialogIfPresent(extendSessionDialogue)

            if (!isDialogActive(extendSessionDialogue)) {

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
    }

    fun showLeavingPageWarningDialogue(
            onStayClicked: () -> Unit,
            onLeaveClicked: () -> Unit
    ) {
        if (activity.isFinishing) {
            return
        }
        Log.d("AppDialogs", "Entering showLeavingPageWarningDialogue")
        showDialogIfPresent(leavingPageWarningDialogue)

        if (!isDialogActive(leavingPageWarningDialogue)) {
            val dialog = with(
                    createDialog(
                            R.string.leavingPageWarningHeader,
                            R.string.leavingPageWarningMessage,
                            R.string.leavingPageWarningStayOnPage,
                            onStayClicked,
                            R.string.leavingPageWarningLeavePage,
                            onLeaveClicked
                    )
            ) {
                setCancelable(false)
                setCanceledOnTouchOutside(false)
                show()
                this
            }

            leavingPageWarningDialogue = dialog
        }
    }

    fun dismissVersionUpgradeDialog() = dismissDialogSafely(upgradeDialog)

    fun dismissExtendSessionDialog() = dismissDialogSafely(extendSessionDialogue)

    fun dismissShowLeavingWarningDialog() = dismissDialogSafely(leavingPageWarningDialogue)

    fun isUpgradeDialogActive() = isDialogActive(upgradeDialog)

    fun dismissAll() {
        dismissExtendSessionDialog()
        dismissShowLeavingWarningDialog()
    }

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

    private fun showDialogIfPresent(dialog: AlertDialog?) {
        if (isDialogActive(dialog))
        {
            dialog?.show()
        }
    }

    private fun dismissDialogSafely(alertDialog: AlertDialog?) {
        if (isDialogActive(alertDialog) && !activity.isFinishing) {
            alertDialog?.dismiss()
        }
    }

    private fun isDialogActive(alertDialog: AlertDialog?) = alertDialog?.isShowing ?: false
}
