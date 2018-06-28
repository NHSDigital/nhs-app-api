package com.nhs.online.nhsonline.webinterfaces

import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.activities.MainActivity

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