package com.nhs.online.nhsonline.utils

import android.content.Context
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNull
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import java.net.URL

@RunWith(RobolectricTestRunner::class)
class UrlHelperTest {
    private lateinit var contextMock: Context
    private lateinit var urlHelper: UrlHelper

    @Before
    fun setUp() {
        contextMock = ResourceMockingClass().mockContext()
        urlHelper = UrlHelper(contextMock)
    }

    @Test
    fun ensureUrlWithScheme_CustomScheme_ReturnsValidUrl() {
        val url = "nhsapp://www.google.co.uk"

        val result = urlHelper.ensureUrlWithScheme(url)

        assertEquals("https", result?.protocol)
        assertEquals("www.google.co.uk", result?.host)
    }

    @Test
    fun ensureUrlWithScheme_NoScheme_ReturnsValidUrl() {
        val url = "www.google.co.uk"

        val result = urlHelper.ensureUrlWithScheme(url)

        assertEquals("https", result?.protocol)
        assertEquals("www.google.co.uk", result?.host)
    }

    @Test
    fun ensureUrlWithScheme_ValidUrlString_ReturnsValidUrl() {
        val url = "http://www.google.co.uk"

        val result = urlHelper.ensureUrlWithScheme(url)

        assertEquals("http", result?.protocol)
        assertEquals("www.google.co.uk", result?.host)
    }

    @Test
    fun ensureUrlWithScheme_InvalidUrlString_ReturnsNull() {
        val url = "fail://this.is.not.a.url"

        val result = urlHelper.ensureUrlWithScheme(url)

        assertNull(result)
    }

    @Test
    fun ensureUrlWithScheme_NullUrlString_ReturnsNull() {
        val url = null

        val result = urlHelper.ensureUrlWithScheme(url)

        assertNull(result)
    }

    @Test
    fun ensureUrlWithScheme_EmptyUrlString_ReturnsNull() {
        val url = ""

        val result = urlHelper.ensureUrlWithScheme(url)
        assertNull(result)
    }

    @Test
    fun getPostRequestReloadUrl_postRequestDataPreferencesUrl() {
        val result =
                urlHelper.getPostRequestReloadUrl("https://ndopapp-int1.thunderbird.service.nhs.uk/")

        assertEquals(getResourceString(R.string.dataSharingURL), result)
    }

    @Test
    fun createRedirectToUrl_BaseUrl_ReturnsBaseUrl() {
        val result = urlHelper.createRedirectToUrl("https://www.baseurl.com/")

        assertEquals(URL("https://www.baseurl.com/"), result)
    }

    @Test
    fun createRedirectToUrl_BaseUrlWithoutScheme_ReturnsBaseUrl() {
        val result = urlHelper.createRedirectToUrl("www.baseurl.com/")

        assertEquals(URL("https://www.baseurl.com/"), result)
    }

    @Test
    fun createRedirectToUrl_ValidUrl_ReturnsRedirectUrl() {
        val url = "file://www.example.com";
        val result = urlHelper.createRedirectToUrl(url)

        assertEquals(URL("https://www.baseurl.com/redirector?redirect_to=${url}"), result)
    }

    @Test
    fun createRedirectToUrl_AppSchemeUrl_ReturnsRedirectUrlWithBaseScheme() {
        val url = "www.example.com"
        val result = urlHelper.createRedirectToUrl("nhsapp://${url}")

        assertEquals(URL("https://www.baseurl.com/redirector?redirect_to=https://${url}"), result)
    }

    @Test
    fun createRedirectToUrl_UrlWithoutScheme_ReturnsRedirectUrlAndAddsScheme() {
        val url = "www.example.com"
        val result = urlHelper.createRedirectToUrl(url)

        assertEquals(URL("https://www.baseurl.com/redirector?redirect_to=https://${url}"), result)
    }

    @Test
    fun createRedirectToUrl_FullInternalAppUrl_ReturnsFullInternalAppUrl() {
        val url = "https://www.baseurl.com/appointments"
        val result = urlHelper.createRedirectToUrl(url)

        assertEquals(URL(url), result)
    }

    @Test
    fun createRedirectToPageUrl_Page_ReturnsRedirectToPageUrl() {
        val page = "foo"
        val result = urlHelper.createRedirectToPageUrl(page)

        assertEquals(URL("https://www.baseurl.com/redirector?redirect_to_page=${page}"), result)
    }

    private fun getResourceString(resourceId: Int): String {
        return contextMock.resources.getString(resourceId)
    }
}
