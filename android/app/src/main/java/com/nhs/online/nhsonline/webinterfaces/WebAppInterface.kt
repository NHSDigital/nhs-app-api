package com.nhs.online.nhsonline.webinterfaces

import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.activities.MainActivity
import android.widget.Toast
import com.nhs.online.nhsonline.R
import android.webkit.WebView
import kotlinx.android.synthetic.main.activity_main.*


class WebAppInterface(private val context: MainActivity) {

    @JavascriptInterface
    fun onLogin() {
        context.loggedIn()
    }

    @JavascriptInterface
    fun onLogout() {
        context.loggedOut()
    }

    @JavascriptInterface
    fun updateHeaderText(text: String) {
        context.setHeaderText(text)
    }

    @JavascriptInterface
    fun clearMenuBarItem() {
        context.clearMenuBarItem()
    }

    @JavascriptInterface
    fun checkSymptoms() {
        context.goToCheckSymptoms()
    }

    @JavascriptInterface
    fun hideHeader() {
        context.hideHeader()
    }

    @JavascriptInterface
    fun showHeader() {
        context.showHeader()
    }

    @JavascriptInterface
    fun hideWhiteScreen() {
        context.hideBlankScreen()
    }

    @JavascriptInterface
    fun completeAppIntro() {
        context.webview.post(Runnable {
            context.webview.loadUrl(context.resources.getString(
                    R.string.baseURL))
        })
    }
}