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
    fun updateHeaderText(text: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering updateHeaderText")
        activity.runOnUiThread {
            if (!isConnectedToNetwork) {
                nhsWeb.showConnectionError(null)
            } else {
                uiInteractor.setHeaderText(text)
            }
        }
    }

    @JavascriptInterface
    fun clearMenuBarItem() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering clearMenuBarItem")
        activity.runOnUiThread { uiInteractor.clearMenuBarItem() }
    }

    @JavascriptInterface
    fun checkSymptoms() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering checkSymptoms")
        activity.runOnUiThread { uiInteractor.goToCheckSymptoms() }
    }

    @JavascriptInterface
    fun hideHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideHeader")
        activity.runOnUiThread { uiInteractor.hideHeader() }
    }

    @JavascriptInterface
    fun showHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showHeader")
        activity.runOnUiThread { uiInteractor.showHeader() }
    }

    @JavascriptInterface
    fun hideMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideMenuBar")
        activity.runOnUiThread { uiInteractor.hideMenuBar() }
    }

    @JavascriptInterface
    fun setMenuBarItem(index: Int) {
        Log.d(Application.TAG, "${this::class.java.simpleName} Entering setMenuBarItem")
        activity.runOnUiThread { uiInteractor.setMenuBarItem(index) }
    }

    @JavascriptInterface
    fun resetPageFocus() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering resetPageFocus")
        activity.runOnUiThread { uiInteractor.resetFocusToNhsLogoForAccessibility() }
    }

    @JavascriptInterface
    fun hideWhiteScreen() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideWhiteScreen")
        activity.runOnUiThread { uiInteractor.hideBlankScreen() }
    }

    @JavascriptInterface
    fun goToLoginOptions() {
        activity.runOnUiThread { uiInteractor.showNativeBiometricOptions() }
    }

    @JavascriptInterface
    fun completeAppIntro() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering completeAppIntro")
        activity.runOnUiThread { nhsWeb.onThrottlingCarouselComplete() }
    }

    @JavascriptInterface
    fun storeBetaCookie() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering storeBetaCookie")
        activity.runOnUiThread { nhsWeb.onBetaCookieStoreRequest() }
    }

    @JavascriptInterface
    fun onSessionExpiring(sessionDuration: Int) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showExtendSessionDialogue")
        activity.runOnUiThread { uiInteractor.showExtendSessionDialogue(sessionDuration) }
    }

    @JavascriptInterface
    fun fetchNativeAppVersion(): String {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering fetchNativeAppVersion")
        return BuildConfig.VERSION_NAME
    }
}
