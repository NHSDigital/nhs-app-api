package com.nhs.online.nhsonline.support

import android.support.v7.app.AlertDialog
import android.content.Context
import android.util.Log
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.interfaces.IInteractor

class AlertHelper(val context: Context, val interactor: IInteractor) {

    fun showDialog(title: String, message: String, helpLink: String = "") {
        val builder = createDialogBuilder(title, message, helpLink)
        val dialog = builder.create()
        dialog.show()
    }

    private fun createDialogBuilder(
        title: String,
        message: String,
        helpLink: String = ""
    ): AlertDialog.Builder {
        val builder = AlertDialog.Builder(context)
        builder.setTitle(title)
        builder.setMessage(message)

        if (!helpLink.isNullOrEmpty()) {
            builder.setNeutralButton(context.resources.getString(R.string.biometrics_dialog_get_help)) { _, _ ->
                val biometricHelpBrowserActivity =
                    OpenUrlInBrowserActivity(context.resources.getStringArray(R.array.nativeAppHosts))
                biometricHelpBrowserActivity.start(context, helpLink, interactor)
            }
        }

        builder.setNegativeButton(context.resources.getString(R.string.cancel)) { _, _ ->
            Log.d(Application.TAG, "User cancelled the dialog")
        }
        return builder
    }
}