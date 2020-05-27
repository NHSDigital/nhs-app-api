package com.nhs.online.nhsonline.support

import android.app.Activity
import android.content.Intent
import android.provider.CalendarContract
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.data.AddToCalendarData
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class AddToCalendarHandlerTest {

    private lateinit var addToCalendarHandler: AddToCalendarHandler
    private lateinit var mockContext: Activity

    @Before
    fun setUp() {
        mockContext = mock()
        addToCalendarHandler = AddToCalendarHandler(mockContext)
    }

    @Test
    fun addToCalendar_IntentIsStarted() {
        val intentCaptor = argumentCaptor<Intent>()

        addToCalendarHandler.addToCalendar(
                AddToCalendarData("my subject", "my body", "my location", 12345678, 12345679))

        verify(mockContext).startActivity(intentCaptor.capture())

        assertEquals("my subject", intentCaptor.firstValue.extras[CalendarContract.Events.TITLE])
        assertEquals("my body", intentCaptor.firstValue.extras[CalendarContract.Events.DESCRIPTION])
        assertEquals("my location", intentCaptor.firstValue.extras[CalendarContract.Events.EVENT_LOCATION])
        assertEquals(12345678L * 1000, intentCaptor.firstValue.extras[CalendarContract.EXTRA_EVENT_BEGIN_TIME])
        assertEquals(12345679L * 1000, intentCaptor.firstValue.extras[CalendarContract.EXTRA_EVENT_END_TIME])
        assertEquals(CalendarContract.Events.AVAILABILITY_BUSY, intentCaptor.firstValue.extras[CalendarContract.Events.AVAILABILITY])
    }
}