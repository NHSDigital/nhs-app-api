package com.nhs.online.nhsonline

import android.Manifest
import android.content.pm.PackageManager
import android.os.Bundle
import android.support.v4.app.ActivityCompat
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.view.View
import android.webkit.GeolocationPermissions
import android.webkit.WebChromeClient
import com.nhs.online.nhsonline.activity.ActivityInterface
import com.nhs.online.nhsonline.activity.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.navigation.MenuBarItem
import kotlinx.android.synthetic.main.activity_main.*

private const val LOCATION_REQUEST_CODE = 101

class MainActivity : IInteractor, AppCompatActivity() {
    private lateinit var chromeClient: ChromeClient
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        configureWebView()
        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        retryButton.setOnClickListener { onSymptomMenuSelected() }
        loadWelcomePage()
    }

    private fun configureWebView() {
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true

        chromeClient = ChromeClient()
        webview.webChromeClient = chromeClient

        webview.webViewClient =
                WebClientInterceptor(this, resources.getStringArray(R.array.serviceUrls), createAvtivities())
    }

    private fun createAvtivities(): List<ActivityInterface>
    {
        val openBrowserActivity = OpenUrlInBrowserActivity(resources.getStringArray(R.array.nativeAppHosts))
        return listOf(openBrowserActivity)
    }

    private fun onMenuSelected(menuBarItem: MenuBarItem) {
        when (menuBarItem.id) {
            R.id.symptoms -> onSymptomMenuSelected()
            else -> loadWelcomePage()
        }
    }

    private fun onSymptomMenuSelected() = loadPage(resources.getString(R.string.nhs111))

    private fun loadWelcomePage() = loadPage(resources.getString(R.string.baseURL))

    private fun loadPage(url: String) {
        webview.loadUrl(url)
    }

    override fun showProgressDialog() {
        if (progressBarLayout.visibility == View.GONE)
            progressBarLayout.visibility = View.VISIBLE
    }

    override fun dismissProgressDialog() {
        if (progressBarLayout.visibility == View.VISIBLE)
            progressBarLayout.visibility = View.GONE
    }

    override fun selectSymptomsMenuActive() {
        menuBar.switchActiveMenuItemTo(R.id.symptoms)
    }

    override fun showUnavailabilityError() {
        showErrorScreen()

        val errorMessage = resources.getString(R.string.nhs111_connection_error)
        errorTextView.text = errorMessage

    }

    private fun showErrorScreen() {
        errorViewLayout.visibility = View.VISIBLE
        webview.visibility = View.GONE
    }

    override fun showWebviewScreen() {
        errorViewLayout.visibility = View.GONE
        webview.visibility = View.VISIBLE
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<out String>,
        grantResults: IntArray
    ) {
        if (requestCode == LOCATION_REQUEST_CODE) {
            if (grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                chromeClient.onLocationPermissionResponded(true)
            } else {
                chromeClient.onLocationPermissionResponded(false)
            }
        }
    }

    private fun requiresLocationPermissionLocationRationale(): Boolean {
        return ActivityCompat.shouldShowRequestPermissionRationale(this,
            Manifest.permission.ACCESS_FINE_LOCATION)
    }

    private fun showLocationPermissionPopup() {
        ActivityCompat.requestPermissions(this,
            arrayOf(Manifest.permission.ACCESS_FINE_LOCATION),
            LOCATION_REQUEST_CODE)
    }

    private fun isLocationPermissionGranted(): Boolean {
        return ActivityCompat.checkSelfPermission(this,
            Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED
    }

    private inner class ChromeClient : WebChromeClient() {
        private var mCallback: GeolocationPermissions.Callback? = null
        private var mOrigin: String? = null
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
}

