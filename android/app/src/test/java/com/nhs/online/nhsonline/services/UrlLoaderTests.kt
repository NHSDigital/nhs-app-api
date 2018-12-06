package com.nhs.online.nhsonline.services

import android.webkit.WebView
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.never
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify
import org.robolectric.RobolectricTestRunner


@RunWith(RobolectricTestRunner::class)
class UrlLoaderTests {

    private val baseUrl = "https://unit-tests.com"
    private val appPageUrl = "$baseUrl/page-one"

    private lateinit var urlLoader: UrlLoader
    private lateinit var appWebInterfaceMock: AppWebInterface
    private lateinit var webviewMock: WebView

    @Before
    fun setUp() {
        webviewMock = mock {
            on { url } doReturn baseUrl
        }

        appWebInterfaceMock = mock(AppWebInterface::class.java)

        val webClientMock: WebClientInterceptor = mock {
            on { isConnectedToInternet() } doReturn true
        }

        val knownServicesMock: KnownServices = mock {
            on { findKnownServiceAndAddMissingQueryFor(appPageUrl) } doReturn appPageUrl
        }

        this.urlLoader = UrlLoader(webviewMock,
            webClientMock,
            appWebInterfaceMock,
            knownServicesMock,
            baseUrl)
    }

    @Test
    fun testInternalLinkLoadsInSpaWhenLoggedIn() {

        login()

        val spaPagePath = "/page-one"

        urlLoader.loadUrl(spaPagePath)

        verify(appWebInterfaceMock).loadSpaPage("/page-one", baseUrl)
    }

    @Test
    fun testInternalLinkLoadsInBrowserWhenNotLoggedIn() {

        logout()

        urlLoader.loadUrl(appPageUrl)

        verify(appWebInterfaceMock, never()).loadSpaPage(appPageUrl, baseUrl)
        verify(webviewMock).loadUrl(appPageUrl)
    }

    private fun login() {
        urlLoader.usingAbsoluteUri = false
    }

    private fun logout() {
        urlLoader.usingAbsoluteUri = true
    }
}
