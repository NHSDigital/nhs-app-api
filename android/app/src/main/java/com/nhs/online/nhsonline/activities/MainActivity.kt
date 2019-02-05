package com.nhs.online.nhsonline.activities

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.location.LocationManager
import android.os.Bundle
import android.preference.PreferenceManager
import android.support.v4.app.FragmentActivity
import android.support.v7.app.AlertDialog
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.text.method.LinkMovementMethod
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.view.View.GONE
import android.view.View.VISIBLE
import android.view.WindowManager
import android.view.accessibility.AccessibilityEvent
import android.view.accessibility.AccessibilityManager
import android.webkit.CookieManager
import android.webkit.WebSettings
import android.widget.Button
import android.widget.TextView
import com.android.volley.RequestQueue
import com.android.volley.toolbox.Volley
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.FingerprintService
import com.nhs.online.nhsonline.biometrics.IBiometricsInteractor
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSystemChecker
import com.nhs.online.nhsonline.browseractivities.ActivityInterface
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.interfaces.IVolleyCallback
import com.nhs.online.nhsonline.navigation.MenuBarItem
import com.nhs.online.nhsonline.network.Reachability
import com.nhs.online.nhsonline.services.ConfigurationResponse
import com.nhs.online.nhsonline.services.ConfigurationService
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.services.UrlLoader
import com.nhs.online.nhsonline.support.ActivityView
import com.nhs.online.nhsonline.support.LifeCycleObserver
import com.nhs.online.nhsonline.support.MainActivityViewSwitcher
import com.nhs.online.nhsonline.support.setServiceError
import com.nhs.online.nhsonline.utils.Html
import com.nhs.online.nhsonline.webclients.ChromeClientLocationHandler
import com.nhs.online.nhsonline.webclients.LOCATION_REQUEST_CODE
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.webinterfaces.WebAppInterface
import com.scottyab.rootbeer.RootBeer
import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.biometric_layout_content.*
import kotlinx.android.synthetic.main.error_layout.*
import kotlinx.android.synthetic.main.header_layout.*
import kotlinx.android.synthetic.main.success_layout.*
import java.net.URL
import java.util.logging.Logger

private val TAG = MainActivity::class.java.simpleName

class MainActivity : IInteractor, AppCompatActivity(), IBiometricsInteractor {

    private lateinit var mRequestQueue: RequestQueue
    private lateinit var configurationService: ConfigurationService
    private lateinit var chromeClient: ChromeClientLocationHandler
    private lateinit var knownServices: KnownServices
    private val logger = Logger.getLogger(TAG)

    private var fingerprintService: FingerprintService? = null
    private lateinit var urlLoader: UrlLoader
    private lateinit var appWebInterface: AppWebInterface
    private lateinit var upgradeDialog: AlertDialog
    private lateinit var rootedDeviceDialog: AlertDialog
    private var lifeCycleObserver: LifeCycleObserver? = null
    private lateinit var activityViewSwitcher: MainActivityViewSwitcher
    private var isLoggedIn = false
    private var extendSessionDialogue: AlertDialog? = null

    var isSuccessfulConfigCheck = false
    var originalWebviewZoom = 0

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        if (resources.getString(R.string.secureFlag) != "disabled") {
            window.setFlags(WindowManager.LayoutParams.FLAG_SECURE,
                WindowManager.LayoutParams.FLAG_SECURE)
        }

        CookieManager.getInstance().removeAllCookies(null)

