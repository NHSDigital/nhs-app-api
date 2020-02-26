package com.nhs.online.nhsonline.webclients

import android.app.Activity
import android.content.Context
import android.content.res.AssetManager
import android.content.res.Resources
import android.net.Uri
import android.webkit.WebResourceRequest
import android.webkit.WebView
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.MockConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.KnownService
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.support.schemehandlers.SchemeHandlers
import com.nhs.online.nhsonline.web.NhsWeb
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.junit.Assert.assertEquals
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import java.io.InputStream
import java.net.URL


@Suppress("DEPRECATION")
@RunWith(RobolectricTestRunner::class)
class WebClientInterceptorTest {

    private lateinit var webClientInterceptor: WebClientInterceptor
    private lateinit var uiInteractorMock: IInteractor
    private lateinit var knownServicesMock: KnownServices
    private lateinit var contextMock: Context
    private lateinit var webViewMock: WebView
    private lateinit var nhsWebMock: NhsWeb
    private lateinit var requestMock: WebResourceRequest
    private lateinit var schemeHandlersMock: SchemeHandlers
    private lateinit var errorMessageHandler: ErrorMessageHandler
    private val resourceMock = ResourceMockingClass()


    @Before
    fun setUp() {
        uiInteractorMock = mock()
        knownServicesMock = mock()
        contextMock = ResourceMockingClass().mockContext()
        errorMessageHandler = ErrorMessageHandler(contextMock)
        nhsWebMock = mock()
        schemeHandlersMock = mock { on { handleUrl(any()) } doReturn false }
        webClientInterceptor = WebClientInterceptor(uiInteractorMock,
                nhsWebMock, contextMock, knownServicesMock, schemeHandlersMock)

        webViewMock = mock()
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
    }

    @Test
    fun shouldInterceptRequest_NullRequest_returnNull() {

        val request: WebResourceRequest? = null
        val response = webClientInterceptor.shouldInterceptRequest(webViewMock, request)

        assertEquals(null, response)
    }

    @Test
    fun shouldInterceptRequest_nonWOFF2Request_returnNull() {

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
                on { url } doReturn Uri.Builder().path(i).build()
            }

            val response = webClientInterceptor.shouldInterceptRequest(webViewMock, requestMock)

