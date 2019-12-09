package com.nhs.online.nhsonline.activities

import android.annotation.SuppressLint
import android.app.Activity
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Build
import android.os.Bundle
import android.support.v4.app.FragmentActivity
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.util.Log
import android.view.MenuItem
import android.view.MotionEvent
import android.view.View.GONE
import android.view.View.VISIBLE
import android.view.WindowManager
import android.view.accessibility.AccessibilityEvent
import android.view.accessibility.AccessibilityManager
import android.webkit.WebSettings
import com.nhs.online.fidoclient.interfaces.IBiometricsInteractor
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.clients.FirebaseClient
import com.nhs.online.nhsonline.services.NotificationsService
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.BiometricsInterface
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.navigation.MenuBarItem
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhs.online.nhsonline.support.*
import com.nhs.online.nhsonline.utils.NotificationManagerCompat
import com.nhs.online.nhsonline.web.NhsWeb
import com.nhs.online.nhsonline.webclients.CAMERA_STORAGE_REQUEST_CODE
import com.nhs.online.nhsonline.webclients.LOCATION_REQUEST_CODE
import com.nhs.online.nhsonline.webclients.UPLOAD_FILE_REQUEST_CODE
import com.nhs.online.nhsonline.support.STORAGE_REQUEST_CODE
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.biometric_layout_content.*
import kotlinx.android.synthetic.main.breadcrumb_layout.*
import kotlinx.android.synthetic.main.error_layout.*
import kotlinx.android.synthetic.main.header_layout.*
import kotlinx.android.synthetic.main.success_layout.*
import java.net.MalformedURLException
import java.net.URL
import java.util.logging.Level
import java.util.logging.Logger


private val TAG = MainActivity::class.java.simpleName

class MainActivity : IInteractor, AppCompatActivity(), IBiometricsInteractor {
    private val logger = Logger.getLogger(TAG)
    private val biometricsInterface: BiometricsInterface = BiometricsInterface(this)
    private lateinit var connectionStateMonitor: ConnectionStateMonitor
    private lateinit var nhsWeb: NhsWeb
    private lateinit var appDialogs: AppDialogs
    private lateinit var appWebInterface: AppWebInterface
    private var lifeCycleObserver: LifeCycleObserver? = null
    private lateinit var activityViewSwitcher: MainActivityViewSwitcher
    private val nhsAndroidUserAgent = "nhsapp-android/" + com.nhs.online.nhsonline.BuildConfig.VERSION_NAME
    private lateinit  var downloadHelper: FileDownloadHelper

    private val headerViewSwitcherLoggedInHeaderIndex = 0
    private val headerViewSwitcherLoggedOutSymptomsHeaderIndex = 1

    var isSuccessfulConfigCheck = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        if (resources.getString(R.string.secureFlag) != "disabled" &&
              Build.VERSION.SDK_INT < Build.VERSION_CODES.O){
            window.setFlags(WindowManager.LayoutParams.FLAG_SECURE,
                WindowManager.LayoutParams.FLAG_SECURE)
        }

        setContentView(R.layout.activity_main)
        setSupportActionBar(findViewById(R.id.logged_in_header))
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        activityViewSwitcher = MainActivityViewSwitcher(this)
        appDialogs = AppDialogs(this)

        configureWebView()

        appWebInterface = AppWebInterface(webview)
        val notificationManager = NotificationManagerCompat(this)
        val notificationsService = NotificationsService(appWebInterface, FirebaseClient(), notificationManager)
        downloadHelper = FileDownloadHelper(this)
        nhsWeb = NhsWeb(this, this, webview, notificationsService, appWebInterface)

        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        menuBar.nhsWeb = nhsWeb

        setHelpUrl(resources.getString(R.string.helpURL))

