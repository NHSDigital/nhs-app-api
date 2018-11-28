package com.nhs.online.nhsonline.services

import android.net.Uri
import android.webkit.URLUtil
import android.webkit.WebView
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface


class UrlLoader(
    var webView: WebView,
    var wc: WebClientInterceptor,
    var appWebInterface: AppWebInterface,
    var knownServices: KnownServices,
    val baseURL: String
) {

    var reloadUrl: String? = null
    var usingAbsoluteUri: Boolean = true

    private fun getValidUrl(url: String): String {
        var uriToUse = url
        var validUri = URLUtil.isValidUrl(uriToUse)

        if (!validUri) {
            uriToUse = baseURL + uriToUse
        }
        return uriToUse
    }

    fun loadUrl(pageEndPoint: String) {
        if (!wc.isConnectedToInternet()) {
            reloadUrl = getValidUrl(pageEndPoint)
            wc.stopLoadingWebviewAndShowNoConnectionError(webView)
            return
        }

        var uriToUse = getValidUrl(pageEndPoint)

        if (usingAbsoluteUri || knownServices.isCIDRedirectUrl(uriToUse)) {
            loadPage(uriToUse)
        } else if (webView.url != null && (webView.url.contains(baseURL))) {
            if (appWebInterface.isRecoveringFromDroppedConnection(pageEndPoint, baseURL)) {
                loadPage(uriToUse)
            } else {
                appWebInterface.loadSpaPage(pageEndPoint, baseURL)
            }
        } else {
            if (URLUtil.isValidUrl(uriToUse)) {
                loadExternalPage(uriToUse)
            } else {
                loadPage(uriToUse)
            }
        }
    }

    fun loadExternalPage(pageUrl: String) {
        val builtUri = Uri.parse(pageUrl)
            .buildUpon()
            .build()

        val fullUrl = builtUri.toString()
        loadPage(fullUrl)
    }

    fun loadPage(url: String) {
        val urlWithMissingQueryStrings =
            knownServices.findKnownServiceAndAddMissingQueryFor(url)

        webView.loadUrl(urlWithMissingQueryStrings)
    }

    fun reloadRequest() {
        if (reloadUrl != null) {
            webView.loadUrl(reloadUrl)
        } else {
            webView.reload()
        }
    }
}