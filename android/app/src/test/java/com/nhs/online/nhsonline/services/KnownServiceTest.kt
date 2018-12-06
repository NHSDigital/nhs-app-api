package com.nhs.online.nhsonline.services

import com.nhs.online.nhsonline.data.ErrorMessage
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class KnownServiceTest {
    private val emptyErrorMessage = ErrorMessage("")

    @Test
    fun hasMissingQueryString_returnsTrue_forMissingQueryString() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000")

        Assert.assertTrue(result)
    }

    @Test
    fun hasMissingQueryString_returnsTrue_forMissingOneOfMany() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android&param2=param2Value")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?param2=param2Value")
        Assert.assertTrue(result)

        val result2 = testService.hasMissingQueryString("http://10.0.2.2:3000?source=android")
        Assert.assertTrue(result2)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forValidQueryString() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?source=android")

        Assert.assertFalse(result)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forValidQueryStringDifferentCase() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?SOURCE=ANDROID")

        Assert.assertFalse(result)
    }

    @Test
    fun hasMissingQueryString_returnsTrue_forValidQueryStringMismatchValue() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?source=")

        Assert.assertTrue(result)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forEmptyUrl() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")

        val result = testService.hasMissingQueryString("")

        Assert.assertFalse(result)
    }

    @Test
    fun hasOnlyRequiredQueries_returnsFalseWhenUrlHasAnExtraQuery() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")
        val url = "http://10.0.2.2:3000??source=android&param=1"
        val result = testService.hasOnlyRequiredQueries(url)
        Assert.assertFalse("An extra query is present in the url,expected only source=android",
            result)

    }

    @Test
    fun hasOnlyRequiredQueries_returnsTrueWhenServiceAndUrlHaveSameQuery() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")
        val url = "http://10.0.2.2:3000?source=android"
        val result = testService.hasOnlyRequiredQueries(url)
        Assert.assertTrue("Url query and service query are not matching",
            result)
    }

    @Test
    fun hasOnlyRequiredQueries_returnsFalseWhenServiceHasQueryButUrlHasNot() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")
        val url = "http://10.0.2.2:3000"
        val result = testService.hasOnlyRequiredQueries(url)
        Assert.assertFalse("Url has an unexpected query",
            result)
    }

    @Test
    fun addMissingQueryStrings_returnsFullUrl_forMissingQueryString() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000")

        Assert.assertEquals("http://10.0.2.2:3000?source=android", result)
    }

    @Test
    fun addMissingQueryStrings_returnsFullUrl_forIncludedQueryString() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000?source=android")

        Assert.assertEquals("http://10.0.2.2:3000?source=android", result)
    }

    @Test
    fun addMissingQueryStrings_returnsOriginalUrl_forNonMatchingHost() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")

        val result = testService.addMissingQueryStrings("http://www.google.com")

        Assert.assertEquals("http://www.google.com", result)
    }

    @Test
    fun addMissingQueryStrings_returnsFullUrl_forNoQueryString() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage)

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000")

        Assert.assertEquals("http://10.0.2.2:3000", result)
    }

    @Test
    fun addMissingQueryStrings_returnsOriginalUrl_forNoQueryStringWithInputQueryString() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage)

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000?param1=param1Value")

        Assert.assertEquals("http://10.0.2.2:3000?param1=param1Value", result)
    }

    @Test
    fun addMissingQueryStrings_returnsOriginalUrl_forQueryStringWithAdditionalInputQueryString() {
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage,
            queryStrings = "?source=android")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000?param1=param1Value")

        Assert.assertEquals("http://10.0.2.2:3000?param1=param1Value&source=android", result)
    }

    @Test
    fun hasNativeHeader_returnsTrue_forServiveWithNativeHeader() {
        val testService =
            KnownService("http://testUrl", emptyErrorMessage, "Header 1")

        val result = testService.hasDefaultNativeHeader()
        Assert.assertTrue(result)
    }

    @Test
    fun hasNativeHeader_returnsFalse_forServiveWithoutNativeHeader() {
        val testService = KnownService("http://testUrl", emptyErrorMessage)

        val result = testService.hasDefaultNativeHeader()
        Assert.assertFalse(result)
    }

    @Test
    fun addPathInfo_generatesPathInfoForSpecifiedPath() {
        val paths = arrayListOf("pathOne", "pathTwo", "pathThree")
        val headers = arrayListOf("HeaderOne", "HeaderTwo", "HeaderThree")
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage)
        testService.addPathInfo(paths[0], true, headers[0])
        testService.addPathInfo(paths[1], true, headers[1])
        testService.addPathInfo(paths[2], true, headers[2])

        for (i in 0 until paths.size) {
            val pathInfo =
                testService.findMatchingServicePathInfo("http://10.0.2.2:3000/${paths[i]}")
            Assert.assertNotNull(pathInfo)
            Assert.assertEquals(headers[i], pathInfo?.header)
        }
    }

    @Test
    fun addPathInfo_WithOrWithoutStartingForwardSlashHasNoEffect() {
        val path1 = "pathOne"
        val path1WithSlash = "/$path1"
        val path2 = "pathTwo"
        val header1 = "HeaderOne"
        val header2 = "HeaderTwo"
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage)
        testService.addPathInfo(path1WithSlash, false, header1)
        testService.addPathInfo(path2, false, header2)

        val pathInfoOne = testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$path1")
        val pathInfoTwo = testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$path2")
        Assert.assertEquals(header1, pathInfoOne?.header)
        Assert.assertEquals(header2, pathInfoTwo?.header)
    }

    @Test
    fun addPathInfo_WithoutErrorMessageDefaultToServiceDefaultErrorMessage() {
        val defaultErrorText = "Default Text"
        val defaultErrorMessage = ErrorMessage(defaultErrorText)
        val path = "Path"
        val header = "Header"
        val testService = KnownService("http://10.0.2.2:3000", defaultErrorMessage)
        testService.addPathInfo(path, false, header)

        val pathInfo = testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$path")
        Assert.assertEquals(header, pathInfo?.header)
        Assert.assertEquals(defaultErrorText, pathInfo?.errorMessage?.title)
    }

    @Test
    fun addPathInfo_EmptyPathOrPathWithJustSlashDoesNotOverrideDefaultPathInfo() {
        val emptyPath = ""
        val pathWithSlash = "/"
        val header1 = "HeaderOne"
        val header2 = "HeaderTwo"
        val defaultHeader = "DefaultHeader"
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage, defaultHeader)
        testService.addPathInfo(emptyPath, false, header1)
        testService.addPathInfo(pathWithSlash, false, header2)
        val emptyPathInfo =
            testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$emptyPath")
        val slashPathInfo =
            testService.findMatchingServicePathInfo("http://10.0.2.2:3000$pathWithSlash")
        Assert.assertNotEquals(header1, emptyPathInfo?.header)
        Assert.assertNotEquals(header2, slashPathInfo?.header)
    }


    @Test
    fun findMatchingServicePathInfo_matchesPathInfoWithSameBaseUrlAndStartWithThePathInfoPath() {
        val path = "appointments"
        val pathHeader = "Appointment Header"
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage)
        testService.addPathInfo(path, true, pathHeader)
        val pathInfo =
            testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$path/extra-path")
        Assert.assertNotNull(pathInfo)
        Assert.assertEquals(pathHeader, pathInfo?.header)
    }

    @Test
    fun findMatchingServicePathInfo_returnNull_whenProvidedUrlDoesNotMatchRequiredBaseUrl() {
        val path = "appointments"
        val randomUrl = "https://www.google.co.uk/$path"
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage)
        testService.addPathInfo(path, true, "")
        val pathInfo =
            testService.findMatchingServicePathInfo(randomUrl)
        Assert.assertNull(pathInfo)
    }

    @Test
    fun findMatchingServicePathInfoByPath_resolveToMatchingPathInfo() {
        val path = "appointments"
        val pathHeader = "Appointment Header"
        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage)
        testService.addPathInfo(path, true, pathHeader)
        val pathInfo =
            testService.findMatchingServicePathInfoByPath(path)
        Assert.assertNotNull(pathInfo)
        Assert.assertEquals(pathHeader, pathInfo?.header)
    }

    @Test
    fun findMatchingServicePathInfo_returnsClosestMatchingPathInfo_fromMultipleMatchingPath() {
        val path1 = "path/path1"
        val header1 = "headerOne"
        val pathPlusPath2 = "$path1/path2"
        val header2 = "headerTwo"
        val pathPlusPath3 = "$pathPlusPath2/path3"
        val header3 = "headerThree"

        val closestPath1 = "$path1/random/path"
        val closestPath3 = "$pathPlusPath3/random-path"

        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage)
        testService.addPathInfo(path1, false, header1)
        testService.addPathInfo(pathPlusPath2, false, header2)
        testService.addPathInfo(pathPlusPath3, false, header3)

        val closestPath1Info =
            testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$closestPath1")
        val closestPath2Info =
            testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$closestPath3")
        Assert.assertEquals(header1, closestPath1Info?.header)
        Assert.assertEquals(header3, closestPath2Info?.header)
    }

    @Test
    fun findMatchingServicePathInfo_doesNotMatchClosestPath_whenSpecifyExactMatch() {
        val path1 = "path/path1"
        val header1 = "headerOne"

        val closestPath1 = "$path1/random/path"

        val testService = KnownService("http://10.0.2.2:3000", emptyErrorMessage)
        testService.addPathInfo(path1, false, header1)

        val closestPath1Info =
            testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$closestPath1", true)
        Assert.assertNull(closestPath1Info)
    }
}