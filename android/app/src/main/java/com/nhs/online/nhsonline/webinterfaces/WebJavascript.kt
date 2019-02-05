package com.nhs.online.nhsonline.webinterfaces

import android.webkit.WebView


class WebJavascript(private val webView: WebView) {
    fun loadSpaPath(path: String) {
        webView.evaluateJavascript("window.\$nuxt.\$router.push('$path')", null)
    }
}