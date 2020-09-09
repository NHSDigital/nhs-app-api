package com.nhs.online.nhsonline.webclients

import android.app.Activity
import android.content.Context
import android.content.res.AssetManager
import android.content.res.Resources
import android.net.Network
import android.net.Uri
import android.webkit.WebResourceError
import android.webkit.WebResourceRequest
import android.webkit.WebResourceResponse
import android.webkit.WebView
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.logging.LoggingService
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import com.nhs.online.nhsonline.services.knownservices.RootService
import com.nhs.online.nhsonline.services.knownservices.SubService
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.knownservices.enums.IntegrationLevel
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.support.schemehandlers.SchemeHandlers
import com.nhs.online.nhsonline.web.NhsWeb
import org.junit.Assert.assertEquals
import org.junit.Ignore
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.Mockito
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import java.io.InputStream
import java.net.URL

@Suppress("DEPRECATION")
@RunWith(RobolectricTestRunner::class)
class WebClientInterceptorTest {

    @Test
    fun shouldInterceptRequest_NullRequest_returnNull() {
        val webClientInterceptor: WebClientInterceptor
        val connectionStateMonitor: ConnectionStateMonitor

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        webClientInterceptor = WebClientInterceptor(uiInteractorMock,
                nhsWebMock, contextMock, knownServicesMock, schemeHandlersMock, loggingServiceMock,
                connectionStateMonitor)

        val request: WebResourceRequest? = null
        val response = webClientInterceptor.shouldInterceptRequest(webViewMock, request)

        assertEquals(null, response)
    }

    @Test
    fun shouldInterceptRequest_nonWOFF2Request_returnNull() {
        val webClientInterceptor: WebClientInterceptor
        var requestMock: WebResourceRequest
        val connectionStateMonitor: ConnectionStateMonitor

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        webClientInterceptor = WebClientInterceptor(uiInteractorMock,
                nhsWebMock, contextMock, knownServicesMock, schemeHandlersMock, loggingServiceMock,
                connectionStateMonitor)

        val testRequestPaths = arrayOf(
                "/",
                "https://www.bazz.com/file.js",
                "https://www.bazz.com/file.woff",
                "https://www.bazz.com/file.css",
                "https://www.bazz.com/file.woff",
                "https://www.bazz.com/file.html"
        )

        testRequestPaths.forEach { i ->
            requestMock = mock {
                on { url } doReturn Uri.parse(i)
            }

            val response = webClientInterceptor.shouldInterceptRequest(webViewMock, requestMock)

            assertEquals(null, response)
            verify(requestMock, times(2)).url
        }
    }

    @Test
    fun shouldInterceptRequest_WOFF2Request_notAppFont_returnNull() {
        val webClientInterceptor: WebClientInterceptor
        val connectionStateMonitor: ConnectionStateMonitor

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        webClientInterceptor = WebClientInterceptor(uiInteractorMock,
                nhsWebMock, contextMock, knownServicesMock, schemeHandlersMock, loggingServiceMock,
                connectionStateMonitor)

        val requestMock: WebResourceRequest = mock {
            on { url } doReturn Uri.parse("https://www.bazz.com/file.woff2")
        }

        val appFonts = arrayOf("bazz.woff2", "bizz.woff2")
        val resourceMock = mock<Resources> {
            on { getStringArray(R.array.fonts) } doReturn appFonts
        }

        whenever(contextMock.resources).thenReturn(resourceMock)

        val response = webClientInterceptor.shouldInterceptRequest(webViewMock, requestMock)

        assertEquals(null, response)
        verify(requestMock, times(2)).url
        verify(contextMock, atLeastOnce()).resources
        verify(resourceMock).getStringArray(R.array.fonts)
    }

    @Test
    fun shouldInterceptRequest_WOFF2Request_AppFont_returnBundle() {
        val webClientInterceptor: WebClientInterceptor
        val connectionStateMonitor: ConnectionStateMonitor

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        webClientInterceptor = WebClientInterceptor(uiInteractorMock,
                nhsWebMock, contextMock, knownServicesMock, schemeHandlersMock, loggingServiceMock,
                connectionStateMonitor)

        val requestMock: WebResourceRequest = mock {
            on { url } doReturn Uri.parse("https://www.bazz.com/bazz.woff2")
        }

        val appFonts = arrayOf("bazz.woff2", "bizz.woff2")
        val resourceMock = mock<Resources> {
            on { getStringArray(R.array.fonts) } doReturn appFonts
        }

        val inputStreamMock: InputStream = mock()
        val assetManagerMock = mock<AssetManager> {
            on { open("fonts/bazz.woff2") } doReturn inputStreamMock
        }

        whenever(contextMock.resources).thenReturn(resourceMock)
        whenever(contextMock.assets).thenReturn(assetManagerMock)

        val response = webClientInterceptor.shouldInterceptRequest(webViewMock, requestMock)

        assertEquals(inputStreamMock, response!!.data)
        assertEquals("woff2", response.encoding)
        assertEquals("application/font-woff2", response.mimeType)
        verify(requestMock).url
        verify(contextMock, atLeastOnce()).resources
        verify(contextMock).assets
        verify(resourceMock).getStringArray(R.array.fonts)
        verify(assetManagerMock).open("fonts/bazz.woff2")
    }

