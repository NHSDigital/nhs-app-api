package com.nhs.online.nhsonline.support

import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import com.nhs.online.nhsonline.services.knownservices.RootService
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.services.knownservices.enums.ViewMode
import com.nhs.online.nhsonline.web.NhsWebView
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import kotlinx.android.synthetic.main.activity_main.*
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import java.net.URL

@RunWith(RobolectricTestRunner::class)
class LifeCycleObserverTest : ResourceMockingClass() {

    private val mainActivity = Robolectric.buildActivity(MainActivity::class.java).create().get()
    private lateinit var spyMainActivity: MainActivity
    private lateinit var lifeCycleObserver: LifeCycleObserver
    private lateinit var webViewMock: NhsWebView
    private lateinit var appWebInterfaceMock: AppWebInterface
    private lateinit var knownServicesMock: KnownServices
    private lateinit var appDialogsMock: AppDialogs
    private lateinit var configurationResponseMock: ConfigurationResponse

    @Before
    fun setUp() {
        webViewMock = mock()
        appDialogsMock = mock()
        configurationResponseMock = mock()

        spyMainActivity = spy(mainActivity)
        whenever(spyMainActivity.configurationResponse).thenReturn(configurationResponseMock)
        whenever(spyMainActivity.appDialogs).thenReturn(appDialogsMock)

        appWebInterfaceMock = mock()
        knownServicesMock = mock()

        lifeCycleObserver = LifeCycleObserver(spyMainActivity, appWebInterfaceMock, knownServicesMock)
    }

    @Test
    fun onMoveToBackground_showsBlankScreen() {
        doNothing().whenever(spyMainActivity).showBlankScreen()
        lifeCycleObserver.onMoveToBackground()
        verify(spyMainActivity).showBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenThereIsNotKnownServiceAndNotCid_hidesBlankScreen() {
        val url = "https://www.notcid.com/"
        spyMainActivity.webview.loadUrl(url)
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(null)

        lifeCycleObserver.onMoveToForeground()
        verify(spyMainActivity).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsFalseAndNotCid_hidesBlankScreen() {
        val url = "https://www.notcid.com/"
        spyMainActivity.webview.loadUrl(url)
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = false,
                        menuTab = MenuTab.Unknown,
                        viewMode = ViewMode.Unknown,
                        url = url,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(spyMainActivity).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsTrueAndNotCid_doesNotHideBlankScreen() {
        val url = "https://www.notcid.com/"
        spyMainActivity.webview.loadUrl(url)
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.Unknown,
                        viewMode = ViewMode.Unknown,
                        url = url,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(spyMainActivity, never()).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsFalseAndIsAuthReturnCid_doesNotHideBlankScreen() {
        val url = "https://www.iscid.com/auth-return"
        spyMainActivity.webview.loadUrl(url)
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = false,
                        menuTab = MenuTab.Unknown,
                        viewMode = ViewMode.Unknown,
                        url = url,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(spyMainActivity, never()).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsFalseAndIsFidoCid_doesNotHideBlankScreen() {
        val url = "https://www.iscid.com/?fidoAuthResponse"
        spyMainActivity.webview.loadUrl(url)
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = false,
                        menuTab = MenuTab.Unknown,
                        viewMode = ViewMode.Unknown,
                        url = url,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(spyMainActivity, never()).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsTrueAndIsAuthReturnCid_doesNotHideBlankScreen() {
        val url = "https://www.iscid.com/auth-return"
        spyMainActivity.webview.loadUrl(url)
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.Unknown,
                        viewMode = ViewMode.Unknown,
                        url = url,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(spyMainActivity, never()).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsTrueAndIsFidoCid_doesNotHideBlankScreen() {
        val url = "https://www.iscid.com/?fidoAuthResponse"
        spyMainActivity.webview.loadUrl(url)
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.Unknown,
                        viewMode = ViewMode.Unknown,
                        url = url,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(spyMainActivity, never()).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_SupportedVersionFalse_DialogActiveFalse() {
        whenever(configurationResponseMock.isSupportedVersion).thenReturn(false)
        whenever(appDialogsMock.isUpgradeDialogActive()).thenReturn(false)

        doNothing().whenever(appDialogsMock).showVersionUpgradeDialog()

        lifeCycleObserver.onMoveToForeground()

        verify(appDialogsMock).showVersionUpgradeDialog()
    }

    @Test
    fun onMoveToForeground_SupportedVersionTrue_DialogActiveFalse() {
        whenever(configurationResponseMock.isSupportedVersion).thenReturn(true)
        whenever(appDialogsMock.isUpgradeDialogActive()).thenReturn(false)

        lifeCycleObserver.onMoveToForeground()

        verify(appDialogsMock, never()).showVersionUpgradeDialog()
    }

    @Test
    fun onMoveToForeground_SupportedVersionFalse_DialogActiveTrue() {
        whenever(configurationResponseMock.isSupportedVersion).thenReturn(false)
        whenever(appDialogsMock.isUpgradeDialogActive()).thenReturn(true)

        lifeCycleObserver.onMoveToForeground()

        verify(appDialogsMock, never()).showVersionUpgradeDialog()
    }

    @Test
    fun onMoveToForeground_SupportedVersionTrue_DialogActiveTrue() {
        whenever(configurationResponseMock.isSupportedVersion).thenReturn(true)
        whenever(appDialogsMock.isUpgradeDialogActive()).thenReturn(true)

        lifeCycleObserver.onMoveToForeground()

        verify(appDialogsMock, never()).showVersionUpgradeDialog()
    }
}