package com.nhs.online.nhsonline.webinterfaces

import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class WebAppInterfaceThirdPartyTest {
    private lateinit var contextMock: MainActivity
    private lateinit var nhsWebMock: NhsWeb
    private lateinit var webAppInterfaceThirdParty: WebAppInterfaceThirdParty
    private lateinit var appWebInterface: AppWebInterface


    private fun setUp(mode: JavaScriptInteractionMode) {
        contextMock = mock()
        nhsWebMock = mock{
            on { javaScriptInteractionMode }.thenReturn( mode )
        }
        appWebInterface = mock()
        doNothing().whenever(nhsWebMock).loadWelcomePage()
        webAppInterfaceThirdParty = WebAppInterfaceThirdParty(contextMock, nhsWebMock, contextMock, appWebInterface)
    }

    @Test
    fun onGoToHomepage_javaScriptInteractionMode_IsSilverThirdParty() {
        setUp(mode = JavaScriptInteractionMode.SilverThirdParty)

        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterfaceThirdParty.goToHomepage()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).loadWelcomePage()
    }

    @Test
    fun onGoToHomepage_javaScriptInteractionMode_IsNone() {
        setUp(mode = JavaScriptInteractionMode.None)

        webAppInterfaceThirdParty.goToHomepage()
        verifyNoMoreInteractions(contextMock)
    }

    @Test
    fun onGoToPage_javaScriptInteractionMode_IsSilverThirdParty() {
        setUp(mode = JavaScriptInteractionMode.SilverThirdParty)

        val page = "foo"
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterfaceThirdParty.goToPage(page)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(appWebInterface).goToPage(page)
    }

    @Test
    fun onGoToPage_javaScriptInteractionMode_IsNone() {
        setUp(mode = JavaScriptInteractionMode.None)

        webAppInterfaceThirdParty.goToPage("page")
        verifyNoMoreInteractions(contextMock)
    }
}