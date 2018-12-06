package com.nhs.online.nhsonline.services

import android.net.Uri
import android.webkit.URLUtil
import android.webkit.WebView
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface


class UrlLoader(
    private val webView: WebView,
    private val wc: WebClientInterceptor,
    private val appWebInterface: AppWebInterface,
    private val knownServices: KnownServices,
    val baseURL: String
) {

    var reloadUrl: String? = null
    var usingAbsoluteUri: Boolean = true

    private fun getValidUrl(url: String): String {
        val validUri = URLUtil.isValidUrl(url)

        return if (!validUri) baseURL + url else {
            url
        }
    }

    fun loadUrl(pageEndPoint: String) {
        if (!wc.isConnectedToInternet()) {
            reloadUrl = getValidUrl(pageEndPoint)
            wc.stopLoadingWebviewAndShowNoConnectionError(webView)
            return
        }

        val uriToUse = getValidUrl(pageEndPoint)

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

    private fun loadExternalPage(pageUrl: String) {
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