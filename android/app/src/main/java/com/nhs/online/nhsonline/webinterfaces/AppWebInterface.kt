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
        dispatchNativeAppCallback("loginSettingsBiometricCompletion", response)
    }

    fun biometricSpec(biometricTypeReference: String, enabled: Boolean) {
        val response = """
            {
                biometricTypeReference: '$biometricTypeReference',
                enabled: $enabled
            }
        """
        dispatchNativeAppCallback("loginSettingsBiometricSpec", response)
    }

    fun biometricLoginFailure() {
        dispatchNativeAppCallback("loginHandleBiometricLoginFailure")
    }

    fun leavePage() {
        dispatchNativeAppCallback("pageLeaveWarningLeavePage")
    }

    fun logout() {
        dispatchNativeAppCallback("authLogout")
    }

    fun extendSession() {
        dispatchNativeAppCallback("sessionExtend")
    }

    fun getNotificationsStatus(status: String) {
        dispatchNativeAppCallback("notificationsSettingsStatus", "'$status'")
    }

    fun notificationsAuthorised(devicePns: String, trigger: String) {
        val response = """
            {
                devicePns: '$devicePns',
                deviceType: 'android',
                trigger: '$trigger'
            }
        """
        dispatchNativeAppCallback("notificationsAuthorised", response)
    }

    fun notificationsUnauthorised() {
        dispatchNativeAppCallback("notificationsUnauthorised")
    }

    fun stayOnPage() {
        dispatchNativeAppCallback("pageLeaveWarningStayOnPage")
    }

    fun goTo(path: String) {
        dispatchNativeAppCallback("navigationGoTo", "'$path'")
    }

    private fun dispatchNativeAppCallback(function: String, args: String = "") {
        evaluateWebviewJavascript(
                "window.nativeAppCallbacks.${function}(" +
                "${ if (args != "") "$args" else "" })")
    }

    private fun evaluateWebviewJavascript(javascriptText: String) {
        webView.evaluateJavascript(javascriptText, null)
    }
}