        backToAccountButton.setOnClickListener { onSuccessButton() }
        retryButton.setOnClickListener { onErrorRetryButton() }
        homeLogoIcon.setOnClickListener { onNhsOnlineLogoIconSelected() }
        myAccountIcon.setOnClickListener { onMyAccountIconSelected() }
        helpIcon.setOnClickListener { onHelpIconSelected() }
        breadcrumb.setOnClickListener { onBreadcrumbSelected() }
        connectionStateMonitor = ConnectionStateMonitor(this)
        connectionStateMonitor.registerNetworkCallback()
    }

    @SuppressLint("SetJavaScriptEnabled")
    private fun configureWebView() {
        logger.info("Entering configureWebView")
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true
        webview.settings.javaScriptCanOpenWindowsAutomatically = true
        webview.settings.allowUniversalAccessFromFileURLs = true
        webview.settings.cacheMode = WebSettings.LOAD_DEFAULT
        val userAgent = webview.settings.userAgentString
        webview.settings.setUserAgentString("$userAgent $nhsAndroidUserAgent")
    }

    private fun onErrorRetryButton() {
        logger.info("${this::class.java.simpleName}: Entering OnErrorRetryButton")

        if (!isConnectedToNetwork) {
            logger.info(
                "${this::class.java.simpleName}: Leaving OnErrorRetryButton as presently no network access")
            return
        }

        if (isSuccessfulConfigCheck) {
            clearMenuBarItem()
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
                appWebInterface, nhsWeb, appDialogs)
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
        connectionStateMonitor.deregisterNetworkCallback()
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, intent: Intent?) {
        super.onActivityResult(requestCode, resultCode, intent)

        var results: Array<Uri>? = null
        val fileUploadCallback = nhsWeb.getFileUploadCallback()

        logger.log(Level.WARNING,
            "${this::class.java.simpleName}: Entering onActivityResult with request code: $requestCode")

        try {
            if (resultCode == Activity.RESULT_OK) {
                if (requestCode == UPLOAD_FILE_REQUEST_CODE) {

                    val uploadedFileLocation = nhsWeb.getUploadedFileLocation()

                    if (fileUploadCallback == null) {
                        return
                    }

                    if (intent?.data == null) {
                        if (uploadedFileLocation != null) {
                            results = arrayOf(Uri.parse(uploadedFileLocation))
                        }
                    } else {
                        val dataString = intent.dataString
                        if (dataString != null) {
                            results = arrayOf(Uri.parse(dataString))
                        }
                    }

                    fileUploadCallback.onReceiveValue(results)
                    nhsWeb.resetFileUploadCallback()
                }
            } else {
                if (requestCode == UPLOAD_FILE_REQUEST_CODE) {
                    if (fileUploadCallback == null) return

                    fileUploadCallback.onReceiveValue(null)
                    nhsWeb.resetFileUploadCallback()
                }
            }
        } catch (exception: Exception) {
            logger.log(Level.SEVERE, "Unexpected error in onActivityResult" , exception)
        }


    }

    override fun onNewIntent(intent: Intent?) {
        super.onNewIntent(intent)
        handleNewIntent(intent)
    }

    private fun handleNewIntent(intent: Intent?) {
        intent?.data?.let { uri ->
            val uriPath = uri.path ?: ""

            val hasFidoLoginError = uriPath.contains(getString(R.string.authRedirectPath)) &&
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

    override fun onOptionsItemSelected(menuItem: MenuItem): Boolean {
        return nhsWeb.onSlimHeaderBackButtonPressed()
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
        nhsWeb.stopLoading()
        nhsWeb.loadUrl(path)
    }

    override fun loadBiometricLoginPage(url: String) {
        nhsWeb.requiresFullPageLoad = true
        nhsWeb.loadUrl(url)
    }

    override fun startDownload(base64Data: String, fileName: String, mimeType: String) {

        downloadHelper.fileName = fileName
        downloadHelper.fileMimeType = mimeType
        downloadHelper.base64Data = base64Data

        // User needs to allow access to their
        // internal storage on the device.
        if (downloadHelper.isStoragePermissionGranted()) {
            try {
                downloadHelper.convertBase64StringToFileAndStoreIt()
            } catch (e: Exception) {
                Log.e(TAG, "Download document resulted in error: ", e)
                showDownloadDocumentFailureError()
            }
        } else {
            downloadHelper.showStoragePermissionsPopup()
        }
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
        nhsWeb.loadUrlInChromeTab(nhsWeb.getHelpLocation())
    }

    private fun onBreadcrumbSelected() {
        if(!nhsWeb.reloadUrl.isNullOrBlank()){
            nhsWeb.loadUrl(nhsWeb.reloadUrl!!)
        } else {
            nhsWeb.loadWelcomePage()
        }
        menuBar.deselectActiveItem()
    }

    override fun setHeaderText(text: String, description: String?) {
        logger.info("Entering setHeaderText")

        nhsWeb.announceForAccessibility(description ?: text)
    }

    override fun setHelpUrl(url: String) {
        logger.info("Entering setHelpUrl")
        nhsWeb.setHelpLocation(url)
    }

    override fun setRetryPath(url: String) {
        logger.info("Entering setRetryUrl")
        nhsWeb.setReloadPath(url)

        logger.info("New retry URL: ${nhsWeb.reloadUrl}")
    }

    override fun onBackPressed() {
        logger.info("${this::class.java.simpleName}: Entering onBackPressed")

        val path = getCurrentPath(webview.url)

        when {
            nhsWeb.isUserLoggedIn -> showExitDialog()
            path == "/" + resources.getString(R.string.gpFinderPath) ->
                this.finishAndRemoveTask()
            path.contains(resources.getString(R.string.gpFinderPath), ignoreCase = true) ->
                appWebInterface.resetGPFinderFlow(getString(R.string.gpFinderPath))
            path == "/" + resources.getString(R.string.checkYourSymptoms)
                    || nhsWeb.isCheckSymptomsUnsecureURL(webview.url) ->
                nhsWeb.onbackButtonPressedOnCheckSymptomsUnsecurePage()
            nhsWeb.shouldReloadHomepageOnBackReturn(nhsWeb.reloadUrl) -> nhsWeb.reloadHomepageOnBackReturn()
            else -> this.finishAndRemoveTask()
        }
    }

    override fun onPause() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            window.addFlags(WindowManager.LayoutParams.FLAG_SECURE)
        }
        super.onPause()
    }

    override fun onResume() {
        super.onResume()

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            window.clearFlags(WindowManager.LayoutParams.FLAG_SECURE)
        }

        if (isSuccessfulConfigCheck) {
            if (nhsWeb.isLoginPath()) {
                showBiometricLoginIfEnabled(true)
            }
            nhsWeb.reloadLoginUrl()
        }
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
        if (progressBarLayout.visibility == GONE) {
            progressBarLayout.visibility = VISIBLE
            window.setFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE,
                WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE)
        }

    }

    override fun dismissProgressDialog() {
        if (progressBarLayout.visibility == VISIBLE) {
            window.clearFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE)
            progressBarLayout.visibility = GONE
        }
    }

    override fun selectNavigationMenuActive(navigationMenuId: Int) {
        if (menuBar.visibility == VISIBLE)
            menuBar.switchActiveMenuItemTo(navigationMenuId)
    }

    override fun getActivity(): FragmentActivity = this

    override fun toggleBiometricSwitch(isChecked: Boolean) {
        nhsWeb.onBiometricOptionChanged()
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
        val biometricDeviceErrorMessage = ErrorMessage(this, ErrorType.BiometricRegistrationFailure)
        Log.d(Application.TAG, "Biometric registration failed")
        showErrorScreen( biometricDeviceErrorMessage )
    }

    override fun showBiometricDeviceError() {
        val biometricDeviceErrorMessage = ErrorMessage(this, ErrorType.BiometricDeviceFailure)
        Log.d(Application.TAG, "Biometric device failed")
        showErrorScreen( biometricDeviceErrorMessage )
    }

    private fun showDownloadDocumentFailureError() {
        Log.d(Application.TAG, "Download document failed")
        showErrorScreen( ErrorMessage(this, ErrorType.DownloadDocumentError) )
    }

    override fun showUnavailabilityError(errorMessage: ErrorMessage) {
        showErrorScreen(errorMessage)
    }

    private fun showErrorScreen (errorMessage: ErrorMessage) {
        try {
            if(errorMessage.header.isEmpty()) {
                errorHeader.visibility = GONE
            }
            errorHeader.text = errorMessage.header
            errorTextView.setServiceError(errorMessage.title, errorMessage.message)
            errorTextView.contentDescription = errorMessage.title + ". " +
                    errorMessage.accessibleMessage

            appDialogs.dismissVersionUpgradeDialog()
            showErrorScreen()
        } catch (e: Exception) {
            logger.log(Level.WARNING, "${this::class.java.simpleName}: Unable to show error page $e")
        }

    }

    private fun showErrorScreen() {
        hideBlankScreen()
        activityViewSwitcher.switchTo(ActivityView.ERROR)
        nhsWeb.requiresFullPageLoad = true
    }

    override fun showWebviewScreen() {
        logger.info("Entering showWebViewScreen")

        nhsWeb.applicationState.unBlock()

        if (isSuccessfulConfigCheck) {
            activityViewSwitcher.switchTo(ActivityView.WEBVIEW)
            hideBlankScreen()

            if (nhsWeb.isUserLoggedIn) {
                nhsWeb.requiresFullPageLoad = false
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
        } else if (requestCode == CAMERA_STORAGE_REQUEST_CODE) {
            logger.info("Checking Camera Storage Request Code: $CAMERA_STORAGE_REQUEST_CODE")

            nhsWeb.handleCameraFilePermissionResult(grantResults)
        } else if (requestCode == STORAGE_REQUEST_CODE) {
            logger.info("Storage permission granted, starting download")
            handleDownloadPermissionResult(grantResults)
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

    fun enableMenuBar() {
        nhsWeb.applicationState.unBlock()
    }

    override fun showNativeBiometricOptions() {
        nhsWeb.requiresFullPageLoad = true
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
        header_view_switcher.visibility = GONE
    }

    override fun hideHeaderSlim() {
        logger.info("Entering hideHeaderSlim")
        header_view_switcher.visibility = GONE
    }

    override fun showMenuBar() {
        logger.info("Entering showMenuBar")
        menuBar.visibility = VISIBLE
    }

    override fun showHeader() {
        logger.info("Entering showHeader")
        header_view_switcher.visibility = VISIBLE
        header_view_switcher.displayedChild = headerViewSwitcherLoggedInHeaderIndex
        setupToolbar(R.id.logged_in_header, false)
    }

    override fun showHeaderSlim() {
        logger.info("Entering showHeaderSlim")
        header_view_switcher.visibility = VISIBLE
        header_view_switcher.displayedChild = headerViewSwitcherLoggedOutSymptomsHeaderIndex

        setupToolbar(R.id.symptoms_toolbar, true)
    }

    override fun showBlankScreen() {
        logger.info("Entering showBlankScreen")
        viewSwitcher.visibility = GONE
        blankScreen.visibility = VISIBLE
    }

    override fun resetFocusToNhsLogoForAccessibility() {
        val a11yMng = getSystemService(Context.ACCESSIBILITY_SERVICE) as AccessibilityManager
        if (logged_in_header.visibility == VISIBLE && a11yMng.isTouchExplorationEnabled && a11yMng.isEnabled) {
            homeLogoIcon.sendAccessibilityEvent(AccessibilityEvent.TYPE_VIEW_FOCUSED)
        }
    }

    override fun setZoomable(canZoom: Boolean) {
        logger.info("Entering setZoomable - setting zoomable to: $canZoom")
        webview.settings.displayZoomControls = false
        webview.settings.builtInZoomControls = canZoom
    }

    override fun hideBlankScreen() {
        logger.info("Entering hideBlankScreen")
        dismissProgressDialog()
        viewSwitcher.visibility = VISIBLE
        blankScreen.visibility = GONE
    }

    override fun pageLoadComplete() {
        logger.info("Entering pageLoadComplete")
        nhsWeb.applicationState.unBlock()
    }

    override fun dismissSessionExtensionDialog() {
        appDialogs.dismissExtendSessionDialog()
    }


    override fun showBiometricLoginIfEnabled(forceStart: Boolean): Boolean {
        if (!isSuccessfulConfigCheck){
            return false
        }

        return biometricsInterface.showBiometricLoginIfEnabled(forceStart)
    }

    override fun displayBiometricLoginErrorOccurrence() {
        biometricsInterface.notifyLoginErrorOccurrence()
    }

    override fun canDisplayBiometricLogin(): Boolean {
        return biometricsInterface.isFingerprintRegistered
    }

    private fun onSuccessButton() {
        activityViewSwitcher.switchTo(ActivityView.WEBVIEW)
        nhsWeb.reloadCurrentUrl()
    }

    private fun getCurrentPath(currentUrl: String?): String {
        var path = ""

        if (currentUrl == null) {
            logger.log(Level.WARNING, "${this::class.java.simpleName}: Current webview url is null")
            return path
        }

        try {
            path = URL(currentUrl).path
        } catch (e: MalformedURLException) {
            logger.log(Level.WARNING,
                "${this::class.java.simpleName}: MalformedUrlException: ${webview.url} $e")
        }

        return path
    }

    private fun setupToolbar(id: Int, isHomeEnabled: Boolean) {
        setSupportActionBar(findViewById(id))
        supportActionBar?.apply {
            title = null
            setHomeButtonEnabled(isHomeEnabled)
            setDisplayHomeAsUpEnabled(isHomeEnabled)
        }
    }

    private fun handleDownloadPermissionResult(grantResults: IntArray) {
        if (grantResults.isNotEmpty() && grantResults.all { it == PackageManager.PERMISSION_GRANTED }) {
            Log.d(TAG, "Permissions granted for storage")

            startDownload(downloadHelper.base64Data, downloadHelper.fileName, downloadHelper.fileMimeType)
        } else {
            Log.d(TAG, "Permission not granted")
        }
    }
}