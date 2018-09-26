package com.nhs.online.nhsonline.activities

import android.content.Intent
import android.content.pm.PackageManager
import android.os.Bundle
import android.preference.PreferenceManager
import android.support.v7.app.AlertDialog
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.util.Log
import android.view.View
import android.view.View.GONE
import android.view.View.VISIBLE
import android.view.WindowManager
import android.webkit.CookieManager
import android.webkit.WebSettings
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.R.id.menuBar
import com.nhs.online.nhsonline.R.id.webview
import com.nhs.online.nhsonline.browseractivities.ActivityInterface
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.navigation.MenuBarItem
import com.nhs.online.nhsonline.services.KnownService
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.services.UrlLoader
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
import java.util.*


class MainActivity : IInteractor, AppCompatActivity() {

    private lateinit var chromeClient: ChromeClientLocationHandler
    private lateinit var knownServices: KnownServices
    private lateinit var urlLoader: UrlLoader
    private lateinit var appWebInterface: AppWebInterface
    private var lifeCycleObserver: LifeCycleObserver? = null

    private var isLoggedIn = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        window.setFlags(WindowManager.LayoutParams.FLAG_SECURE, WindowManager.LayoutParams.FLAG_SECURE)
        CookieManager.getInstance().removeAllCookies(null)

        setContentView(R.layout.activity_main)
        setSupportActionBar(findViewById(R.id.header))
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        knownServices = KnownServices(this)
        appWebInterface = AppWebInterface(this)

        configureWebView()
        var wvClient = WebClientInterceptor(this, knownServices, createActivities(), this)
        webview.webViewClient = wvClient
        urlLoader = UrlLoader(webview, wvClient, appWebInterface, knownServices, resources.getString(R.string.baseURL))

        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        retryButton.setOnClickListener { urlLoader.reloadRequest() }
        nhsOnlineLogoIcon.setOnClickListener { onNhsOnlineLogoIconSelected() }
        myAccountIcon.setOnClickListener { onMyAccountIconSelected() }
        helpIcon.setOnClickListener { onHelpIconSelected() }

