package com.nhs.online.nhsonline.services

import android.net.Uri
import android.webkit.URLUtil
import android.webkit.WebView
import com.nhs.online.nhsonline.webinterfaces.WebJavascript


class UrlLoader(
    private val webView: WebView,
    private val knownServices: KnownServices,
    private val baseURL: String
) {
    private val webJavascript = WebJavascript(webView)
    var reloadUrl: String? = null
    var usingAbsoluteUri: Boolean = true

    fun loadUrl(pageEndPoint: String) {
        val uriToUse = produceValidUrl(pageEndPoint)

        if (usingAbsoluteUri || knownServices.isCIDRedirectUrl(uriToUse)) {
            loadPage(uriToUse)
        } else if (webView.url != null && (webView.url.contains(baseURL))) {
            val url = produceValidUrl(pageEndPoint)
            if (url == reloadUrl) {
                loadPage(uriToUse)
            } else {
                reloadUrl = url
                val path = getPath(pageEndPoint)
                webJavascript.loadSpaPath(path)
            }
        } else {
            if (URLUtil.isValidUrl(uriToUse)) {
                loadExternalPage(uriToUse)
            } else {
                loadPage(uriToUse)
            }
        }
    }

    fun reloadRequest() {
        if (reloadUrl != null) {
            webView.loadUrl(reloadUrl)
        } else {
            webView.reload()
        }
    }

    fun produceValidUrl(endPoint: String): String {
        val validUri = URLUtil.isValidUrl(endPoint)
        return if (validUri) endPoint else baseURL + endPoint
    }

    private fun loadExternalPage(pageUrl: String) {
        val builtUri = Uri.parse(pageUrl)

        val fullUrl = builtUri.toString()
        webView.loadUrl(fullUrl)
    }

    private fun loadPage(url: String) {
        val urlWithMissingQueryStrings =
            knownServices.findKnownServiceAndAddMissingQueryFor(url)

        webView.loadUrl(urlWithMissingQueryStrings)
    }

    private fun getPath(endPoint: String): String {
        var spaPath = endPoint.replace(baseURL, "/")

        if (!spaPath.startsWith("/")) {
            spaPath = "/$spaPath"
        }
        return spaPath
    }
}