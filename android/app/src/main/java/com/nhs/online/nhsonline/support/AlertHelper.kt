package com.nhs.online.nhsonline.support

import android.support.v7.app.AlertDialog
import android.content.Context
import android.text.method.LinkMovementMethod
import android.util.Log
import android.widget.TextView
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.utils.Html

class AlertHelper(val context: Context) {

    fun showDialog(title: String, message: String, isHtmlText: Boolean = false) {
        val builder = createDialogBuilder(title, message)
        val dialog = builder.create()
        dialog.show()

        if (isHtmlText) {
            val dialogTextView: TextView? = dialog.findViewById(android.R.id.message)
            dialogTextView?.apply {
                text = Html.fromHtml(message)
                movementMethod = LinkMovementMethod.getInstance()
            }
        }
    }

    private fun createDialogBuilder(title: String, message: String): AlertDialog.Builder {
        val builder = AlertDialog.Builder(context)
        builder.setTitle(title)
        builder.setMessage(message)
        builder.setNegativeButton(context.resources.getString(R.string.cancel)) { _, _ ->
            Log.d(Application.TAG, "User cancelled the dialog")
        }
        return builder
    }
}