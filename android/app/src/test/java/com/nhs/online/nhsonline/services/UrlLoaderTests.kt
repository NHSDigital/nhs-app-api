package com.nhs.online.nhsonline.services

import android.webkit.WebView
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.never
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.Mockito.mock
import org.mockito.Mockito.verify
import org.robolectric.RobolectricTestRunner

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

        var knownServicesMock : KnownServices = mock {
            on { findKnownServiceAddMissingQueryFor(appPageUrl) } doReturn appPageUrl
        }

        this.urlLoader = UrlLoader(webviewMock, appWebInterfaceMock, knownServicesMock, baseUrl)
    }

    @Test
    fun TestInternalLinkLoadsInSpaWhenLoggedIn() {

        login()

        var spaPagePath = "/page-one"

        urlLoader.loadUrl(spaPagePath)

        verify(appWebInterfaceMock).loadSpaPage("/page-one", baseUrl)
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
