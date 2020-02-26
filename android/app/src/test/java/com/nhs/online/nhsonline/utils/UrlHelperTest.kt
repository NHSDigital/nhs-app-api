package com.nhs.online.nhsonline.utils

import android.app.Activity
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.services.KnownServices
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.internal.util.reflection.FieldSetter
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class UrlHelperTest {
    private val activity = Robolectric.buildActivity(Activity::class.java).get()
    private lateinit var urlHelper: UrlHelper

    @Before
    fun setUp() {
        urlHelper = UrlHelper(activity)
    }

    @Test
    fun ensureUrlWithScheme_CustomScheme_ReturnsValidUrl() {
        val url = "nhsapp://www.google.co.uk"

        val result = urlHelper.ensureUrlWithScheme(url)

        Assert.assertEquals("https", result?.protocol)
        Assert.assertEquals("www.google.co.uk", result?.host)
    }

    @Test
    fun ensureUrlWithScheme_NoScheme_ReturnsValidUrl() {
        val url = "www.google.co.uk"

        val result = urlHelper.ensureUrlWithScheme(url)

        Assert.assertEquals("https", result?.protocol)
        Assert.assertEquals("www.google.co.uk", result?.host)
    }

    @Test
    fun ensureUrlWithScheme_ValidUrlString_ReturnsValidUrl() {
        val url = "https://www.google.co.uk"

        val result = urlHelper.ensureUrlWithScheme(url)

        Assert.assertEquals("https", result?.protocol)
        Assert.assertEquals("www.google.co.uk", result?.host)
    }

    @Test
    fun ensureUrlWithScheme_InvalidUrlString_ReturnsNull() {
        val url = "fail://this.is.not.a.url"

        val result = urlHelper.ensureUrlWithScheme(url)

        Assert.assertNull(result)
    }

    @Test
    fun ensureUrlWithScheme_NullUrlString_ReturnsNull() {
        val url = null

        val result = urlHelper.ensureUrlWithScheme(url)

        Assert.assertNull(result)
    }

    @Test
    fun ensureUrlWithScheme_EmptyUrlString_ReturnsNull() {
        val url = ""

        val result = urlHelper.ensureUrlWithScheme(url)

        Assert.assertNull(result)
    }
}