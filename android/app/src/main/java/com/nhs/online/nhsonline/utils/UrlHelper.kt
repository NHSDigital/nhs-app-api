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
    private val redirector =
            context.getString(R.string.baseURL).plus(context.getString(R.string.redirectorPath))

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
    fun getPostRequestReloadUrl(url: String): String? {
        return when {
            url.startsWith((fetchStringResource(R.string.dataPreferencesBaseUrl))) -> fetchStringResource(
                    R.string.dataSharingURL)
            else -> null
        }
    }

    fun isSameHostAndSchemeAsHomeUrl(urlString: String?): Boolean {
        if (urlString.isNullOrBlank())
            return false
        val homeUrl = URL(fetchStringResource(R.string.baseURL))
        val url = URL(urlString)
        return homeUrl.host == url.host && homeUrl.protocol == url.protocol
    }

    private fun fetchStringResource(resourceId: Int): String {
        return context.resources.getString(resourceId)
    }

    fun ensureSchemeAndBuildRedirectorUrl(url: String?) : URL? {
        if (ensureUrlWithScheme(url).toString() == context.getString(R.string.baseURL)) {
           return ensureUrlWithScheme(url)
        }
        return URL(
                redirector.plus(ensureUrlWithScheme(url).toString()))
    }
}