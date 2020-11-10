package com.nhs.online.nhsonline.client

import com.android.installreferrer.api.InstallReferrerClient
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.clients.ReferrerClient
import com.nhs.online.nhsonline.support.AppSharedPref
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class ReferrerClientTests: ResourceMockingClass() {

    private lateinit var referrerClientMock : InstallReferrerClient
    private lateinit var referrerClient: ReferrerClient
    private lateinit var appWebInterfaceMock: AppWebInterface
    private lateinit var resourceMockingClass: ResourceMockingClass
    private lateinit var prefs: AppSharedPref

    @Before
    fun setup(){
        resourceMockingClass = ResourceMockingClass()
        appWebInterfaceMock = mock()
        referrerClientMock = mock()
        prefs = mock()
        referrerClient = ReferrerClient(prefs, referrerClientMock)
    }

    @Test
    fun storeReferrer_willNotCallGoogleIfPrefExists() {
        whenever(prefs.readString("appReferrer", "")).thenReturn("true")
        referrerClient.storeReferrer()
        verify(referrerClientMock, never()).endConnection()

    }

    @Test
    fun storeReferrer_willCallGoogleIfPrefDoesntExists() {
        whenever(prefs.readString("appReferrer", "")).thenReturn(null)
        doNothing().whenever(referrerClientMock).startConnection(any())
        referrerClient.storeReferrer()
        verify(referrerClientMock, times(1)).startConnection(any())

    }

    @Test
    fun getReferrer_willReturnReferrerIfPrefExists() {
        whenever(prefs.readString("appReferrer", "")).thenReturn("test")
        val referrer = referrerClient.getReferrer()
        Assert.assertEquals("test", referrer)

    }

    @Test
    fun getReferrer_willReturnBlankIfPrefDoesntExists() {
        whenever(prefs.readString("appReferrer", "")).thenReturn(null)
        val referrer = referrerClient.getReferrer()
        Assert.assertEquals("", referrer)

    }


}
