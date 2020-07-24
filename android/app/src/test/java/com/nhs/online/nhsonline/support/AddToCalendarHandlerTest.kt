package com.nhs.online.nhsonline.support

import android.app.Activity
import android.content.Intent
import android.provider.CalendarContract
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.data.AddToCalendarData
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.logging.ILoggingService
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

private val CAL_SUBJECT = "my subject"
private val CAL_BODY= "my body"
private val CAL_LOCATION = "my location"
private val CAL_START_TIME = 1591788300L
private val CAL_END_TIME = 1591789860L

@RunWith(RobolectricTestRunner::class)
class AddToCalendarHandlerTest {

    private lateinit var addToCalendarHandler: AddToCalendarHandler
    private lateinit var mockContext: Activity
    private lateinit var mockLogger: ILoggingService

    @Before
    fun setUp() {
        mockContext = mock ()
        mockLogger = mock()
        addToCalendarHandler = AddToCalendarHandler(mockContext, mockLogger)
    }

    @Test
    fun addToCalendar_IntentIsStarted() {
        val intentCaptor = argumentCaptor<Intent>()

        addToCalendarHandler.addToCalendar(
                AddToCalendarData(
                        "my subject",
                        "my body",
                        "my location",
                        12345678,
                        12345679,
                        JavaScriptInteractionMode.SilverThirdParty))

        verify(mockContext).startActivity(intentCaptor.capture())
        verify(mockLogger).logInfo(any())

        assertEquals("my subject", intentCaptor.firstValue.extras[CalendarContract.Events.TITLE])
        assertEquals("my body", intentCaptor.firstValue.extras[CalendarContract.Events.DESCRIPTION])
        assertEquals("my location", intentCaptor.firstValue.extras[CalendarContract.Events.EVENT_LOCATION])
        assertEquals(12345678L * 1000, intentCaptor.firstValue.extras[CalendarContract.EXTRA_EVENT_BEGIN_TIME])
        assertEquals(12345679L * 1000, intentCaptor.firstValue.extras[CalendarContract.EXTRA_EVENT_END_TIME])
        assertEquals(CalendarContract.Events.AVAILABILITY_BUSY, intentCaptor.firstValue.extras[CalendarContract.Events.AVAILABILITY])
    }

    @Test
    fun onAddToCalendar_javaScriptInteractionMode_WithBlankFields() {
        val stringifiedData = buildStringifiedCalendarData(subject = "", body = "", location = "")
        var addToCalendarData = addToCalendarHandler.parseCalendarData(stringifiedData, JavaScriptInteractionMode.SilverThirdParty)

        assertEquals("", addToCalendarData.subject)
        assertEquals("", addToCalendarData.body)
        assertEquals("", addToCalendarData.location)
        assertEquals(CAL_START_TIME, addToCalendarData.startTimeEpochInSeconds)
        assertEquals(CAL_END_TIME, addToCalendarData.endTimeEpochInSeconds)
        assertEquals(JavaScriptInteractionMode.SilverThirdParty, addToCalendarData.source)
    }

    @Test
    fun onAddToCalendar_javaScriptInteractionMode_WithNulls() {
        val stringifiedData = buildStringifiedCalendarData(subject = null, body = null, location = null)
        var addToCalendarData = addToCalendarHandler.parseCalendarData(stringifiedData, JavaScriptInteractionMode.SilverThirdParty)

        assertEquals(null, addToCalendarData.subject)
        assertEquals(null, addToCalendarData.body)
        assertEquals(null, addToCalendarData.location)
        assertEquals(CAL_START_TIME, addToCalendarData.startTimeEpochInSeconds)
        assertEquals(CAL_END_TIME, addToCalendarData.endTimeEpochInSeconds)
        assertEquals(JavaScriptInteractionMode.SilverThirdParty, addToCalendarData.source)
    }

    @Test
    fun onAddToCalendar_javaScriptInteractionMode_WithNoTimes() {
        val stringifiedData = buildStringifiedCalendarData(startTimeEpochInSeconds = null, endTimeEpochInSeconds = null)
        var addToCalendarData = addToCalendarHandler.parseCalendarData(stringifiedData, JavaScriptInteractionMode.SilverThirdParty)

        assertEquals(CAL_SUBJECT, addToCalendarData.subject)
        assertEquals(CAL_BODY, addToCalendarData.body)
        assertEquals(CAL_LOCATION, addToCalendarData.location)
        assertEquals(null, addToCalendarData.startTimeEpochInSeconds)
        assertEquals(null, addToCalendarData.endTimeEpochInSeconds)
        assertEquals(JavaScriptInteractionMode.SilverThirdParty, addToCalendarData.source)
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