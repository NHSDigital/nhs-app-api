package com.nhs.online.nhsonline.support

import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import com.nhs.online.nhsonline.services.knownservices.RootService
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.knownservices.enums.IntegrationLevel
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.services.knownservices.enums.ViewMode
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import java.net.URL

@RunWith(RobolectricTestRunner::class)
class LifeCycleObserverTest : ResourceMockingClass() {

    private lateinit var lifeCycleObserver: LifeCycleObserver
    private lateinit var context: LifeCycleObserverContext
    private lateinit var appWebInterfaceMock: AppWebInterface
    private lateinit var knownServicesMock: KnownServices

    @Before
    fun setUp() {
        context = mock()
        appWebInterfaceMock = mock()
        knownServicesMock = mock()

        whenever(context.getString(R.string.authRedirectPath)).doReturn("/auth-return")
        whenever(context.getString(R.string.fidoAuthQueryKey)).doReturn("fidoAuthResponse")

        lifeCycleObserver = LifeCycleObserver(context, appWebInterfaceMock, knownServicesMock)
    }

    @Test
    fun onMoveToBackground_showsBlankScreen() {
        doNothing().whenever(context).showBlankScreen()
        lifeCycleObserver.onMoveToBackground()
        verify(context).showBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenThereIsNotKnownServiceAndNotCid_hidesBlankScreen() {
        val url = "https://www.notcid.com/"
        doReturn(url).whenever(context).url
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(null)

        lifeCycleObserver.onMoveToForeground()
        verify(context).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsFalseAndNotCid_hidesBlankScreen() {
        val url = "https://www.notcid.com/"
        doReturn(url).whenever(context).url
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = false,
                        menuTab = MenuTab.Unknown,
                        javaScriptInteractionMode = JavaScriptInteractionMode.Unknown,
                        integrationLevel = IntegrationLevel.Unknown,
                        url = url,
                        showSpinner = false,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(context).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsTrueAndNotCid_doesNotHideBlankScreen() {
        val url = "https://www.notcid.com/"
        doReturn(url).whenever(context).url
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.Unknown,
                        javaScriptInteractionMode = JavaScriptInteractionMode.Unknown,
                        integrationLevel = IntegrationLevel.Unknown,
                        url = url,
                        showSpinner = false,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(context, never()).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsTrueAndIsAuthReturnCid_doesNotHideBlankScreen() {
        val url = "https://www.iscid.com/auth-return"
        doReturn(url).whenever(context).url
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.Unknown,
                        javaScriptInteractionMode = JavaScriptInteractionMode.Unknown,
                        integrationLevel = IntegrationLevel.Unknown,
                        url = url,
                        showSpinner = false,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(context, never()).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_whenKnownServiceValidateIsTrueAndIsFidoCid_doesNotHideBlankScreen() {
        val url = "https://www.iscid.com/?fidoAuthResponse"
        doReturn(url).whenever(context).url
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.Unknown,
                        javaScriptInteractionMode = JavaScriptInteractionMode.Unknown,
                        integrationLevel = IntegrationLevel.Unknown,
                        url = url,
                        showSpinner = false,
                        subServices = null
                )
        )

        lifeCycleObserver.onMoveToForeground()
        verify(context, never()).hideBlankScreen()
    }

    @Test
    fun onMoveToForeground_calls_ensureSupportedVersion() {
        doNothing().whenever(context).ensureSupportedVersion()

        lifeCycleObserver.onMoveToForeground()

        verify(context).ensureSupportedVersion()
    }
}
