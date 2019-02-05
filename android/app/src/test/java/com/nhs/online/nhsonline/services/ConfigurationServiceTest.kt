package com.nhs.online.nhsonline.services

import android.content.Context
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IVolleyCallback
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class ConfigurationServiceTest : ResourceMockingClass() {

    private var mainActivityMock: MainActivity =
        Robolectric.buildActivity(MainActivity::class.java).create().get()
    private lateinit var context: Context
    private var configurationService: ConfigurationService = ConfigurationService(mainActivityMock)

    @Before
    fun setUp() {
        context = mockContext()
    }

    @Test
    fun handleConfigurationResponse_invokeOnError_whenResponseIsHTML() {
        configurationService.handleGetConfigurationResponse("<html><body></body></html>",
            object : IVolleyCallback {
                override fun onSuccess(configurationResponse: ConfigurationResponse) {
                    Assert.fail("Failed as onSuccess callback should not be invoked")
                }

                override fun onError(errorMessage: ErrorMessage) {
                    Assert.assertEquals(serverConnectionError(), errorMessage)
                }
            })
    }

    @Test
    fun handleConfigurationResponse_invokeOnSuccess_whenResponseIsValid() {
        configurationService.handleGetConfigurationResponse(getValidResponse(),
            object : IVolleyCallback {
                override fun onSuccess(configurationResponse: ConfigurationResponse) {
                    Assert.assertTrue(configurationResponse.isValidConfiguration)
                    Assert.assertTrue(configurationResponse.isThrottlingEnabled)
                    Assert.assertEquals("https://uaf.ext.signin.nhs.uk",
                        configurationResponse.fidoServerUrl)
                }

                override fun onError(errorMessage: ErrorMessage) {
                    Assert.fail("Failed as onError callback should not be invoked")
                }
            })
    }

    private fun serverConnectionError(): ErrorMessage {
        return ErrorMessage("We're experiencing technical difficulties",
            "\nTry again later. If the problem continues and you need to book an " +
                    "appointment or get a prescription now, contact your GP surgery directly. " +
                    "For urgent medical advice, call 111.",
            "\nTry again later. If the problem continues and you need to book an " +
                    "appointment or get a prescription now, contact your GP surgery directly. " +
                    "For urgent medical advice, call one. one. one..")
    }

    private fun getValidResponse(): String {
        return "{\"isDeviceSupported\":true,\"isThrottlingEnabled\":true,\"fidoServerUrl\":\"https://uaf.ext.signin.nhs.uk\"}"
    }

}