package com.nhs.online.nhsonline.web

import android.app.Activity
import android.content.res.Resources
import android.webkit.CookieManager
import android.webkit.WebSettings
import android.webkit.WebView
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.MockConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.NotificationsService
import com.nhs.online.nhsonline.services.UrlLoader
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNull
import org.junit.Assert.assertTrue
import com.nhs.online.nhsonline.support.PersistData
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.util.ReflectionHelpers
import java.net.URL

@RunWith(RobolectricTestRunner::class)
class NhsWebTest {
    private val activity = Robolectric.buildActivity(Activity::class.java).get()
    private lateinit var spyActivity: Activity
    private lateinit var persistData: PersistData
    private lateinit var interactorMock: IInteractor
    private lateinit var webViewMock: WebView
    private lateinit var notificationsServiceMock: NotificationsService
    private lateinit var knownServicesMock: KnownServices
    private lateinit var nhsWeb: NhsWeb
    private lateinit var urlLoader: UrlLoader
    private lateinit var spyWeb: NhsWeb
    private lateinit var webSettings: WebSettings
    private var nhsLoginLoggedInPaths: List<String> = listOf()

    @Before
    fun setUp() {
        spyActivity = spy(activity)
        persistData = PersistData(spyActivity)
        interactorMock = mock()
        webSettings = mock()
        webViewMock = mock{
            on { settings }.doReturn(webSettings)
        }
        urlLoader = mock()
        interactorMock = mock()
        notificationsServiceMock = mock()
        knownServicesMock = mock()
        nhsLoginLoggedInPaths = mock()

        nhsWeb = NhsWeb(spyActivity, interactorMock, webViewMock, notificationsServiceMock, mock(), knownServicesMock, nhsLoginLoggedInPaths)
        spyWeb = spy(nhsWeb)
        ReflectionHelpers.setField(nhsWeb, "urlLoader", urlLoader)
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
    }

    @Test
    fun stopLoading_callsNativeAndroidWebViewStopLoadingFunction() {
        spyWeb.stopLoading()
        verify(webViewMock).stopLoading()
    }

    @Test
    fun loadUrl_LoadsRequestUrl_AndPassesValueOf_RequiresFullPageLoad_True() {
        val url = "http://unit-test.com"
        nhsWeb.requiresFullPageLoad = true
        whenever(urlLoader.produceValidUrl(url)).thenReturn(url)
        nhsWeb.loadUrl(url)
        verify(urlLoader).loadUrl(url, true)
        assertEquals(url, nhsWeb.reloadUrl)
    }

    @Test
    fun loadUrl_LoadsRequestUrl_AndPassesValueOf_RequiresFullPageLoad_False() {
        val url = "http://unit-test.com"
        nhsWeb.requiresFullPageLoad = false
        whenever(urlLoader.produceValidUrl(url)).thenReturn(url)
        nhsWeb.loadUrl(url)
        verify(urlLoader).loadUrl(url, false)
        assertEquals(url, nhsWeb.reloadUrl)
    }

    @Test
    fun loadUrl_WithNoConnection_Calls_ShowUnavailabilityError() {
        val url = "http://unit-test.com"

        whenever(spyActivity.getString(R.string.baseHost)).thenReturn(URL(url).host)
        whenever(webViewMock?.url).thenReturn(url)

        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())

