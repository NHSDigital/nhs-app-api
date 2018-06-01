package com.nhs.online.nhsonline.services

import android.content.Context
import android.content.res.Resources
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import org.junit.Assert
import org.junit.Test
import com.nhs.online.nhsonline.R
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import java.net.URL


@RunWith(RobolectricTestRunner::class)
class KnownServicesTest {

    fun mockContext():Context {
        var mockresource: Resources = mock() {on {getString(R.string.baseURL)} doReturn "http://10.0.2.2:3000"
            on {getString(R.string.nhs111)} doReturn "https://111.nhs.uk"
            on {getString(R.string.nhs111Location)} doReturn "https://111.service.nhs.uk/"
            on {getString(R.string.organDonation)} doReturn "https://www.organdonation.nhs.uk/"
            on {getString(R.string.nhs111_connection_error)} doReturn "NHS 111 unavailable"
            on {getString(R.string.organ_donation_connection_error)} doReturn "Organ donation unavailable"
            on {getString(R.string.service_unavailable)} doReturn "Service unavailable"
            on {getString(R.string.nhsOnlineRequiredQueries)} doReturn "?source=mobile"
        }
        var context: Context = mock(){on {resources} doReturn mockresource}

        return context
    }

    @Test
    fun findMatchingKnownService_returnsNull_forNotAUrl() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findMatchingKnownService("Not A Url")

        Assert.assertNull(result)
    }

    @Test
    fun findMatchingKnownService_returnsService_forValidServiceUrl() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findMatchingKnownService("https://111.nhs.uk")

        Assert.assertEquals(URL("https://111.nhs.uk"), result?.urlList?.get(0))
    }

    @Test
    fun findMatchingKnownService_returnsService_forValidServiceUrlDifferentCasing() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findMatchingKnownService("https://111.NHS.UK")

        Assert.assertEquals(URL("https://111.nhs.uk"),result?.urlList?.get(0))
    }

    @Test
    fun findMatchingKnownService_returnsService_forValidServiceUrlWithQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findMatchingKnownService("http://10.0.2.2:3000?source=mobile")

        Assert.assertEquals(URL("http://10.0.2.2:3000"),result?.urlList?.get(0))
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceNoQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("https://111.nhs.uk")

        Assert.assertEquals("https://111.nhs.uk",result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceNoQueryStringDifferentCasing() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("https://111.NHS.UK")

        Assert.assertEquals("https://111.NHS.UK",result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceWithQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("http://10.0.2.2:3000")

        Assert.assertEquals("http://10.0.2.2:3000?source=mobile",result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceNoQueryStringWithInputQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("https://111.nhs.uk?param1=param1Value")

        Assert.assertEquals("https://111.nhs.uk?param1=param1Value",result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceWithQueryStringWithInputQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("http://10.0.2.2:3000?source=mobile")

        Assert.assertEquals("http://10.0.2.2:3000?source=mobile",result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsValidUrl_forValidServiceWithQueryStringWithAdditionalInputQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("http://10.0.2.2:3000?param1=param1Value")

        Assert.assertEquals("http://10.0.2.2:3000?param1=param1Value&source=mobile",result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsOriginalUrl_forInvalidServiceWithInputQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("https://www.google.co.uk?param1=param1Value")

        Assert.assertEquals("https://www.google.co.uk?param1=param1Value",result)
    }

    @Test
    fun findKnownServiceAddMissingQuery_returnsOriginalUrl_forInvalidServiceNoQueryString() {
        var context: Context = mockContext()
        val testKnownServices = KnownServices(context)

        val result = testKnownServices.findKnownServiceAddMissingQueryFor("https://www.google.co.uk")

        Assert.assertEquals("https://www.google.co.uk",result)
    }
    
}