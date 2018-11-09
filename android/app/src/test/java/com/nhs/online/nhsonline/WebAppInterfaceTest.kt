package com.nhs.online.nhsonline

import com.nhaarman.mockito_kotlin.argumentCaptor
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.verify
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.webinterfaces.WebAppInterface
import org.junit.Test

import org.junit.Before
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class WebAppInterfaceTest {

    private lateinit var contextMock: MainActivity
    private lateinit var webAppInterface: WebAppInterface

    @Before
    fun SetUp() {
        contextMock = mock()
        webAppInterface = WebAppInterface(contextMock)
    }

    @Test
    fun onLogin() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()
        webAppInterface.onLogin()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).loggedIn()
    }

    @Test
    fun onLogout() {
        val runOnUiArgCaptor = argumentCaptor<Runnable>()

        webAppInterface.onLogout()
        verify(contextMock).runOnUiThread(runOnUiArgCaptor.capture())
        runOnUiArgCaptor.firstValue.run()
        verify(contextMock).loggedOut()
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
}