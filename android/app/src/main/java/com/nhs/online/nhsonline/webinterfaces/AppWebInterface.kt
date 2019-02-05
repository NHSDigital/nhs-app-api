package com.nhs.online.nhsonline.webinterfaces

import android.util.Log
import android.webkit.WebView

private val TAG = AppWebInterface::class.java.simpleName

class AppWebInterface(private val webview: WebView) {
    private val validateSessionString: String = "window.validateSession();"

    fun validateSession(callback: () -> Unit) {
        Log.d(TAG, "${this::class.java.simpleName}: Entering validateSession")
        webview.evaluateJavascript(validateSessionString) {
            callback.invoke()
        }
    }

    fun resetGPFinderFlow(gpFinderPath: String) {
        val spaPath = if (gpFinderPath.startsWith("/")) gpFinderPath else "/$gpFinderPath"
        evaluateWebviewJavascript("window.\$nuxt.\$router.push({path:'$spaPath', query: { reset: true } })")
    }

    fun loadDispatchEvent(event: String) {
        evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('$event')")
    }

    fun logout() {
        loadDispatchEvent("auth/logout")
    }

    fun extendSession() {
        loadDispatchEvent("session/extend")
    }

    private fun evaluateWebviewJavascript(javascriptText: String) {
        webview.evaluateJavascript(javascriptText, null)
    }
}