package com.nhs.online.nhsonline

import android.webkit.WebView
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.verify
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.webinterfaces.WebJavascript
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class AppWebInterfaceTest {
    private val webviewMock: WebView = mock()

    @Test
    fun loadSPATest() {
        val webJavascript = WebJavascript(webviewMock)
        webJavascript.loadSpaPath("testpath")
        verify(webviewMock).evaluateJavascript("window.\$nuxt.\$router.push('testpath')", null)
    }

    @Test
    fun loadDispatchEventTest() {
        val appWebInterface = AppWebInterface(webviewMock)
        appWebInterface.loadDispatchEvent("auth/logout")
        verify(webviewMock).evaluateJavascript("window.\$nuxt.\$store.dispatch('auth/logout')",
            null)
    }

    @Test
    fun logoutTest() {
        val appWebInterface = AppWebInterface(webviewMock)
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
}