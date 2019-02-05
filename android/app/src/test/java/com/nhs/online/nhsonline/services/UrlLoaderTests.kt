package com.nhs.online.nhsonline.services

import android.webkit.WebView
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.never
import com.nhaarman.mockito_kotlin.whenever
import com.nhs.online.nhsonline.webinterfaces.WebJavascript
import junit.framework.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.Mockito.verify
import org.robolectric.RobolectricTestRunner
import org.robolectric.util.ReflectionHelpers


@RunWith(RobolectricTestRunner::class)
class UrlLoaderTests {

    private val baseUrl = "https://unit-tests.com/"
    private val path = "page-one"
    private val appPageUrl = baseUrl + path

    private lateinit var urlLoader: UrlLoader
    private lateinit var webviewMock: WebView
    private lateinit var knownServicesMock: KnownServices
    private lateinit var webJavascriptMock: WebJavascript

    @Before
    fun setUp() {
        webviewMock = mock {
            on { url } doReturn baseUrl
        }

        knownServicesMock = mock {
            on { findKnownServiceAndAddMissingQueryFor(appPageUrl) } doReturn appPageUrl
        }
        webJavascriptMock = mock()

        this.urlLoader = UrlLoader(webviewMock, knownServicesMock, baseUrl)
        ReflectionHelpers.setField(urlLoader, "webJavascript", webJavascriptMock)
    }

    @Test
    fun testInternalPathLinkLoadsInSpa_When_UsingAbsoluteUri_SetTo_False() {
        urlLoader.usingAbsoluteUri = false

        urlLoader.loadUrl(path)

        verify(webJavascriptMock).loadSpaPath("/$path")
    }

    @Test
    fun testInternalFullUrlLinkLoadsInSpa_When_UsingAbsoluteUri_SetTo_False() {
        urlLoader.usingAbsoluteUri = false

        urlLoader.loadUrl(appPageUrl)

        verify(webJavascriptMock).loadSpaPath("/$path")
    }

    @Test
    fun testInternalFullUrlLinkLoads_FullUrl_And_DoestNotLoadSpaPath_When_UsingAbsoluteUri_SetTo_True() {
        urlLoader.usingAbsoluteUri = true
        urlLoader.loadUrl(appPageUrl)

        verify(webJavascriptMock, never()).loadSpaPath("/$path")
        verify(webviewMock).loadUrl(appPageUrl)
    }

    @Test
    fun testInternalSpaPathLinkLoads_FullUrl_And_DoestNotLoadSpaPath_When_UsingAbsoluteUri_SetTo_True() {
        urlLoader.usingAbsoluteUri = true
        urlLoader.loadUrl(path)

        verify(webJavascriptMock, never()).loadSpaPath("/$path")
        verify(webviewMock).loadUrl(appPageUrl)
    }

    @Test
    fun testExternalLinkLoads_FullUrl_And_DoestNotLoadSpaPath_OnEither_UsingAbsoluteUri_TrueOrFalse() {
        val externalUrl = "https://some-random-link.com/"
        val externalPath = "random-path"
        val fullExternalUrl = externalUrl + externalPath
        whenever(knownServicesMock.findKnownServiceAndAddMissingQueryFor(fullExternalUrl)).thenReturn(
            fullExternalUrl)
        booleanArrayOf(true, false).forEach { absoluteUri ->
            urlLoader.usingAbsoluteUri = absoluteUri
            urlLoader.loadUrl(fullExternalUrl)

            verify(webJavascriptMock, never()).loadSpaPath("/$externalPath")
            verify(webviewMock).loadUrl(fullExternalUrl)
        }
    }

    @Test
    fun testProduceValidUrl_ForExternalUrl_Or_FullNhsUrl_Returns_AsHowTheUrlIs() {
        val externalUrl = "https://some-random-link.com/path"
        arrayOf(externalUrl, appPageUrl).forEach { url ->
            val validUrl = urlLoader.produceValidUrl(url)
            Assert.assertEquals(url, validUrl)
        }
    }

    @Test
    fun testProduceValidUrl_ForAPath_Returns_BaseUrlWithSuffixedPath() {
        val path = "random-path"
        val validUrl = urlLoader.produceValidUrl(path)
        Assert.assertEquals(validUrl, baseUrl + path)
    }
}
