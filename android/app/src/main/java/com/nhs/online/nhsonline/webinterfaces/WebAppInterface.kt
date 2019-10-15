package com.nhs.online.nhsonline.webinterfaces

import android.app.Activity
import android.util.Log
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.BuildConfig
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhs.online.nhsonline.web.NhsWeb

class WebAppInterface(
    private val activity: Activity,
    private val uiInteractor: IInteractor,
    private val nhsWeb: NhsWeb
) {

    @JavascriptInterface
    fun areNotificationsEnabled() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering areNotificationsEnabled")
        activity.runOnUiThread { nhsWeb.areNotificationsEnabled() }
    }

    @JavascriptInterface
    fun clearMenuBarItem() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering clearMenuBarItem")
        activity.runOnUiThread { uiInteractor.clearMenuBarItem() }
    }

    @JavascriptInterface
    fun fetchNativeAppVersion(): String {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering fetchNativeAppVersion")
        return BuildConfig.VERSION_NAME
    }

    @JavascriptInterface
    fun goToLoginOptions() {
        activity.runOnUiThread { uiInteractor.showNativeBiometricOptions() }
    }

    @JavascriptInterface
    fun hideHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideHeader")
        activity.runOnUiThread { uiInteractor.hideHeader() }
    }

    @JavascriptInterface
    fun hideHeaderSlim() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideHeaderSlim")
        activity.runOnUiThread { uiInteractor.hideHeaderSlim() }
    }

    @JavascriptInterface
    fun hideMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideMenuBar")
        activity.runOnUiThread { uiInteractor.hideMenuBar() }
    }

    @JavascriptInterface
    fun hideWhiteScreen() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideWhiteScreen")
        activity.runOnUiThread { uiInteractor.hideBlankScreen() }
    }

    @JavascriptInterface
    fun onLogin() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLogin")
        activity.runOnUiThread { nhsWeb.onWebLoggedIn() }
    }

    @JavascriptInterface
    fun onLogout() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLogout")
        activity.runOnUiThread {
            nhsWeb.onWebLoggedOut()
        }
    }

    @JavascriptInterface
    fun showMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showMenuBar")
        activity.runOnUiThread { uiInteractor.showMenuBar() }
    }

    @JavascriptInterface
    fun onSessionExpiring(sessionDuration: Int) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showExtendSessionDialogue")
        activity.runOnUiThread { uiInteractor.showExtendSessionDialogue(sessionDuration) }
    }

    @JavascriptInterface
    fun pageLoadComplete() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering pageLoadComplete")
        nhsWeb.applicationState.unBlock()
    }

    @JavascriptInterface
    fun requestPnsToken(trigger: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering requestPnsToken")
        activity.runOnUiThread { nhsWeb.requestPnsToken(trigger) }
    }

    @JavascriptInterface
    fun resetPageFocus() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering resetPageFocus")
        activity.runOnUiThread { uiInteractor.resetFocusToNhsLogoForAccessibility() }
    }

    @JavascriptInterface
    fun setMenuBarItem(index: Int) {
        Log.d(Application.TAG, "${this::class.java.simpleName} Entering setMenuBarItem")
        activity.runOnUiThread { uiInteractor.setMenuBarItem(index) }
    }

    @JavascriptInterface
    fun setHelpUrl(url: String) {
        activity.runOnUiThread { uiInteractor.setHelpUrl(url) }
    }

    @JavascriptInterface
    fun showHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showHeader")
        activity.runOnUiThread { uiInteractor.showHeader() }
    }

    @JavascriptInterface
    fun showHeaderSlim() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showHeader")
        activity.runOnUiThread { uiInteractor.showHeaderSlim() }
    }

    @JavascriptInterface
    fun storeBetaCookie() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering storeBetaCookie")
        activity.runOnUiThread { nhsWeb.onBetaCookieStoreRequest() }
    }

    @JavascriptInterface
    fun updateHeaderText(text: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering updateHeaderText")
        activity.runOnUiThread {
            if (!isConnectedToNetwork) {
                nhsWeb.showNoConnectionError()
            } else {
                uiInteractor.setHeaderText(text)
            }
        }
    }
}
