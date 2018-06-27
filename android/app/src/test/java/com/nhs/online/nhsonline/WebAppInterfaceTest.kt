package com.nhs.online.nhsonline

import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.verify
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
        webAppInterface.onLogin()
        verify(contextMock).showHeader()
        verify(contextMock).showMenuBar()
    }

    @Test
    fun onLogout() {
        webAppInterface.onLogout()
        verify(contextMock).hideHeader()
        verify(contextMock).hideMenuBar()
    }

    @Test
    fun changeHeader() {
        val testText = "Test"
        webAppInterface.updateHeaderText("Test")
        verify(contextMock).setHeaderText(testText)
    }
}