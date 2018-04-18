package com.nhs.online.nhsonline

import android.webkit.JavascriptInterface

class WebAppInterface(private val context: MainActivity) {

    @JavascriptInterface
    fun loggedIn() {
        context.showMenuBar()
        context.showHeader()
    }

    @JavascriptInterface
    fun updateHeaderText(text:String) {
        context.setHeaderText(text)
    }
}