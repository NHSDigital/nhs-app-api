package com.nhs.online.nhsonline.activity

import android.content.Context
import android.widget.TextView
import com.nhaarman.mockito_kotlin.mock
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class OpenUrlInBrowserActivityTest {

    @Test
    fun canStart_returnsFalse_forSupportedHosts() {
        val openUrlInBrowserActivity = OpenUrlInBrowserActivity(arrayOf("https://111.nhs.uk/"))
        val context: Context = mock()

        val urls = listOf("https://111.nhs.uk/", "https://111.nhs.uk/Help/Terms")

        urls.forEach{url ->
            val result = openUrlInBrowserActivity.canStart(context, url)
            Assert.assertFalse(result)
        }
    }

    @Test
    fun canStart_returnsTrue_forNotSupportedHosts() {
        val openUrlInBrowserActivity = OpenUrlInBrowserActivity(arrayOf("https://111.nhs.uk/"))
        val context: Context = mock()

        val urls = listOf("https://www.google.co.uk/", "https://www.nhs.uk")

        urls.forEach{url ->
             val result = openUrlInBrowserActivity.canStart(context, url)
            Assert.assertTrue(result)
        }
    }

    @Test
    fun start_throwsException_supportedHosts() {
        val openUrlInBrowserActivity = OpenUrlInBrowserActivity(arrayOf("https://111.nhs.uk/"))
        val context: Context = mock()

        val urls = listOf("https://111.nhs.uk/", "https://111.nhs.uk/Help/Terms")

        var message = ""
        try {
            urls.forEach{url ->
                openUrlInBrowserActivity.start(context, url)
            }
        } catch (exception: RuntimeException) {
            message =  exception?.message ?:  ""
        }

        Assert.assertEquals("Cannot open url in browser", message)
    }
}