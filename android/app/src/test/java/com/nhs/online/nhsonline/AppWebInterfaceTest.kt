package com.nhs.online.nhsonline

import android.webkit.WebView
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.verify
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.webinterfaces.WebJavascript
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
    fun loadSPATest() {
        val webJavascript = WebJavascript(webviewMock)
        webJavascript.loadSpaPath("testpath")
        verify(webviewMock).evaluateJavascript("window.\$nuxt.\$router.push('testpath')", null)
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
        verify(webviewMock).evaluateJavascript("window.\$nuxt.\$store.dispatch('notifications/authorised', " +
                "'{\"devicePns\":\"1234\",\"deviceType\":\"android\",\"trigger\":\"load\"}')",
                null)
    }

    @Test
    fun notificationsUnauthorised() {
        appWebInterface.notificationsUnauthorised()
        verify(webviewMock).evaluateJavascript("window.\$nuxt.\$store.dispatch('notifications/unauthorised')",
                null)
    }
}