package com.nhs.online.nhsonline.webinterfaces

import android.util.Log
import android.webkit.WebView

private val TAG = AppWebInterface::class.java.simpleName

class AppWebInterface(private val webView: WebView) {
    private val validateSessionString: String = "window.validateSession();"

    fun validateSession(callback: () -> Unit) {
        Log.d(TAG, "${this::class.java.simpleName}: Entering validateSession")
        webView.evaluateJavascript(validateSessionString) {
            callback.invoke()
        }
    }

    fun biometricCompletion(action: String, outcome: String, errorCode: String) {
        val response = """
            {
                action: '$action',
                outcome: '$outcome',
                errorCode: '$errorCode'
            }
        """
        loadDispatchEvent("loginSettings/biometricCompletion", response)
    }

    fun biometricSpec(biometricTypeReference: String, enabled: Boolean) {
        val response = """
            {
                biometricTypeReference: '$biometricTypeReference',
                enabled: $enabled
            }
        """
        loadDispatchEvent("loginSettings/biometricSpec", response)
    }

    fun biometricLoginFailure() {
        loadDispatchEvent("login/handleBiometricLoginFailure")
    }

    fun leavePage() {
        loadDispatchEvent("pageLeaveWarning/leavePage")
    }

    fun logout() {
        loadDispatchEvent("auth/logout")
    }

    fun extendSession() {
        loadDispatchEvent("session/extend")
    }

    fun getNotificationsStatus(status: String) {
        loadDispatchEvent("notifications/settingsStatus", "'$status'")
    }

    fun notificationsAuthorised(devicePns: String, trigger: String) {
        val response = """
            {
                devicePns: '$devicePns',
                deviceType: 'android',
                trigger: '$trigger'
            }
        """
        loadDispatchEvent("notifications/authorised", response)
    }

    fun notificationsUnauthorised() {
        loadDispatchEvent("notifications/unauthorised")
    }

    fun stayOnPage() {
        loadDispatchEvent("pageLeaveWarning/stayOnPage")
    }

    fun goTo(path: String) {
        loadDispatchEvent("navigation/goTo", "'$path'")
    }

    private fun loadDispatchEvent(event: String, args: String = "") {
        evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('$event'" +
                "${ if (args != "") ", $args" else "" })")
    }

    private fun evaluateWebviewJavascript(javascriptText: String) {
        webView.evaluateJavascript(javascriptText, null)
    }
}