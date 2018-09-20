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
import org.mockito.Mockito



@RunWith(RobolectricTestRunner::class)
class UrlLoaderTests {

    var baseUrl = "https://unit-tests.com"
    var appPageUrl = "$baseUrl/page-one"

    lateinit var urlLoader: UrlLoader
    lateinit var appWebInterfaceMock: AppWebInterface
    lateinit var webviewMock: WebView

    @Before
    fun SetUp() {
        webviewMock = mock {
            on { url } doReturn baseUrl
        }

        appWebInterfaceMock = mock(AppWebInterface::class.java)

        var webClientMock: WebClientInterceptor = mock {
            on { isConnectedToInternet() } doReturn true
        }

        var knownServicesMock : KnownServices = mock {
            on { findKnownServiceAddMissingQueryFor(appPageUrl) } doReturn appPageUrl
        }

        this.urlLoader = UrlLoader(webviewMock, webClientMock, appWebInterfaceMock, knownServicesMock, baseUrl)
    }

    @Test
    fun TestInternalLinkLoadsInSpaWhenLoggedIn() {

        login()

        var spaPagePath = "/page-one"

        urlLoader.loadUrl(spaPagePath)

        verify(webviewMock).loadUrl(baseUrl + "/page-one")
    }

    @Test
    fun TestInternalLinkLoadsInBrowserWhenNotLoggedIn() {

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
