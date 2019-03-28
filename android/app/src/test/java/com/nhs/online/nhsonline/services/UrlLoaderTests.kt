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
import org.mockito.Mockito.anyString
import org.mockito.Mockito.verify
import org.robolectric.RobolectricTestRunner
import org.robolectric.util.ReflectionHelpers


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
            on { findKnownServiceAndAddMissingQueryFor(prescriptionsAppPageUrl) } doReturn appPageUrlWithQueryParams
            on { findKnownServiceAndAddMissingQueryFor(externalPageUrl) } doReturn externalPageUrl
            on { findKnownServiceAndAddMissingQueryFor(appointmentsAppPageUrl) } doReturn appPageUrlWithQueryParams
        }

        this.urlLoader = UrlLoader(webviewMock, knownServicesMock, baseUrl)

        webJavascriptMock = mock()
        ReflectionHelpers.setField(urlLoader, "webJavascript", webJavascriptMock)
    }

    @Test
    fun testThatPassedUrlIsCheckedForMissingQueryParams_AndReturnsResultUrlAfterCheck() {
        urlLoader.loadUrl(prescriptionsAppPageUrl, true)

        verify(knownServicesMock).findKnownServiceAndAddMissingQueryFor(prescriptionsAppPageUrl)
        verify(webviewMock).loadUrl(appPageUrlWithQueryParams)
    }

    @Test
    fun testThatWhenDoesNotRequireFullPageReload_AndComingFromNhsAppPage_AndGoingToNhsAppPage_LoadsInSPAMode() {
        // navigate from prescriptions to appointments (page on nhs app to another page on nhs app)
        urlLoader.loadUrl(appointmentsAppPageUrl, false)

        verify(webJavascriptMock).loadSpaPath("/$appointmentsPath")
        verify(knownServicesMock, never()).findKnownServiceAndAddMissingQueryFor(anyString())
        verify(webviewMock, never()).loadUrl(anyString())
    }

    @Test
    fun testThatEvenWhenComingFromNhsAppPage_AndGoingToNhsAppPage_DoesRequireFullPageReload_DoesNotLoadInSPAMode() {
        // navigate from prescriptions to appointments (page on nhs app to another page on nhs app)
        urlLoader.loadUrl(appointmentsAppPageUrl, true)

        verify(webJavascriptMock, never()).loadSpaPath(anyString())
        verify(knownServicesMock).findKnownServiceAndAddMissingQueryFor(appointmentsAppPageUrl)
        verify(webviewMock).loadUrl(appPageUrlWithQueryParams)
    }

    @Test
    fun testThatWhenDoesNotRequireFullPageReload_AndComingFromExternalPage_AndGoingToNhsAppPage_DoesNotLoadInSPAMode() {
        whenever(webviewMock.url).thenReturn(externalPageUrl)

        // navigate from prescriptions to appointments (page on nhs app to another page on nhs app)
        urlLoader.loadUrl(appointmentsAppPageUrl, false)

        verify(webJavascriptMock, never()).loadSpaPath(anyString())
        verify(knownServicesMock).findKnownServiceAndAddMissingQueryFor(appointmentsAppPageUrl)
        verify(webviewMock).loadUrl(appointmentsPageUrlWithQueryParams)
    }

    @Test
    fun testThatWhenDoesNotRequireFullPageReload_AndComingFromNhsAppPage_AndGoingToNhsExternalPage_DoesNotLoadInSPAMode() {
        // navigate from prescriptions to appointments (page on nhs app to another page on nhs app)
        urlLoader.loadUrl(externalPageUrl, false)

        verify(webJavascriptMock, never()).loadSpaPath(anyString())
        verify(knownServicesMock).findKnownServiceAndAddMissingQueryFor(externalPageUrl)
        verify(webviewMock).loadUrl(externalPageUrl)
    }

    @Test
    fun testThatReloadRequest_LoadsTheValueOfReloadUrl_WhenReloadUrlIsNotNull() {
        val reloadUrl = "http://page-to-reload.com"

        urlLoader.reloadRequest(reloadUrl)

        verify(webviewMock).loadUrl(reloadUrl)
    }

    @Test
    fun testThatReloadRequest_CallsTheWebViewToReload_WhenReloadUrlIsNull() {
        urlLoader.reloadRequest(null)

        verify(webviewMock).reload()
    }

    @Test
    fun testThatProduceValidUrl_PrefixesUrlWithBaseUrl_IfTheUrlIsNotValid() {
        val result = urlLoader.produceValidUrl(prescriptionsPath)

        Assert.assertEquals(result, prescriptionsAppPageUrl)
    }

    @Test
    fun testThatProduceValidUrl_ReturnsUrlAsIs_IfTheUrlIsValid() {
        val result = urlLoader.produceValidUrl(prescriptionsAppPageUrl)

        Assert.assertEquals(result, prescriptionsAppPageUrl)
    }
}
