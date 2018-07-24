package com.nhs.online.nhsonline.webinterfaces

import android.view.View
import android.webkit.ValueCallback
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.MainActivity
import kotlinx.android.synthetic.main.activity_main.*

class AppWebInterface(private val context: MainActivity) {
    private val validateSessionString: String = "window.validateSession();"

    fun validateSession() {
        context.webview.evaluateJavascript(validateSessionString) {
            hideBlankScreen()
        }
    }

    private fun hideBlankScreen() {
        context.viewSwitcher.visibility = View.VISIBLE
        context.blankScreen.visibility = View.GONE
    }
}
