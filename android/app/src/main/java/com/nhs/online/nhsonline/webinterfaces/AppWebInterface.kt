package com.nhs.online.nhsonline.webinterfaces

import com.nhs.online.nhsonline.activities.MainActivity
import kotlinx.android.synthetic.main.activity_main.*

class AppWebInterface(private val context: MainActivity) {
    private val validateSessionString: String = "window.validateSession();"

    fun validateSession() {
        context.webview.evaluateJavascript(validateSessionString, null)
    }
}