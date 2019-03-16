package com.nhs.online.nhsonline.webclients


import android.webkit.WebView
import com.nhaarman.mockito_kotlin.*
import org.junit.Test
import org.junit.runner.RunWith
import com.nhs.online.nhsonline.interfaces.UnsecureInteractor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.support.schemehandlers.SchemeHandlers
import org.junit.Assert
import org.junit.Before
import org.robolectric.RobolectricTestRunner



@Suppress("DEPRECATION")
@RunWith(RobolectricTestRunner::class)
class UnsecureWebClientTest {

    private lateinit var webViewMock: WebView
    private lateinit var knownServicesMock: KnownServices
    private lateinit var unSecureClientMock: UnsecureWebClient
    private lateinit var unSecureInteractorMock: UnsecureInteractor
    private lateinit var schemeHandlersMock: SchemeHandlers

    private val resourceMock = ResourceMockingClass()

    private val knownUrlErrMsg = KnownServices(resourceMock.mockContext()).getServiceUnavailabilityError()

    @Before
    fun setUp() {
        webViewMock = mock()
        knownServicesMock = mock()
        unSecureClientMock = mock()
        unSecureInteractorMock = mock()
        schemeHandlersMock = mock()
    }

    @Test
    fun pageStartedDisconnectedTest() {
        val context = resourceMock.mockDisconnectedContext()
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                context,
                schemeHandlersMock
        )

        unSecureClient.onPageStarted(webViewMock, "https://111.nhs.uk/", null )

        verify(webViewMock).stopLoading()
        verify(unSecureInteractorMock).showUnavailabilityError(knownUrlErrMsg)

    }

    @Test
    fun pageStartedConnectedKnownURL() {
        val context = resourceMock.mockConnectedContext()
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                context,
                schemeHandlersMock
        )

        unSecureClient.onPageStarted(webViewMock, "https://111.nhs.uk/", null )

        verify(webViewMock, never()).stopLoading()

    }

    @Test
    fun loadResourceConnected() {
        val context = resourceMock.mockConnectedContext()
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                knownServicesMock,
                context,
                schemeHandlersMock
        )

        unSecureClient.onLoadResource(webViewMock, "https://111.nhs.uk/")

        verify(unSecureInteractorMock, never()).dismissProgressDialog()
        verify(webViewMock, never()).stopLoading()
    }

    @Test
    fun loadResourceNotConnected() {
        val context = resourceMock.mockDisconnectedContext()
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                knownServicesMock,
                context,
                schemeHandlersMock
        )

        unSecureClient.onLoadResource(webViewMock, "https://111.nhs.uk/")

        verify(unSecureInteractorMock).dismissProgressDialog()
        verify(webViewMock).stopLoading()
    }

    @Test
    fun loadResourceAfterPageStarted() {
        // After onPageStarted is called then having no connection is handled, therefore
        // the process should not be repeated when loadResource is called

        val context = resourceMock.mockDisconnectedContext()
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                knownServicesMock,
                context,
                schemeHandlersMock
        )

        // stop loading should be called once to handle no connection
        unSecureClient.onPageStarted(webViewMock, "https://nhs.uk", null)
        verify(webViewMock, times(1)).stopLoading()

        unSecureClient.onLoadResource(webViewMock, "https://111.nhs.uk/")
        verify(unSecureInteractorMock, never()).dismissProgressDialog()

        // Still only called once, not called in onLoadResource
        verify(webViewMock, times(1)).stopLoading()

    }

    @Test
    fun receivedErrorNonKnownAddress() {
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                knownServicesMock,
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        unSecureClient.onReceivedError(webViewMock, 404,
                "Error", "https://google.com")

        verify(unSecureInteractorMock, never()).dismissProgressDialog()
    }

    @Test
    fun receivedErrorKnownAddress() {
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        unSecureClient.onReceivedError(webViewMock, 404,
                "Error", "https://www.nhs.uk")

        verify(unSecureInteractorMock).showUnavailabilityError(knownUrlErrMsg)
        verify(unSecureInteractorMock).dismissProgressDialog()
    }

    @Test
    fun pageFinishedNonKnownAddressNoConnection() {
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                knownServicesMock,
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        unSecureClient.onPageFinished(webViewMock, "https://www.google.com")
        verify(unSecureInteractorMock, never()).dismissProgressDialog()

    }

    @Test
    fun pageFinishedKnownAddressNoConnection() {
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        unSecureClient.onPageFinished(webViewMock, "https://www.nhs.uk")
        verify(unSecureInteractorMock).dismissProgressDialog()
    }

    @Test
    fun pageFinishedEmptyStringNoConnection(){
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        unSecureClient.onPageFinished(webViewMock, "")
        verify(unSecureInteractorMock, never()).dismissProgressDialog()
        verify(unSecureInteractorMock).showWebviewScreen()
    }

    @Test
    fun pageCommitKnownAddressNoConnection(){
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        unSecureClient.onPageCommitVisible(webViewMock, "https://www.nhs.uk")
        verify(unSecureInteractorMock).dismissProgressDialog()
        verify(unSecureInteractorMock).showWebviewScreen()
    }

    @Test
    fun pageCommitShouldNonNHSAddressNoConnection(){
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        unSecureClient.onPageCommitVisible(webViewMock, "https://www.google.com")
        verify(unSecureInteractorMock, never()).dismissProgressDialog()
        verify(unSecureInteractorMock).showWebviewScreen()
    }

    @Test
    fun pageCommitEmptyURL(){
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        unSecureClient.onPageCommitVisible(webViewMock, "")
        verify(unSecureInteractorMock, never()).dismissProgressDialog()
        verify(unSecureInteractorMock).showWebviewScreen()
    }

    @Test
    fun shouldOverrideUrlLoading_returnsTrueForHandledScheme() {

        val url = "handled:xyz"
        schemeHandlersMock = mock { on { handleUrl(url) } doReturn true }
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        val result = unSecureClient.shouldOverrideUrlLoading(webViewMock, url)

        verify(schemeHandlersMock, times(1)).handleUrl(url)
        Assert.assertTrue(result)
    }

    @Test
    fun shouldOverrideUrlLoading_returnsFalseForUnHandledScheme() {

        val url = "handled:xyz"
        schemeHandlersMock = mock { on { handleUrl(url) } doReturn false }
        val unSecureClient = UnsecureWebClient(
                unSecureInteractorMock,
                KnownServices(resourceMock.mockContext()),
                resourceMock.mockDisconnectedContext(),
                schemeHandlersMock
        )

        val result = unSecureClient.shouldOverrideUrlLoading(webViewMock, url)

        Assert.assertFalse(result)
        verify(schemeHandlersMock, times(1)).handleUrl(url)
    }

}
