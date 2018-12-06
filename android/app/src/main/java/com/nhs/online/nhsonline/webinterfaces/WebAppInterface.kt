package com.nhs.online.nhsonline.webinterfaces

import android.preference.PreferenceManager
import android.util.Log
import android.webkit.CookieManager
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.BuildConfig
import kotlinx.android.synthetic.main.activity_main.*


class WebAppInterface(private val context: MainActivity) {

    @JavascriptInterface
    fun onLogin() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLogin")
        context.runOnUiThread { context.loggedIn() }
    }

    @JavascriptInterface
    fun onLogout() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLogout")
        context.runOnUiThread {
            context.showWebviewScreen()
            context.loggedOut()
        }
    }

    @JavascriptInterface
    fun updateHeaderText(text: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering updateHeaderText")
        context.runOnUiThread { context.setHeaderText(text) }
    }

    @JavascriptInterface
    fun clearMenuBarItem() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering clearMenuBarItem")
        context.runOnUiThread { context.clearMenuBarItem() }
    }

    @JavascriptInterface
    fun checkSymptoms() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering checkSymptoms")
        context.runOnUiThread { context.goToCheckSymptoms() }
    }

    @JavascriptInterface
    fun hideHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideHeader")
        context.runOnUiThread { context.hideHeader() }
    }

    @JavascriptInterface
    fun showHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showHeader")
        context.runOnUiThread { context.showHeader() }
    }

    @JavascriptInterface
    fun hideMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideMenuBar")
        context.runOnUiThread{context.hideMenuBar()}
    }

    @JavascriptInterface
    fun setMenuBarItem(index: Int){
        Log.d(Application.TAG, "${this::class.java.simpleName} Entering setMenuBarItem")
        context.runOnUiThread{context.setMenuBarItem(index)}
    }

    @JavascriptInterface
    fun resetPageFocus() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering resetPageFocus")
        context.runOnUiThread { context.resetFocusToNhsLogoForA11y() }
    }

    @JavascriptInterface
    fun hideWhiteScreen() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideWhiteScreen")
        context.runOnUiThread { context.hideBlankScreen() }
    }

    @JavascriptInterface
    fun goToLoginOptions() {
        context.runOnUiThread { context.goToNativeBiometricPage() }
    }

    @JavascriptInterface
    fun completeAppIntro() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering completeAppIntro")

        val prefs = PreferenceManager.getDefaultSharedPreferences(context)
        val edit = prefs.edit()
        edit.putBoolean(context.getString(R.string.haveShownThrottlingCarouselBefore),
            java.lang.Boolean.TRUE)
        edit.apply()

        context.runOnUiThread {
            context.webview.settings.textZoom = context.originalWebviewZoom
        }

        context.webview.post {
            context.webview.loadUrl(context.resources.getString(
                R.string.baseURL))
        }
    }

    @JavascriptInterface
    fun storeBetaCookie() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering storeBetaCookie")
        context.runOnUiThread {
            val cookies: String? = CookieManager.getInstance()
                .getCookie(context.resources.getString(R.string.cookieDomain))
                ?.takeIf { it.contains("BetaCookie=") }
            if (cookies != null) {
                val betaCookie = cookies.split("; ").first { it.startsWith("BetaCookie=") }
                val prefs = PreferenceManager.getDefaultSharedPreferences(context)
                prefs.edit().putString("BetaCookie", betaCookie).apply()
            }
        }
    }

    @JavascriptInterface
    fun onSessionExpiring(sessionDuration: Int) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showExtendSessionDialogue")
        context.runOnUiThread { context.showExtendSessionDialogue(sessionDuration) }
    }

    @JavascriptInterface
    fun fetchNativeAppVersion() : String  {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering fetchNativeAppVersion")
        return BuildConfig.VERSION_NAME
    }
}
