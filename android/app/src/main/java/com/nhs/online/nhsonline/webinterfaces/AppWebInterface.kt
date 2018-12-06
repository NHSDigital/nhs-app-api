package com.nhs.online.nhsonline.webinterfaces

import android.util.Log
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.MainActivity
import kotlinx.android.synthetic.main.activity_main.*

class AppWebInterface(private val context: MainActivity) {
    private val validateSessionString: String = "window.validateSession();"

    fun validateSession() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering validateSession")
        context.webview.evaluateJavascript(validateSessionString) {
            context.hideBlankScreen()
        }
    }

    fun isRecoveringFromDroppedConnection(path: String, baseUrl: String): Boolean {
        val reloadUrl = createReloadUrl(path, baseUrl)
        return (context.getReloadUrl() == reloadUrl)
    }

    private fun createReloadUrl(path: String, baseUrl: String): String {
        var reloadUrl = baseUrl + path
        if (path.indexOf(baseUrl) > -1) {
            reloadUrl = path
        }
        return reloadUrl
    }

    fun loadSpaPage(path: String, baseUrl: String) {
        var spaPath = generateSpaPathAndReloadUrl(path, baseUrl)
        context.evaluateWebviewJavascript("window.\$nuxt.\$router.push('$spaPath')")
    }

    fun resetGPFinderFlow() {
        var spaPath =
            generateSpaPathAndReloadUrl(context.resources.getString(R.string.gpFinderPath),
                context.resources.getString(R.string.baseURL))
        context.evaluateWebviewJavascript("window.\$nuxt.\$router.push({path:'$spaPath', query: { reset: true } })")
    }

    fun loadDispatchEvent(event: String) {
        context.evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('$event')")
    }

    fun logout() {
        loadDispatchEvent("auth/logout")
    }

    fun extendSession() {
        loadDispatchEvent("session/extend")
    }

    private fun generateSpaPathAndReloadUrl(path: String, baseUrl: String): String {
        var spaPath = path.replace(baseUrl, "/")

        if (!spaPath.startsWith("/")) {
            spaPath = "/$spaPath"
        }

        val reloadUrl = createReloadUrl(path, baseUrl)

        context.setReloadUrl(reloadUrl)

        return spaPath
    }

}