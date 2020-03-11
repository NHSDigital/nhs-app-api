package com.nhs.online.nhsonline.support.schemehandlers

import android.content.Context
import android.content.Intent
import android.net.ParseException
import android.net.Uri
import android.util.Log
import com.nhs.online.nhsonline.Application

class TelSchemeHandler(private val context: Context) : ISchemeHandler {
    override val scheme = "tel:"
    override fun handle(url: String): Boolean {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering handle")
        try {
            val intent = Intent(Intent.ACTION_DIAL).apply {
                data = Uri.parse(url)
            }
            if (intent.resolveActivity(context.packageManager) != null) {
                context.startActivity(intent)
                return true
            }
        } catch (e: ParseException) {
            return false
        }
        return false
    }
}

