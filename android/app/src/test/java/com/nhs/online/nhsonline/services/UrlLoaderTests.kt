package com.nhs.online.nhsonline.services

import android.webkit.WebView
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.Mockito.*
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class UrlLoaderTests {

    private val baseUrl = "https://unit-tests.com/"
    private val prescriptionsPath = "prescriptions"
    private val prescriptionsAppPageUrl = baseUrl + prescriptionsPath
    private val reloadUrl = "http://page-to-reload.com"


    private lateinit var urlLoader: UrlLoader
    private lateinit var webViewMock: WebView
    private lateinit var appWebInterfaceMock: AppWebInterface

    @Before
    fun setUp() {
        webViewMock = mock {
            on { url } doReturn baseUrl
        }

        appWebInterfaceMock = mock()

        urlLoader = UrlLoader(webViewMock, baseUrl, appWebInterfaceMock)
    }

    @Test
    fun testThatReloadRequest_LoadsTheValueOfReloadUrl_WhenReloadUrlIsNotNull() {
        urlLoader.reloadRequest(reloadUrl)

        verify(webViewMock).loadUrl(reloadUrl)
    }

    @Test
    fun testThatReloadRequest_CallsTheWebViewToReload_WhenReloadUrlIsNull() {
        urlLoader.reloadRequest(null)

        verify(webViewMock).reload()
    }

    @Test
    fun testThatProduceValidUrl_PrefixesUrlWithBaseUrl_IfTheUrlIsNotValid() {
        val result = urlLoader.produceValidUrl(prescriptionsPath)

        assertEquals(result, prescriptionsAppPageUrl)
    }

    @Test
    fun testThatProduceValidUrl_ReturnsUrlAsIs_IfTheUrlIsValid() {
        val result = urlLoader.produceValidUrl(prescriptionsAppPageUrl)

        assertEquals(result, prescriptionsAppPageUrl)
    }

    @Test
    fun loadUrl_whenRequiresFullPageReload_callsWebViewLoadUrlWithUrl() {
        urlLoader.loadUrl(prescriptionsAppPageUrl, true)

        verify(webViewMock).loadUrl(prescriptionsAppPageUrl)
    }

    @Test
    fun loadUrl_whenNavigatingFromANonNhsAppUrl_callsWebViewLoadUrlWithUrl() {
        urlLoader.loadUrl(reloadUrl, false)

        verify(webViewMock).loadUrl(reloadUrl)
    }

    @Test
    fun loadUrl_whenNavigatingFromNhsAppUrlToAnotherNhsAppUrl_passesPathToAppWebInterfaceGoTo() {
        urlLoader.loadUrl(prescriptionsAppPageUrl, false)

        verify(appWebInterfaceMock).goTo("/$prescriptionsPath")
    }
}
