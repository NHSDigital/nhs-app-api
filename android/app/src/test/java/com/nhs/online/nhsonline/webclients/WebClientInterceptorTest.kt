package com.nhs.online.nhsonline.webclients

import android.content.Context
import android.content.res.AssetManager
import android.content.res.Resources
import android.net.Uri
import android.webkit.WebResourceRequest
import android.webkit.WebView
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.ActivityInterface
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.services.KnownServices
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.junit.Assert.assertEquals
import org.robolectric.RobolectricTestRunner
import java.io.InputStream

@RunWith(RobolectricTestRunner::class)
class WebClientInterceptorTest {

    private lateinit var webClientInterceptor: WebClientInterceptor
    private lateinit var uiInteractorMock: IInteractor
    private lateinit var knownServicesMock: KnownServices
    private lateinit var activitiesMock: List<ActivityInterface>
    private lateinit var contextMock: Context
    private lateinit var webViewMock: WebView
    private lateinit var requestMock: WebResourceRequest

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
}