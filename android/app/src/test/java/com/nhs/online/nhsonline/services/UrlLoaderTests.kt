package com.nhs.online.nhsonline.services

import android.webkit.WebView
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.never
import com.nhaarman.mockito_kotlin.whenever
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
    private val appointmentsPath = "prescriptions"
    private val prescriptionsAppPageUrl = baseUrl + prescriptionsPath
    private val appointmentsAppPageUrl = baseUrl + appointmentsPath
    private val appPageUrlWithQueryParams = "$prescriptionsAppPageUrl?source=android"
    private val appointmentsPageUrlWithQueryParams = "$appointmentsAppPageUrl?source=android"
    private val externalPageUrl = "http://external-page"
    private val reloadUrl = "http://page-to-reload.com"
    private val reloadUrlWithQueryParams = "$reloadUrl?source=android"


    private lateinit var urlLoader: UrlLoader
    private lateinit var webViewMock: WebView
    private lateinit var knownServicesMock: KnownServices
    private lateinit var appWebInterfaceMock: AppWebInterface

    @Before
    fun setUp() {
        webViewMock = mock {
            on { url } doReturn baseUrl
        }

        knownServicesMock = mock {
            on { findKnownServiceAndAddMissingQueryFor(prescriptionsAppPageUrl) } doReturn appPageUrlWithQueryParams
            on { findKnownServiceAndAddMissingQueryFor(externalPageUrl) } doReturn externalPageUrl
            on { findKnownServiceAndAddMissingQueryFor(appointmentsAppPageUrl) } doReturn appPageUrlWithQueryParams
            on { findKnownServiceAndAddMissingQueryFor(reloadUrl) } doReturn (reloadUrlWithQueryParams)
        }

        appWebInterfaceMock = mock()

        this.urlLoader = UrlLoader(webViewMock, knownServicesMock, baseUrl, appWebInterfaceMock)
    }

    @Test
    fun testThatPassedUrlIsCheckedForMissingQueryParams_AndReturnsResultUrlAfterCheck() {
        urlLoader.loadUrl(prescriptionsAppPageUrl, true)

        verify(knownServicesMock).findKnownServiceAndAddMissingQueryFor(prescriptionsAppPageUrl)
        verify(webViewMock).loadUrl(appPageUrlWithQueryParams)
    }

    @Test
    fun testThatWhenDoesNotRequireFullPageReload_AndComingFromNhsAppPage_AndGoingToNhsAppPage_LoadsInSPAMode() {
        // navigate from prescriptions to appointments (page on nhs app to another page on nhs app)
        urlLoader.loadUrl(appointmentsAppPageUrl, false)

        verify(appWebInterfaceMock).goTo("/$appointmentsPath")
        verify(knownServicesMock, never()).findKnownServiceAndAddMissingQueryFor(anyString())
        verify(webViewMock, never()).loadUrl(anyString())
    }

    @Test
    fun testThatEvenWhenComingFromNhsAppPage_AndGoingToNhsAppPage_DoesRequireFullPageReload_DoesNotLoadInSPAMode() {
        // navigate from prescriptions to appointments (page on nhs app to another page on nhs app)
        urlLoader.loadUrl(appointmentsAppPageUrl, true)

        verify(appWebInterfaceMock, never()).goTo(anyString())
        verify(knownServicesMock).findKnownServiceAndAddMissingQueryFor(appointmentsAppPageUrl)
        verify(webViewMock).loadUrl(appPageUrlWithQueryParams)
    }

    @Test
    fun testThatWhenDoesNotRequireFullPageReload_AndComingFromExternalPage_AndGoingToNhsAppPage_DoesNotLoadInSPAMode() {
        whenever(webViewMock.url).thenReturn(externalPageUrl)

        // navigate from prescriptions to appointments (page on nhs app to another page on nhs app)
        urlLoader.loadUrl(appointmentsAppPageUrl, false)

        verify(appWebInterfaceMock, never()).goTo(anyString())
        verify(knownServicesMock).findKnownServiceAndAddMissingQueryFor(appointmentsAppPageUrl)
        verify(webViewMock).loadUrl(appointmentsPageUrlWithQueryParams)
    }

    @Test
    fun testThatWhenDoesNotRequireFullPageReload_AndComingFromNhsAppPage_AndGoingToNhsExternalPage_DoesNotLoadInSPAMode() {
        // navigate from prescriptions to appointments (page on nhs app to another page on nhs app)
        urlLoader.loadUrl(externalPageUrl, false)

        verify(appWebInterfaceMock, never()).goTo(anyString())
        verify(knownServicesMock).findKnownServiceAndAddMissingQueryFor(externalPageUrl)
        verify(webViewMock).loadUrl(externalPageUrl)
    }

    @Test
    fun testThatReloadRequest_LoadsTheValueOfReloadUrl_WhenReloadUrlIsNotNull() {
        urlLoader.reloadRequest(reloadUrl)

        verify(webViewMock).loadUrl(reloadUrlWithQueryParams)
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
}
