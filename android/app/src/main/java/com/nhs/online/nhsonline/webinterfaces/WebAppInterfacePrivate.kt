package com.nhs.online.nhsonline.webinterfaces

import android.app.Activity
import android.util.Log
import android.webkit.JavascriptInterface
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.BuildConfig
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.services.SettingsService
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb

class WebAppInterfacePrivate(
    private val activity: Activity,
    private val nhsWeb: NhsWeb,
    private val uiInteractor: IInteractor,
    private val settingsService: SettingsService
) {
    @JavascriptInterface
    fun attemptBiometricLogin() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering attemptBiometricLogin")
        runAction { uiInteractor.showBiometricLoginIfEnabled() }
    }

    @JavascriptInterface
    fun clearMenuBarItem() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering clearMenuBarItem")
        runAction { uiInteractor.clearMenuBarItem() }
    }

    @JavascriptInterface
    fun configureWebContext(helpUrl: String, retryPath: String) {
        runAction {
            uiInteractor.setHelpUrl(helpUrl)
            uiInteractor.setRetryPath(retryPath)
        }
    }

    @JavascriptInterface
    fun dismissProgressBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering dismissProgressBar")
        runAction { uiInteractor.dismissProgressDialog() }
    }

    @JavascriptInterface
    fun fetchNativeAppVersion(): String {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering fetchNativeAppVersion")
        return BuildConfig.VERSION_NAME
    }

    @JavascriptInterface
    fun fetchBiometricSpec() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering fetchBiometricSpec")
        runAction { uiInteractor.fetchBiometricSpec() }
    }

    @JavascriptInterface
    fun getNotificationsStatus() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering getNotificationsStatus")
        runAction { nhsWeb.getNotificationsStatus() }
    }

    @JavascriptInterface
    fun hideHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideHeader")
        runAction { uiInteractor.hideHeader() }
    }

    @JavascriptInterface
    fun hideHeaderSlim() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideHeaderSlim")
        runAction { uiInteractor.hideHeaderSlim() }
    }

    @JavascriptInterface
    fun hideMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideMenuBar")
        runAction { uiInteractor.hideMenuBar() }
    }

    @JavascriptInterface
    fun hideWhiteScreen() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideWhiteScreen")
        runAction { uiInteractor.hideBlankScreen() }
    }

    @JavascriptInterface
    fun onLogin() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLogin")
        runAction { nhsWeb.onWebLoggedIn() }
    }

    @JavascriptInterface
    fun onLogout() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLogout")
        runAction { nhsWeb.onWebLoggedOut() }
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
        runAction { uiInteractor.showExtendSessionDialogue() }
    }

    @JavascriptInterface
    fun pageLoadComplete() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering pageLoadComplete")
        nhsWeb.applicationState.unBlock()
    }

    @JavascriptInterface
    fun requestPnsToken(trigger: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering requestPnsToken")
        runAction { nhsWeb.requestPnsToken(trigger) }
    }

    @JavascriptInterface
    fun resetPageFocus() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering resetPageFocus")
        runAction { uiInteractor.resetFocusToNhsLogoForAccessibility() }
    }

    @JavascriptInterface
    fun setHelpUrl(url: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName} Entering setHelpUrl")
        runAction { uiInteractor.setHelpUrl(url) }
    }

    @JavascriptInterface
    fun setMenuBarItem(index: Int) {
        Log.d(Application.TAG, "${this::class.java.simpleName} Entering setMenuBarItem")
        runAction { uiInteractor.setMenuBarItem(index) }
    }

    @JavascriptInterface
    fun setZoomable(canZoom: Boolean) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering setZoomable")
        runAction{ uiInteractor.setZoomable(canZoom) }
    }

    @JavascriptInterface
    fun showHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showHeader")
        runAction { uiInteractor.showHeader() }
    }

    @JavascriptInterface
    fun showHeaderSlim() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showHeader")
        runAction { uiInteractor.showHeaderSlim() }
    }

    @JavascriptInterface
    fun showMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showMenuBar")
        runAction { uiInteractor.showMenuBar() }
    }

    @JavascriptInterface
    fun startDownload(base64Data: String, fileName: String, mimeType: String) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering startDownload")
        runAction{ uiInteractor.startDownload(base64Data, fileName, mimeType) }
    }

    @JavascriptInterface
    fun updateBiometricRegistration() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering updateBiometricRegistration")
        runAction { uiInteractor.updateBiometricRegistration() }
    }

    private fun runAction(action: () -> Unit){
        if(nhsWeb.javaScriptInteractionMode == JavaScriptInteractionMode.NhsApp){
            activity.runOnUiThread(action)
        }
    }
}
