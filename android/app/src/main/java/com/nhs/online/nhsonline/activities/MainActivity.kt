package com.nhs.online.nhsonline.activities

import android.content.Context
import android.annotation.SuppressLint
import android.app.Activity
import android.content.Intent
import android.content.SharedPreferences
import android.content.pm.PackageManager
import android.os.Bundle
import android.support.v7.app.AlertDialog
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.view.View
import android.view.View.GONE
import android.view.View.VISIBLE
import android.view.WindowManager
import android.webkit.CookieManager
import android.webkit.WebSettings
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.ActivityInterface
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.navigation.MenuBarItem
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
import android.location.LocationManager
import android.preference.PreferenceManager
import android.view.accessibility.AccessibilityEvent
import android.util.Log
import com.nhs.online.nhsonline.Application
import android.view.accessibility.AccessibilityManager
import com.android.volley.RequestQueue
import com.android.volley.toolbox.Volley
import android.widget.Button
import android.widget.TextView
import com.nhs.online.nhsonline.BuildConfig
import com.nhs.online.nhsonline.services.KnownServices
import java.net.URL
import com.nhs.online.nhsonline.interfaces.IVolleyCallback
import com.nhs.online.nhsonline.network.Reachability
import com.nhs.online.nhsonline.services.ConfigurationResponse
import com.nhs.online.nhsonline.services.ConfigurationService
import java.util.*


class MainActivity : IInteractor, AppCompatActivity() {

    private lateinit var mRequestQueue: RequestQueue
    private lateinit var configurationService: ConfigurationService
    private lateinit var chromeClient: ChromeClientLocationHandler
    private lateinit var knownServices: KnownServices
    private lateinit var urlLoader: UrlLoader
    private lateinit var appWebInterface: AppWebInterface
    private lateinit var upgradeDialog: AlertDialog
    private var lifeCycleObserver: LifeCycleObserver? = null
    private var isLoggedIn = false
    private var extendSessionDialogue: AlertDialog? = null

    var isSuccessfulConfigCheck = false
    var originalWebviewZoom = 0

    override fun onCreate(savedInstanceState: Bundle?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering OnCreate")

        super.onCreate(savedInstanceState)

        if (resources.getString(R.string.secureFlag) != "disabled") {
            window.setFlags(WindowManager.LayoutParams.FLAG_SECURE,
                    WindowManager.LayoutParams.FLAG_SECURE)
        }

        CookieManager.getInstance().removeAllCookies(null)

        val prefs = PreferenceManager.getDefaultSharedPreferences(baseContext)
        val persistedBetaCookie = prefs.getString("BetaCookie", null)
        if (!persistedBetaCookie.isNullOrBlank()) {
            CookieManager.getInstance().setCookie(getString(R.string.cookieDomain), "$persistedBetaCookie; max-age=${60 * 60 * 24 * 365}")
        }

        setContentView(R.layout.activity_main)
        setSupportActionBar(findViewById(R.id.header))
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)
        mRequestQueue = Volley.newRequestQueue(this)

        knownServices = KnownServices(this)
        appWebInterface = AppWebInterface(this)
        configurationService = ConfigurationService(this)

        dismissProgressDialog()

        configureWebView()
        var wvClient = WebClientInterceptor(this, knownServices, createActivities(), this)
        webview.webViewClient = wvClient
        urlLoader = UrlLoader(webview,
                wvClient,
                appWebInterface,
                knownServices,
                resources.getString(R.string.baseURL))

        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        retryButton.setOnClickListener { onErrorRetryButton() }
        nhsOnlineLogoIcon.setOnClickListener { onNhsOnlineLogoIconSelected() }
        myAccountIcon.setOnClickListener { onMyAccountIconSelected() }
        helpIcon.setOnClickListener { onHelpIconSelected() }

