package com.nhs.online.nhsonline.activities

import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.view.View
import android.view.View.*
import android.view.WindowManager
import android.webkit.CookieManager
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.ActivityInterface
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.navigation.MenuBarItem
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.support.LifeCycleObserver
import com.nhs.online.nhsonline.support.setServiceError
import com.nhs.online.nhsonline.webclients.ChromeClientLocationHandler
import com.nhs.online.nhsonline.webclients.LOCATION_REQUEST_CODE
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.webinterfaces.WebAppInterface
import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.error_layout.*
import kotlinx.android.synthetic.main.header_layout.*

class MainActivity : IInteractor, AppCompatActivity() {

    private lateinit var chromeClient: ChromeClientLocationHandler
    private lateinit var knownServices: KnownServices
    private var lifeCycleObserver: LifeCycleObserver? = null
    private var reloadUrl: String? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        window.setFlags(WindowManager.LayoutParams.FLAG_SECURE, WindowManager.LayoutParams.FLAG_SECURE)
        CookieManager.getInstance().removeAllCookies(null)

        setContentView(R.layout.activity_main)
        setSupportActionBar(findViewById(R.id.header))
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        configureWebView()
        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        retryButton.setOnClickListener { reloadRequest() }
        nhsOnlineLogoIcon.setOnClickListener { onNhsOnlineLogoIconSelected() }
        myAccountIcon.setOnClickListener { onMyAccountIconSelected() }

        val urlPath = intent?.data?.path
        val authRedirectPath = resources.getString(R.string.authRedirectPath)

        if (urlPath == authRedirectPath) {
            loadPage(intent.data.toString())
        } else {
            loadWelcomePage()
        }
    }

    override fun onStart() {
        super.onStart()
        if (lifeCycleObserver == null) {
            lifeCycleObserver = LifeCycleObserver(this,
                        AppWebInterface(this),
                        knownServices)
        }

        lifeCycleObserver?.onMoveToForeground()
    }

    override fun onStop() {
        super.onStop()
        lifeCycleObserver?.onMoveToBackground()
    }

    override fun onNewIntent(intent: Intent?) {
        super.onNewIntent(intent)

        val data = intent?.data
        if (data != null) {
            loadPage(data.toString())
        }
    }

    override fun setReloadUrl(url: String?) {
        reloadUrl = url
    }

    private fun configureWebView() {
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true

        chromeClient = ChromeClientLocationHandler(this)
        webview.webChromeClient = chromeClient

        knownServices = KnownServices(this)
        webview.webViewClient = WebClientInterceptor(this, knownServices, createActivities(), this)

        webview.addJavascriptInterface(WebAppInterface(this), "nativeApp")
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
            R.id.myRecord -> onMyRecordMenuSelected()
            R.id.appointments -> onAppointmentsMenuSelected()
            R.id.prescriptions -> onPrescriptionsMenuSelected()
            else -> loadWelcomePage()
        }
    }

    private fun onSymptomMenuSelected() {
        loadPage(resources.getString(R.string.nhs111))
        setHeaderText(resources.getString(R.string.nhs_111_header))
    }

    private fun onMyRecordMenuSelected() =
        loadSubPage(resources.getString(R.string.myRecordPath),
            resources.getString(R.string.my_record_header))

    private fun onMoreMenuSelected() =
        loadSubPage(resources.getString(R.string.morePath), resources.getString(
            R.string.more))

    private fun onAppointmentsMenuSelected() =
        loadSubPage(resources.getString(R.string.appointmentsPath),
            resources.getString(R.string.appointments_header))

    private fun onPrescriptionsMenuSelected() =
        loadSubPage(resources.getString(R.string.prescriptionsPath),
            resources.getString(R.string.prescriptions_header))

    private fun onNhsOnlineLogoIconSelected() {
        loadWelcomePage()
        menuBar.deselectActiveItem()
        setHeaderText(resources.getString(R.string.home_header))
    }

    private fun onMyAccountIconSelected() {
        loadSubPage(resources.getString(R.string.myAccountPath),
            resources.getString(R.string.my_account_header))
        menuBar.deselectActiveItem()
    }

    private fun loadWelcomePage() = loadPage(resources.getString(R.string.baseURL))

    override fun loadPage(url: String) {
        val urlWithMissingQueryStrings =
            knownServices.findKnownServiceAddMissingQueryFor(url)

        webview.loadUrl(urlWithMissingQueryStrings)
    }

    private fun loadSubPage(pageEndPoint: String, headerText: String?) {
        val builtUri = Uri.parse(resources.getString(R.string.baseURL))
            .buildUpon()
            .appendEncodedPath(pageEndPoint)
            .build()
        val fullUrl = builtUri.toString()
        loadPage(fullUrl)
        if (headerText != null) {
            setHeaderText(headerText)
        }
    }

    private fun reloadRequest() {
        if (reloadUrl != null) {
            webview.loadUrl(reloadUrl)
        } else {
            webview.reload()
        }
    }

    override fun setHeaderText(text: String) {
        runOnUiThread {
            header_text_view.text = text
            webview.announceForAccessibility(text)
        }
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
        if (menuBar.visibility == VISIBLE)
            menuBar.switchActiveMenuItemTo(R.id.symptoms)
    }

    override fun selectMoreMenuActive() {
        if (menuBar.visibility == VISIBLE)
            menuBar.switchActiveMenuItemTo(R.id.more)
    }

    override fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage) {
        showErrorScreen()
        errorTextView.setServiceError(unavailabilityErrorMessage.title,
            unavailabilityErrorMessage.message)
        if (unavailabilityErrorMessage.message != null) {
            tryAgainTextView.visibility = GONE
        } else {
            tryAgainTextView.visibility = VISIBLE
        }
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

    override fun clearMenuBarItem() {
        runOnUiThread {
            menuBar.deselectActiveItem()
        }
    }

    override fun hideMenuBar() {
        runOnUiThread {
            menuBar.visibility = GONE
        }
    }

    override fun hideHeader() {
        runOnUiThread {
            header.visibility = GONE
        }
    }

    fun showMenuBar() {
        runOnUiThread {
            menuBar.visibility = VISIBLE
        }
    }

    fun showHeader() {
        runOnUiThread {
            header.visibility = VISIBLE
        }
    }

    fun showBlankScreen() {
        viewSwitcher.visibility = View.GONE
        blankScreen.visibility = View.VISIBLE
    }

    fun hideBlankScreen() {
        viewSwitcher.visibility = View.VISIBLE
        blankScreen.visibility = View.GONE
    }
}
