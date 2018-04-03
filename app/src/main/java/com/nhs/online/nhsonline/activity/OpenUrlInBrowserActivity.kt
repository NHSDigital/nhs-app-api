package com.nhs.online.nhsonline.activity

import android.content.Context
import android.content.Intent
import android.net.Uri
import android.support.v4.content.ContextCompat
import java.net.URL

class OpenUrlInBrowserActivity(val nativeAppHosts: Array<String>) : ActivityInterface
{
    override fun canStart(context: Context, url: String): Boolean
    {
        val currentHost = URL(url).host
        nativeAppHosts.forEach { nativeAppHost ->
            if (URL(nativeAppHost).host == currentHost) {
                return false
            }
        }

        return true
    }

    override fun start(context: Context, url: String)
    {
        if (!canStart(context, url)) {
            throw RuntimeException("Cannot open url in browser")
        }

        val intent = Intent(Intent.ACTION_VIEW, Uri.parse(url))
        ContextCompat.startActivity(context, intent, null)
    }
}
