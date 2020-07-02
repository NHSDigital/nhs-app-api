package com.nhs.online.nhsonline.webinterfaces

import android.webkit.WebView
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.verify
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
        verify(webviewMock).evaluateJavascript("window.\$nuxt.\$store.dispatch('navigation/goTo', 'testpath')", null)
    }

    @Test
    fun logoutTest() {
        appWebInterface.logout()
        verify(webviewMock).evaluateJavascript("window.\$nuxt.\$store.dispatch('auth/logout')",
                null)
    }

    @Test
    fun extendSessionTest() {
        val appWebInterface = AppWebInterface(webviewMock)
        appWebInterface.extendSession()
        verify(webviewMock).evaluateJavascript("window.\$nuxt.\$store.dispatch('session/extend')",
                null)
    }

    @Test
    fun notificationsAuthorised() {
        appWebInterface.notificationsAuthorised("1234", "load")
        verify(webviewMock).evaluateJavascript(
            """window.${'$'}nuxt.${'$'}store.dispatch('notifications/authorised', 
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
        verify(webviewMock).evaluateJavascript("window.\$nuxt.\$store.dispatch('notifications/unauthorised')",
                null)
    }

    @Test
    fun biometricCompletion() {
        appWebInterface.biometricCompletion("Register", "Success", "")
        verify(webviewMock).evaluateJavascript(
                """window.${'$'}nuxt.${'$'}store.dispatch('loginSettings/biometricCompletion', 
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
                """window.${'$'}nuxt.${'$'}store.dispatch('loginSettings/biometricSpec', 
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
                "window.\$nuxt.\$store.dispatch('login/handleBiometricLoginFailure')",
                null)
    }

    @Test
    fun stayOnPage() {
        appWebInterface.stayOnPage()
        verify(webviewMock).evaluateJavascript(
                "window.\$nuxt.\$store.dispatch('pageLeaveWarning/stayOnPage')",
                null)
    }

    @Test
    fun leavePage() {
        appWebInterface.leavePage()
        verify(webviewMock).evaluateJavascript(
                "window.\$nuxt.\$store.dispatch('pageLeaveWarning/leavePage')",
                null)
    }
}