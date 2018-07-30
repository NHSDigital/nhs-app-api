package com.nhs.online.nhsonline.services

import com.nhs.online.nhsonline.data.ErrorMessage
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class KnownServiceServiceTest {
    private val emptyErrorMessage = ErrorMessage("")

    @Test
    fun hasMissingQueryString_returnsTrue_forMissingQueryString() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000")

        Assert.assertTrue(result)
    }

    @Test
    fun hasMissingQueryString_returnsTrue_forMissingOneOfMany() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android&param2=param2Value")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?param2=param2Value")
        Assert.assertTrue(result)

        val result2 = testService.hasMissingQueryString("http://10.0.2.2:3000?source=android")
        Assert.assertTrue(result2)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forValidQueryString() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?source=android")

        Assert.assertFalse(result)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forValidQueryStringDifferentCase() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?SOURCE=ANDROID")

        Assert.assertFalse(result)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forValidQueryStringNoValue() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?source=")

        Assert.assertFalse(result)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forEmptyUrl() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android")

        val result = testService.hasMissingQueryString("")

        Assert.assertFalse(result)
    }

    @Test
    fun addMissingQueryStrings_returnsFullUrl_forMissingQueryString() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000")

        Assert.assertEquals("http://10.0.2.2:3000?source=android", result)
    }

    @Test
    fun addMissingQueryStrings_returnsFullUrl_forIncludedQueryString() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000?source=android")

        Assert.assertEquals("http://10.0.2.2:3000?source=android", result)
    }

    @Test
    fun addMissingQueryStrings_returnsOriginalUrl_forNonMatchingHost() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android")

        val result = testService.addMissingQueryStrings("http://www.google.com")

        Assert.assertEquals("http://www.google.com", result)
    }

    @Test
    fun addMissingQueryStrings_returnsFullUrl_forNoQueryString() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage)

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000")

        Assert.assertEquals("http://10.0.2.2:3000", result)
    }

    @Test
    fun addMissingQueryStrings_returnsOriginalUrl_forNoQueryStringWithInputQueryString() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage)

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000?param1=param1Value")

        Assert.assertEquals("http://10.0.2.2:3000?param1=param1Value", result)
    }

    @Test
    fun addMissingQueryStrings_returnsOriginalUrl_forQueryStringWithAdditionalInputQueryString() {
        val testService = KnownService(arrayOf("http://10.0.2.2:3000"), emptyErrorMessage,
            queryString = "?source=android")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000?param1=param1Value")

        Assert.assertEquals("http://10.0.2.2:3000?param1=param1Value&source=android", result)
    }

    @Test
    fun hasNativeHeader_returnsTrue_forServiveWithNativeHeader() {
        val testService = KnownService(arrayOf("http://testUrl"), emptyErrorMessage, nativeHeader = "Header 1")

        val result = testService.hasNativeHeader()
        Assert.assertTrue(result)
    }

    @Test
    fun hasNativeHeader_returnsFalse_forServiveWithoutNativeHeader() {
        val testService = KnownService(arrayOf("http://testUrl"), emptyErrorMessage)

        val result = testService.hasNativeHeader()
        Assert.assertFalse(result)
    }
}