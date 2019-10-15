package com.nhs.online.nhsonline

import android.content.Context
import android.content.SharedPreferences
import android.content.res.Resources
import android.net.ConnectivityManager
import android.net.NetworkInfo
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.network.MockConnectionStateMonitor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.web.NhsWeb
import com.nhs.online.nhsonline.webinterfaces.WebAppInterface
import junit.framework.Assert
import org.junit.Test

import org.junit.Before
import org.junit.runner.RunWith
import org.mockito.Mockito
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class WebAppInterfaceTest {
    private lateinit var contextMock: MainActivity
    private lateinit var webAppInterface: WebAppInterface
    private lateinit var nhsWebMock: NhsWeb

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
            Mockito.`when`(networkInfo.isAvailable).thenReturn(true)
            Mockito.`when`(networkInfo.isConnectedOrConnecting).thenReturn(true)

            on { resources }.thenReturn(resourcesMock)
            on { getString(any()) }.thenReturn("text")
            on { getSharedPreferences(any(), any()) }.thenReturn(sharedPref)
            on {getSystemService(Context.CONNECTIVITY_SERVICE) } doReturn connectivityManager
        }
        nhsWebMock = mock()
        webAppInterface = WebAppInterface(contextMock, contextMock, nhsWebMock)
        MockConnectionStateMonitor().mockNetworkCallback(ResourceMockingClass().mockConnectedContext())

    }

    @Test
    fun onLogin() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.onLogin()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).onWebLoggedIn()
    }

    @Test
    fun onLogout() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterface.onLogout()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).onWebLoggedOut()
    }

    @Test
    fun requestPnsToken() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterface.requestPnsToken("load")
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).requestPnsToken("load")
    }

    @Test
    fun changeHeader() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        val testText = "Test"
        webAppInterface.updateHeaderText("Test")
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).setHeaderText(testText)
    }

    @Test
    fun clearMenuBar() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.clearMenuBarItem()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).clearMenuBarItem()
    }


    @Test
    fun hideHeaderTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.hideHeader()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).hideHeader()
    }

    @Test
    fun hideHeaderSlimTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.hideHeaderSlim()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).hideHeaderSlim()
    }

    @Test
    fun setMenuBarItemTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.setMenuBarItem(0)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).setMenuBarItem(0)
    }

    @Test
    fun showHeaderTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.showHeader()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).showHeader()
    }

    @Test
    fun showHeaderSlimTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.showHeaderSlim()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).showHeaderSlim()
    }

    @Test
    fun hideMenuBarTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.hideMenuBar()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).hideMenuBar()
    }

    @Test
    fun showMenuBarTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.showMenuBar()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).showMenuBar()
    }

    @Test
    fun pageFocus() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.resetPageFocus()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).resetFocusToNhsLogoForAccessibility()
    }

    @Test
    fun whiteScreen() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.hideWhiteScreen()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).hideBlankScreen()
    }

    @Test
    fun biometrics() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.goToLoginOptions()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).showNativeBiometricOptions()
    }

    @Test
    fun storeBetaCookie() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.storeBetaCookie()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(nhsWebMock).onBetaCookieStoreRequest()
    }

    @Test
    fun onSessionExpiringTest() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.onSessionExpiring(10)
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).showExtendSessionDialogue(10)
    }

    @Test
    fun fetchNativeAppVersionTest() {
        val version = webAppInterface.fetchNativeAppVersion()
        Assert.assertNotNull(version)
        Assert.assertEquals(version, BuildConfig.VERSION_NAME)
    }
}