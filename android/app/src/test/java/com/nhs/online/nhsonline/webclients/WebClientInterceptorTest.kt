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
import com.nhs.online.nhsonline.browseractivities.ActivityInterface
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.KnownServices
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.junit.Assert.assertEquals
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import java.io.InputStream

@Suppress("DEPRECATION")
@RunWith(RobolectricTestRunner::class)
class WebClientInterceptorTest {

    private lateinit var webClientInterceptor: WebClientInterceptor
    private lateinit var uiInteractorMock: IInteractor
    private lateinit var knownServicesMock: KnownServices
    private lateinit var activitiesMock: List<ActivityInterface>
    private lateinit var contextMock: Context
    private lateinit var webViewMock: WebView
    private lateinit var requestMock: WebResourceRequest

    private val resourceMock = ResourceMockingClass()


    @Before
    fun setUp() {
        uiInteractorMock = mock()
        knownServicesMock = mock()
        activitiesMock = mock()
        contextMock = mock()
        webClientInterceptor = WebClientInterceptor(
                uiInteractorMock,
                knownServicesMock,
                activitiesMock,
                contextMock
        )

        webViewMock = mock()

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
    fun isConnectedForConnectedContext(){

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                knownServicesMock,
                activitiesMock,
                resourceMock.mockConnectedContext()
        )

        assert(webInterceptor.isConnectedToInternet()) {
            "WebClientInterceptor: Returns disconnected for a connected context"
        }
    }

    @Test
    fun isConnectedForDisconnectedContext(){
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                knownServicesMock,
                activitiesMock,
                resourceMock.mockDisconnectedContext()
        )

        assert(!webInterceptor.isConnectedToInternet()) {
            "WebClientInterceptor: Returns connected for disconnected context"
        }
    }

    @Test
    fun overrideUrlLoad(){
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                KnownServices(resourceMock.mockContext()),
                createActivities(),
                resourceMock.mockContext()
        )

        val tmpWebView = createWebView()

        assert(!webInterceptor.shouldOverrideUrlLoading(tmpWebView,
                "https://ndopapp-int1.thunderbird.service.nhs.uk/")) {
            "WebClientInterceptor: Failed to override data preferences URL"
        }

        assert(webInterceptor.shouldOverrideUrlLoading(tmpWebView, "http://www.google.com")){
            "WebClientInterceptor: Failed to override non-known URL"
        }
    }


    @Test
    fun onPageStartedNoConnection(){
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                knownServicesMock,
                activitiesMock,
                resourceMock.mockDisconnectedContext()
        )

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
                KnownServices(tmpContext),
                activitiesMock,
                resourceMock.mockConnectedContext()
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
        if (header != null) {
            verify(uiInteractorMock).setHeaderText(header, null)
        }
    }

    @Test
    fun stopLoadingWebViewTest(){

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                KnownServices(resourceMock.mockContext()),
                activitiesMock,
                resourceMock.mockDisconnectedContext())

        webInterceptor.stopLoadingWebviewAndShowNoConnectionError(webViewMock)


        val header = resourceMock.mockDisconnectedContext().resources.getString(R.string.connection_error_header)

        verify(webViewMock).stopLoading()
        verify(uiInteractorMock).setHeaderText(header)
    }

    @Test
    fun pageFinishUnknownURL(){
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                KnownServices(resourceMock.mockContext()),
                activitiesMock,
                resourceMock.mockDisconnectedContext()
        )


        webInterceptor.onPageFinished(webViewMock, "https://google.com")
        verify(uiInteractorMock, never()).dismissProgressDialog()

        webInterceptor.onPageFinished(webViewMock, "https://www.nhs.uk")

        verify(uiInteractorMock).dismissProgressDialog()
    }

    @Test
    fun pageFinishKnownURL(){

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                KnownServices(resourceMock.mockContext()),
                activitiesMock,
                resourceMock.mockDisconnectedContext()
        )

        webInterceptor.onPageFinished(webViewMock, "https://www.nhs.uk")

        verify(uiInteractorMock, times(1)).dismissProgressDialog()
    }

    @Test
    fun receivedErrorNonKnownURL(){

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                KnownServices(resourceMock.mockContext()),
                activitiesMock,
                resourceMock.mockDisconnectedContext()
        )

        webInterceptor.onReceivedError(webViewMock, 404,
                "Error", "https://google.com")

        val knownUrlErrMsg = KnownServices(resourceMock.mockContext()).getServiceUnavailabilityError()

        verify(uiInteractorMock, never()).dismissProgressDialog()
        verify(uiInteractorMock, never()).showUnavailabilityError(knownUrlErrMsg)
    }

    @Test
    fun receivedErrorKnownURL(){

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                KnownServices(resourceMock.mockContext()),
                activitiesMock,
                resourceMock.mockDisconnectedContext()
        )

        webInterceptor.onReceivedError(webViewMock, 404,
                "Error", "https://www.nhs.uk")

        val knownUrlErrMsg = KnownServices(resourceMock.mockContext()).getServiceUnavailabilityError()

        verify(uiInteractorMock).dismissProgressDialog()
        verify(uiInteractorMock).showUnavailabilityError(knownUrlErrMsg)

    }

    @Test
    fun loadResourceConnected() {

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                knownServicesMock,
                activitiesMock,
                resourceMock.mockConnectedContext()
        )

        webInterceptor.onLoadResource(webViewMock, "https://111.nhs.uk/")

        verify(uiInteractorMock, never()).dismissProgressDialog()
        verify(webViewMock, never()).stopLoading()
    }

    @Test
    fun loadResourceNotConnected() {
        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                knownServicesMock,
                activitiesMock,
                resourceMock.mockDisconnectedContext()
        )

        webInterceptor.onLoadResource(webViewMock, "https://111.nhs.uk/")
        verify(uiInteractorMock).dismissProgressDialog()
    }

    @Test
    fun loadResourceNotConnectedPreviouslyHandled() {

        val webInterceptor = WebClientInterceptor(
                uiInteractorMock,
                knownServicesMock,
                activitiesMock,
                resourceMock.mockDisconnectedContext()
        )

        webInterceptor.onPageStarted(webViewMock, "https://www.111.nhs.uk", null)
        webInterceptor.onLoadResource(webViewMock, "https://111.nhs.uk/")
        verify(uiInteractorMock, times(1)).dismissProgressDialog()
    }

    private fun createWebView(): WebView {
        val activity = Robolectric.buildActivity(Activity::class.java).create().get()
        return WebView(activity)
    }

    private fun createActivities(): List<ActivityInterface> {
        val openBrowserActivity =
                OpenUrlInBrowserActivity(resourceMock.mockContext()
                        .resources.getStringArray(R.array.nativeAppHosts))
        return listOf(openBrowserActivity)
    }

}