        val prefs = PreferenceManager.getDefaultSharedPreferences(this)
        val persistedBetaCookie = prefs.getString("BetaCookie", null)
        if (!persistedBetaCookie.isNullOrBlank()) {
            CookieManager.getInstance().setCookie(getString(R.string.cookieDomain),
                "$persistedBetaCookie; max-age=${60 * 60 * 24 * 365}")
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
        val wvClient = WebClientInterceptor(this, knownServices, createActivities(), this)
        webview.webViewClient = wvClient
        urlLoader = UrlLoader(webview,
            wvClient,
            appWebInterface,
            knownServices,
            resources.getString(R.string.baseURL))

        activityViewSwitcher = MainActivityViewSwitcher(this)

        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        backToAccountButton.setOnClickListener { onSuccessButton() }
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
            toggleBiometricSwitch(false)
            loadPage(intent.data.toString())
        } else {
            loadWelcomePage()
        }
    }

    override fun loadThrottlingCarousel() {
        val prefs = PreferenceManager.getDefaultSharedPreferences(baseContext)
        val haveShownThrottlingCarouselBefore =
            prefs.getBoolean(getString(R.string.haveShownThrottlingCarouselBefore), false)

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

    private fun onErrorRetryButton() {
        logger.info("${this::class.java.simpleName}: Entering OnErrorRetryButton")

        if (!Reachability.isConnectedToNetwork(this)) {
            logger.info(
                "${this::class.java.simpleName}: Leaving OnErrorRetryButton as presently no network access")
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

                if (!isLoggedIn) {
                    if (configurationResponse.isThrottlingEnabled) {
                        loadThrottlingCarousel()
                    } else {
                        loadAuthReturnOrWelcomePage()
                    }
                }

                if (fingerprintService == null) {
                    configBiometricSetup(configurationResponse.fidoServerUrl)
                }
            }

            override fun onError(errorMessage: ErrorMessage) {
                isSuccessfulConfigCheck = false
                showUnavailabilityError(errorMessage)
            }
        })
    }

    override fun onStart() {
        logger.info("Entering OnStart")
        super.onStart()

        // reloadIfAppWasInBackground()
        if (lifeCycleObserver == null) {
            lifeCycleObserver = LifeCycleObserver(this,
                appWebInterface, knownServices, configurationService, RootBeer(this))
        }

        lifeCycleObserver?.onMoveToForeground()
    }

    override fun onStop() {
        logger.info(" Entering OnStop")
        super.onStop()

        lifeCycleObserver?.onMoveToBackground()
    }

    override fun onDestroy() {
        super.onDestroy()
        fingerprintService?.cancelAllProgressingTasks()
    }

    override fun onNewIntent(intent: Intent?) {
        super.onNewIntent(intent)
        handleNewIntent(intent)
    }

    private fun handleNewIntent(intent: Intent?) {
        intent?.data?.let { uri ->
            val hasFidoLoginError = uri.path.contains(getString(R.string.authRedirectPath)) &&
                    fingerprintService?.biometricState?.registered ?: false &&
                    uri.queryParameterNames.contains(getString(R.string.redirectErrorQueryParam))
            if (hasFidoLoginError) {
                Log.d(TAG, "Fido login error response url: $uri")
                fingerprintService?.notifyLoginErrorOccurrence()
                loadWelcomePage()
                return
            }

            val hasAppScheme = uri.scheme == getString(R.string.appScheme)
            val url = if (hasAppScheme) uri.buildUpon()
                .scheme(getString(R.string.baseScheme)).toString()
            else uri.toString()

            if (hasAppScheme)
                showBlankScreen()

            loadPage(url)
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

    fun configBiometricSetup(fidoServerUrl: String) {
        if (fingerprintService != null) return

        fingerprintService = FingerprintService.createIfDeviceSupported(this, fidoServerUrl)
        biometricToggleSwitch.setOnTouchListener { _, event ->
            onBiometricSwitchTouch(event)
        }
    }

    private fun onBiometricSwitchTouch(event: MotionEvent): Boolean {
        if (event.action == MotionEvent.ACTION_UP) {
            if (fingerprintService == null) {
                if (FingerprintSystemChecker.checkIfAndroidMOrAbove()) {
                    showBiometricRegistrationError()
                } else {
                    FingerprintSystemChecker.showCurrentOSNotSupportDialog(this)
                }

                return false
            }

            fingerprintService?.let {
                if (it.biometricState.registrationStateChangeInProgress) {
                    biometricToggleSwitch.isChecked = it.biometricState.registered
                    return false
                }

                if (it.biometricState.registered) {
                    it.deRegisterBiometrics()
                } else {
                    it.startFidoRegistration()
                }
            }
        }

        return true
    }

    @SuppressLint("SetJavaScriptEnabled")
    private fun configureWebView() {
        logger.info("Entering configureWebView")
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true
        webview.settings.javaScriptCanOpenWindowsAutomatically = true

        chromeClient = ChromeClientLocationHandler(this)
        webview.webChromeClient = chromeClient

        webview.addJavascriptInterface(WebAppInterface(this), "nativeApp")
        webview.settings.cacheMode = WebSettings.LOAD_DEFAULT
    }

    private fun createActivities(): List<ActivityInterface> {
        val openBrowserActivity =
            OpenUrlInBrowserActivity(resources.getStringArray(R.array.nativeAppHosts))
        return listOf(openBrowserActivity)
    }

    private fun onMenuSelected(menuBarItem: MenuBarItem) {
        logger.info("Entering onMenuSelected")
        val path: String

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

    override fun loadBiometricLoginPage(url: String) {
        urlLoader.usingAbsoluteUri = true
        loadUrl(url)
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
        activityViewSwitcher.switchTo(ActivityView.WEBVIEW)
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
        logger.info("Entering setHeaderText")
        runOnUiThread {
            header_text_view.text = text
            header_text_view.contentDescription = description
            webview.announceForAccessibility(description ?: text)
        }
    }

    override fun onBackPressed() {
        logger.info("${this::class.java.simpleName}: Entering onBackPressed")

        val path = URL(webview.url).path

        when {
            isLoggedIn -> showExitDialog()
            path == "/" + resources.getString(R.string.gpFinderPath) -> this.finishAndRemoveTask()
            path.contains(resources.getString(R.string.gpFinderPath),
                ignoreCase = true) -> appWebInterface.resetGPFinderFlow()
            else -> this.finishAndRemoveTask()
        }
    }

    override fun onResume() {
        super.onResume()

        if (isSuccessfulConfigCheck) {
            val loginUrl =
                resources.getString(R.string.baseURL) + resources.getString(R.string.loginPath)
            val currentUrl = webview.url ?: ""

            val isFidoLoginUrl = currentUrl.contains(loginUrl) &&
                    currentUrl.contains(getString(R.string.fidoAuthQueryKey))

            if (currentUrl.isEmpty() || isFidoLoginUrl) {
                loadPage(loginUrl)
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

        val dialog: AlertDialog = builder.create()
        dialog.show()
    }

    fun showVersionUpgradeDialog() {
        if ((::upgradeDialog.isInitialized && !upgradeDialog.isShowing) || !::upgradeDialog.isInitialized) {

            val content = resources.getString(R.string.UpdateHeader) +
                    "<br/><br/>" +
                    resources.getString(R.string.UpdateNativeLink) +
                    "<br/><br/>" +
                    resources.getString(R.string.UpdateDesc)

            val builder: AlertDialog.Builder = AlertDialog.Builder(this)
                .setTitle(resources.getString(R.string.UpdateRequiredHeader))
                .setMessage(Html.fromHtml(content))
                .setNegativeButton(resources.getString(R.string.Close)) { _, _ ->
                    this.finishAndRemoveTask()
                }

            builder.setCancelable(false)
            upgradeDialog = builder.create()
            upgradeDialog.setCanceledOnTouchOutside(false)
            upgradeDialog.setCancelable(false)
            upgradeDialog.show()

            (upgradeDialog.findViewById<TextView>(android.R.id.message) as TextView).movementMethod =
                    LinkMovementMethod.getInstance()
        }
    }

    fun hideVersionUpgradeDialog() {
        if (::upgradeDialog.isInitialized && upgradeDialog.isShowing) {
            upgradeDialog.dismiss()
        }
    }


    fun showRootedDeviceDialog() {
        if ((::rootedDeviceDialog.isInitialized && !rootedDeviceDialog.isShowing) || !::rootedDeviceDialog.isInitialized) {

            val tc1 = resources.getString(R.string.rootedDeviceDialogDescriptionLine1)
            val tc2 = resources.getString(R.string.rootedDeviceDialogDescriptionLine2)
            val content = "$tc1 <br/><br/> $tc2"

            val builder: AlertDialog.Builder = AlertDialog.Builder(this)
                .setTitle(resources.getString(R.string.rootedDeviceDialogHeader))
                .setMessage(Html.fromHtml(content))
                .setNegativeButton(resources.getString(R.string.rootedDeviceDialogClose)) { _, _ ->
                    this.finishAndRemoveTask()
                }
            builder.setCancelable(false)
            rootedDeviceDialog = builder.create()
            rootedDeviceDialog.setCanceledOnTouchOutside(false)
            rootedDeviceDialog.setCancelable(false)
            rootedDeviceDialog.show()

            (rootedDeviceDialog.findViewById<TextView>(android.R.id.message) as TextView).movementMethod =
                    LinkMovementMethod.getInstance()
        }
    }

    override fun showExtendSessionDialogue(sessionDuration: Int) {
        logger.info("Entering showExtendSessionDialogue")

        extendSessionDialogue = extendSessionDialogue ?:
                initialiseExtendSessionDialogue(sessionDuration)
        extendSessionDialogue?.show()
    }

    @SuppressLint("InflateParams")
    private fun initialiseExtendSessionDialogue(sessionDuration: Int): AlertDialog {
        val builder: AlertDialog.Builder = AlertDialog.Builder(this)

        val inflater = layoutInflater
        val dialogView = inflater.inflate(R.layout.session_expiry_warning_dialogue, null)
        builder.setView(dialogView)
        builder.setCancelable(false)
        var textView =
            dialogView.findViewById(R.id.sessionExpiryWarningDurationInformation) as TextView
        val sessionExpiryMessage =
            resources.getString(R.string.sessionExpiryWarningDurationInformation)
                .format(sessionDuration)
        textView.text = sessionExpiryMessage
        textView.contentDescription = sessionExpiryMessage
        val extendSession = dialogView.findViewById(R.id.extendSession) as Button
        val logOut = dialogView.findViewById(R.id.logOut) as Button
        val dialog: AlertDialog = builder.create()
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

    override fun getActivity(): FragmentActivity = this

    override fun toggleBiometricSwitch(isChecked: Boolean) {
        biometricToggleSwitch.isChecked = isChecked
    }

    override fun showBiometricsOnRegistrationSuccessMessage() {
        successTextView.text =
                resources.getString(R.string.fingerprint_registration_success_dialog_message)
        activityViewSwitcher.switchTo(ActivityView.FINGERPRINT_SUCCESS)
    }

    override fun showBiometricsOnDeRegistrationSuccessMessage() {
        successTextView.text =
                resources.getString(R.string.fingerprint_de_registration_success_dialog_message)
        activityViewSwitcher.switchTo(ActivityView.FINGERPRINT_SUCCESS)
    }

    override fun showBiometricRegistrationError() {
        errorTextView.setServiceError(getString(R.string.errorIconText),
            getString(R.string.biometric_registration_failure_message))
        activityViewSwitcher.switchTo(ActivityView.ERROR)
    }

    override fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage) {
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
        activityViewSwitcher.switchTo(ActivityView.ERROR)

        urlLoader.usingAbsoluteUri = true
    }

    override fun showWebviewScreen() {
        logger.info("Entering showWebViewScreen")

        if (isSuccessfulConfigCheck) {
            activityViewSwitcher.switchTo(ActivityView.WEBVIEW)
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
        val gpsEnabled = lm.isProviderEnabled(LocationManager.GPS_PROVIDER)

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

    override fun setMenuBarItem(index: Int) {
        runOnUiThread {
            when (index) {
                0 -> menuBar.switchActiveMenuItemTo(R.id.symptoms)
                1 -> menuBar.switchActiveMenuItemTo(R.id.appointments)
                2 -> menuBar.switchActiveMenuItemTo(R.id.prescriptions)
                3 -> menuBar.switchActiveMenuItemTo(R.id.myRecord)
                4 -> menuBar.switchActiveMenuItemTo(R.id.more)
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
        activityViewSwitcher.switchTo(ActivityView.FINGERPRINT)
        setHeaderText(resources.getString(R.string.biometric_header))
    }

    override fun announcePageTitle(title: String?) {
        title?.let { webview.announceForAccessibility(it) }
    }

    override fun hideMenuBar() {
        logger.info("Entering hideMenuBar")
        runOnUiThread {
            menuBar.visibility = GONE
        }
    }

    override fun hideHeader() {
        logger.info("Entering hideHeader")
        runOnUiThread {
            header.visibility = GONE
        }
    }

    fun loggedIn() {
        logger.info("Entering loggedIn")
        if (isLoggedIn) return

        showMenuBar()
        showHeader()
        urlLoader.usingAbsoluteUri = false
        isLoggedIn = true
    }

    fun loggedOut() {
        logger.info("Entering loggedOut")
        urlLoader.usingAbsoluteUri = true
        isLoggedIn = false

        if (extendSessionDialogue?.isShowing == true) {
            extendSessionDialogue?.dismiss()
        }
        showBiometricLoginIfEnabled()
    }


    override fun showMenuBar() {
        logger.info("Entering showMenuBar")
        runOnUiThread {
            menuBar.visibility = VISIBLE
        }
    }


    override fun showHeader() {
        logger.info("Entering showHeader")
        runOnUiThread {
            header.visibility = VISIBLE
        }
    }

    fun showBlankScreen() {
        logger.info("Entering showBlankScreen")
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
        logger.info("Entering hideBlankScreen")
        dismissProgressDialog()
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

    override fun showBiometricLoginIfEnabled(): Boolean =
        fingerprintService?.showBiometricLoginIfEnabled() ?: false

    private fun onSuccessButton() {
        activityViewSwitcher.switchTo(ActivityView.WEBVIEW)
        urlLoader.reloadRequest()
    }
}