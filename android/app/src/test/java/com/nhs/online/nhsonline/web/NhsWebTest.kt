package com.nhs.online.nhsonline.web

import android.app.Activity
import android.content.res.Resources
import android.net.Network
import android.view.accessibility.AccessibilityEvent
import android.webkit.CookieManager
import android.webkit.WebView
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.NotificationsService
import com.nhs.online.nhsonline.services.UrlLoader
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNull
import org.junit.Assert.assertTrue
import com.nhs.online.nhsonline.support.PersistData
import org.junit.After
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.util.ReflectionHelpers
import java.io.File
import java.net.URL

@RunWith(RobolectricTestRunner::class)
class NhsWebTest {
    private val activity = Robolectric.buildActivity(Activity::class.java).get()

    private val baseURL = "https://unit-test.com/"
    private val loginPath = "login"
    private val loginUrl = baseURL + loginPath
    private val resourceMocks = createResourceMocks()

    private lateinit var nhsWeb: NhsWeb
    private lateinit var spyWeb: NhsWeb
    private lateinit var spyActivity: Activity
    private lateinit var persistData: PersistData
    private lateinit var interactorMock: IInteractor
    private lateinit var webViewMock: WebView
    private lateinit var notificationsServiceMock: NotificationsService
    private lateinit var network: Network
    private lateinit var urlLoader: UrlLoader
    private lateinit var tempAppWebViewDir: File
    private lateinit var tempCacheDir: File
    private lateinit var connectionStateMonitor: ConnectionStateMonitor

    @Before
    fun setUp() {
        spyActivity = spy(activity)
        persistData = PersistData(spyActivity)
        interactorMock = mock()
        webViewMock = mock{
            on { settings }.doReturn(mock())
        }

        notificationsServiceMock = mock()
        network = mock()
        connectionStateMonitor = ConnectionStateMonitor(ResourceMockingClass().mockConnectionStateMonitorContext())
        connectionStateMonitor.onAvailable(network)

        nhsWeb = NhsWeb(spyActivity, interactorMock, webViewMock, notificationsServiceMock, mock(),
                mock(), mock(), mock(), connectionStateMonitor, mock())

        spyWeb = spy(nhsWeb)

        urlLoader = mock()
        tempCacheDir = mock()
        tempAppWebViewDir = mock()
        ReflectionHelpers.setField(nhsWeb, "urlLoader", urlLoader)
        ReflectionHelpers.setField(nhsWeb, "cacheDir", tempCacheDir)
        ReflectionHelpers.setField(nhsWeb, "appWebViewDir", tempAppWebViewDir)
    }

    @After
    fun tearDown() {
        connectionStateMonitor.onLost(network)
    }

    @Test
    fun stopLoading_callsNativeAndroidWebViewStopLoadingFunction() {
        spyWeb.stopLoading()
        verify(webViewMock).stopLoading()
    }

    @Test
    fun loadUrl_LoadsRequestUrl_AndPassesValueOf_RequiresFullPageLoad_True() {
        nhsWeb.requiresFullPageLoad = true
        whenever(urlLoader.produceValidUrl(baseURL)).thenReturn(baseURL)

        nhsWeb.loadUrl(baseURL)

        verify(urlLoader).loadUrl(baseURL, true)
        assertEquals(baseURL, nhsWeb.reloadUrl)
    }

    @Test
    fun loadUrl_LoadsRequestUrl_AndPassesValueOf_RequiresFullPageLoad_False() {
        nhsWeb.requiresFullPageLoad = false
        whenever(urlLoader.produceValidUrl(baseURL)).thenReturn(baseURL)

        nhsWeb.loadUrl(baseURL)

        verify(urlLoader).loadUrl(baseURL, false)
        assertEquals(baseURL, nhsWeb.reloadUrl)
    }

