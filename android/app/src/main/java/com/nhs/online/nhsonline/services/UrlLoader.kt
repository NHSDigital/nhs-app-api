package com.nhs.online.nhsonline.services

import android.net.Uri
import android.webkit.URLUtil
import android.webkit.WebView
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface

class UrlLoader (
        var webView: WebView,
        var appWebInterface: AppWebInterface,
        var knownServices: KnownServices,
        val baseURL:String
) {

    var reloadUrl: String? = null
    var usingAbsoluteUri: Boolean = true


    fun loadUrl(pageEndPoint: String) {

        var uriToUse = pageEndPoint
        var validUri = URLUtil.isValidUrl(uriToUse)

        if(!validUri) {
            uriToUse = baseURL + uriToUse
        }

        if (usingAbsoluteUri) {
            loadPage(uriToUse)
        }
        else if (webView.url != null && (webView.url.contains(baseURL))) {
            appWebInterface.loadSpaPage(pageEndPoint, baseURL)
        }
        else {
            if(validUri) {
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
                knownServices.findKnownServiceAddMissingQueryFor(url)

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