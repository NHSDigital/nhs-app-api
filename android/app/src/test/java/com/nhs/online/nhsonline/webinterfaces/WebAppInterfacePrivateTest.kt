package com.nhs.online.nhsonline.webinterfaces

import android.content.Context
import android.content.SharedPreferences
import android.content.res.Resources
import android.net.ConnectivityManager
import android.net.NetworkInfo
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.BuildConfig
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.data.AddToCalendarData
import com.nhs.online.nhsonline.interfaces.IAddToCalendarHandler
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.services.SettingsService
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.web.NhsWeb
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.Mockito
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class WebAppInterfacePrivateTest {
    private lateinit var contextMock: MainActivity
    private lateinit var nhsWebMock: NhsWeb
    private lateinit var webAppInterfacePrivate: WebAppInterfacePrivate
    private lateinit var settingsService: SettingsService
    private lateinit var addToCalendarHandlerMock: IAddToCalendarHandler
    private lateinit var addToCalendarData: AddToCalendarData

    @Before
    fun setUp() {
        val resourcesMock: Resources = mock {
            on { getString(any()) }.thenReturn("text")
        }
        val sharedPref: SharedPreferences = mock()
        contextMock = mock {
            val connectivityManager = Mockito.mock(ConnectivityManager::class.java)
            val networkInfo = Mockito.mock( NetworkInfo::class.java )

            Mockito.`when`( connectivityManager.activeNetworkInfo ).thenReturn(networkInfo)
            Mockito.`when`( networkInfo.isConnected).thenReturn( true )
            Mockito.`when`( networkInfo.isAvailable).thenReturn(true)
            Mockito.`when`( networkInfo.isConnectedOrConnecting).thenReturn(true)

            on { resources }.thenReturn(resourcesMock)
            on { getString(any()) }.thenReturn("text")
            on { getSharedPreferences(any(), any()) }.thenReturn(sharedPref)
            on { getSystemService(Context.CONNECTIVITY_SERVICE) } doReturn connectivityManager
        }

        settingsService = mock()
        doNothing().whenever(settingsService).openSettings()

        nhsWebMock = mock{
            on { applicationState }.thenReturn((mock()))
            on { javaScriptInteractionMode }.thenReturn( JavaScriptInteractionMode.NhsApp )
        }

        addToCalendarData = AddToCalendarData(
                "subject",
                "body",
                "location",
                123L,
                124L,
                JavaScriptInteractionMode.SilverThirdParty)

        addToCalendarHandlerMock = mock {
            on { parseCalendarData(any(), any()) }.thenReturn(addToCalendarData)
        }
        webAppInterfacePrivate = WebAppInterfacePrivate(contextMock, nhsWebMock, contextMock, settingsService, addToCalendarHandlerMock)
        ConnectionStateMonitor(ResourceMockingClass().mockConnectedContext()).onAvailable(mock())
    }

    @Test
    fun onLogin() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.onLogin()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).onWebLoggedIn()
    }

    @Test
    fun onLogout() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.onLogout()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).onWebLoggedOut()
    }

    @Test
    fun requestPnsToken() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.requestPnsToken("load")
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).requestPnsToken("load")
    }

    @Test
    fun clearMenuBar() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.clearMenuBarItem()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).clearMenuBarItem()
    }

    @Test
    fun displayPageLeaveWarningTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.displayPageLeaveWarning()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).showLeavingPageWarningDialogue()
    }


    @Test
    fun setMenuBarItemTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.setMenuBarItem(0)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).setMenuBarItem(0)
    }

    @Test
    fun pageFocus() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterfacePrivate.resetPageFocus()

        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).resetFocusToNhsLogoForAccessibility()
    }

    @Test
    fun whiteScreen() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.hideWhiteScreen()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).hideBlankScreen()
    }

    @Test
    fun onSessionExpiringTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.onSessionExpiring()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).showExtendSessionDialogue()
    }

    @Test
    fun addNotificationCookieTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.addNotificationCookie("loginId")
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).addNotificationCookie("loginId")
    }

    @Test
    fun checkNotificationCookieTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.checkNotificationCookie("nhsLoginId")
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).checkNotificationCookie("nhsLoginId")
    }

    @Test
    fun openAppSettingsTest() {
        webAppInterfacePrivate.openAppSettings()
        verify(settingsService).openSettings()
    }

    @Test
    fun fetchNativeAppVersionTest() {
        val version = webAppInterfacePrivate.fetchNativeAppVersion()
        Assert.assertNotNull(version)
        Assert.assertEquals(version, BuildConfig.VERSION_NAME)
    }

    @Test
    fun dismissProgressBarTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.dismissProgressBar()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).dismissProgressDialog()
    }

    @Test
    fun getNotificationStatusTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.getNotificationsStatus()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).getNotificationsStatus()
    }

    @Test
    fun attemptBiometricLoginTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.attemptBiometricLogin()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).showBiometricLoginIfEnabled()
    }

    @Test
    fun fetchBiometricSpecTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.fetchBiometricSpec()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).fetchBiometricSpec()
    }

    @Test
    fun configureWebContextTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.configureWebContext("url", "retryPath")
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).setHelpUrl("url")
        verify(contextMock).setRetryPath("retryPath")
    }

    @Test
    fun pageLoadCompleteTest() {
        webAppInterfacePrivate.pageLoadComplete()
        verify(nhsWebMock.applicationState).unBlock()
    }

    @Test
    fun setHelpUrlTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.setHelpUrl("url")
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).setHelpUrl("url")
    }

    @Test
    fun setZoomableTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.setZoomable(true)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).setZoomable(true)
    }

    @Test
    fun startDownloadTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.startDownload("base64", "file", "mime")
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).startDownload("base64", "file", "mime")
    }

    @Test
    fun showInternetConnectionError() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterfacePrivate.showInternetConnectionError()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).showInternetConnectionError()
    }

    @Test
    fun updateBiometricRegistrationWithTokenTest() {
        val mockAccessToken = "mockAccessToken"
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.updateBiometricRegistrationWithToken(mockAccessToken)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
    }

    @Test
    fun onAddToCalendar() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterfacePrivate.addEventToCalendar("stringifiedData")
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())

        runOnUiArgCaptor.firstValue.run()
        verify(addToCalendarHandlerMock).addToCalendar(addToCalendarData)
    }
}
