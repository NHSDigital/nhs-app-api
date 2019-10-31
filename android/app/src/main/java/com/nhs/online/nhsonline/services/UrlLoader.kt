package com.nhs.online.nhsonline.services

import android.webkit.URLUtil
import android.webkit.WebView
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface

class UrlLoader(
    private val webView: WebView,
    private val knownServices: KnownServices,
    private val baseURL: String,
    private val appWebInterface: AppWebInterface
) {
    fun loadUrl(url: String, requiresFullPageLoad: Boolean) {
        if (!requiresFullPageLoad && isNavigatingFromNhsAppPageToAnotherNhsAppPage(webView.url, url)) {
            loadUrlThroughSpa(url)
        } else {
            hardLoadUrl(url)
        }
    }

    fun reloadRequest(reloadUrl: String?) {
        if (reloadUrl != null) {
            hardLoadUrl(produceValidUrl(reloadUrl))
        } else {
            webView.reload()
        }
    }

    fun produceValidUrl(endPoint: String): String {
        val validUri = URLUtil.isValidUrl(endPoint)
        return if (validUri) endPoint else baseURL + endPoint
    }

    private fun loadUrlThroughSpa(url: String) {
        val path = getPath(url)
        appWebInterface.goTo(path)
    }

    private fun hardLoadUrl(url: String) {
        val urlWithMissingQueryStrings = knownServices.findKnownServiceAndAddMissingQueryFor(url)
        webView.loadUrl(urlWithMissingQueryStrings)
    }


    private fun getPath(endPoint: String): String {
        var spaPath = endPoint.replace(baseURL, "/")

        if (!spaPath.startsWith("/")) {
            spaPath = "/$spaPath"
        }
        return spaPath
    }

    private fun isNavigatingFromNhsAppPageToAnotherNhsAppPage(fromUrl: String?, toUrl: String): Boolean {
        return fromUrl?.contains(baseURL) == true && toUrl.contains(baseURL)
    }
}
