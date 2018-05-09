package com.nhs.online.nhsonline

import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.view.View
import android.view.View.*
import android.support.v7.widget.Toolbar
import android.webkit.WebView
import android.widget.TextView
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
        setSupportActionBar(findViewById(R.id.header))
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        configureWebView()
        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        retryButton.setOnClickListener { webview.reload() }

        nhsOnlineLogoIcon.setOnClickListener{ onNhsOnlineLogoIconSelected() }

        val urlPath = intent?.data?.path
        val authRedirectPath = resources.getString(R.string.authRedirectPath)

        if (urlPath == authRedirectPath) {
            loadPage(intent.data.toString())

        } else {
            loadWelcomePage()
        }
    }

    override fun onNewIntent(intent: Intent?) {
        super.onNewIntent(intent)

        val data = intent?.data
        loadPage(data.toString())
    }

    private fun configureWebView() {
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true

        chromeClient = ChromeClientLocationHandler(this)
        webview.webChromeClient = chromeClient

        knownServices = KnownServices(this)
        webview.webViewClient = WebClientInterceptor(this, knownServices, createActivities(), this)

        webview.addJavascriptInterface( WebAppInterface(this), "nativeApp")
    }

    fun showMenuBar()
    {
        runOnUiThread({
            run {
                menuBar.visibility = VISIBLE
            }
        })
    }

    fun showHeader()
    {
        runOnUiThread({
            run {
                findViewById<Toolbar>(R.id.header).visibility = VISIBLE
            }
        })
    }

    private fun createActivities(): List<ActivityInterface> {
        val openBrowserActivity =
            OpenUrlInBrowserActivity(resources.getStringArray(R.array.nativeAppHosts))
        return listOf(openBrowserActivity)
    }

    private fun onMenuSelected(menuBarItem: MenuBarItem) {
        when (menuBarItem.id) {
            R.id.symptoms -> onSymptomMenuSelected()
            R.id.more -> onMoreMenuSelected()
            R.id.appointments -> onAppointmentsMenuSelected()
            R.id.prescriptions -> onPrescriptionsMenuSelected()
            else -> loadWelcomePage()
        }
    }

    private fun onSymptomMenuSelected() = loadPage(resources.getString(R.string.nhs111))

    private fun onMoreMenuSelected() = loadMorePage()

    private fun onAppointmentsMenuSelected() = loadAppointmentsPage()

    private fun onPrescriptionsMenuSelected() = loadPrescriptionsPage()

    private fun onNhsOnlineLogoIconSelected() = loadPage(resources.getString(R.string.baseURL))

    private fun loadWelcomePage() = loadPage(resources.getString(R.string.baseURL))

    private fun loadPage(url: String) {
        val urlWithMissingQueryStrings =
            knownServices.findKnownServiceAddMissingQueryFor(url)

        webview.loadUrl(urlWithMissingQueryStrings)
    }

    private fun loadMorePage() {
        val builtUri = Uri.parse(resources.getString(R.string.baseURL))
                .buildUpon()
                .appendEncodedPath(resources.getString(R.string.morePath))
                .build()
        val moreURL = builtUri.toString()
        loadPage(moreURL)
    }

    private fun loadAppointmentsPage() {
        val builtUri = Uri.parse(resources.getString(R.string.baseURL))
                .buildUpon()
                .appendEncodedPath(resources.getString(R.string.appointmentsPath))
                .build()
        val appointmentsURL = builtUri.toString()
        loadPage(appointmentsURL)
    }

    private fun loadPrescriptionsPage() {
        val builtUri = Uri.parse(resources.getString(R.string.baseURL))
                .buildUpon()
                .appendEncodedPath(resources.getString(R.string.prescriptionsPath))
                .build()
        val prescriptionsURL = builtUri.toString()
        loadPage(prescriptionsURL)
    }

    override fun setHeaderText(text: String) {
        runOnUiThread({
            run {
                findViewById<TextView>(R.id.header_text_view).text = text
            }
        })
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

    override fun selectMoreMenuActive() {
        menuBar.switchActiveMenuItemTo(R.id.more)
    }

    override fun showUnavailabilityError(unavailabilityErrorMessage: String?) {
        showErrorScreen()

        errorTextView.text = unavailabilityErrorMessage
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

