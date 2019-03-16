package com.nhs.online.nhsonline.support.schemehandlers

import android.content.Context
import android.content.Intent
import android.net.MailTo
import android.net.MailTo.MAILTO_SCHEME
import android.net.ParseException
import android.net.Uri
import android.util.Log
import com.nhs.online.nhsonline.Application

class MailToSchemeHandler(private val context: Context) : ISchemeHandler
{
    override val scheme = MAILTO_SCHEME

    override fun handle(url: String): Boolean {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering handle")

        try {
            val mt = MailTo.parse(url)

            val intent = Intent(Intent.ACTION_SENDTO).apply {
                data = Uri.parse(this@MailToSchemeHandler.scheme)
                putExtra(Intent.EXTRA_EMAIL, arrayOf(mt.to))
                putExtra(Intent.EXTRA_TEXT, mt.body)
                putExtra(Intent.EXTRA_SUBJECT, mt.subject)
                putExtra(Intent.EXTRA_CC, mt.cc)
            }

            if (intent.resolveActivity(context.packageManager) != null) {
                context.startActivity(intent)
                return true
            }
        } catch (e: ParseException) {
            return false
        } catch (e: NullPointerException) {
            return false
        }

        return false
    }
}