        nhsWeb.loadUrl(url)
        verify(interactorMock).showUnavailabilityError(any())
        assertNull(nhsWeb.reloadUrl)
        verifyZeroInteractions(urlLoader)
    }

    @Test
    fun loadWelcomePage_Loads_BaseUrl() {
        val baseUrl = "http://unit-test.com"
        val resourceMock: Resources = mock {
            on { getString(R.string.baseURL) } doReturn baseUrl
            on { getString(R.string.loginPath) } doReturn "login"
            on { getString(R.string.authRedirectPath) } doReturn "/auth-return"
        }
        whenever(spyActivity.resources).thenReturn(resourceMock)
        val spyNhsWeb = spy(nhsWeb)
        spyNhsWeb.loadWelcomePage()
        verify(spyNhsWeb).loadUrl(baseUrl)
    }

    @Test
    fun loadUrl_ResetsIsLoggedInAndRequiresFullPageReload_WhenUserNavigatesToLoginPage() {
        // arrange
        val baseUrl = "http://unit-test.com/"
        val loginPath = "login"
        val resourceMock: Resources = mock {
            on { getString(R.string.baseURL) } doReturn baseUrl
            on { getString(R.string.loginPath) } doReturn loginPath
            on { getString(R.string.authRedirectPath) } doReturn "/auth-return"
        }
        val loginUrl = baseUrl + "login"
        whenever(spyActivity.resources).thenReturn(resourceMock)
        nhsWeb.requiresFullPageLoad = false
        nhsWeb.isUserLoggedIn = true
        whenever(urlLoader.produceValidUrl(loginUrl)).thenReturn(loginUrl)
        nhsWeb.loadUrl(loginUrl)

        // act
        verify(urlLoader).loadUrl(loginUrl, true)

        // assert
        assertEquals(loginUrl, nhsWeb.reloadUrl)
    }

    @Test
    fun loadUrl_DisplaysBiometricErrorAndLoadsWelcomePage_WhenThereIsAFidoLoginError() {
        // arrange
        val baseUrl = "http://unit-test.com/"
        val loginPath = "login"
        val resourceMock: Resources = mock {
            on { getString(R.string.baseURL) } doReturn baseUrl
            on { getString(R.string.loginPath) } doReturn loginPath
            on { getString(R.string.authRedirectPath) } doReturn "/auth-return"
            on { getString(R.string.redirectErrorQueryParam) } doReturn "error"
        }
        val redirectUrl = "${baseUrl}auth-return?error=blah"
        whenever(spyActivity.resources).thenReturn(resourceMock)
        whenever(interactorMock.canDisplayBiometricLogin()).thenReturn(true)
        nhsWeb.requiresFullPageLoad = false
        nhsWeb.isUserLoggedIn = true
        whenever(urlLoader.produceValidUrl(baseUrl)).thenReturn(baseUrl)

        // act
        nhsWeb.loadUrl(redirectUrl)

        // assert
        verify(interactorMock).displayBiometricLoginErrorOccurrence()
        verify(urlLoader).loadUrl(baseUrl, false)
    }

    @Test
    fun loadUrlInChromeTab() {
        val openBrowserActivityMock: OpenUrlInBrowserActivity = mock()
        ReflectionHelpers.setField(nhsWeb, "openBrowserActivity", openBrowserActivityMock)
        val url = "http://unit-test.com"
        nhsWeb.loadUrlInChromeTab(url)
        verify(openBrowserActivityMock).start(any(), any(), any())
    }

    @Test
    fun reloadLoginUrl_Loads_CallsLoadUrlWithLoginUrl_When_CurrentWebViewUrlIsNotLoginUrl() {
        whenever(webViewMock.url).thenReturn(null)
        val baseUrl = "http://unit-test.com/"
        val resourceMock: Resources = mock {
            on { getString(R.string.baseURL) } doReturn baseUrl
            on { getString(R.string.loginPath) } doReturn "login"
            on { getString(R.string.authRedirectPath) } doReturn "/auth-return"
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
            on { getString(R.string.authRedirectPath) } doReturn "/auth-return"
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
        nhsWeb.onWebLoggedOut()
        assertFalse(nhsWeb.isUserLoggedIn)
        assertFalse(webViewMock.settings.builtInZoomControls)
        verify(interactorMock).dismissSessionExtensionDialog()
    }

    @Test
    fun onShouldReloadHomepageOnBackReturn_ReturnsTrue_IfInArray() {
        val url = "http://auth.ext.signin.nhs.uk"
        val resourceMock: Resources = mock {
            on { getStringArray(R.array.nativeReloadOnBackUrls) } doReturn arrayOf(
                    "https://ext.signin.nhs.uk"
            )
        }
        whenever(spyActivity.resources).thenReturn(resourceMock)

        val result = nhsWeb.shouldReloadHomepageOnBackReturn(url)

        assertTrue(result)
    }

    @Test
    fun onShouldReloadHomepageOnBackReturn_ReturnsFalse_IfNotInArray() {
        val url = "http://any.fake.uk"
        val resourceMock: Resources = mock {
            on { getStringArray(R.array.nativeReloadOnBackUrls) } doReturn arrayOf(
                    "https://ext.signin.nhs.uk"
            )
        }
        whenever(spyActivity.resources).thenReturn(resourceMock)

        val result = nhsWeb.shouldReloadHomepageOnBackReturn(url)

        assertFalse(result)
    }

    @Test
    fun onShouldReloadHomepageOnBackReturn_ReturnsFalse_IfUrlNull() {
        val url: String? = null
        val resourceMock: Resources = mock {
            on { getStringArray(R.array.nativeReloadOnBackUrls) } doReturn arrayOf(
                    "https://ext.signin.nhs.uk"
            )
        }
        whenever(spyActivity.resources).thenReturn(resourceMock)

        val result = nhsWeb.shouldReloadHomepageOnBackReturn(url)

        assertFalse(result)
    }

    @Test
    fun onbackButtonPressedOnCheckSymptomsUnsecurePage_CallsReloadHomepageOnBackReturn_IfCanGoBackIsFalse() {
        whenever(webViewMock.canGoBack()).thenReturn(false)
        spyWeb.onbackButtonPressedOnCheckSymptomsUnsecurePage()

        verify(spyWeb).reloadHomepageOnBackReturn()
    }

    @Test
    fun onbackButtonPressedOnCheckSymptomsUnsecurePage_CallsGoBack_IfCanGoBackIsTrue() {
        whenever(webViewMock.canGoBack()).thenReturn(true)
        spyWeb.onbackButtonPressedOnCheckSymptomsUnsecurePage()

        verify(webViewMock).goBack()
    }

    @Test
    fun requestPnsToken_CallsNotificationsServiceRegisterForPushNotifications() {
        nhsWeb.requestPnsToken("load")

        verify(notificationsServiceMock).registerForPushNotifications("load")
    }
}