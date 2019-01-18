package com.nhs.online.nhsonline.webclients

import android.Manifest
import android.app.Activity
import android.webkit.GeolocationPermissions
import android.webkit.WebView
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.verify
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner


@RunWith(RobolectricTestRunner::class)
class ChromeClientLocationTest {

    private lateinit var webviewMock: WebView
    private lateinit var callBack : GeolocationPermissions.Callback
    
    @Before
    fun setUp() {
        webviewMock =  mock()
        callBack = mock()
    }

    @Test
    fun locationPermissionRespondedNullCallBack(){
        // Callback function is null at this point, handler should handle null callback
        val activity: Activity = mock()
        val chromeClient = ChromeClientLocationHandler(activity)

        chromeClient.onLocationPermissionResponded(true)
    }

    @Test
    fun showPromptPermissionGranted(){
        // Use context that allows use of geolocation, should just invoke the callback to store
        // the settings
        val activity = ResourceMockingClass().mockGeolocationPermissionsAllow()
        val chromeClient = ChromeClientLocationHandler(activity)

        chromeClient.onGeolocationPermissionsShowPrompt("https://www.nhs.uk", callBack)
        verify(callBack).invoke("https://www.nhs.uk", true, false)
    }

    @Test
    fun showPromptPermissionAllowedAfterRequest() {
        // Does not allow permission to begin with, and denies attempts for requests
        val activity = ResourceMockingClass().mockGeolocationPermissionsDenyPermissionPopup()
        val chromeClient = ChromeClientLocationHandler(activity)

        // Should ensure callback is made allowing permission for the given site
        chromeClient.onGeolocationPermissionsShowPrompt("https://www.nhs.uk", callBack)
        verify(callBack).invoke("https://www.nhs.uk", false, false)
    }

    @Test
    fun showPromptShowPermissionRequest() {
        // Does not allow permission initially, but allows a popup to be shown to the user
        // to allow permission if they wish
        val activity = ResourceMockingClass().mockGeolocationPermissionsDenyRational()
        val chromeClient = ChromeClientLocationHandler(activity)


        chromeClient.onGeolocationPermissionsShowPrompt("https://www.nhs.uk", callBack)
        // This checks that permission is requested on this activity
        verify(activity).requestPermissions(arrayOf(Manifest.permission.ACCESS_FINE_LOCATION),101)
    }

}