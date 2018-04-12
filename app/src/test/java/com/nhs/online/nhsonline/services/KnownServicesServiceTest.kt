package com.nhs.online.nhsonline.services

import android.content.Context
import com.nhaarman.mockito_kotlin.mock
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import com.nhs.online.nhsonline.services.KnownServices

@RunWith(RobolectricTestRunner::class)
class KnownServicesServiceTest {

    @Test
    fun hasMissingQueryString_returnsTrue_forMissingQueryString(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000")

        Assert.assertTrue(result)
    }

    @Test
    fun hasMissingQueryString_returnsTrue_forMissingOneOfMany(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile&param2=param2Value")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?param2=param2Value")
        Assert.assertTrue(result)

        val result2 = testService.hasMissingQueryString("http://10.0.2.2:3000?source=mobile")
        Assert.assertTrue(result2)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forValidQueryString(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?source=mobile")

        Assert.assertFalse(result)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forValidQueryStringDifferentCase(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?SOURCE=MOBILE")

        Assert.assertFalse(result)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forValidQueryStringNoValue(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile")

        val result = testService.hasMissingQueryString("http://10.0.2.2:3000?source=")

        Assert.assertFalse(result)
    }

    @Test
    fun hasMissingQueryString_returnsFalse_forEmptyUrl(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile")

        val result = testService.hasMissingQueryString("")

        Assert.assertFalse(result)
    }

    @Test
    fun addMissingQueryStrings_returnsFullUrl_forMissingQueryString(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000")

        Assert.assertEquals("http://10.0.2.2:3000?source=mobile",result)
    }

    @Test
    fun addMissingQueryStrings_returnsFullUrl_forIncludedQueryString(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000?source=mobile")

        Assert.assertEquals("http://10.0.2.2:3000?source=mobile",result)
    }

    @Test
    fun addMissingQueryStrings_returnsOriginalUrl_forNonMatchingHost(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile")

        val result = testService.addMissingQueryStrings("http://www.google.com")

        Assert.assertEquals("http://www.google.com",result)
    }

    @Test
    fun addMissingQueryStrings_returnsFullUrl_forNoQueryString(){
        val testService = KnownServices.Service("http://10.0.2.2:3000")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000")

        Assert.assertEquals("http://10.0.2.2:3000",result)
    }

    @Test
    fun addMissingQueryStrings_returnsOriginalUrl_forNoQueryStringWithInputQueryString(){
        val testService = KnownServices.Service("http://10.0.2.2:3000")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000?param1=param1Value")

        Assert.assertEquals("http://10.0.2.2:3000?param1=param1Value",result)
    }

    @Test
    fun addMissingQueryStrings_returnsOriginalUrl_forQueryStringWithAdditionalInputQueryString(){
        val testService = KnownServices.Service("http://10.0.2.2:3000",queryString = "?source=mobile")

        val result = testService.addMissingQueryStrings("http://10.0.2.2:3000?param1=param1Value")

        Assert.assertEquals("http://10.0.2.2:3000?param1=param1Value&source=mobile",result)
    }
}