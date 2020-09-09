package com.nhs.online.nhsonline.services

import android.app.Activity
import android.content.Context
import android.net.Network
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.clients.HttpClient
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.knownservices.RootService
import com.nhs.online.nhsonline.services.knownservices.SubService
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.knownservices.enums.IntegrationLevel
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNull
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import java.io.IOException
import java.net.SocketTimeoutException

@RunWith(RobolectricTestRunner::class)
class ConfigurationServiceTest {

    private lateinit var activity: Activity
    private lateinit var uiIInteractor: IInteractor
    private lateinit var context: Context
    private lateinit var httpClientMock: HttpClient
    private lateinit var configurationService: ConfigurationService
    private lateinit var errorMessageHandler: ErrorMessageHandler
    private lateinit var resourceMockingClass: ResourceMockingClass
    private lateinit var connectionStateMonitor: ConnectionStateMonitor
    private lateinit var network: Network

    @Before
    fun setUp() {
        resourceMockingClass = ResourceMockingClass()
        context = resourceMockingClass.mockConnectedContext()
        errorMessageHandler = ErrorMessageHandler(context.resources)
        httpClientMock = mock()
        uiIInteractor = mock()
        activity = mock()
        network = mock()
        connectionStateMonitor = ConnectionStateMonitor(context)
        connectionStateMonitor.onAvailable(network)
        configurationService = ConfigurationService(activity, "configurationUrlMock",
                uiIInteractor, errorMessageHandler, httpClientMock, connectionStateMonitor)
    }

    @After
    fun tearDown() {
        connectionStateMonitor.onLost(network)
    }

    @Test
    fun getConfigurationResponse_WhenThereIsNoConnection_ItShowsNoConnectionErrorAndReturnsNull() {
        connectionStateMonitor.onLost(network)
        whenever(httpClientMock.readText(any(), any())).thenAnswer { throw IOException() }

        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        val configuration = configurationService.call()

        verify(activity).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(uiIInteractor).showUnavailabilityError(errorMessageHandler.getErrorMessage(ErrorType.NoConnection))
        assertNull(configuration)
    }

    @Test
    fun getConfigurationResponse_WhenThereIsAConnectionTimeout_ItShowsNoConnectionErrorAndReturnsNull() {
        connectionStateMonitor.onLost(network)
        whenever(httpClientMock.readText(any(), any())).thenAnswer { throw SocketTimeoutException() }

        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        val configuration = configurationService.call()

        verify(activity).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(uiIInteractor).showUnavailabilityError(errorMessageHandler.getErrorMessage(ErrorType.NoConnection))
        assertNull(configuration)
    }

    @Test
    fun getConfigurationResponse_WhenResponseIsEmpty_ItShowsApiCallErrorAndReturnsNull() {
        whenever(httpClientMock.readText(any(), any())).thenReturn("")

        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        val configuration = configurationService.call()

        verify(activity).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(uiIInteractor).showUnavailabilityError(apiCallFailureError())
        assertNull(configuration)
    }

    @Test
    fun getConfigurationResponse_WhenResponseIsEmptyObject_ItShowsApiCallErrorAndReturnsNull() {
        whenever(httpClientMock.readText(any(), any())).thenReturn("{}")

        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        val configuration = configurationService.call()

        verify(activity).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(uiIInteractor).showUnavailabilityError(apiCallFailureError())
        assertNull(configuration)
    }

