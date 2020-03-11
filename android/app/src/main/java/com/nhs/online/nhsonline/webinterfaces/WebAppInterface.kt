package com.nhs.online.nhsonline.webinterfaces

import android.app.Activity
import android.util.Log
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.BuildConfig
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhs.online.nhsonline.services.SettingsService
import com.nhs.online.nhsonline.web.NhsWeb

class WebAppInterface(
    private val activity: Activity,
    private val uiInteractor: IInteractor,
    private val nhsWeb: NhsWeb,
    private val settingsService: SettingsService
) {
    @JavascriptInterface
    fun attemptBiometricLogin() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering attemptBiometricLogin")
        activity.runOnUiThread { uiInteractor.showBiometricLoginIfEnabled() }
    }

    @JavascriptInterface
    fun clearMenuBarItem() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering clearMenuBarItem")
        activity.runOnUiThread { uiInteractor.clearMenuBarItem() }
    }

    @JavascriptInterface
    fun configureWebContext(helpUrl: String, retryPath: String) {
        activity.runOnUiThread {
            uiInteractor.setHelpUrl(helpUrl)
            uiInteractor.setRetryPath(retryPath)
        }
    }

    @JavascriptInterface
    fun dismissProgressBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering dismissProgressBar")
        activity.runOnUiThread { uiInteractor.dismissProgressDialog() }
    }

    @JavascriptInterface
    fun fetchNativeAppVersion(): String {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering fetchNativeAppVersion")
        return BuildConfig.VERSION_NAME
    }

    @JavascriptInterface
    fun getNotificationsStatus() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering getNotificationsStatus")
        activity.runOnUiThread { nhsWeb.getNotificationsStatus() }
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
    fun openAppSettings() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering openAppSettings")
        settingsService.openSettings()
    }

    @JavascriptInterface
    @Deprecated(message = "since 1.23.0 (NHSO-5818), here for backwards compatibility",
            replaceWith = ReplaceWith(expression = "onSessionExpiring()"))
    fun onSessionExpiring(sessionDuration: Int) {
        onSessionExpiring()
    }

    @JavascriptInterface
    fun onSessionExpiring() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onSessionExpiring")
        activity.runOnUiThread { uiInteractor.showExtendSessionDialogue() }
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
    fun setHelpUrl(url: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName} Entering setHelpUrl")
        activity.runOnUiThread { uiInteractor.setHelpUrl(url) }
    }

    @JavascriptInterface
    fun setMenuBarItem(index: Int) {
        Log.d(Application.TAG, "${this::class.java.simpleName} Entering setMenuBarItem")
        activity.runOnUiThread { uiInteractor.setMenuBarItem(index) }
    }

    @JavascriptInterface
    fun setZoomable(canZoom: Boolean) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering setZoomable")
        activity.runOnUiThread{ uiInteractor.setZoomable(canZoom) }
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
    fun showMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showMenuBar")
        activity.runOnUiThread { uiInteractor.showMenuBar() }
    }

    @JavascriptInterface
    fun startDownload(base64Data: String, fileName: String, mimeType: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering startDownload")
        activity.runOnUiThread{ uiInteractor.startDownload(base64Data, fileName, mimeType) }
    }
}
