package com.nhs.online.nhsonline

import android.webkit.JavascriptInterface
import kotlinx.android.synthetic.main.activity_main.*

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

    @JavascriptInterface
    fun clearMenuBarItem() {
        context.clearMenuBarItem()
    }
}