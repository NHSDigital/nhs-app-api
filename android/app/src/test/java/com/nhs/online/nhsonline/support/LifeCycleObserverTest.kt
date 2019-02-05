package com.nhs.online.nhsonline.support

import android.content.res.Resources
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IVolleyCallback
import com.nhs.online.nhsonline.services.ConfigurationResponse
import com.nhs.online.nhsonline.services.ConfigurationService
import com.nhs.online.nhsonline.services.KnownService
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.scottyab.rootbeer.RootBeer
import kotlinx.android.synthetic.main.activity_main.*
import org.junit.Test

import org.junit.Assert.*
import org.junit.Before
import org.junit.runner.RunWith
import org.mockito.Mockito.mock
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class LifeCycleObserverTest {

    private val mainActivity: MainActivity =
        Robolectric.buildActivity(MainActivity::class.java).create().get()

    private lateinit var lifeCycleObserver: LifeCycleObserver
    private lateinit var contextSpy: MainActivity
    private lateinit var appWebInterfaceMock: AppWebInterface
    private lateinit var knownServicesMock: KnownServices
    private lateinit var configurationServiceMock: ConfigurationService
    private lateinit var rootBeerServiceMock: RootBeer

    @Before
    fun setUp() {
        contextSpy = spy(mainActivity)

        val r = mock(Resources::class.java)
        whenever(r.getString(any())).thenReturn("true")
        whenever(contextSpy.resources).thenReturn(r)

        appWebInterfaceMock = mock(AppWebInterface::class.java)
        knownServicesMock = mock(KnownServices::class.java)
        configurationServiceMock = mock(ConfigurationService::class.java)
        rootBeerServiceMock = mock(RootBeer::class.java)
        rootBeerServiceMock.setLogging(true)

        lifeCycleObserver = LifeCycleObserver(
            contextSpy,
            appWebInterfaceMock,
            knownServicesMock,
            configurationServiceMock,
            rootBeerServiceMock)
    }

    @Test
    fun onMoveToForeground_notRooted_nullUrl_configurationError() {
        doNothing().whenever(rootBeerServiceMock).setLogging(true)
        whenever(rootBeerServiceMock.isRootedWithoutBusyBoxCheck).thenReturn(false)
        val em = ErrorMessage("my Error")
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onError(em)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(rootBeerServiceMock, times(2)).setLogging(true)
        verify(rootBeerServiceMock, times(1)).isRootedWithoutBusyBoxCheck
        verify(contextSpy, times(1)).showUnavailabilityError(em)
        assertEquals(false, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_RootedDevice() {
        doNothing().whenever(rootBeerServiceMock).setLogging(true)
        whenever(rootBeerServiceMock.isRootedWithoutBusyBoxCheck).thenReturn(true)
        doNothing().whenever(contextSpy).showRootedDeviceDialog()

        lifeCycleObserver.onMoveToForeground()

        verify(rootBeerServiceMock, times(2)).setLogging(true)
        verify(rootBeerServiceMock, times(1)).isRootedWithoutBusyBoxCheck
        verify(contextSpy, times(1)).showRootedDeviceDialog()
    }

    @Test
    fun onMoveToBackground() {
        doNothing().whenever(contextSpy).showBlankScreen()
        lifeCycleObserver.onMoveToBackground()
        verify(contextSpy).showBlankScreen()
    }

    @Test
    fun onMoveToForeground_notRooted_withAuthReturnUrl_configurationError() {
        doNothing().whenever(rootBeerServiceMock).setLogging(true)
        whenever(rootBeerServiceMock.isRootedWithoutBusyBoxCheck).thenReturn(false)

        contextSpy.webview.loadUrl("auth-return")

        val em = ErrorMessage("my Error")
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onError(em)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(rootBeerServiceMock, times(2)).setLogging(true)
        verify(rootBeerServiceMock, times(1)).isRootedWithoutBusyBoxCheck
        verify(contextSpy, times(1)).showUnavailabilityError(em)
        assertEquals(false, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_notRooted_withAUrl_shouldValidateSession_configurationError() {
        doNothing().whenever(rootBeerServiceMock).setLogging(true)
        whenever(rootBeerServiceMock.isRootedWithoutBusyBoxCheck).thenReturn(false)

        val url = "Bazz"
        contextSpy.webview.loadUrl(url)
        val ksi = KnownService.Info(url, ErrorMessage("Danger Will Robinson"), url)
        whenever(knownServicesMock.findMatchingServiceInfo(url)).thenReturn(ksi)
        doNothing().whenever(appWebInterfaceMock).validateSession()

        val em = ErrorMessage("my Error")
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onError(em)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(rootBeerServiceMock, times(2)).setLogging(true)
        verify(rootBeerServiceMock, times(1)).isRootedWithoutBusyBoxCheck
        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(appWebInterfaceMock, times(1)).validateSession()
        verify(contextSpy, times(1)).showUnavailabilityError(em)
        assertEquals(false, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_notRooted_withAUrl_shouldNotValidateSession_configurationError() {
        doNothing().whenever(rootBeerServiceMock).setLogging(true)
        whenever(rootBeerServiceMock.isRootedWithoutBusyBoxCheck).thenReturn(false)

        val url = "Bazz"
        contextSpy.webview.loadUrl(url)
        val ksi = KnownService.Info(url, ErrorMessage("Danger Will Robinson"), url, false)
        whenever(knownServicesMock.findMatchingServiceInfo(url)).thenReturn(ksi)
        doNothing().whenever(contextSpy).hideBlankScreen()

        val em = ErrorMessage("my Error")
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onError(em)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(rootBeerServiceMock, times(2)).setLogging(true)
        verify(rootBeerServiceMock, times(1)).isRootedWithoutBusyBoxCheck
        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(2)).hideBlankScreen()
        verify(contextSpy, times(1)).showUnavailabilityError(em)
        assertEquals(false, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_notRooted_withAUrl_shouldNotValidateSession_configurationSuccessAlreadyChecked() {
        doNothing().whenever(rootBeerServiceMock).setLogging(true)
        whenever(rootBeerServiceMock.isRootedWithoutBusyBoxCheck).thenReturn(false)

        val url = "Bazz"
        contextSpy.webview.loadUrl(url)
        val ksi = KnownService.Info(url, ErrorMessage("Danger Will Robinson"), url, false)
        whenever(knownServicesMock.findMatchingServiceInfo(url)).thenReturn(ksi)
        doNothing().whenever(contextSpy).hideBlankScreen()

        val cr = ConfigurationResponse()
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onSuccess(cr)
        }.whenever(configurationServiceMock).getConfiguration(any())

        contextSpy.isSuccessfulConfigCheck = true


        doNothing().whenever(contextSpy).showVersionUpgradeDialog()

        lifeCycleObserver.onMoveToForeground()

        verify(rootBeerServiceMock, times(2)).setLogging(true)
        verify(rootBeerServiceMock, times(1)).isRootedWithoutBusyBoxCheck
        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(1)).hideBlankScreen()
        verify(contextSpy, times(1)).showVersionUpgradeDialog()
        assertEquals(true, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_notRooted_withAUrl_shouldNotValidateSession_configurationSuccessAlreadyChecked_withValidConfiguration() {
        doNothing().whenever(rootBeerServiceMock).setLogging(true)
        whenever(rootBeerServiceMock.isRootedWithoutBusyBoxCheck).thenReturn(false)

        val url = "Bazz"
        contextSpy.webview.loadUrl(url)
        val ksi = KnownService.Info(url, ErrorMessage("Danger Will Robinson"), url, false)
        whenever(knownServicesMock.findMatchingServiceInfo(url)).thenReturn(ksi)
        doNothing().whenever(contextSpy).hideBlankScreen()

        val cr = ConfigurationResponse()
        cr.isValidConfiguration = true
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onSuccess(cr)
        }.whenever(configurationServiceMock).getConfiguration(any())

        contextSpy.isSuccessfulConfigCheck = true

        lifeCycleObserver.onMoveToForeground()

        verify(rootBeerServiceMock, times(2)).setLogging(true)
        verify(rootBeerServiceMock, times(1)).isRootedWithoutBusyBoxCheck
        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(1)).hideBlankScreen()
        verify(contextSpy, times(1)).hideVersionUpgradeDialog()
        assertEquals(true, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_notRooted_withAUrl_shouldNotValidateSession_noPreviousConfigurationCheck_withValidConfiguration() {
        doNothing().whenever(rootBeerServiceMock).setLogging(true)
        whenever(rootBeerServiceMock.isRootedWithoutBusyBoxCheck).thenReturn(false)

        val url = "Bazz"
        contextSpy.webview.loadUrl(url)
        val ksi = KnownService.Info(url, ErrorMessage("Danger Will Robinson"), url, false)
        whenever(knownServicesMock.findMatchingServiceInfo(url)).thenReturn(ksi)
        doNothing().whenever(contextSpy).hideBlankScreen()

        val cr = ConfigurationResponse()
        cr.isValidConfiguration = true
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onSuccess(cr)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(rootBeerServiceMock, times(2)).setLogging(true)
        verify(rootBeerServiceMock, times(1)).isRootedWithoutBusyBoxCheck
        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(1)).hideBlankScreen()
        verify(contextSpy, times(1)).loadAuthReturnOrWelcomePage()
        verify(contextSpy, times(1)).hideVersionUpgradeDialog()
        assertEquals(true, contextSpy.isSuccessfulConfigCheck)
    }

    @Test
    fun onMoveToForeground_notRooted_withAUrl_shouldNotValidateSession_noPreviousConfigurationCheck_withValidConfiguration_throttlingEnable() {
        doNothing().whenever(rootBeerServiceMock).setLogging(true)
        whenever(rootBeerServiceMock.isRootedWithoutBusyBoxCheck).thenReturn(false)

        val url = "Bazz"
        val fidoServerUrl = "https://test@test.com"
        contextSpy.webview.loadUrl(url)
        val ksi = KnownService.Info(url, ErrorMessage("Danger Will Robinson"), url, false)
        whenever(knownServicesMock.findMatchingServiceInfo(url)).thenReturn(ksi)
        doNothing().whenever(contextSpy).hideBlankScreen()

        val cr = ConfigurationResponse()
        cr.isValidConfiguration = true
        cr.isThrottlingEnabled = true
        cr.fidoServerUrl = fidoServerUrl
        doAnswer {
            val callback = it.arguments[0] as IVolleyCallback
            callback.onSuccess(cr)
        }.whenever(configurationServiceMock).getConfiguration(any())

        lifeCycleObserver.onMoveToForeground()

        verify(rootBeerServiceMock, times(2)).setLogging(true)
        verify(rootBeerServiceMock, times(1)).isRootedWithoutBusyBoxCheck
        verify(knownServicesMock, times(1)).findMatchingServiceInfo(url)
        verify(contextSpy, times(1)).hideBlankScreen()
        verify(contextSpy, times(1)).loadThrottlingCarousel()
        verify(contextSpy, times(1)).configBiometricSetup(fidoServerUrl)
        verify(contextSpy, times(1)).hideVersionUpgradeDialog()
        assertEquals(true, contextSpy.isSuccessfulConfigCheck)
    }
}