        val prefs = PreferenceManager.getDefaultSharedPreferences(baseContext)
        val isFirstTimeOpened = prefs.getBoolean(getString(R.string.isFirstTimeOpened), true)
        if (isFirstTimeOpened) {
            val edit = prefs.edit()
            edit.putBoolean(getString(R.string.isFirstTimeOpened), java.lang.Boolean.FALSE)
            edit.commit()
            urlLoader.loadUrl(getString(R.string.appIntroPath))
        } else {
            val urlPath = intent?.data?.path
            val authRedirectPath = resources.getString(R.string.authRedirectPath)
            if (urlPath == authRedirectPath) {
                loadPage(intent.data.toString())
            } else {
                loadWelcomePage()
            }
        }
    }

    override fun onStart() {
        super.onStart()

        if (lifeCycleObserver == null) {
            lifeCycleObserver = LifeCycleObserver(this,
                    appWebInterface, knownServices)
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
            if (data.scheme == getString(R.string.appScheme)) {
                val url = data.buildUpon()
                        .scheme(getString(R.string.baseScheme))
                        .toString()

                loadPage(url)
            } else {
                loadPage(data.toString())
            }
        }
    }

    override fun setReloadUrl(url: String?) {
        urlLoader.reloadUrl = url
    }

    fun getReloadUrl() : String? {
        return urlLoader.reloadUrl
    }

    private fun configureWebView() {
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true
        webview.settings.javaScriptCanOpenWindowsAutomatically = true

        chromeClient = ChromeClientLocationHandler(this)
        webview.webChromeClient = chromeClient

        webview.addJavascriptInterface(WebAppInterface(this), "nativeApp")

        webview.settings.cacheMode = WebSettings.LOAD_DEFAULT
    }

    fun createActivities(): List<ActivityInterface> {
        val openBrowserActivity =
                OpenUrlInBrowserActivity(resources.getStringArray(R.array.nativeAppHosts))
        return listOf(openBrowserActivity)
    }

    private fun onMenuSelected(menuBarItem: MenuBarItem) {
        var path:String

        when (menuBarItem.id) {
            R.id.symptoms -> {
                path = resources.getString(R.string.symptomsPath)
            }
            R.id.myRecord -> {
                path = resources.getString(R.string.myRecordPath)
            }
            R.id.more -> {
                path = resources.getString(R.string.morePath)
            }
            R.id.appointments -> {
                path = resources.getString(R.string.appointmentsPath)
            }
            R.id.prescriptions -> {
                path = resources.getString(R.string.prescriptionsPath)
            }
            else -> {
                path = resources.getString(R.string.baseURL)
            }
        }
        loadUrl(path)
    }

    override fun loadPage(url: String) {
        loadUrl(url)
    }

    private fun loadUrl(path:String) {
        var knownService: KnownService? = knownServices.findMatchingInternalService(path)
        if (knownService != null) {
            setHeaderText(knownService.nativeHeader!!)
        } else {
            knownService = knownServices.findMatchingKnownService(path)
            if (knownService != null) {
                setHeaderText(knownService.nativeHeader!!)
            }
        }
        urlLoader.loadUrl(path)
    }

    private fun onNhsOnlineLogoIconSelected() {
        loadWelcomePage()
        menuBar.deselectActiveItem()
    }

    private fun onMyAccountIconSelected() {
        loadUrl(resources.getString(R.string.myAccountPath))
        menuBar.deselectActiveItem()
    }

    private fun onHelpIconSelected() {
        val temp = OpenUrlInBrowserActivity(resources.getStringArray(R.array.nativeAppHosts))
        temp.start(this, resources.getString(R.string.helpURL))
    }

    private fun loadWelcomePage() = loadPage(resources.getString(R.string.baseURL))

    override fun setHeaderText(text: String, description: String?) {
        runOnUiThread {
            header_text_view.text = text
            if(!description.isNullOrEmpty()) {
                header_text_view.contentDescription = description
            }
            webview.announceForAccessibility(text)
            this.title = text
        }
    }

    override fun onBackPressed() {
        if (isLoggedIn) {
            showExitDialog()
        } else {
            this.finishAndRemoveTask()
        }
    }

    private fun showExitDialog() {
        val builder: AlertDialog.Builder = AlertDialog.Builder(this)

        builder.setMessage(resources.getString(R.string.logoutWarning))
                .setPositiveButton(resources.getString(R.string.logout)) { _, _ ->
                    urlLoader.loadPage(resources.getString(R.string.baseURL) + resources.getString(R.string.logoutPath))
                }
                .setNegativeButton(resources.getString(R.string.cancel)) { _, _ -> }

        var dialog: AlertDialog = builder.create()
        dialog.show()
    }

    override fun showProgressDialog() {
        if (progressBarLayout.visibility == View.GONE)
            progressBarLayout.visibility = View.VISIBLE
    }

    override fun dismissProgressDialog() {
        if (progressBarLayout.visibility == View.VISIBLE)
            progressBarLayout.visibility = View.GONE
    }

    override fun selectNavigationMenuActive(navigationMenuId: Int) {
        if (menuBar.visibility == VISIBLE)
            menuBar.switchActiveMenuItemTo(navigationMenuId)
    }

    override fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage) {
        showErrorScreen()
        errorTextView.setServiceError(unavailabilityErrorMessage.title,
                unavailabilityErrorMessage.message)
        errorTextView.contentDescription = unavailabilityErrorMessage.title + ". " + unavailabilityErrorMessage.accessibleMessage
        if (unavailabilityErrorMessage.message != null) {
            tryAgainTextView.visibility = GONE

        } else {
            tryAgainTextView.visibility = VISIBLE
        }
    }

    private fun showErrorScreen() {
        errorViewLayout.visibility = View.VISIBLE
        webview.visibility = View.GONE

        urlLoader.usingAbsoluteUri = true
    }

    override fun showWebviewScreen() {
        errorViewLayout.visibility = View.GONE
        webview.visibility = View.VISIBLE

        if (isLoggedIn) {
            urlLoader.usingAbsoluteUri = false
        }
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

    override fun goToCheckSymptoms() {
        val intent = Intent(this, SymptomsActivity::class.java)
        startActivity(intent)
    }

    private fun hideMenuBar() {
        runOnUiThread {
            menuBar.visibility = GONE
        }
    }

    private fun hideHeader() {
        runOnUiThread {
            header.visibility = GONE
        }
    }

    fun loggedIn() {
        if (isLoggedIn) return

        showMenuBar()
        showHeader()
        urlLoader.usingAbsoluteUri = false
        isLoggedIn = true
    }

    fun loggedOut() {
        hideHeader()
        hideMenuBar()
        urlLoader.usingAbsoluteUri = true
        isLoggedIn = false
    }


    private fun showMenuBar() {
        runOnUiThread {
            menuBar.visibility = VISIBLE
        }
    }

    private fun showHeader() {
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

    fun evaluateWebviewJavascript(javascriptText: String) {
        webview.evaluateJavascript(javascriptText, null)
    }
}