            assertEquals(null, response)
            verify(requestMock, times(2)).url
        }
    }


    @Test
    fun shouldInterceptRequest_WOFF2Request_notAppFont_returnNull() {

        requestMock = mock {
            on { url } doReturn Uri.Builder().path("https://www.bazz.com/file.woff2").build()
        }

        val appFonts = arrayOf("bazz.woff2", "bizz.woff2")
        val resourceMock = mock<Resources> {
            on { getStringArray(R.array.fonts) } doReturn appFonts
        }

        whenever(contextMock.resources).thenReturn(resourceMock)

        val response = webClientInterceptor.shouldInterceptRequest(webViewMock, requestMock)

        assertEquals(null, response)
        verify(requestMock, times(2)).url
        verify(contextMock).resources
        verify(resourceMock).getStringArray(R.array.fonts)
    }

    @Test
    fun shouldInterceptRequest_WOFF2Request_AppFont_returnBundle() {

        requestMock = mock {
            on { url } doReturn Uri.Builder().path("https://www.bazz.com/bazz.woff2").build()
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
        verify(contextMock).resources
        verify(contextMock).assets
        verify(resourceMock).getStringArray(R.array.fonts)
        verify(assetManagerMock).open("fonts/bazz.woff2")
    }


    @Test
    fun shouldInterceptRequest_WOFF2Request_AppFont_CaseInsensitive_returnBundle() {

        requestMock = mock {
            on { url } doReturn Uri.Builder().path("https://www.bazz.com/bazz.woFF2").build()
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
        verify(contextMock).resources
        verify(contextMock).assets
        verify(resourceMock).getStringArray(R.array.fonts)
        verify(assetManagerMock).open("fonts/bazz.woff2")
    }

    @Test
    fun overrideUrlLoad() {
        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock())
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                KnownServices(resourceMock.mockContext()),
                schemeHandlersMock
        )

        val tmpWebView = createWebView()

        assert(webInterceptor.shouldOverrideUrlLoading(tmpWebView,
                "https://ndopapp-int1.thunderbird.service.nhs.uk/")) {
            "WebClientInterceptor: Failed to override data preferences URL"
        }

        assert(webInterceptor.shouldOverrideUrlLoading(tmpWebView, "http://www.google.com")) {
            "WebClientInterceptor: Failed to override non-known URL"
        }
    }

    @Test
    fun overrideUrlLoad_returnsTrueForHandledScheme() {
        val url = "handled:"
        schemeHandlersMock = mock { on { handleUrl(url) } doReturn true }
        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock())
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                KnownServices(resourceMock.mockContext()),
                schemeHandlersMock
        )

        val tmpWebView = createWebView()

        assert(webInterceptor.shouldOverrideUrlLoading(tmpWebView,
                url)) {
            "WebClientInterceptor: Failed to handle url scheme"
        }
        verify(schemeHandlersMock, times(1)).handleUrl(url)
    }

    @Test
    fun overrideUrlLoad_returnsFalseForMalformedURLException() {
        val activity = Robolectric.buildActivity(MainActivity::class.java).get()
        val nhsWebMock = NhsWeb(activity, activity, mock(), mock(), mock())
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                KnownServices(resourceMock.mockContext()),
                schemeHandlersMock
        )

        val tmpWebView = createWebView()
        val response = webInterceptor.shouldOverrideUrlLoading(tmpWebView, "")

        assertEquals(false, response)
    }

    @Test
    fun onPageStartedNoConnection() {
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                knownServicesMock,
                schemeHandlersMock
        )
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        webInterceptor.onPageStarted(webViewMock, "https://111.nhs.uk/", null)

        val errorHeader = resourceMock.mockDisconnectedContext().resources.getString(
                R.string.connection_error_header
        )

        verify(webViewMock).stopLoading()
        verify(uiInteractorMock).setHeaderText(errorHeader)

    }

    @Test
    fun onPageStartedConnectedKnownURL() {
        val tmpContext = resourceMock.mockContext()

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockConnectedContext(),
                KnownServices(tmpContext),
                schemeHandlersMock
        )

        webInterceptor.onPageStarted(
                webViewMock,
                tmpContext.resources.getString(R.string.conditions),
                null
        )

        val serviceInfo = KnownServices(tmpContext).findMatchingServiceInfo(
                tmpContext.resources.getString(R.string.conditions)
        )

        val header = serviceInfo?.header
        val description = serviceInfo?.nativeHeaderDescription
        verify(uiInteractorMock).setHeaderText(header!!, description)
    }

    @Test
    fun stopLoadingWebViewTest() {

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                KnownServices(resourceMock.mockContext()),
                schemeHandlersMock)

        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        webInterceptor.stopLoadingWebviewAndShowNoConnectionError(webViewMock)

        val header = resourceMock.mockDisconnectedContext()
                .resources.getString(R.string.connection_error_header)

        verify(webViewMock).stopLoading()
        verify(uiInteractorMock).setHeaderText(header)
    }

    @Test
    fun pageFinishWithUnknownURLFollowedByKnownURLToBeOpenedInWebview() {
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                KnownServices(resourceMock.mockContext()),
                schemeHandlersMock
        )

        webInterceptor.onPageFinished(webViewMock, "https://google.com")
        verify(uiInteractorMock, never()).dismissProgressDialog()

        webInterceptor.onPageFinished(webViewMock, "https://111.nhs.uk")
        verify(uiInteractorMock).dismissProgressDialog()
    }

    @Test
    fun pageFinishKnownURLToBeOpenedInWebview() {
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                KnownServices(resourceMock.mockContext()),
                schemeHandlersMock
        )

        webInterceptor.onPageFinished(webViewMock, "https://111.nhs.uk")

        verify(uiInteractorMock, times(1)).dismissProgressDialog()
    }

    @Test
    fun receivedErrorNonKnownURL() {

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                KnownServices(resourceMock.mockContext()),
                schemeHandlersMock
        )

        webInterceptor.onReceivedError(webViewMock, 404,
                "Error", "https://google.com")

        val knownUrlErrorMessage = errorMessageHandler.getErrorMessage(ErrorType.ServiceUnavailable)
        verify(uiInteractorMock, never()).dismissProgressDialog()
        verify(uiInteractorMock, never()).showUnavailabilityError(knownUrlErrorMessage)
    }

    @Test
    fun receivedErrorKnownURLToBeOpenedInWebview() {

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                contextMock,
                KnownServices(resourceMock.mockContext()),
                schemeHandlersMock
        )

        val url = "https://111.nhs.uk"
        whenever(contextMock.getString(R.string.baseHost)).thenReturn("111.nhs.uk")
        whenever(webViewMock?.url).thenReturn(url)

        webInterceptor.onReceivedError(webViewMock, 404, "Error", url)

        val knownUrlErrorMessage = errorMessageHandler.getErrorMessage(ErrorType.ServiceUnavailable)

        verify(uiInteractorMock).dismissProgressDialog()
        verify(uiInteractorMock).showUnavailabilityError(knownUrlErrorMessage)

    }

    @Test
    fun loadResource_isKnownService_isConnected() {
        val url = "https://111.nhs.uk/"
        val expectedKnownService = KnownService(url)
        val knownServicesMock: KnownServices = mock {
            on { findMatchingKnownService(url) }.thenReturn(expectedKnownService)
        }
        val webViewMock: WebView = mock {
            on { getUrl() }.thenReturn(url)
        }

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockConnectedContext(),
                knownServicesMock,
                schemeHandlersMock
        )

        webInterceptor.onLoadResource(webViewMock, url)

        verify(knownServicesMock).findMatchingKnownService(url)
        verify(webViewMock, never()).stopLoading()
        verify(nhsWebMock, never()).loadWelcomePage()
        verify(uiInteractorMock, never()).dismissProgressDialog()
    }

    @Test
    fun loadResource_isKnownService_isNotConnected_dismissesProgressDialog() {
        val url = "https://111.nhs.uk/"
        val expectedKnownService = KnownService(url)
        val knownServicesMock: KnownServices = mock {
            on { findMatchingKnownService(url) }.thenReturn(expectedKnownService)
        }
        val webViewMock: WebView = mock {
            on { getUrl() }.thenReturn(url)
        }

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                knownServicesMock,
                schemeHandlersMock
        )
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())

        webInterceptor.onLoadResource(webViewMock, url)

        verify(uiInteractorMock).dismissProgressDialog()
    }

    @Test
    fun loadResource_isUnknownService_loadsWelcomePage() {
        val url = "https://111.nhs.uk/"
        val knownServicesMock: KnownServices = mock {
            on { findMatchingKnownService(url) }.thenReturn(null)
        }
        val webViewMock: WebView = mock {
            on { getUrl() }.thenReturn(url)
        }

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockContext(),
                knownServicesMock,
                schemeHandlersMock
        )

        webInterceptor.onLoadResource(webViewMock, url)

        verify(knownServicesMock).findMatchingKnownService(url)
        verify(webViewMock).stopLoading()
        verify(nhsWebMock).loadWelcomePage()
    }

    @Test
    fun loadResource_NotConnectedPreviouslyHandled_dismissesProgressDialog() {

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                nhsWebMock,
                resourceMock.mockDisconnectedContext(),
                knownServicesMock,
                schemeHandlersMock
        )
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())

        whenever(webViewMock?.url).thenReturn("https://www.111.nhs.uk")
        webInterceptor.onPageStarted(webViewMock, "https://www.111.nhs.uk", null)

        whenever(webViewMock?.url).thenReturn("https://111.nhs.uk")
        webInterceptor.onLoadResource(webViewMock, "https://111.nhs.uk/")
        verify(uiInteractorMock, times(1)).dismissProgressDialog()
    }

    private fun createWebView(): WebView {
        val activity = Robolectric.buildActivity(Activity::class.java).create().get()
        return WebView(activity)
    }
}
