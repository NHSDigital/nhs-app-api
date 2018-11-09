package com.nhs.online.nhsonline.webinterfaces

import android.util.Log
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.BuildConfig
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

    fun isRecoveringFromDroppedConnection(path: String, baseUrl: String) : Boolean {
        var reloadUrl = createReloadUrl(path, baseUrl)
        return (context.getReloadUrl() == reloadUrl)
    }

    private fun createReloadUrl(path: String, baseUrl: String) : String {
        var reloadUrl = baseUrl + path
        if (path.indexOf(baseUrl) > -1){
            reloadUrl = path
        }
        return reloadUrl
    }

    fun loadSpaPage(path: String, baseUrl: String) {
        var spaPath = path.replace(baseUrl, "/")

        if(!spaPath.startsWith("/")) {
            spaPath = "/$spaPath"
        }

        var reloadUrl = createReloadUrl(path, baseUrl)

        context.setReloadUrl("$reloadUrl")
        context.evaluateWebviewJavascript("window.\$nuxt.\$router.push('$spaPath')")
    }

    fun loadDispatchEvent(event: String) {
        context.evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('$event')")
    }
}