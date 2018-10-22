package com.nhs.online.nhsonline.webinterfaces

import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.R
import kotlinx.android.synthetic.main.activity_main.*


class WebAppInterface(private val context: MainActivity) {

    @JavascriptInterface
    fun onLogin() {
        context.runOnUiThread{context.loggedIn()}
    }

    @JavascriptInterface
    fun onLogout() {
        context.runOnUiThread{context.loggedOut()}
    }

    @JavascriptInterface
    fun updateHeaderText(text: String) {
        context.runOnUiThread{context.setHeaderText(text)}
    }

    @JavascriptInterface
    fun clearMenuBarItem() {
        context.runOnUiThread{context.clearMenuBarItem()}
    }

    @JavascriptInterface
    fun checkSymptoms() {
        context.runOnUiThread{context.goToCheckSymptoms()}
    }

    @JavascriptInterface
    fun hideHeader() {
        context.runOnUiThread{context.hideHeader()}
    }

    @JavascriptInterface
    fun showHeader() {
        context.runOnUiThread{context.showHeader()}
    }

    @JavascriptInterface
    fun resetPageFocus() {
        context.runOnUiThread{context.resetFocusToNhsLogoForA11y()}
    }

    @JavascriptInterface
    fun hideWhiteScreen() {
        context.runOnUiThread{context.hideBlankScreen()}
    }

    @JavascriptInterface
    fun completeAppIntro() {
        context.webview.post {
            context.webview.loadUrl(context.resources.getString(
                R.string.baseURL))
        }
    }
}