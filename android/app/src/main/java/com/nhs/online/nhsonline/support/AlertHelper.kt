package com.nhs.online.nhsonline.support

import android.app.Activity
import androidx.appcompat.app.AlertDialog
import android.util.Log
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.interfaces.IInteractor

class AlertHelper(val context: Activity, val interactor: IInteractor) {

    fun showDialog(title: String, message: String, helpLink: String = "") {
        if (!context.isFinishing) {
            val builder = createDialogBuilder(title, message, helpLink)
            val dialog = builder.create()
            dialog.show()
        }
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
                interactor.loadPage(helpLink)
            }
        }

        builder.setNegativeButton(context.resources.getString(R.string.cancel)) { _, _ ->
            Log.d(Application.TAG, "User cancelled the dialog")
        }
        return builder
    }
}
