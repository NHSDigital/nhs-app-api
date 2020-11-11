package com.nhs.online.nhsonline.webinterfaces

import android.webkit.WebView
import com.nhaarman.mockitokotlin2.mock
import com.nhaarman.mockitokotlin2.verify
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class AppWebInterfaceTest {
    private lateinit var webviewMock: WebView
    private lateinit var appWebInterface: AppWebInterface

    @Before
    fun setUp() {
        webviewMock = mock()
        appWebInterface = AppWebInterface(webviewMock)
    }

    @Test
    fun goToTest() {
        appWebInterface.goTo("testpath")
        verify(webviewMock).evaluateJavascript("window.nativeAppCallbacks.navigationGoTo('testpath')", null)
    }

    @Test
    fun logoutTest() {
        appWebInterface.logout()
        verify(webviewMock).evaluateJavascript("window.nativeAppCallbacks.authLogout()",
                null)
    }

    @Test
    fun extendSessionTest() {
        val appWebInterface = AppWebInterface(webviewMock)
        appWebInterface.extendSession()
        verify(webviewMock).evaluateJavascript("window.nativeAppCallbacks.sessionExtend()",
                null)
    }

    @Test
    fun notificationsAuthorised() {
        appWebInterface.notificationsAuthorised("1234", "load")
        verify(webviewMock).evaluateJavascript(
            """window.nativeAppCallbacks.notificationsAuthorised(
            {
                devicePns: '1234',
                deviceType: 'android',
                trigger: 'load'
            }
        )""",
            null)
    }

    @Test
    fun notificationsUnauthorised() {
        appWebInterface.notificationsUnauthorised()
        verify(webviewMock).evaluateJavascript("window.nativeAppCallbacks.notificationsUnauthorised()",
                null)
    }

    @Test
    fun biometricCompletion() {
        appWebInterface.biometricCompletion("Register", "Success", "")
        verify(webviewMock).evaluateJavascript(
                """window.nativeAppCallbacks.loginSettingsBiometricCompletion(
            {
                action: 'Register',
                outcome: 'Success',
                errorCode: ''
            }
        )""",
                null)
    }

    @Test
    fun biometricSpec() {
        appWebInterface.biometricSpec("FaceID", false)
        verify(webviewMock).evaluateJavascript(
                """window.nativeAppCallbacks.loginSettingsBiometricSpec(
            {
                biometricTypeReference: 'FaceID',
                enabled: false
            }
        )""",
                null)
    }

    @Test
    fun biometricLoginFailure() {
        appWebInterface.biometricLoginFailure()
        verify(webviewMock).evaluateJavascript(
                "window.nativeAppCallbacks.loginHandleBiometricLoginFailure()",
                null)
    }

    @Test
    fun stayOnPage() {
        appWebInterface.stayOnPage()
        verify(webviewMock).evaluateJavascript(
                "window.nativeAppCallbacks.pageLeaveWarningStayOnPage()",
                null)
    }

    @Test
    fun leavePage() {
        appWebInterface.leavePage()
        verify(webviewMock).evaluateJavascript(
                "window.nativeAppCallbacks.pageLeaveWarningLeavePage()",
                null)
    }
}
