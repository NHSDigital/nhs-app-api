package com.nhs.online.nhsonline.support

import android.webkit.WebView
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IVolleyCallback
import com.nhs.online.nhsonline.network.MockConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.ConfigurationResponse
import com.nhs.online.nhsonline.services.ConfigurationService
import com.nhs.online.nhsonline.services.KnownService
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.web.NhsWeb
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import kotlinx.android.synthetic.main.activity_main.*
import org.junit.Test

import org.junit.Assert.*
import org.junit.Before
import org.junit.runner.RunWith
import org.mockito.Mockito.mock
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.util.ReflectionHelpers

@RunWith(RobolectricTestRunner::class)
class LifeCycleObserverTest {

    private val mainActivity: MainActivity =
        Robolectric.buildActivity(MainActivity::class.java).create().get()
    private lateinit var lifeCycleObserver: LifeCycleObserver
    private lateinit var contextSpy: MainActivity
    private lateinit var appWebInterfaceMock: AppWebInterface
    private lateinit var configurationServiceMock: ConfigurationService
    private lateinit var appDialogsMock: AppDialogs
    private lateinit var nhsWebMock: NhsWeb
    private lateinit var errorMessageHandler: ErrorMessageHandler

    @Before
    fun setUp() {
        contextSpy = spy(mainActivity)

        val r = spy(mainActivity.resources)
        whenever(contextSpy.resources).thenReturn(r)
        errorMessageHandler = ErrorMessageHandler(contextSpy)
        appWebInterfaceMock = mock(AppWebInterface::class.java)
        configurationServiceMock = mock(ConfigurationService::class.java)
        appDialogsMock = mock()
        val webViewMock: WebView = mock {
            on { settings }.thenReturn(mock())
        }
        val nhsWeb = NhsWeb(contextSpy, contextSpy, webViewMock, mock(), appWebInterfaceMock)
        nhsWebMock = spy(nhsWeb)

        lifeCycleObserver = LifeCycleObserver(contextSpy, appWebInterfaceMock,
            nhsWebMock, appDialogsMock)
        ReflectionHelpers.setField(lifeCycleObserver,
            "configurationService",
            configurationServiceMock)
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
    }

    @Test
    fun onMoveToForeground_nullUrl_configurationError() {
        val em = errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure)
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onError(em)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(contextSpy, times(1)).showUnavailabilityError(em)
        assertEquals(false, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToBackground() {
        doNothing().whenever(contextSpy).showBlankScreen()
        lifeCycleObserver.onMoveToBackground()
        verify(contextSpy).showBlankScreen()
    }

    @Test
    fun onMoveToForeground_withNullUrl_configurationError() {
        contextSpy.webview.loadUrl(null)

        val em = errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure)
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onError(em)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(contextSpy).showUnavailabilityError(em)
        assertEquals(false, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_withAUrl_shouldValidateSession_configurationError() {
        val url = "Bazz"
        val knownServicesMock: KnownServices = overrideKnownServicesField(url)

        contextSpy.webview.loadUrl(url)

        doNothing().whenever(appWebInterfaceMock).validateSession(any())

        val em = errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure)
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onError(em)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(appWebInterfaceMock, times(1)).validateSession(any())
        verify(contextSpy, times(1)).showUnavailabilityError(em)
        assertEquals(false, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_withAUrl_shouldNotValidateSession_configurationError() {
        val url = "Bazz"
        val knownServicesMock: KnownServices = overrideKnownServicesField(url)

        contextSpy.webview.loadUrl(url)
        doNothing().whenever(contextSpy).hideBlankScreen()
        doAnswer {
            val callback = it.arguments[0] as () -> Unit
            callback.invoke()
        }.whenever(appWebInterfaceMock).validateSession(any())

        val em = errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure)
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onError(em)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(2)).hideBlankScreen()
        verify(contextSpy, times(1)).showUnavailabilityError(em)
        assertEquals(false, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_withAUrl_shouldNotValidateSession_configurationSuccessAlreadyChecked() {
        val url = "Bazz"
        val knownServicesMock: KnownServices = overrideKnownServicesField(url, false)

        contextSpy.webview.loadUrl(url)
        doNothing().whenever(contextSpy).hideBlankScreen()

        val cr = ConfigurationResponse()
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onSuccess(cr)
        }.whenever(configurationServiceMock).getConfiguration(any())

        contextSpy.isSuccessfulConfigCheck = true


        doNothing().whenever(appDialogsMock).showVersionUpgradeDialog()

        lifeCycleObserver.onMoveToForeground()

        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(1)).hideBlankScreen()
        verify(appDialogsMock, times(0)).showVersionUpgradeDialog()
        assertEquals(true, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_withAUrl_shouldNotValidateSession_configurationSuccessAlreadyChecked_withValidConfiguration() {
        val url = "Bazz"
        val knownServicesMock: KnownServices = overrideKnownServicesField(url, false)

        contextSpy.webview.loadUrl(url)
        doNothing().whenever(contextSpy).hideBlankScreen()

        val cr = ConfigurationResponse()
        cr.isValidConfiguration = true
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onSuccess(cr)
        }.whenever(configurationServiceMock).getConfiguration(any())

        contextSpy.isSuccessfulConfigCheck = true

        lifeCycleObserver.onMoveToForeground()

        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(1)).hideBlankScreen()
        verify(appDialogsMock, times(0)).showVersionUpgradeDialog()
        assertEquals(true, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_withAUrl_shouldNotValidateSession_noPreviousConfigurationCheck_withValidConfiguration() {
        val url = "Bazz"
        val knownServicesMock: KnownServices = overrideKnownServicesField(url, false)

        contextSpy.webview.loadUrl(url)
        doNothing().whenever(contextSpy).hideBlankScreen()

        val cr = ConfigurationResponse()
        cr.isValidConfiguration = true
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onSuccess(cr)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(1)).hideBlankScreen()
        verify(nhsWebMock, times(1)).loadWelcomePage()
        verify(appDialogsMock, times(0)).showVersionUpgradeDialog()
        assertEquals(true, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_withAUrl_shouldNotValidateSession_noPreviousConfigurationCheck_withInvalidConfiguration_showVersionUpgradeDialog() {
        val url = "Bazz"
        val knownServicesMock: KnownServices = overrideKnownServicesField(url, false)

        contextSpy.webview.loadUrl(url)
        doNothing().whenever(contextSpy).hideBlankScreen()

        val fidoServerUrl = "https://test@test.com"
        val cr = ConfigurationResponse()
        cr.isValidConfiguration = false
        cr.fidoServerUrl = fidoServerUrl
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onSuccess(cr)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(1)).hideBlankScreen()
        verify(contextSpy, times(1)).configBiometricSetup(fidoServerUrl)
        verify(appDialogsMock, times(1)).showVersionUpgradeDialog()
        assertEquals(false, contextSpy.isSuccessfulConfigCheck)
    }

    private fun overrideKnownServicesField(
        url: String, shouldValidateSession: Boolean = true
    ): KnownServices {
        val ksi =
            KnownService.Info(url, url, shouldValidateSession)
        val knownServicesMock: KnownServices = mock {
            on { findMatchingServiceInfo(url) }.thenReturn(ksi)
        }
        ReflectionHelpers.setField(lifeCycleObserver, "knownServices", knownServicesMock)
        return knownServicesMock
    }
}