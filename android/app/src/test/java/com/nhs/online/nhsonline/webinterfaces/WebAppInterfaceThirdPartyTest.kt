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

private val CAL_SUBJECT = "my subject"
private val CAL_BODY= "my body"
private val CAL_LOCATION = "my location"
private val CAL_START_TIME = 1591788300L
private val CAL_END_TIME = 1591789860L

@RunWith(RobolectricTestRunner::class)
class WebAppInterfaceThirdPartyTest {
    private lateinit var contextMock: MainActivity
    private lateinit var nhsWebMock: NhsWeb
    private lateinit var addToCalendarHandlerMock: IAddToCalendarHandler
    private lateinit var webAppInterfaceThirdParty: WebAppInterfaceThirdParty

    private lateinit var appWebInterface: AppWebInterface

    private fun setUp(mode: JavaScriptInteractionMode) {
        contextMock = mock()
        nhsWebMock = mock {
            on { javaScriptInteractionMode }.thenReturn(mode)
        }
        appWebInterface = mock()
        addToCalendarHandlerMock = mock()
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
    fun onAddToCalendar_javaScriptInteractionMode_IsSilverThirdParty() {
        setUp(mode = JavaScriptInteractionMode.SilverThirdParty)

        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfaceThirdParty.addEventToCalendar(buildStringifiedCalendarData())
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()

        verify(addToCalendarHandlerMock).addToCalendar(
                AddToCalendarData(CAL_SUBJECT, CAL_BODY, CAL_LOCATION, CAL_START_TIME, CAL_END_TIME))
    }

    @Test
    fun onAddToCalendar_javaScriptInteractionMode_WithBlankFields() {
        setUp(mode = JavaScriptInteractionMode.SilverThirdParty)

        val calendarData = buildStringifiedCalendarData(subject = "", body = "", location = "")

        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfaceThirdParty.addEventToCalendar(calendarData)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()

        verify(addToCalendarHandlerMock).addToCalendar(
                AddToCalendarData("", "", "", CAL_START_TIME, CAL_END_TIME))
    }

    @Test
    fun onAddToCalendar_javaScriptInteractionMode_WithNulls() {
        setUp(mode = JavaScriptInteractionMode.SilverThirdParty)

        val calendarData = buildStringifiedCalendarData(subject = null, body = null, location = null)

        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfaceThirdParty.addEventToCalendar(calendarData)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()

        verify(addToCalendarHandlerMock).addToCalendar(
                AddToCalendarData(null, null, null, CAL_START_TIME, CAL_END_TIME))
    }

    @Test
    fun onAddToCalendar_javaScriptInteractionMode_WithNoTimes() {
        setUp(mode = JavaScriptInteractionMode.SilverThirdParty)

        val calendarData = buildStringifiedCalendarData(startTimeEpochInSeconds = null, endTimeEpochInSeconds = null)

        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfaceThirdParty.addEventToCalendar(calendarData)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()

        verify(addToCalendarHandlerMock).addToCalendar(
                AddToCalendarData(CAL_SUBJECT, CAL_BODY, CAL_LOCATION,null,null))
    }

    @Test
    fun onAddToCalendar_javaScriptInteractionMode_IsNone() {
        setUp(mode = JavaScriptInteractionMode.None)

        webAppInterfaceThirdParty.addEventToCalendar(buildStringifiedCalendarData())
        verifyNoMoreInteractions(addToCalendarHandlerMock)
    }


    private fun buildStringifiedCalendarData(subject: String? = CAL_SUBJECT,
                                             body: String? = CAL_BODY,
                                             location: String? = CAL_LOCATION,
                                             startTimeEpochInSeconds: Long? = CAL_START_TIME,
                                             endTimeEpochInSeconds: Long? = CAL_END_TIME) : String {

        val subjectToUse = when {
            subject == null -> null
            subject.equals("") -> "''"
            else -> "'$subject'"
        }

        val bodyToUse = when {
            body == null -> null
            body.equals("") -> "''"
            else -> "'$body'"
        }

        val locationToUse = when {
            location == null -> null
            location.equals("") -> "''"
            else -> "'$location'"
        }

        val startTimeToUse = when {
            startTimeEpochInSeconds == null -> null
            else -> startTimeEpochInSeconds
        }

        val endTimeToUse = when {
            endTimeEpochInSeconds == null -> null
            else -> endTimeEpochInSeconds
        }

        return "{" +
                "'subject': $subjectToUse, " +
                "'body': $bodyToUse, " +
                "'location': $locationToUse, " +
                "'startTimeEpochInSeconds': $startTimeToUse, " +
                "'endTimeEpochInSeconds': $endTimeToUse" +
                "}"
    }
}