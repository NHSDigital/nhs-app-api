package com.nhs.online.nhsonline.webclients

import android.Manifest
import android.app.Activity
import android.content.pm.PackageManager
import android.support.v4.app.ActivityCompat
import android.webkit.GeolocationPermissions
import android.webkit.WebChromeClient

internal const val LOCATION_REQUEST_CODE = 101

class ChromeClientLocationHandler(private val activity: Activity) : WebChromeClient() {
    private var mCallback: GeolocationPermissions.Callback? = null
    private var mOrigin: String? = null
    private fun requiresLocationPermissionLocationRationale(): Boolean {
        return ActivityCompat.shouldShowRequestPermissionRationale(activity,
            Manifest.permission.ACCESS_FINE_LOCATION)
    }

    private fun showLocationPermissionPopup() {
        ActivityCompat.requestPermissions(activity,
            arrayOf(Manifest.permission.ACCESS_FINE_LOCATION),
            LOCATION_REQUEST_CODE)
    }

    private fun isLocationPermissionGranted(): Boolean {
        return ActivityCompat.checkSelfPermission(activity,
            Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED
    }

    override fun onGeolocationPermissionsShowPrompt(
        origin: String?,
        callback: GeolocationPermissions.Callback?
    ) {
        if (isLocationPermissionGranted()) {
            callback?.invoke(origin, true, false)
        } else {
            if (!requiresLocationPermissionLocationRationale()) {
                showLocationPermissionPopup()
                mCallback = callback
                mOrigin = origin
            } else {
                callback?.invoke(origin, false, false)
            }
        }
    }

    fun onLocationPermissionResponded(permissionGranted: Boolean) {
        mCallback?.invoke(mOrigin, permissionGranted, false)
    }
}