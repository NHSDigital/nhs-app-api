package com.nhs.online.nhsonline.services

import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class KnownServiceTest {

    @Test
    fun hasNativeHeader_returnsTrue_forServiveWithNativeHeader() {
        val testService =
            KnownService("http://testUrl", "Header 1")

        val result = testService.hasDefaultNativeHeader()
        Assert.assertTrue(result)
    }

    @Test
    fun hasNativeHeader_returnsFalse_forServiveWithoutNativeHeader() {
        val testService = KnownService("http://testUrl")

        val result = testService.hasDefaultNativeHeader()
        Assert.assertFalse(result)
    }

    @Test
    fun addPathInfo_generatesPathInfoForSpecifiedPath() {
        val paths = arrayListOf("pathOne", "pathTwo", "pathThree")
        val headers = arrayListOf("HeaderOne", "HeaderTwo", "HeaderThree")
        val testService = KnownService("http://10.0.2.2:3000")
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
        val testService = KnownService("http://10.0.2.2:3000")
        testService.addPathInfo(path1WithSlash, false, header1)
        testService.addPathInfo(path2, false, header2)

        val pathInfoOne = testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$path1")
        val pathInfoTwo = testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$path2")
        Assert.assertEquals(header1, pathInfoOne?.header)
        Assert.assertEquals(header2, pathInfoTwo?.header)
    }

    @Test
    fun addPathInfo_WithoutErrorMessageDefaultToServiceDefaultErrorMessage() {
        val path = "Path"
        val header = "Header"
        val testService = KnownService("http://10.0.2.2:3000")
        testService.addPathInfo(path, false, header)

        val pathInfo = testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$path")
        Assert.assertEquals(header, pathInfo?.header)
    }

    @Test
    fun addPathInfo_EmptyPathOrPathWithJustSlashDoesNotOverrideDefaultPathInfo() {
        val emptyPath = ""
        val pathWithSlash = "/"
        val header1 = "HeaderOne"
        val header2 = "HeaderTwo"
        val defaultHeader = "DefaultHeader"
        val testService = KnownService("http://10.0.2.2:3000", defaultHeader)
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
        val testService = KnownService("http://10.0.2.2:3000")
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
        val testService = KnownService("http://10.0.2.2:3000")
        testService.addPathInfo(path, true, "")
        val pathInfo =
            testService.findMatchingServicePathInfo(randomUrl)
        Assert.assertNull(pathInfo)
    }

    @Test
    fun findMatchingServicePathInfoByPath_resolveToMatchingPathInfo() {
        val path = "appointments"
        val pathHeader = "Appointment Header"
        val testService = KnownService("http://10.0.2.2:3000")
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

        val testService = KnownService("http://10.0.2.2:3000")
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

        val testService = KnownService("http://10.0.2.2:3000")
        testService.addPathInfo(path1, false, header1)

        val closestPath1Info =
            testService.findMatchingServicePathInfo("http://10.0.2.2:3000/$closestPath1", true)
        Assert.assertNull(closestPath1Info)
    }
}