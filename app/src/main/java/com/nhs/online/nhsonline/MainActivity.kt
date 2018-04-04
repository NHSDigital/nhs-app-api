package com.nhs.online.nhsonline

import android.content.Intent
import android.content.pm.PackageManager
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.view.View
import com.nhs.online.nhsonline.activity.ActivityInterface
import com.nhs.online.nhsonline.activity.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.navigation.MenuBarItem
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.webclients.ChromeClientLocationHandler
import com.nhs.online.nhsonline.webclients.LOCATION_REQUEST_CODE
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : IInteractor, AppCompatActivity() {
    private lateinit var chromeClient: ChromeClientLocationHandler

    private lateinit var knownServices: KnownServices
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        configureWebView()
        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        retryButton.setOnClickListener { onSymptomMenuSelected() }

        val urlPath = intent?.data?.path
        val authRedirectPath = resources.getString(R.string.authRedirectPath)

        if (urlPath == authRedirectPath) {
            loadPage(intent.data.toString());

        } else {
            loadWelcomePage()
        }
    }

    override fun onNewIntent(intent: Intent?) {
        super.onNewIntent(intent)

        val data = intent?.data
        webview.loadUrl(data.toString())
    }

    private fun configureWebView() {
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true

        chromeClient = ChromeClientLocationHandler(this)
        webview.webChromeClient = chromeClient

        knownServices = KnownServices(this)
        webview.webViewClient = WebClientInterceptor(this, knownServices, createActivities())
    }

    private fun createActivities(): List<ActivityInterface> {
        val openBrowserActivity =
            OpenUrlInBrowserActivity(resources.getStringArray(R.array.nativeAppHosts))
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
        val urlWithMissingQueryStrings =
            knownServices.findKnownServiceAddMissingQueryFor(url)
        webview.loadUrl(urlWithMissingQueryStrings)
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
}

