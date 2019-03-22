package com.nhs.online.nhsonline.web

import android.app.Activity
import android.content.Context
import android.content.res.Resources
import android.net.ConnectivityManager
import android.net.NetworkInfo
import android.webkit.WebSettings
import android.webkit.WebView
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.MockConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.UrlLoader
import com.nhs.online.nhsonline.support.PersistData
import junit.framework.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.util.ReflectionHelpers

@RunWith(RobolectricTestRunner::class)
class NhsWebTest {
    private val activity = Robolectric.buildActivity(Activity::class.java).get()
    private lateinit var spyActivity: Activity
    private lateinit var interactorMock: IInteractor
    private lateinit var webViewMock: WebView
    private lateinit var nhsWeb: NhsWeb
    private lateinit var urlLoader: UrlLoader

    @Before
    fun setUp() {
        spyActivity = spy(activity)
        interactorMock = mock()
        webViewMock = mock()
        urlLoader = mock()
        nhsWeb = NhsWeb(spyActivity, interactorMock, webViewMock)
        ReflectionHelpers.setField(nhsWeb, "urlLoader", urlLoader)
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
    }

    @Test
    fun loadUrl_Loads_RequestUrl() {
        val url = "http://unit-test.com"
        nhsWeb.loadUrl(url)
        verify(urlLoader).loadUrl(url)
    }

    @Test
    fun loadUrl_WithNoConnection_Calls_ShowUnavailabilityError() {
        val url = "http://unit-test.com"
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        nhsWeb.loadUrl(url)
        verify(interactorMock).showUnavailabilityError(any())
    }

    @Test
    fun loadWelcomePage_Loads_BaseUrl() {
        val baseUrl = "http://unit-test.com"
        val resourceMock: Resources = mock {
            on { getString(R.string.baseURL) } doReturn baseUrl
        }
        whenever(spyActivity.resources).thenReturn(resourceMock)
        val spyNhsWeb = spy(nhsWeb)
        spyNhsWeb.loadWelcomePage()
        verify(spyNhsWeb).loadUrl(baseUrl)
    }

    @Test
    fun loadUrlInChromeTab() {
        val openBrowserActivityMock: OpenUrlInBrowserActivity = mock()
        ReflectionHelpers.setField(nhsWeb, "openBrowserActivity", openBrowserActivityMock)
        val url = "http://unit-test.com"
        nhsWeb.loadUrlInChromeTab(url)
        verify(openBrowserActivityMock).start(any(), any())
    }

    @Test
    fun reloadLoginUrl_Loads_CallsLoadUrlWithLoginUrl_When_CurrentWebViewUrlIsNotLoginUrl() {
        whenever(webViewMock.url).thenReturn(null)
        val baseUrl = "http://unit-test.com/"
        val resourceMock: Resources = mock {
            on { getString(R.string.baseURL) } doReturn baseUrl
            on { getString(R.string.loginPath) } doReturn "login"
        }
        val loginUrl = baseUrl + "login"
        whenever(spyActivity.resources).thenReturn(resourceMock)
        val spyNhsWeb = spy(nhsWeb)
        spyNhsWeb.reloadLoginUrl()
        verify(spyNhsWeb).loadUrl(loginUrl)
    }

    @Test
    fun reloadLoginUrl_Loads_CallsLoadUrlWithLoginUrl_When_CurrentUrlLoginUrlHasFidoQuery() {
        whenever(webViewMock.url).thenReturn(null)
        val baseUrl = "http://unit-test.com/"
        val loginPath = "login"
        val resourceMock: Resources = mock {
            on { getString(R.string.baseURL) } doReturn baseUrl
            on { getString(R.string.loginPath) } doReturn loginPath
            on { getString(R.string.fidoAuthQueryKey) } doReturn "fidoAuthResponse"
        }
        val loginUrl = baseUrl + loginPath
        whenever(webViewMock.url).thenReturn("$loginUrl?fidoAuthResponse=fido")
        whenever(spyActivity.resources).thenReturn(resourceMock)
        val spyNhsWeb = spy(nhsWeb)
        spyNhsWeb.reloadLoginUrl()
        verify(spyNhsWeb).loadUrl(loginUrl)
    }

    @Test
    fun reloadLoginUrl_CallsWebViewReloadMethod__When_CurrentUrlIsLoginUrlWithNoFidoQuery() {
        whenever(webViewMock.url).thenReturn(null)
        val baseUrl = "http://unit-test.com/"
        val loginPath = "login"
        val resourceMock: Resources = mock {
            on { getString(R.string.baseURL) } doReturn baseUrl
            on { getString(R.string.loginPath) } doReturn loginPath
            on { getString(R.string.fidoAuthQueryKey) } doReturn "fidoAuthResponse"
        }
        val loginUrl = baseUrl + loginPath
        whenever(webViewMock.url).thenReturn(loginUrl)
        whenever(spyActivity.resources).thenReturn(resourceMock)
        val spyNhsWeb = spy(nhsWeb)
        spyNhsWeb.reloadLoginUrl()
        verify(webViewMock).reload()
    }

    @Test
    fun loadThrottlingCarousel_Sets_ZoomSizeTo100AndLoadsThrottlingUrl() {
        val throttlingUrl = "file://throttling-url.html"
        val resourceMock: Resources = mock {
            on { getString(R.string.throttleCarouselPath) } doReturn throttlingUrl
        }
        val webviewSettings: WebSettings = mock()
        whenever(webViewMock.settings).thenReturn(webviewSettings)
        whenever(spyActivity.resources).thenReturn(resourceMock)
        nhsWeb.loadThrottlingCarousel()
        verify(webviewSettings).textZoom = 100
        verify(webViewMock).loadUrl(throttlingUrl)
    }

    @Test
    fun onThrottlingCarouselComplete_Resets_WebviewTextZoomToOriginalSizeAndSaveThrottlingStateAndLoadsWelcomePage() {
        val hasThrottlingShown = "haveShownThrottlingCarouselBefore"
        val originalTextSize = 50

        ReflectionHelpers.setField(nhsWeb, "originalWebViewZoom", originalTextSize)
        val appPersistData: PersistData = mock()
        ReflectionHelpers.setField(nhsWeb, "appPersistData", appPersistData)
        val webviewSettings: WebSettings = mock()

        whenever(webViewMock.settings).thenReturn(webviewSettings)
        val spyNhsWeb = spy(nhsWeb)
        whenever(spyActivity.getString(R.string.haveShownThrottlingCarouselBefore)).thenReturn(
            hasThrottlingShown)
        spyNhsWeb.onThrottlingCarouselComplete()

        verify(webviewSettings).textZoom = originalTextSize
        verify(appPersistData).storeBoolean(hasThrottlingShown, true)
        verify(spyNhsWeb).loadWelcomePage()
    }

    @Test
    fun onWebLoggedIn_Sets_IsLoginToTrue_And_ShowsHeaders_And_Menu() {
        nhsWeb.onWebLoggedIn()
        Assert.assertTrue(nhsWeb.isUserLoggedIn)
        verify(interactorMock).showHeader()
        verify(interactorMock).showMenuBar()
    }

    @Test
    fun onWebLoggedOut_Sets_IsLoginToFalse_And_DismissSessionExtensionDialog_And_CallsShowBiometricLoginIfEnabled() {
        nhsWeb.onWebLoggedOut()
        Assert.assertFalse(nhsWeb.isUserLoggedIn)
        verify(interactorMock).dismissSessionExtensionDialog()
        verify(interactorMock).showBiometricLoginIfEnabled()
    }
}