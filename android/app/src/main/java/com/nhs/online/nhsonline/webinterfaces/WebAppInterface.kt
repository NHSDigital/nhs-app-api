package com.nhs.online.nhsonline.webinterfaces

import android.preference.PreferenceManager
import android.util.Log
import android.webkit.CookieManager
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.Application
import kotlinx.android.synthetic.main.activity_main.*


class WebAppInterface(private val context: MainActivity) {

    @JavascriptInterface
    fun onLogin() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLogin")
        context.runOnUiThread{context.loggedIn()}
    }

    @JavascriptInterface
    fun onLogout() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLogout")
        context.runOnUiThread{context.loggedOut()}
    }

    @JavascriptInterface
    fun updateHeaderText(text: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering updateHeaderText")
        context.runOnUiThread{context.setHeaderText(text)}
    }

    @JavascriptInterface
    fun clearMenuBarItem() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering clearMenuBarItem")
        context.runOnUiThread{context.clearMenuBarItem()}
    }

    @JavascriptInterface
    fun checkSymptoms() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering checkSymptoms")
        context.runOnUiThread{context.goToCheckSymptoms()}
    }

    @JavascriptInterface
    fun hideHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideHeader")
        context.runOnUiThread{context.hideHeader()}
    }

    @JavascriptInterface
    fun showHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showHeader")
        context.runOnUiThread{context.showHeader()}
    }

    @JavascriptInterface
    fun hideMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideMenuBar")
        context.hideMenuBar()
    }

    @JavascriptInterface
    fun resetPageFocus() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering resetPageFocus")
        context.runOnUiThread{context.resetFocusToNhsLogoForA11y()}
    }

    @JavascriptInterface
    fun hideWhiteScreen() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideWhiteScreen")
        context.runOnUiThread{context.hideBlankScreen()}
    }

    @JavascriptInterface
    fun goToBiometrics() {
        context.runOnUiThread{context.goToNativeBiometricPage()}
    }

    @JavascriptInterface
    fun completeAppIntro() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering completeAppIntro")

        val prefs = PreferenceManager.getDefaultSharedPreferences(context)
        val edit = prefs.edit()
        edit.putBoolean(context.getString(R.string.haveShownThrottlingCarouselBefore), java.lang.Boolean.TRUE)
        edit.apply()

        context.webview.post {
            context.webview.loadUrl(context.resources.getString(
                R.string.baseURL))
        }
    }

    @JavascriptInterface
    fun storeBetaCookie() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering storeBetaCookie")
        context.runOnUiThread {
            val cookies = CookieManager.getInstance().getCookie(context.resources.getString(R.string.cookieDomain))
            if (cookies != null) {
                val betaCookie = cookies.split("; ").first { it.startsWith("BetaCookie=") }
                val prefs = PreferenceManager.getDefaultSharedPreferences(context)
                prefs.edit().putString("BetaCookie", betaCookie).apply()
            }
        }
    }
}