    @Test
    fun loadUrl_WithNoConnection_Calls_ShowUnavailabilityError() {
        connectionStateMonitor.onLost(network)
        whenever(spyActivity.getString(R.string.baseHost)).thenReturn(URL(baseURL).host)
        whenever(webViewMock.url).thenReturn(baseURL)

        nhsWeb.loadUrl(baseURL)

        verify(interactorMock).showUnavailabilityError(any())
        assertNull(nhsWeb.reloadUrl)
        verify(urlLoader, times(0)).loadUrl(any(), any())
    }

    @Test
    fun loadWelcomePage_Loads_BaseUrl() {
        val resourceMock = resourceMocks
        val spyNhsWeb = spy(nhsWeb)
        whenever(spyActivity.resources).thenReturn(resourceMock)

        spyNhsWeb.loadWelcomePage()

        verify(spyNhsWeb).loadUrl(baseURL)
    }

    @Test
    fun loadWelcomePage_SendsAccessibilityEvent() {
        val resourceMock = resourceMocks
        val spyNhsWeb = spy(nhsWeb)
        whenever(spyActivity.resources).thenReturn(resourceMock)

        spyNhsWeb.loadWelcomePage()

        verify(spyNhsWeb).loadUrl(baseURL)
        verify(webViewMock, times(1))
                .sendAccessibilityEvent(AccessibilityEvent.TYPE_VIEW_ACCESSIBILITY_FOCUSED)
    }

    @Test
    fun loadUrl_ResetsIsLoggedInAndRequiresFullPageReload_WhenUserNavigatesToLoginPage() {
        nhsWeb.requiresFullPageLoad = false
        nhsWeb.isUserLoggedIn = true
        whenever(spyActivity.resources).thenReturn(resourceMocks)
        whenever(urlLoader.produceValidUrl(loginUrl)).thenReturn(loginUrl)

        nhsWeb.loadUrl(loginUrl)

        verify(urlLoader).loadUrl(loginUrl, true)
        assertEquals(loginUrl, nhsWeb.reloadUrl)
    }

    @Test
    fun loadUrl_DisplaysBiometricErrorAndLoadsWelcomePage_WhenThereIsAFidoLoginError() {
        val redirectUrl = "${baseURL}auth-return?error=blah"
        nhsWeb.requiresFullPageLoad = false
        nhsWeb.isUserLoggedIn = true
        whenever(spyActivity.resources).thenReturn(resourceMocks)
        whenever(interactorMock.canDisplayBiometricLogin()).thenReturn(true)
        whenever(urlLoader.produceValidUrl(baseURL)).thenReturn(baseURL)

        nhsWeb.loadUrl(redirectUrl)

        verify(interactorMock).displayBiometricLoginErrorOccurrence()
        verify(urlLoader).loadUrl(baseURL, false)
    }

    @Test
    fun loadUrl_whenCalledWithUrlContainingFidoAuthQuery_willClearPersistedLink_andLoadUrlContainingFidoAuthQuery() {
        val url = "http://unit-test.com?fidoAuthResponse=123abcdef123"
        persistData.storePersistedLink("https://unit-test.com/persisted-link")
        whenever(spyActivity.resources).thenReturn(resourceMocks)
        whenever(urlLoader.produceValidUrl(url)).thenReturn(url)

        nhsWeb.loadUrl(url)

        verify(urlLoader).loadUrl(url, true)
    }

    @Test
    fun loadUrl_whenCalledWithUrlContainingAuthRedirectPath_willClearPersistedLink_andLoadUrlContainingAuthRedirectPath() {
        val url = "http://unit-test.com/auth-return?code=123abcdef123"
        persistData.storePersistedLink("https://unit-test.com/persisted-link")
        whenever(spyActivity.resources).thenReturn(resourceMocks)
        whenever(urlLoader.produceValidUrl(url)).thenReturn(url)

        nhsWeb.loadUrl(url)

        verify(urlLoader).loadUrl(url, true)
    }

