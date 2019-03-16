package com.nhs.online.nhsonline.support.schemehandlers

import com.nhaarman.mockito_kotlin.*
import junit.framework.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class SchemeHandlersTests {

    @Test
    fun handle_ReturnsTrue_WhenUrlIsHandled() {
        val url = "AnyThing:"
        val mockHandler: ISchemeHandler = mock { on { handle(url) } doReturn true
        on { scheme } doReturn "AnyThing:"}

        val systemUnderTest = SchemeHandlers()
        systemUnderTest.registerHandler(mockHandler)

        val response = systemUnderTest.handleUrl(url)

        verify(mockHandler, times(1)).handle(url)
        verify(mockHandler, times(1)).scheme
        Assert.assertTrue(response)
    }

    @Test
    fun handle_ReturnsFalse_WhenNoMatchingHandlerIsRegistered() {
        val url = "SomeThing:"
        val mockHandler: ISchemeHandler = mock { on { handle(url) } doReturn true
            on { scheme } doReturn "AnyThing:"}

        val systemUnderTest = SchemeHandlers()
        systemUnderTest.registerHandler(mockHandler)

        val response = systemUnderTest.handleUrl(url)

        verify(mockHandler, times(1)).scheme
        verify(mockHandler, times(0)).handle(url)
        Assert.assertFalse(response)
    }

    @Test
    fun handle_ReturnsTrueAndUsesTheRightHandler_WhenOneOfTheRegisteredHandlersIsValid() {
        val url = "matching:"
        val mockAnythingHandler: ISchemeHandler = mock { on { handle(url) } doReturn true
            on { scheme } doReturn "AnyThing:"}
        val mockSomeThingHandler: ISchemeHandler = mock { on { handle(url) } doReturn true
            on { scheme } doReturn "Something:"}
        val mockMatchingHandler: ISchemeHandler = mock { on { handle(url) } doReturn true
            on { scheme } doReturn "matching:"}

        val systemUnderTest = SchemeHandlers()
        systemUnderTest.registerHandler(mockAnythingHandler)
        systemUnderTest.registerHandler(mockSomeThingHandler)
        systemUnderTest.registerHandler(mockMatchingHandler)

        val response = systemUnderTest.handleUrl(url)

        verify(mockAnythingHandler, times(0)).handle(url)
        verify(mockAnythingHandler, times(1)).scheme
        verify(mockSomeThingHandler, times(0)).handle(url)
        verify(mockSomeThingHandler, times(1)).scheme
        verify(mockMatchingHandler, times(1)).handle(url)
        verify(mockMatchingHandler, times(1)).scheme
        Assert.assertTrue(response)
    }

    @Test
    fun handle_ReturnsFalse_WhenNoHandlersAreRegistered() {
        val url = "tel:enquiries@nhsdigital.nhs.uk?subject=111%20online%20enquiry"

        val systemUnderTest = SchemeHandlers()
        val response = systemUnderTest.handleUrl(url)

        Assert.assertFalse(response)
    }

}