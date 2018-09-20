package com.nhs.online.nhsonline.webinterfaces

import com.nhs.online.nhsonline.activities.MainActivity
import kotlinx.android.synthetic.main.activity_main.*

class AppWebInterface(private val context: MainActivity) {
    private val validateSessionString: String = "window.validateSession();"

    fun validateSession() {
        context.webview.evaluateJavascript(validateSessionString) {
            context.hideBlankScreen()
        }
    }

    fun loadSpaPage(path: String, baseUrl: String) {
        var spaPath = path.replace(baseUrl, "/")

        if(!spaPath.startsWith("/")) {
            spaPath = "/$spaPath"
        }
        context.setReloadUrl("$baseUrl$path")
        context.evaluateWebviewJavascript("window.\$nuxt.\$router.push('$spaPath')")
    }

    fun loadDispatchEvent(event: String) {
        context.evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('$event')")
    }
}