    @Test
    fun loadUrl_whenCalledWithUrlContaining111Host_willClearPersistedLink_andLoad111Url() {
        val url = "https://111.nhs.uk/"
        persistData.storePersistedLink("https://unit-test.com/persisted-link")
        whenever(spyActivity.resources).thenReturn(resourceMocks)
        whenever(urlLoader.produceValidUrl(url)).thenReturn(url)

        nhsWeb.loadUrl(url)

        verify(urlLoader).loadUrl(url, true)
    }

    @Test
    fun loadUrl_whenCalledWithUrl_withNoPersistedLink_willLoadUrl() {
        val url = "http://unit-test.com/another-url"
        whenever(spyActivity.resources).thenReturn(resourceMocks)
        whenever(urlLoader.produceValidUrl(url)).thenReturn(url)

        nhsWeb.loadUrl(url)

        verify(urlLoader).loadUrl(url, true)
    }

    @Test
    fun loadUrl_whenCalledWithUrlThatDoesNotIgnorePersistedLink_withPersistedLink_willLoadPersistedLink() {
        val persistedLink = "https://unit-test.com/persisted-link"
        persistData.storePersistedLink(persistedLink)
        whenever(spyActivity.resources).thenReturn(resourceMocks)

        nhsWeb.loadUrl("http://unit-test.com/another-url")

        verify(urlLoader).loadUrl(persistedLink, true)
    }

    @Test
    fun loadUrl_whenCalledWithUrl_withInvalidPersistedLink_willLoadBaseUrl() {
        persistData.storePersistedLink("/something-test.com/invalid-persisted-link")
        whenever(spyActivity.resources).thenReturn(resourceMocks)

        nhsWeb.loadUrl("http://unit-test.com/another-url")

        verify(urlLoader).loadUrl(baseURL, true)
    }

    @Test
    fun loadUrlInChromeTab() {
        val url = "http://unit-test.com"

        nhsWeb.loadUrlInChromeTab(url)

        verify(interactorMock).openUrlInBrowserActivity(url)
    }

    @Test
    fun reloadLoginUrl_Loads_CallsLoadUrlWithLoginUrl_When_CurrentWebViewUrlIsNotLoginUrl() {
        val spyNhsWeb = spy(nhsWeb)
        whenever(webViewMock.url).thenReturn(null)
        whenever(spyActivity.resources).thenReturn(resourceMocks)

        spyNhsWeb.reloadLoginUrl()

        verify(spyNhsWeb).loadUrl(loginUrl)
    }

    @Test
    fun reloadLoginUrl_Loads_CallsLoadUrlWithLoginUrl_When_CurrentUrlLoginUrlHasFidoQuery() {
        val spyNhsWeb = spy(nhsWeb)
        whenever(webViewMock.url).thenReturn(null)
        whenever(webViewMock.url).thenReturn("$loginUrl?fidoAuthResponse=fido")
        whenever(spyActivity.resources).thenReturn(resourceMocks)

        spyNhsWeb.reloadLoginUrl()

        verify(spyNhsWeb).loadUrl(loginUrl)
    }

    @Test
    fun reloadLoginUrl_CallsWebViewReloadMethod__When_CurrentUrlIsLoginUrlWithNoFidoQuery() {
        val spyNhsWeb = spy(nhsWeb)
        whenever(webViewMock.url).thenReturn(null)
        whenever(webViewMock.url).thenReturn(loginUrl)
        whenever(spyActivity.resources).thenReturn(resourceMocks)

        spyNhsWeb.reloadLoginUrl()

        verify(webViewMock).reload()
    }

    @Test
    fun onBiometricOptionChanged() {
        nhsWeb.onWebLoggedIn()
        nhsWeb.onBiometricOptionChanged()

        val cookies: String? = CookieManager.getInstance()
                .getCookie(activity.resources.getString(R.string.cookieDomain))
                ?.takeIf { it.contains("HideBiometricBanner=") }

        assert(!cookies.isNullOrBlank())
    }

