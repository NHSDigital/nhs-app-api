package com.nhs.online.nhsonline.services.logging

import android.content.Context
import android.os.Build
import com.android.volley.Request
import com.android.volley.RequestQueue
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class LoggingServiceTest {
    private lateinit var spyRequestQueue: RequestQueue
    private lateinit var mockContext: Context
    private lateinit var systemUnderTest: ILoggingService
    private lateinit var mockVolleyQueueProvider: IVolleyQueueProvider

    @Before
    fun setUp() {
        var requestQueue = RequestQueue(null, null)
        spyRequestQueue = spy(requestQueue)

        mockContext = ResourceMockingClass().mockContext()
        mockVolleyQueueProvider = mock {
            on { newRequestQueue(mockContext) } doReturn spyRequestQueue
        }
        systemUnderTest = LoggingService(mockContext, mockVolleyQueueProvider)
    }

    @Test
    fun logInfo_Success() {
        systemUnderTest.logInfo("Log information message")

        argumentCaptor<NoResponseRequest>().apply {
            verify(spyRequestQueue, times(1)).add(capture())

            assertEquals(Request.Method.POST, firstValue.method)
            assertEquals("https://www.baseapiurl.com/logging", firstValue.url)

            var body = String(firstValue.body)

            val level = "\"level\":\"Information\""
            assert(body.contains(level)) {
                "Expected: ${level} to exist within body. Value was ${body}"
            }

            val message = "\"message\":\"Platform:Android-${Build.VERSION.SDK_INT} Log information message\""
            assert(body.contains(message)) {
                "Expected: ${message} to exist within body. Value was ${body}"
            }

            val timeStamp = "\"timeStamp\":"
            assert(body.contains(timeStamp)) {
                "Expected: ${timeStamp}: to exist within body. Value was ${body}"
            }
        }
    }

    @Test
    fun logError_Success() {
        systemUnderTest.logError("Log error message")

        argumentCaptor<NoResponseRequest>().apply {
            verify(spyRequestQueue, times(1)).add(capture())

            assertEquals(Request.Method.POST, firstValue.method)
            assertEquals("https://www.baseapiurl.com/logging", firstValue.url)

            var body = String(firstValue.body)

            val level = "\"level\":\"Error\""
            assert(body.contains(level)) {
                "Expected: ${level} to exist within body. Value was ${body}"
            }

            val message = "\"message\":\"Platform:Android-${Build.VERSION.SDK_INT} Log error message\""
            assert(body.contains(message)) {
                "Expected: ${message} to exist within body. Value was ${body}"
            }

            val timeStamp = "\"timeStamp\":"
            assert(body.contains(timeStamp)) {
                "Expected: ${timeStamp}: to exist within body. Value was ${body}"
            }
        }
    }
}