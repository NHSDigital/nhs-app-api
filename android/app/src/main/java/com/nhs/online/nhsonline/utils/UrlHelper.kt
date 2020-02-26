package com.nhs.online.nhsonline.utils

import android.content.Context
import android.webkit.URLUtil
import com.nhs.online.nhsonline.R
import java.net.URL

class UrlHelper(val context: Context) {
    private val defaultScheme = "https"
    private val protocolRegex = Regex("^(\\w+)://")
    private val appScheme = context.getString(R.string.appScheme)
    private val appSchemePattern = "^$appScheme://".toRegex()
    private val baseScheme = context.getString(R.string.baseScheme)

    fun ensureUrlWithScheme(url: String?): URL? {
        if (url.isNullOrEmpty()) {
            return null
        }

        val finalUrl = if (!protocolRegex.containsMatchIn(url)) {
            "$defaultScheme://$url"
        } else {
            url.replace(appSchemePattern, "$baseScheme://")
        }

        if (!URLUtil.isValidUrl(finalUrl)) {
            return null
        }

        return URL(finalUrl)
    }
}