    @Test
    fun onWebLoggedIn_Sets_IsLoginToTrue_And_ShowsHeaders_And_Menu_And_Clears_MenuBarItem() {
        nhsWeb.onWebLoggedIn()

        assertTrue(nhsWeb.isUserLoggedIn)
        verify(interactorMock).showHeader()
        verify(interactorMock).showMenuBar()
        verify(interactorMock).clearMenuBarItem()
    }

    @Test
    fun onWebLoggedOut_Sets_IsLoginToFalse_And_DismissSessionExtensionDialog() {
        val mockFiles = arrayOf<File>(mock(), mock())
        tempAppWebViewDir = mock {
            on {listFiles()}.thenReturn(mockFiles)
        }
        ReflectionHelpers.setField(nhsWeb, "appWebViewDir", tempAppWebViewDir)

        nhsWeb.onWebLoggedOut()

        assertFalse(nhsWeb.isUserLoggedIn)
        assertFalse(webViewMock.settings.builtInZoomControls)
        verify(interactorMock).dismissSessionExtensionDialog()
        verify(tempAppWebViewDir, atLeastOnce()).listFiles()

        mockFiles.forEach { file ->
            if (file.name != null && !file.name.contains("Cookies")) {
                file.delete()
            }
        }
    }

    @Test
    fun onShouldReloadHomepageOnBackReturn_ReturnsTrue_IfArrayContainsUrlWithMatchingHost() {
        val url = "http://auth.ext.signin.nhs.uk"
        whenever(spyActivity.resources).thenReturn(resourceMocks)

        val result = nhsWeb.shouldReloadHomepageOnBackReturn(url)

        assertTrue(result)
    }

    @Test
    fun onShouldReloadHomepageOnBackReturn_ReturnsFalse_IfArrayDoesNotContainUrlWithMatchingHost() {
        val url = "http://any.fake.uk"
        whenever(spyActivity.resources).thenReturn(resourceMocks)

        val result = nhsWeb.shouldReloadHomepageOnBackReturn(url)

        assertFalse(result)
    }

    @Test
    fun onShouldReloadHomepageOnBackReturn_ReturnsFalse_IfUrlNull() {
        whenever(spyActivity.resources).thenReturn(resourceMocks)

        val result = nhsWeb.shouldReloadHomepageOnBackReturn(null)

        assertFalse(result)
    }

    @Test
    fun onbackButtonPressedOnCheckSymptomsUnsecurePage_CallsReloadHomepageOnBackReturn_IfCanGoBackIsFalse() {
        whenever(webViewMock.canGoBack()).thenReturn(false)

        spyWeb.onbackButtonPressedOnLoggedOutUnsecurePage()

        verify(spyWeb).reloadHomepageOnBackReturn()
    }

    @Test
    fun onbackButtonPressedOnCheckSymptomsUnsecurePage_CallsGoBack_IfCanGoBackIsTrue() {
        whenever(webViewMock.canGoBack()).thenReturn(true)

        spyWeb.onbackButtonPressedOnLoggedOutUnsecurePage()

        verify(webViewMock).goBack()
    }

    @Test
    fun requestPnsToken_CallsNotificationsServiceRegisterForPushNotifications() {
        val trigger = "load"

        nhsWeb.requestPnsToken(trigger)

        verify(notificationsServiceMock).registerForPushNotifications(trigger)
    }

    private fun createResourceMocks(): Resources {
        return mock {
            on { getString(R.string.baseURL) } doReturn baseURL
            on { getString(R.string.loginPath) } doReturn loginPath
            on { getString(R.string.fidoAuthQueryKey) } doReturn "fidoAuthResponse"
            on { getString(R.string.authRedirectPath) } doReturn "/auth-return"
            on { getString(R.string.redirectErrorQueryParam) } doReturn "error"
            on { getString(R.string.nhs111URL) } doReturn "https://111.nhs.uk/"
            on { getStringArray(R.array.nativeReloadOnBackUrls) } doReturn arrayOf(
                    "https://ext.signin.nhs.uk"
            )
        }
    }
}
