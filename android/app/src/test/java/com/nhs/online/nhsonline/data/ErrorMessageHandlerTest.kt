package com.nhs.online.nhsonline.data

import android.content.Context
import android.content.res.Resources
import com.nhaarman.mockito_kotlin.mock
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Before
import org.junit.Test

import org.junit.Assert.assertEquals
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class ErrorMessageHandlerTest : ResourceMockingClass() {

    private lateinit var contextMock: Context
    private lateinit var errorMessageHandler: ErrorMessageHandler

    @Before
    fun setUp() {
        contextMock = mockContext()
        errorMessageHandler = ErrorMessageHandler(contextMock.resources)
    }

    @Test
    fun getErrorMessage_WhenNoConnectionErrorType_returnNoConnectionErrorMessage() {
        val errorMessage = errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure)

        assertEquals(contextMock.resources.getString(R.string.service_unavailable), errorMessage.title)
        assertEquals(contextMock.resources.getString(R.string.apiUnavailableErrorMessage), errorMessage.message)
        assertEquals(contextMock.resources.getString(R.string.accessible_apiUnavailableErrorMessage), errorMessage.accessibleMessage)
    }
}