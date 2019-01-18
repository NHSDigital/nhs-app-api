package com.nhs.online.nhsonline.browseractivities

import android.content.Context
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class OpenUrlInBrowserActivityTest: ResourceMockingClass() {

    @Test
    fun canStart_returnsFalse_forSupportedHosts() {
        val openUrlInBrowserActivity = OpenUrlInBrowserActivity(arrayOf("https://111.nhs.uk/"))
        val context: Context = mockContext()

        val urls = listOf("https://111.nhs.uk/", "https://111.nhs.uk/Help/Terms")

        urls.forEach{url ->
            val result = openUrlInBrowserActivity.canStart(context, url)
            Assert.assertFalse(result)
        }
    }

    @Test
    fun canStart_returnsTrue_forNotSupportedHosts() {
        val openUrlInBrowserActivity = OpenUrlInBrowserActivity(arrayOf("https://111.nhs.uk/"))
        val context: Context = mockContext()

        val urls = listOf("https://www.google.co.uk/", "https://www.nhs.uk")

        urls.forEach{url ->
             val result = openUrlInBrowserActivity.canStart(context, url)
            Assert.assertTrue(result)
        }
    }

    @Test
    fun start_throwsException_supportedHosts() {
        val openUrlInBrowserActivity = OpenUrlInBrowserActivity(arrayOf("https://111.nhs.uk/"))
        val context: Context = mockContext()

        val urls = listOf("https://111.nhs.uk/", "https://111.nhs.uk/Help/Terms")


        var runtimeException: java.lang.RuntimeException?
        runtimeException = null

        urls.forEach{url ->
            try {
                runtimeException = null
                openUrlInBrowserActivity.start(context, url)
                Assert.fail("Expected `start()` to throw an exception for the given url: $url")
            }catch (exception: RuntimeException){
                runtimeException = exception
            } finally {
                Assert.assertNotNull(runtimeException)
                Assert.assertEquals("Cannot open url in browser", runtimeException!!.message)
            }
        }

    }
}