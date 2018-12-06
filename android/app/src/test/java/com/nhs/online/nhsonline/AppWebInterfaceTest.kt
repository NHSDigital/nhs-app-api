package com.nhs.online.nhsonline

import com.nhaarman.mockito_kotlin.spy
import com.nhaarman.mockito_kotlin.verify
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class AppWebInterfaceTest {

    private val mainActivity = Robolectric.buildActivity(MainActivity::class.java).create().get()
    private val spyActivity = spy(mainActivity)
    private val appWebInterface = AppWebInterface(spyActivity)

    @Test
    fun loadSPATest() {
        appWebInterface.loadSpaPage("testpath", "http://test.com")
        verify(spyActivity).evaluateWebviewJavascript("window.\$nuxt.\$router.push('/testpath')")
    }

    @Test
    fun loadDispatchEventTest() {
        appWebInterface.loadDispatchEvent("auth/logout")
        verify(spyActivity).evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('auth/logout')")
    }

    @Test
    fun logoutTest() {
        appWebInterface.logout()
        verify(spyActivity).evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('auth/logout')")
    }

    @Test
    fun extendSessionTest() {
        appWebInterface.extendSession()
        verify(spyActivity).evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('session/extend')")
    }
}