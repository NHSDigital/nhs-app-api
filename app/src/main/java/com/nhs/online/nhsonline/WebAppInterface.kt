package com.nhs.online.nhsonline

import android.webkit.JavascriptInterface

class WebAppInterface(private val context: MainActivity) {

    @JavascriptInterface
    fun onLogin() {
        context.showMenuBar()
        context.showHeader()
    }

    @JavascriptInterface
    fun onLogout() {
        context.hideMenuBar()
        context.hideHeader()
    }

    @JavascriptInterface
    fun updateHeaderText(text:String) {
        context.setHeaderText(text)
    }
}