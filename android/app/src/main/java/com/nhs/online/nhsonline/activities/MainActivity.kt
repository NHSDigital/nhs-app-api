package com.nhs.online.nhsonline.activities

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.support.v4.app.FragmentActivity
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.view.View.GONE
import android.view.View.VISIBLE
import android.view.WindowManager
import android.view.accessibility.AccessibilityEvent
import android.view.accessibility.AccessibilityManager
import android.webkit.WebSettings
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.BiometricsInterface
import com.nhs.online.nhsonline.biometrics.IBiometricsInteractor
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.navigation.MenuBarItem
import com.nhs.online.nhsonline.network.Reachability
import com.nhs.online.nhsonline.support.*
import com.nhs.online.nhsonline.web.NhsWeb
import com.nhs.online.nhsonline.webclients.LOCATION_REQUEST_CODE
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
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
    private val logger = Logger.getLogger(TAG)
    private val biometricsInterface = BiometricsInterface(this)

    private lateinit var nhsWeb: NhsWeb
    private lateinit var appDialogs: AppDialogs
    private lateinit var appWebInterface: AppWebInterface
    private var lifeCycleObserver: LifeCycleObserver? = null
    private lateinit var activityViewSwitcher: MainActivityViewSwitcher

    var isSuccessfulConfigCheck = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        if (resources.getString(R.string.secureFlag) != "disabled") {
            window.setFlags(WindowManager.LayoutParams.FLAG_SECURE,
                WindowManager.LayoutParams.FLAG_SECURE)
        }

        setContentView(R.layout.activity_main)
        setSupportActionBar(findViewById(R.id.header))
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        activityViewSwitcher = MainActivityViewSwitcher(this)
        appDialogs = AppDialogs(this)

        configureWebView()
        nhsWeb = NhsWeb(this, this, webview)
        appWebInterface = AppWebInterface(webview)

        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        backToAccountButton.setOnClickListener { onSuccessButton() }
        retryButton.setOnClickListener { onErrorRetryButton() }
        nhsOnlineLogoIcon.setOnClickListener { onNhsOnlineLogoIconSelected() }
        myAccountIcon.setOnClickListener { onMyAccountIconSelected() }
        helpIcon.setOnClickListener { onHelpIconSelected() }
    }

    @SuppressLint("SetJavaScriptEnabled")
    private fun configureWebView() {
        logger.info("Entering configureWebView")
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true
        webview.settings.javaScriptCanOpenWindowsAutomatically = true

        webview.settings.cacheMode = WebSettings.LOAD_DEFAULT
    }

    private fun onErrorRetryButton() {
        logger.info("${this::class.java.simpleName}: Entering OnErrorRetryButton")

        if (!Reachability.isConnectedToNetwork(this)) {
            logger.info(
                "${this::class.java.simpleName}: Leaving OnErrorRetryButton as presently no network access")
            return
        }

        if (isSuccessfulConfigCheck) {
            showProgressDialog()
            nhsWeb.reloadCurrentUrl()
            return
        }
        lifeCycleObserver?.checkAndHandleConfiguration()
    }

    override fun onStart() {
        logger.info("Entering OnStart")
        super.onStart()

        if (lifeCycleObserver == null) {
            lifeCycleObserver = LifeCycleObserver(this,
                appWebInterface, nhsWeb, RootBeer(this), appDialogs)
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
        biometricsInterface.cancelAllProgressingTasks()
    }

    override fun onNewIntent(intent: Intent?) {
        super.onNewIntent(intent)
        handleNewIntent(intent)
    }

    private fun handleNewIntent(intent: Intent?) {
        intent?.data?.let { uri ->
            val hasFidoLoginError = uri.path.contains(getString(R.string.authRedirectPath)) &&
                    biometricsInterface.isFingerprintRegistered &&
                    uri.queryParameterNames.contains(getString(R.string.redirectErrorQueryParam))
            if (hasFidoLoginError) {
                Log.d(TAG, "Fido login error response url: $uri")
                biometricsInterface.notifyLoginErrorOccurrence()
                nhsWeb.loadWelcomePage()
                return
            }

            val hasAppScheme = uri.scheme == getString(R.string.appScheme)
            val url = if (hasAppScheme) uri.buildUpon()
                .scheme(getString(R.string.baseScheme)).toString()
            else uri.toString()

            if (hasAppScheme)
                showBlankScreen()

            nhsWeb.loadUrl(url)
        }
    }

    fun configBiometricSetup(fidoServerUrl: String) {
        if (biometricsInterface.isFingerprintServiceInitialised()) return
        biometricsInterface.initializeFingerprintService(fidoServerUrl)

        biometricToggleSwitch.setOnTouchListener { _, event ->
            if (event.action == MotionEvent.ACTION_UP) {
                return@setOnTouchListener biometricsInterface
                    .requestBiometricsRegistrationStateChange()
            }
            return@setOnTouchListener true
        }
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
        nhsWeb.loadUrl(path)
    }

    override fun loadBiometricLoginPage(url: String) {
        nhsWeb.useAbsoluteUri = true
        nhsWeb.loadUrl(url)
    }

    override fun loadPage(url: String) {
        nhsWeb.loadUrl(url)
    }

    private fun onNhsOnlineLogoIconSelected() {
        nhsWeb.loadWelcomePage()
        menuBar.deselectActiveItem()
    }

    private fun onMyAccountIconSelected() {
        nhsWeb.loadUrl(resources.getString(R.string.myAccountPath))
        menuBar.deselectActiveItem()
    }

    private fun onHelpIconSelected() {
        nhsWeb.loadUrlInChromeTab(resources.getString(R.string.helpURL))
    }

    override fun setHeaderText(text: String, description: String?) {
        logger.info("Entering setHeaderText")
        header_text_view.text = text
        header_text_view.contentDescription = description
        nhsWeb.announceForAccessibility(description ?: text)
    }

    override fun onBackPressed() {
        logger.info("${this::class.java.simpleName}: Entering onBackPressed")

        val path = URL(webview.url).path

        when {
            nhsWeb.isUserLoggedIn -> showExitDialog()
            path == "/" + resources.getString(R.string.gpFinderPath) -> this.finishAndRemoveTask()
            path.contains(resources.getString(R.string.gpFinderPath),
                ignoreCase = true) -> appWebInterface.resetGPFinderFlow(getString(R.string.gpFinderPath))
            else -> this.finishAndRemoveTask()
        }
    }

    override fun onResume() {
        super.onResume()

        if (isSuccessfulConfigCheck)
            nhsWeb.reloadLoginUrl()
    }

    private fun showExitDialog() {
        appDialogs.showExitDialog {
            nhsWeb.loadUrl(resources.getString(R.string.baseURL) + resources.getString(R.string.logoutPath))
        }
    }

    override fun showExtendSessionDialogue(sessionDuration: Int) {
        logger.info("Entering showExtendSessionDialogue")

        val sessionExtendCallback = { appWebInterface.extendSession() }
        val logoutCallback = { appWebInterface.logout() }
        appDialogs.showExtendSessionDialogue(sessionDuration, sessionExtendCallback, logoutCallback)
    }

    override fun showProgressDialog() {
        if (progressBarLayout.visibility == View.GONE)
        {
            progressBarLayout.visibility = View.VISIBLE
            window.setFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE,
                WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE)
        }

    }

    override fun dismissProgressDialog() {
        if (progressBarLayout.visibility == View.VISIBLE) {
            window.clearFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE)
            progressBarLayout.visibility = View.GONE
        }
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
        appDialogs.dismissVersionUpgradeDialog()
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
        nhsWeb.useAbsoluteUri = true
    }

    override fun showWebviewScreen() {
        logger.info("Entering showWebViewScreen")

        if (isSuccessfulConfigCheck) {
            activityViewSwitcher.switchTo(ActivityView.WEBVIEW)
            hideBlankScreen()

            if (nhsWeb.isUserLoggedIn) {
                nhsWeb.useAbsoluteUri = false
            }
        }
    }

    override fun setWebViewVisible() = activityViewSwitcher.switchTo(ActivityView.WEBVIEW)

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<out String>, grantResults: IntArray
    ) {
        if (requestCode == LOCATION_REQUEST_CODE) {
            nhsWeb.handleWebClientLocationResult(grantResults)
        }
    }

    override fun setMenuBarItem(index: Int) {
        when (index) {
            0 -> menuBar.switchActiveMenuItemTo(R.id.symptoms)
            1 -> menuBar.switchActiveMenuItemTo(R.id.appointments)
            2 -> menuBar.switchActiveMenuItemTo(R.id.prescriptions)
            3 -> menuBar.switchActiveMenuItemTo(R.id.myRecord)
            4 -> menuBar.switchActiveMenuItemTo(R.id.more)
        }
    }

    override fun clearMenuBarItem() {
        menuBar.deselectActiveItem()
    }

    override fun goToCheckSymptoms() {
        val intent = Intent(this, SymptomsActivity::class.java)
        startActivity(intent)
    }

    override fun showNativeBiometricOptions() {
        activityViewSwitcher.switchTo(ActivityView.FINGERPRINT)
        setHeaderText(resources.getString(R.string.biometric_header))
    }

    override fun announcePageTitle(title: String?) {
        title?.let { nhsWeb.announceForAccessibility(it) }
    }

    override fun hideMenuBar() {
        logger.info("Entering hideMenuBar")
        menuBar.visibility = GONE
    }

    override fun hideHeader() {
        logger.info("Entering hideHeader")
        header.visibility = GONE
    }

    override fun showMenuBar() {
        logger.info("Entering showMenuBar")
        menuBar.visibility = VISIBLE
    }

    override fun showHeader() {
        logger.info("Entering showHeader")
        header.visibility = VISIBLE
    }

    override fun showBlankScreen() {
        logger.info("Entering showBlankScreen")
        viewSwitcher.visibility = View.GONE
        blankScreen.visibility = View.VISIBLE
    }

    override fun resetFocusToNhsLogoForAccessibility() {
        val a11yMng = getSystemService(Context.ACCESSIBILITY_SERVICE) as AccessibilityManager
        if (header.visibility == View.VISIBLE && a11yMng.isTouchExplorationEnabled && a11yMng.isEnabled) {
            nhsOnlineLogoIcon.sendAccessibilityEvent(AccessibilityEvent.TYPE_VIEW_FOCUSED)
        }
    }

    override fun hideBlankScreen() {
        logger.info("Entering hideBlankScreen")
        dismissProgressDialog()
        viewSwitcher.visibility = View.VISIBLE
        blankScreen.visibility = View.GONE
    }

    override fun dismissSessionExtensionDialog() {
        appDialogs.dismissExtendSessionDialog()
    }

    override fun showBiometricLoginIfEnabled() = biometricsInterface.showBiometricLoginIfEnabled()

    private fun onSuccessButton() {
        activityViewSwitcher.switchTo(ActivityView.WEBVIEW)
        nhsWeb.reloadCurrentUrl()
    }
}