    @Test
    fun shouldInterceptRequest_WOFF2Request_AppFont_CaseInsensitive_returnBundle() {
        val webClientInterceptor: WebClientInterceptor
        val connectionStateMonitor: ConnectionStateMonitor

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        webClientInterceptor = WebClientInterceptor(uiInteractorMock,
                nhsWebMock, contextMock, knownServicesMock, schemeHandlersMock, loggingServiceMock,
                connectionStateMonitor)

        val requestMock: WebResourceRequest = mock {
            on { url } doReturn Uri.parse("https://www.bazz.com/bazz.woFF2")
        }

        val appFonts = arrayOf("bazz.woff2", "bizz.woff2")
        val resourceMock = mock<Resources> {
            on { getStringArray(R.array.fonts) } doReturn appFonts
        }

        val inputStreamMock: InputStream = mock()
        val assetManagerMock = mock<AssetManager> {
            on { open("fonts/bazz.woff2") } doReturn inputStreamMock
        }

        whenever(contextMock.resources).thenReturn(resourceMock)
        whenever(contextMock.assets).thenReturn(assetManagerMock)

        val response = webClientInterceptor.shouldInterceptRequest(webViewMock, requestMock)

        assertEquals(inputStreamMock, response!!.data)
        assertEquals("woff2", response.encoding)
        assertEquals("application/font-woff2", response.mimeType)
        verify(requestMock).url
        verify(contextMock, atLeastOnce()).resources
        verify(contextMock).assets
        verify(resourceMock).getStringArray(R.array.fonts)
        verify(assetManagerMock).open("fonts/bazz.woff2")
    }

