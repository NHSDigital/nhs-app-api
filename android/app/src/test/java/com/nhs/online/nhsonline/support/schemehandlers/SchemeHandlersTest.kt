package com.nhs.online.nhsonline.support.schemehandlers

import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.never
import com.nhaarman.mockito_kotlin.verify
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class SchemeHandlersTests {
    private lateinit var schemeHandlers: SchemeHandlers

    @Before
    fun setUp() {
        schemeHandlers = SchemeHandlers()
    }

    @Test
    fun handle_ReturnsTrue_WhenUrlIsHandled() {
        val url = "AnyThing:"
        val handlerMock: ISchemeHandler = mock { on { handle(url) } doReturn true
        on { scheme } doReturn "AnyThing:"}

        schemeHandlers.registerHandler(handlerMock)

        val response = schemeHandlers.handleUrl(url)

        verify(handlerMock).handle(url)
        verify(handlerMock).scheme
        assertTrue(response)
    }

    @Test
    fun handle_ReturnsFalse_WhenNoMatchingHandlerIsRegistered() {
        val url = "SomeThing:"
        val handlerMock: ISchemeHandler = mock { on { handle(url) } doReturn true
            on { scheme } doReturn "AnyThing:"}

        schemeHandlers.registerHandler(handlerMock)

        val response = schemeHandlers.handleUrl(url)

        verify(handlerMock).scheme
        verify(handlerMock, never()).handle(url)
        assertFalse(response)
    }

    @Test
    fun handle_ReturnsTrueAndUsesTheRightHandler_WhenOneOfTheRegisteredHandlersIsValid() {
        val url = "matching:"
        val anythingHandlerMock: ISchemeHandler = mock { on { handle(url) } doReturn true
            on { scheme } doReturn "AnyThing:"}
        val someThingHandlerMock: ISchemeHandler = mock { on { handle(url) } doReturn true
            on { scheme } doReturn "Something:"}
        val matchingHandlerMock: ISchemeHandler = mock { on { handle(url) } doReturn true
            on { scheme } doReturn "matching:"}

        schemeHandlers.registerHandler(anythingHandlerMock)
        schemeHandlers.registerHandler(someThingHandlerMock)
        schemeHandlers.registerHandler(matchingHandlerMock)

        val response = schemeHandlers.handleUrl(url)

        verify(anythingHandlerMock, never()).handle(url)
        verify(anythingHandlerMock).scheme
        verify(someThingHandlerMock, never()).handle(url)
        verify(someThingHandlerMock).scheme
        verify(matchingHandlerMock).handle(url)
        verify(matchingHandlerMock).scheme
        assertTrue(response)
    }

    @Test
    fun handle_ReturnsFalse_WhenNoHandlersAreRegistered() {
        val url = "tel:enquiries@nhsdigital.nhs.uk?subject=111%20online%20enquiry"

        val response = schemeHandlers.handleUrl(url)

        assertFalse(response)
    }
}