    @Test
    fun getConfigurationResponse_WhenEmptyKnownServices_ReturnsMappedResponse() {
        whenever(httpClientMock.readText(any(), any())).thenReturn("{ " +
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
                        knownServices = emptyList()
                )
        )
    }

    @Test
    fun getConfigurationResponse_WhenHasUnknownEnums_ReturnsMappedResponseWithUnknowns() {
        whenever(httpClientMock.readText(any(), any())).thenReturn("{ " +
                "\"minimumSupportedAndroidVersion\": \"v1.27.0\"," +
                "\"fidoServerUrl\": \"www.fidoserver.com\"," +
                "\"nhsLoginLoggedInPaths\": [\"/path\"]," +
                "\"knownServices\": [{" +
                "\"requiresAssertedLoginIdentity\":false," +
                "\"validateSession\":true," +
                "\"menuTab\":\"NotValidValue\"," +
                "\"viewMode\":\"NotValidValue\"," +
                "\"javaScriptInteractionMode\":\"NotValidValue\"," +
                "\"integrationLevel\":\"Unknown\"," +
                "\"url\":\"www.example.com\"," +
                "\"showSpinner\":false," +
                "\"subServices\":null" +
                "}]" +
                "}")

        val configuration = configurationService.call()

        verify(uiIInteractor, never()).showUnavailabilityError(any())
        assertEquals(configuration,
                Configuration(
                        minimumSupportedAndroidVersion = "v1.27.0",
                        fidoServerUrl = "www.fidoserver.com",
                        knownServices = listOf(
                                RootService(
                                        requiresAssertedLoginIdentity = false,
                                        validateSession = true,
                                        menuTab = MenuTab.Unknown,
                                        javaScriptInteractionMode = JavaScriptInteractionMode.Unknown,
                                        integrationLevel = IntegrationLevel.Unknown,
                                        url = "www.example.com",
                                        showSpinner = false,
                                        subServices = null
                                )
                        )
                )
        )
    }

    @Test
    fun getConfigurationResponse_WhenHasRootServiceWithNoSubServices_ReturnsMappedResponse() {
        whenever(httpClientMock.readText(any(), any())).thenReturn("{ " +
                "\"minimumSupportedAndroidVersion\": \"v1.27.0\"," +
                "\"fidoServerUrl\": \"www.fidoserver.com\"," +
                "\"nhsLoginLoggedInPaths\": [\"/path\"]," +
                "\"knownServices\": [{" +
                "\"requiresAssertedLoginIdentity\":false," +
                "\"validateSession\":true," +
                "\"menuTab\":\"None\"," +
                "\"javaScriptInteractionMode\":\"NhsApp\"," +
                "\"integrationLevel\":\"Bronze\"," +
                "\"url\":\"www.example.com\"," +
                "\"showSpinner\":false," +
                "\"subServices\":null" +
                "}]" +
                "}")

        val configuration = configurationService.call()

        verify(uiIInteractor, never()).showUnavailabilityError(any())
        assertEquals(configuration,
                Configuration(
                        minimumSupportedAndroidVersion = "v1.27.0",
                        fidoServerUrl = "www.fidoserver.com",
                        knownServices = listOf(
                                RootService(
                                        requiresAssertedLoginIdentity = false,
                                        validateSession = true,
                                        menuTab = MenuTab.None,
                                        javaScriptInteractionMode = JavaScriptInteractionMode.NhsApp,
                                        integrationLevel = IntegrationLevel.Bronze,
                                        url = "www.example.com",
                                        showSpinner = false,
                                        subServices = null
                                )
                        )
                )
        )
    }

    @Test
    fun getConfigurationResponse_WhenHasRootServiceWitSubServices_ReturnsMappedResponse() {
        whenever(httpClientMock.readText(any(), any())).thenReturn("{ " +
                "\"minimumSupportedAndroidVersion\": \"v1.27.0\"," +
                "\"fidoServerUrl\": \"www.fidoserver.com\"," +
                "\"nhsLoginLoggedInPaths\": [\"/path\"]," +
                "\"knownServices\": [{" +
                "\"requiresAssertedLoginIdentity\":false," +
                "\"validateSession\":true," +
                "\"menuTab\":\"None\"," +
                "\"javaScriptInteractionMode\":\"NhsApp\"," +
                "\"integrationLevel\":\"Gold\"," +
                "\"url\":\"www.example.com\"," +
                "\"showSpinner\":false," +
                "\"subServices\": [{" +
                "\"requiresAssertedLoginIdentity\":true," +
                "\"validateSession\":false," +
                "\"menuTab\":\"Appointments\"," +
                "\"javaScriptInteractionMode\":\"None\"," +
                "\"integrationLevel\":\"SilverWithWebNavigation\"," +
                "\"path\":\"/path\"," +
                "\"showSpinner\":false," +
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
                        knownServices = listOf(
                                RootService(
                                        requiresAssertedLoginIdentity = false,
                                        validateSession = true,
                                        menuTab = MenuTab.None,
                                        javaScriptInteractionMode = JavaScriptInteractionMode.NhsApp,
                                        integrationLevel = IntegrationLevel.Gold,
                                        url = "www.example.com",
                                        showSpinner = false,
                                        subServices = listOf(
                                                SubService(
                                                        requiresAssertedLoginIdentity = true,
                                                        validateSession = false,
                                                        menuTab = MenuTab.Appointments,
                                                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                                                        integrationLevel = IntegrationLevel.SilverWithWebNavigation,
                                                        path = "/path",
                                                        queryString = null,
                                                        showSpinner = false
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
