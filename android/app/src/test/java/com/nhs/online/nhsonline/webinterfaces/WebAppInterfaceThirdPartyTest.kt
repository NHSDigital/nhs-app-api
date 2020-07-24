package com.nhs.online.nhsonline.webinterfaces

import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.AddToCalendarData
import com.nhs.online.nhsonline.interfaces.IAddToCalendarHandler
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class WebAppInterfaceThirdPartyTest {
    private lateinit var contextMock: MainActivity
    private lateinit var nhsWebMock: NhsWeb
    private lateinit var addToCalendarHandlerMock: IAddToCalendarHandler
    private lateinit var webAppInterfaceThirdParty: WebAppInterfaceThirdParty

    private lateinit var appWebInterface: AppWebInterface

    private lateinit var addToCalendarData: AddToCalendarData

    private fun setUp(mode: JavaScriptInteractionMode) {
        contextMock = mock()
        nhsWebMock = mock {
            on { javaScriptInteractionMode }.thenReturn(mode)
        }
        appWebInterface = mock()

        addToCalendarData = AddToCalendarData(
                "subject",
                "body",
                "location",
                123L,
                124L,
                JavaScriptInteractionMode.SilverThirdParty)

        addToCalendarHandlerMock = mock {
            on { parseCalendarData(any(), any()) }.thenReturn(addToCalendarData)
        }


        doNothing().whenever(nhsWebMock).loadWelcomePage()
        webAppInterfaceThirdParty = WebAppInterfaceThirdParty(contextMock, nhsWebMock, addToCalendarHandlerMock )
    }

    @Test
    fun onGoToPage_javaScriptInteractionMode_IsSilverThirdParty() {
        setUp(mode = JavaScriptInteractionMode.SilverThirdParty)

        val page = "foo"
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterfaceThirdParty.goToPage(page)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).goToPage(page)
    }

    @Test
    fun onGoToPage_javaScriptInteractionMode_IsNone() {
        setUp(mode = JavaScriptInteractionMode.None)

        webAppInterfaceThirdParty.goToPage("page")
        verifyNoMoreInteractions(contextMock)
    }

    @Test
    fun onAddToCalendar_javaScriptInteractionMode_SilverThirdParty() {
        setUp(mode = JavaScriptInteractionMode.SilverThirdParty)

        webAppInterfaceThirdParty.addEventToCalendar("stringifiedData")

        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(addToCalendarHandlerMock).addToCalendar(addToCalendarData)

        verifyNoMoreInteractions(contextMock)
    }

    @Test
    fun onAddToCalendar_javaScriptInteractionMode_IsNone() {
        setUp(mode = JavaScriptInteractionMode.None)

        webAppInterfaceThirdParty.addEventToCalendar("stringifiedData")
        verifyNoMoreInteractions(contextMock)
    }
}