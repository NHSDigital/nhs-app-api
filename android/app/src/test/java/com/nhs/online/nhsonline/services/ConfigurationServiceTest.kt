package com.nhs.online.nhsonline.services

import android.content.Context
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.clients.HttpClient
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.MockConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.knownservices.RootService
import com.nhs.online.nhsonline.services.knownservices.SubService
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.services.knownservices.enums.ViewMode
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNull
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import java.io.IOException

@RunWith(RobolectricTestRunner::class)
class ConfigurationServiceTest : ResourceMockingClass() {

    private lateinit var uiIInteractor: IInteractor
    private lateinit var context: Context
    private lateinit var httpClientMock: HttpClient
    private lateinit var configurationService: ConfigurationService
    private lateinit var errorMessageHandler: ErrorMessageHandler

    @Before
    fun setUp() {
        context = mockConnectedContext()
        errorMessageHandler = ErrorMessageHandler(context.resources)
        httpClientMock = mock()
        uiIInteractor = mock()
        configurationService = ConfigurationService("configurationUrlMock", uiIInteractor, errorMessageHandler, httpClientMock)
        MockConnectionStateMonitor().mockNetworkCallback(context)
    }

    @Test
    fun getConfigurationResponse_WhenThereIsNoConnection_ItShowsNoConnectionErrorAndReturnsNull() {
        whenever(httpClientMock.readText(any())).thenAnswer { throw IOException() }
        MockConnectionStateMonitor().mockNetworkCallback(mockDisconnectedContext())

        val configuration = configurationService.call()
        verify(uiIInteractor).showUnavailabilityError(errorMessageHandler.getErrorMessage(ErrorType.NoConnection))
        assertNull(configuration)
    }

    @Test
    fun getConfigurationResponse_WhenResponseIsEmpty_ItShowsApiCallErrorAndReturnsNull() {
        whenever(httpClientMock.readText(any())).thenReturn("")

        val configuration = configurationService.call()
        verify(uiIInteractor).showUnavailabilityError(apiCallFailureError())
        assertNull(configuration)
    }

    @Test
    fun getConfigurationResponse_WhenResponseIsEmptyObject_ItShowsApiCallErrorAndReturnsNull() {
        whenever(httpClientMock.readText(any())).thenReturn("{}")

        val configuration = configurationService.call()

        verify(uiIInteractor).showUnavailabilityError(apiCallFailureError())
        assertNull(configuration)
    }

    @Test
    fun getConfigurationResponse_WhenEmptyKnownServices_ReturnsMappedResponse() {
        whenever(httpClientMock.readText(any())).thenReturn("{ " +
                "\"minimumSupportedAndroidVersion\": \"v1.27.0\"," +
                "\"fidoServerUrl\": \"www.fidoserver.com\"," +
                "\"nhsLoginLoggedInPaths\": [\"/path\"]," +
                "\"knownServices\": []" +
                "}")

        val configuration = configurationService.call()

        verify(uiIInteractor, never()).showUnavailabilityError(any())
        assertEquals(configuration,
                Configuration(
                        minimumSupportedAndroidVersion = "v1.27.0",
                        fidoServerUrl = "www.fidoserver.com",
                        nhsLoginLoggedInPaths = listOf("/path"),
                        knownServices = emptyList()
                )
        )
    }

    @Test
    fun getConfigurationResponse_WhenHasUnknownEnums_ReturnsMappedResponseWithUnknowns() {
        whenever(httpClientMock.readText(any())).thenReturn("{ " +
                "\"minimumSupportedAndroidVersion\": \"v1.27.0\"," +
                "\"fidoServerUrl\": \"www.fidoserver.com\"," +
                "\"nhsLoginLoggedInPaths\": [\"/path\"]," +
                "\"knownServices\": [{" +
                "\"requiresAssertedLoginIdentity\":false," +
                "\"validateSession\":true," +
                "\"menuTab\":\"NotValidValue\"," +
                "\"viewMode\":\"NotValidValue\"," +
                "\"url\":\"www.example.com\"," +
                "\"subServices\":null" +
                "}]" +
                "}")

        val configuration = configurationService.call()

        verify(uiIInteractor, never()).showUnavailabilityError(any())
        assertEquals(configuration,
                Configuration(
                        minimumSupportedAndroidVersion = "v1.27.0",
                        fidoServerUrl = "www.fidoserver.com",
                        nhsLoginLoggedInPaths = listOf("/path"),
                        knownServices = listOf(
                                RootService(
                                        requiresAssertedLoginIdentity = false,
                                        validateSession = true,
                                        menuTab = MenuTab.Unknown,
                                        viewMode = ViewMode.Unknown,
                                        url = "www.example.com",
                                        subServices = null
                                )
                        )
                )
        )
    }