        if (isSuccessfulConfigCheck) {
            loadAuthReturnOrWelcomePage()
        }
    }

    fun loadAuthReturnOrWelcomePage() {
        val urlPath = intent?.data?.path
        val authRedirectPath = resources.getString(R.string.authRedirectPath)
        if (urlPath == authRedirectPath) {
            loadPage(intent.data.toString())
        } else {
            loadWelcomePage()
        }
    }

    override fun loadThrottlingCarousel() {
        val prefs = PreferenceManager.getDefaultSharedPreferences(baseContext)
        val haveShownThrottlingCarouselBefore = prefs.getBoolean(getString(R.string.haveShownThrottlingCarouselBefore), false)

        if (!haveShownThrottlingCarouselBefore) {
            runOnUiThread {
                originalWebviewZoom = webview.settings.textZoom
                webview.settings.textZoom = 100
                webview.loadUrl(getString(R.string.throttleCarouselPath))
            }
        } else {
            loadAuthReturnOrWelcomePage()
        }
    }

    private fun setupAppVersion() {
        evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('appVersion/updateNativeVersion', '${BuildConfig.VERSION_NAME}')")
        evaluateWebviewJavascript("window.\$nuxt.\$store.dispatch('appVersion/updatePlatform', 'Android')")
    }

    private fun onErrorRetryButton() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering OnErrorRetryButton")

        if (!Reachability.isConnectedToNetwork(this)) {
            Log.d(Application.TAG, "${this::class.java.simpleName}: Leaving OnErrorRetryButton as presently no network access")
            return
        }

        showProgressDialog()
        urlLoader.reloadRequest()
        configurationService.getConfiguration(object : IVolleyCallback {
            override fun onSuccess(configurationResponse: ConfigurationResponse) {
                isSuccessfulConfigCheck = true
                if (!configurationResponse.isValidConfiguration) {
                    showVersionUpgradeDialog()
                }

                if(!isLoggedIn && configurationResponse.isThrottlingEnabled) {
                    loadThrottlingCarousel()
                }
            }

            override fun onError(errorMessage: ErrorMessage) {
                isSuccessfulConfigCheck = false
                showUnavailabilityError(errorMessage)
            }
        })
    }

    override fun onStart() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering OnStart")
        super.onStart()

        if (lifeCycleObserver == null) {
            lifeCycleObserver = LifeCycleObserver(this,
                    appWebInterface, knownServices, configurationService)
        }

        lifeCycleObserver?.onMoveToForeground()
    }

    override fun onStop() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering OnStop")
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
                showBlankScreen()
                loadPage(url)
            } else {
                loadPage(data.toString())
            }
        }
    }

    override fun setReloadUrl(url: String?) {
        if (!knownServices.shouldURLOpenExternally(URL(url))) {
            urlLoader.reloadUrl = url
        }
    }

    fun getReloadUrl(): String? {
        return urlLoader.reloadUrl
    }

    @SuppressLint("SetJavaScriptEnabled")
    private fun configureWebView() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering configureWebView")
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
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMenuSelected")
        var path: String

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

    private fun loadUrl(path: String) {
        var knownService = knownServices.findNHSAppInternalServiceInfoByPath(path)
        if (knownService == null) {
            knownService = knownServices.findMatchingServiceInfo(path)
        }
        knownService?.header?.let { nativeHeader ->
            setHeaderText(nativeHeader)
        }
        hideBiometrics()
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
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering setHeaderText")
        runOnUiThread {
            header_text_view.text = text
            header_text_view.contentDescription = description
            webview.announceForAccessibility(description ?: text)
        }
    }

    override fun onBackPressed() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onBackPressed")

        val  path = URL(webview.url).path

        if (isLoggedIn) {
            showExitDialog()

        } else if (path.equals("/" + resources.getString(R.string.gpFinderPath))) {
            this.finishAndRemoveTask()

        } else if (path.contains(resources.getString(R.string.gpFinderPath), ignoreCase = true)) {
            appWebInterface.resetGPFinderFlow()

        } else {
            this.finishAndRemoveTask()
        }
    }

    override fun onResume() {
        super.onResume()

        if (isSuccessfulConfigCheck) {
            val loginUrl = resources.getString(R.string.baseURL) + resources.getString(R.string.loginPath)
            if (webview.url == null) {
                webview.loadUrl(loginUrl)
            } else if (webview.url.toLowerCase().startsWith(loginUrl.toLowerCase())) {
                webview.reload()
            }
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

    fun showVersionUpgradeDialog() {
        if ((::upgradeDialog.isInitialized && !upgradeDialog.isShowing) || !::upgradeDialog.isInitialized) {

            val builder: AlertDialog.Builder = AlertDialog.Builder(this)
                    .setTitle(resources.getString(R.string.UpdateRequiredHeader))
                    .setMessage(resources.getString(R.string.UpdateHeader) + "\n" + resources.getString(R.string.UpdateDesc))
                    .setNegativeButton(resources.getString(R.string.Close)) { _, _ ->
                        this.finishAndRemoveTask()
                    }
            builder.setCancelable(false)
            upgradeDialog = builder.create()
            upgradeDialog.setCanceledOnTouchOutside(false)
            upgradeDialog.setCancelable(false)
            upgradeDialog.show()
        }
    }

    fun hideVersionUpgradeDialog() {
        if (::upgradeDialog.isInitialized && upgradeDialog.isShowing) {
            upgradeDialog.dismiss()
        }
    }

    override fun showExtendSessionDialogue(sessionDuration: Int) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showExtendSessionDialogue")

        extendSessionDialogue = extendSessionDialogue ?: initialiseExtendSessionDialogue(sessionDuration)
        extendSessionDialogue?.show()
    }

    private fun initialiseExtendSessionDialogue(sessionDuration: Int): AlertDialog {
        val builder: AlertDialog.Builder = AlertDialog.Builder(this)

        val inflater = layoutInflater
        val dialogView = inflater.inflate(R.layout.session_expiry_warning_dialogue, null)
        builder.setView(dialogView)
        builder.setCancelable(false)
        var textView = dialogView.findViewById(R.id.sessionExpiryWarningDurationInformation) as TextView
        textView.text = resources.getString(R.string.sessionExpiryWarningDurationInformation).format(sessionDuration)
        val extendSession = dialogView.findViewById(R.id.extendSession) as Button
        val logOut = dialogView.findViewById(R.id.logOut) as Button
        var dialog: AlertDialog = builder.create()
        extendSession.setOnClickListener { dialog.dismiss(); appWebInterface.extendSession() }
        logOut.setOnClickListener { dialog.dismiss(); appWebInterface.logout() }
        dialog.setCanceledOnTouchOutside(false)

        return dialog
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

    public override fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage) {
        if (::upgradeDialog.isInitialized) {
            upgradeDialog.dismiss()
        }
        showErrorScreen()
        errorTextView.setServiceError(unavailabilityErrorMessage.title,
                unavailabilityErrorMessage.message)
        errorTextView.contentDescription = unavailabilityErrorMessage.title + ". " +
                unavailabilityErrorMessage.accessibleMessage
        if (unavailabilityErrorMessage.message != null) {
            tryAgainTextView.visibility = GONE

        } else {
            tryAgainTextView.visibility = VISIBLE
        }
    }

    private fun showErrorScreen() {
        hideBlankScreen()
        errorViewLayout.visibility = View.VISIBLE
        webview.visibility = View.GONE

        urlLoader.usingAbsoluteUri = true
    }

    override fun showWebviewScreen() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showWebViewScreen")
        if (isSuccessfulConfigCheck) {
            errorViewLayout.visibility = View.GONE
            webview.visibility = View.VISIBLE
            hideBlankScreen()

            if (isLoggedIn) {
                urlLoader.usingAbsoluteUri = false
            }
        }
    }

    override fun onRequestPermissionsResult(
            requestCode: Int,
            permissions: Array<out String>,
            grantResults: IntArray
    ) {
        val lm = this.getSystemService(Context.LOCATION_SERVICE) as LocationManager
        var gpsEnabled = lm.isProviderEnabled(LocationManager.GPS_PROVIDER)

        if (requestCode == LOCATION_REQUEST_CODE) {
            if (grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                if (gpsEnabled) {
                    chromeClient.onLocationPermissionResponded(true)
                } else {
                    val gpsOptionsIntent =
                            Intent(android.provider.Settings.ACTION_LOCATION_SOURCE_SETTINGS)
                    startActivity(gpsOptionsIntent)
                    chromeClient.onLocationPermissionResponded(true)
                }
            } else {
                chromeClient.onLocationPermissionResponded(false)
            }
        }
    }

    override fun clearMenuBarItem() {
        menuBar.deselectActiveItem()
    }

    override fun goToCheckSymptoms() {
        val intent = Intent(this, SymptomsActivity::class.java)
        startActivity(intent)
    }

    fun goToNativeBiometricPage() {
        showBiometrics();
        setHeaderText(resources.getString(R.string.biometric_header))
    }

    fun showBiometrics() {
        biometricLayoutContent.visibility = View.VISIBLE
        webview.visibility = View.GONE
    }

    fun hideBiometrics() {
        biometricLayoutContent.visibility = View.GONE
        webview.visibility = View.VISIBLE;
    }

    override fun announcePageTitle(title: String?) {
        title?.let { webview.announceForAccessibility(it) }
    }

    override fun hideMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideMenuBar")
        runOnUiThread {
            menuBar.visibility = GONE
        }
    }

    override fun hideHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideHeader")
        runOnUiThread {
            header.visibility = GONE
            setupAppVersion()
        }
    }

    fun loggedIn() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering loggedIn")
        if (isLoggedIn) return

        showMenuBar()
        showHeader()
        urlLoader.usingAbsoluteUri = false
        isLoggedIn = true
    }

    fun loggedOut() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering loggedOut")
        urlLoader.usingAbsoluteUri = true
        isLoggedIn = false

        if (extendSessionDialogue?.isShowing == true) {
            extendSessionDialogue?.dismiss()
        }
    }


    override fun showMenuBar() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showMenuBar")
        runOnUiThread {
            menuBar.visibility = VISIBLE
        }
    }

    override fun showHeader() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showHeader")
        runOnUiThread {
            header.visibility = VISIBLE
            setupAppVersion()
        }
    }

    fun showBlankScreen() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering showBlankScreen")
        runOnUiThread {
            viewSwitcher.visibility = View.GONE
            blankScreen.visibility = View.VISIBLE
        }
    }

    fun resetFocusToNhsLogoForA11y() {
        val a11yMng = getSystemService(Context.ACCESSIBILITY_SERVICE) as AccessibilityManager
        if (header.visibility == View.VISIBLE && a11yMng.isTouchExplorationEnabled && a11yMng.isEnabled) {
            nhsOnlineLogoIcon.sendAccessibilityEvent(AccessibilityEvent.TYPE_VIEW_FOCUSED)
        }
    }

    fun hideBlankScreen() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering hideBlankScreen")
        runOnUiThread {
            viewSwitcher.visibility = View.VISIBLE
            blankScreen.visibility = View.GONE
        }
    }

    fun evaluateWebviewJavascript(javascriptText: String) {
        webview.evaluateJavascript(javascriptText, null)
    }

    fun getRequestQueue(): RequestQueue {
        return mRequestQueue
    }
}