    @Test
    @Ignore("Create MainActivity is too slow")
    fun overrideUrlLoad() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)

        assert(webInterceptor.shouldOverrideUrlLoading(tmpWebView,
                "https://ndopapp-int1.thunderbird.service.nhs.uk/")) {
            "WebClientInterceptor: Failed to override data preferences URL"
        }

        assert(webInterceptor.shouldOverrideUrlLoading(tmpWebView, "http://www.google.com")) {
            "WebClientInterceptor: Failed to override non-known URL"
        }
    }

    @Test
    @Ignore("Create MainActivity is too slow")
    fun overrideUrlLoad_returnsTrueForHandledScheme() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val loggingServiceMock: LoggingService = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val url = "handled:"
        //schemeHandlersMock = mock { on { handleUrl(url) } doReturn true }
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)

        assert(webInterceptor.shouldOverrideUrlLoading(tmpWebView,
                url)) {
            "WebClientInterceptor: Failed to handle url scheme"
        }
        verify(schemeHandlersMock).handleUrl(url)
    }

    @Test
    @Ignore("Create MainActivity is too slow")
    fun overrideUrlLoad_returnsFalseForMalformedURLException() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)
        val response = webInterceptor.shouldOverrideUrlLoading(tmpWebView, "")

        assertEquals(false, response)
    }

    @Test
    fun overrideUrlLoad_knownServicesSpinnerShown() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = resourceMock.mockContext()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val url = "https://111.nhs.uk/"
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.None,
                        integrationLevel = IntegrationLevel.Gold,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        url = url,
                        showSpinner = true,
                        subServices = null
                )
        )

        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)
        tmpWebView.loadUrl("https://google.com")
        webInterceptor.shouldOverrideUrlLoading(tmpWebView, url)

        verify(uiInteractorMock, times(1)).showProgressDialog()
    }

    @Test
    @Ignore("Passing locally but fails in azure, Needs further investigation")
    fun overrideUrlLoad_knownServicesSpinnerShownWithDifferentCasing() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()

        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = resourceMock.mockContext()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val url = "https://111.nhs.uk/"
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.None,
                        integrationLevel = IntegrationLevel.Gold,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        url = url,
                        showSpinner = true,
                        subServices = null
                )
        )

        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)

        Mockito.clearInvocations(uiInteractorMock)

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)
        tmpWebView.loadUrl("https://google.com")
        webInterceptor.shouldOverrideUrlLoading(tmpWebView, "httPs://111.nHs.uK/")

        verify(uiInteractorMock, times(1)).showProgressDialog()
    }

    @Test
    fun overrideUrlLoad_knownServicesSpinnerNotShownWhenConfigSetToFalse() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val url = "https://111.nhs.uk/"
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.None,
                        integrationLevel = IntegrationLevel.Gold,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        url = url,
                        showSpinner = false,
                        subServices = null
                )
        )

        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)
        tmpWebView.loadUrl("https://google.com")
        webInterceptor.shouldOverrideUrlLoading(tmpWebView, url)

        verify(uiInteractorMock, never()).showProgressDialog()
    }

    @Test
    fun overrideUrlLoad_knownServicesSpinnerNotShownWithUrlMismatch() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val url = "https://111.nhs.uk/"
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.None,
                        integrationLevel = IntegrationLevel.Gold,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        url = url,
                        showSpinner = true,
                        subServices = null
                )
        )

        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)
        tmpWebView.loadUrl(url)
        webInterceptor.shouldOverrideUrlLoading(tmpWebView, "https://google.com/")

        verify(uiInteractorMock, never()).showProgressDialog()
    }

    @Test
    fun overrideUrlLoad_knownServicesSpinnerNotShownWithNoBaseUrlChange() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val url = "https://111.nhs.uk/"
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.None,
                        integrationLevel = IntegrationLevel.Gold,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        url = url,
                        showSpinner = true,
                        subServices = null
                )
        )

        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)
        tmpWebView.loadUrl(url)
        webInterceptor.shouldOverrideUrlLoading(tmpWebView, url)

        verify(uiInteractorMock, never()).showProgressDialog()
    }

    @Test
    fun onPageStartedNoConnection() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)

        connectionStateMonitor.onLost(network)

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val url = "https://111.nhs.uk/"
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.None,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        integrationLevel = IntegrationLevel.SilverWithWebNavigation,
                        url = url,
                        showSpinner = false,
                        subServices = null
                )
        )

        webInterceptor.onPageStarted(webViewMock, url, null)

        verify(webViewMock).stopLoading()

    }

    @Test
    fun onPageStartedConnectedKnownURL() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockConnectedContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val url = "https://test.com"
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                SubService(
                        requiresAssertedLoginIdentity = false,
                        validateSession = false,
                        menuTab = MenuTab.None,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        integrationLevel = IntegrationLevel.SilverWithoutWebNavigation,
                        path = "/conditions",
                        showSpinner = false,
                        queryString = null
                )
        )

        webInterceptor.onPageStarted(
                webViewMock,
                url,
                null
        )
    }

    @Test
    fun onPageFinished_knownServicesSpinnerHidden() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val url = "https://111.nhs.uk/"
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.None,
                        integrationLevel = IntegrationLevel.Gold,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        url = url,
                        showSpinner = true,
                        subServices = null
                )
        )

        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)
        webInterceptor.onPageFinished(tmpWebView, url)

        verify(uiInteractorMock, times(2)).dismissProgressDialog()
    }

    @Test
    fun onPageFinished_knownServicesSpinnerNotHidden() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val url = "https://111.nhs.uk/"
        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.None,
                        integrationLevel = IntegrationLevel.Gold,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        url = url,
                        showSpinner = false,
                        subServices = null
                )
        )

        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock(), mock(), mock(), mock(),
                connectionStateMonitor)
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val tmpWebView = createWebView(activity)
        webInterceptor.onPageFinished(tmpWebView, url)

        verify(uiInteractorMock, times(1)).dismissProgressDialog()
    }

    @Test
    fun stopLoadingWebViewTest() {
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        connectionStateMonitor.onLost(network)

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        webInterceptor.stopLoadingWebviewAndShowNoConnectionError(webViewMock)

        verify(webViewMock).stopLoading()
    }

    @Test
    fun receivedErrorNonKnownURL() {
        val errorMessageHandler: ErrorMessageHandler
        val connectionStateMonitor: ConnectionStateMonitor
        val resourceMock = ResourceMockingClass()

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        errorMessageHandler = ErrorMessageHandler(contextMock.resources)
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        webInterceptor.onReceivedError(webViewMock, 404,
                "Error", "https://google.com")

        val knownUrlErrorMessage = errorMessageHandler.getErrorMessage(ErrorType.ServiceUnavailable)
        verify(uiInteractorMock, never()).dismissProgressDialog()
        verify(uiInteractorMock, never()).showUnavailabilityError(knownUrlErrorMessage)
    }

    @Test
    fun receivedErrorKnownURLToBeOpenedInWebview() {
        val errorMessageHandler: ErrorMessageHandler
        val connectionStateMonitor: ConnectionStateMonitor

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        errorMessageHandler = ErrorMessageHandler(contextMock.resources)
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                contextMock,
                knownServicesMock,
                schemeHandlersMock,
                loggingServiceMock,
                connectionStateMonitor
        )

        val url = "https://111.nhs.uk"
        whenever(contextMock.getString(R.string.baseHost)).thenReturn("111.nhs.uk")
        whenever(webViewMock.url).thenReturn(url)

        whenever(knownServicesMock.findMatchingKnownService(URL(url))).thenReturn(
                RootService(
                        requiresAssertedLoginIdentity = true,
                        validateSession = true,
                        menuTab = MenuTab.None,
                        javaScriptInteractionMode = JavaScriptInteractionMode.None,
                        integrationLevel = IntegrationLevel.SilverWithWebNavigation,
                        url = url,
                        showSpinner = false,
                        subServices = null
                )
        )

        webInterceptor.onReceivedError(webViewMock, 404, "Error", url)

        val knownUrlErrorMessage = errorMessageHandler.getErrorMessage(ErrorType.ServiceUnavailable)

        verify(uiInteractorMock).showUnavailabilityError(knownUrlErrorMessage)
    }

    @Test
    fun onReceivedHttpError_logsApiError() {
        val webClientInterceptor: WebClientInterceptor
        val connectionStateMonitor: ConnectionStateMonitor

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        webClientInterceptor = WebClientInterceptor(uiInteractorMock,
                nhsWebMock, contextMock, knownServicesMock, schemeHandlersMock, loggingServiceMock,
                connectionStateMonitor)

        val requestMock: WebResourceRequest = mock {
            on { url } doReturn Uri.parse("https://www.example.com/")
        }

        val resourceResponseMock: WebResourceResponse = mock {
            on { statusCode } doReturn 1234
        }

        webClientInterceptor.onReceivedHttpError(webViewMock, requestMock, resourceResponseMock)

        verify(loggingServiceMock).logError("Failed HTTP Call from webview. url:https://www.example.com/ httpResponseCode:1234")
    }

    @Test
    fun onReceivedError_logsApiError() {
        val webClientInterceptor: WebClientInterceptor
        val connectionStateMonitor: ConnectionStateMonitor

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        webClientInterceptor = WebClientInterceptor(uiInteractorMock,
                nhsWebMock, contextMock, knownServicesMock, schemeHandlersMock, loggingServiceMock,
                connectionStateMonitor)

        val requestMock: WebResourceRequest = mock {
            on { url } doReturn Uri.parse("https://www.example.com/")
        }

        val resourceErrorMock: WebResourceError = mock {
            on { errorCode } doReturn 1234
        }

        webClientInterceptor.onReceivedError(webViewMock, requestMock, resourceErrorMock)

        verify(loggingServiceMock).logError("Failed HTTP Call from webview. url:https://www.example.com/ AndroidError:1234")
    }

    @Test
    fun onReceivedError_olderAndroidVersions_logsApiError() {
        val webClientInterceptor: WebClientInterceptor
        val connectionStateMonitor: ConnectionStateMonitor

        val uiInteractorMock: IInteractor = mock()
        val knownServicesMock: KnownServices = mock()
        val contextMock: Context = ResourceMockingClass().mockContext()
        val nhsWebMock: NhsWeb = mock()
        val schemeHandlersMock: SchemeHandlers = mock { on { handleUrl(any()) } doReturn false }
        val loggingServiceMock: LoggingService = mock()
        val webViewMock: WebView = mock()

        val network: Network = mock()
        connectionStateMonitor = ConnectionStateMonitor(contextMock)
        connectionStateMonitor.onAvailable(network)

        webClientInterceptor = WebClientInterceptor(uiInteractorMock,
                nhsWebMock, contextMock, knownServicesMock, schemeHandlersMock, loggingServiceMock,
                connectionStateMonitor)

        webClientInterceptor.onReceivedError(webViewMock, 1234, null, "https://www.example.com/")

        verify(loggingServiceMock).logError("Failed HTTP Call from webview. url:https://www.example.com/ AndroidError:1234")
    }

    private fun createWebView(activity: Activity): WebView {
        return WebView(activity)
    }
}