    @Test
    fun getConfigurationResponse_WhenHasRootServiceWithNoSubServices_ReturnsMappedResponse() {
        whenever(httpClientMock.readText(any())).thenReturn("{ " +
                "\"minimumSupportedAndroidVersion\": \"v1.27.0\"," +
                "\"fidoServerUrl\": \"www.fidoserver.com\"," +
                "\"nhsLoginLoggedInPaths\": [\"/path\"]," +
                "\"knownServices\": [{" +
                "\"requiresAssertedLoginIdentity\":false," +
                "\"validateSession\":true," +
                "\"menuTab\":\"None\"," +
                "\"viewMode\":\"AppTab\"," +
                "\"url\":\"www.example.com\"," +
                "\"subServices\":null" +
                "}]" +
                "}")

        val configuration = configurationService.call()

        verify(uiIInteractor, never()).showUnavailabilityError(any())
        assertEquals(configuration,
                Configuration(
                        minimumSupportedAndroidVersion = "v1.27.0",
                        fidoServerUrl = "www.fidoserver.com",
                        nhsLoginLoggedInPaths = listOf("/path"),
                        knownServices = listOf(
                                RootService(
                                        requiresAssertedLoginIdentity = false,
                                        validateSession = true,
                                        menuTab = MenuTab.None,
                                        viewMode = ViewMode.AppTab,
                                        url = "www.example.com",
                                        subServices = null
                                )
                        )
                )
        )
    }

    @Test
    fun getConfigurationResponse_WhenHasRootServiceWitSubServices_ReturnsMappedResponse() {
        whenever(httpClientMock.readText(any())).thenReturn("{ " +
                "\"minimumSupportedAndroidVersion\": \"v1.27.0\"," +
                "\"fidoServerUrl\": \"www.fidoserver.com\"," +
                "\"nhsLoginLoggedInPaths\": [\"/path\"]," +
                "\"knownServices\": [{" +
                "\"requiresAssertedLoginIdentity\":false," +
                "\"validateSession\":true," +
                "\"menuTab\":\"None\"," +
                "\"viewMode\":\"AppTab\"," +
                "\"url\":\"www.example.com\"," +
                "\"subServices\": [{" +
                "\"requiresAssertedLoginIdentity\":true," +
                "\"validateSession\":false," +
                "\"menuTab\":\"Appointments\"," +
                "\"viewMode\":\"WebView\"," +
                "\"path\":\"/path\"," +
                "\"queryString\":null" +
                "}]" +
                "}]" +
                "}")

        val configuration = configurationService.call()

        verify(uiIInteractor, never()).showUnavailabilityError(any())
        assertEquals(configuration,
                Configuration(
                        minimumSupportedAndroidVersion = "v1.27.0",
                        fidoServerUrl = "www.fidoserver.com",
                        nhsLoginLoggedInPaths = listOf("/path"),
                        knownServices = listOf(
                                RootService(
                                        requiresAssertedLoginIdentity = false,
                                        validateSession = true,
                                        menuTab = MenuTab.None,
                                        viewMode = ViewMode.AppTab,
                                        url = "www.example.com",
                                        subServices = listOf(
                                                SubService(
                                                        requiresAssertedLoginIdentity = true,
                                                        validateSession = false,
                                                        menuTab = MenuTab.Appointments,
                                                        viewMode = ViewMode.WebView,
                                                        path = "/path",
                                                        queryString = null
                                                )
                                        )
                                )
                        )
                )
        )
    }

    private fun apiCallFailureError(): ErrorMessage {
        return errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure)
    }
}