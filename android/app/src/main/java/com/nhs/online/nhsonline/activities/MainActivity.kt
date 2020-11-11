package com.nhs.online.nhsonline.activities

import android.annotation.SuppressLint
import android.app.Activity
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Build.*
import android.os.Bundle
import androidx.fragment.app.FragmentActivity
import androidx.appcompat.app.ActionBar
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.app.AppCompatDelegate
import android.util.Log
import android.view.MenuItem
import android.view.View
import android.view.View.GONE
import android.view.View.VISIBLE
import android.view.WindowManager
import android.view.accessibility.AccessibilityEvent
import android.view.accessibility.AccessibilityManager
import android.webkit.WebSettings
import com.nhs.online.fidoclient.exceptions.FidoInvalidSignatureException
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.BiometricsInteractor
import com.nhs.online.nhsonline.biometrics.BiometricsInterface
import com.nhs.online.nhsonline.biometrics.utils.BiometricConstants
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.clients.FirebaseClient
import com.nhs.online.nhsonline.clients.HttpClient
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.navigation.MenuBarItem
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.registration.PaycassoFlowFactory
import com.nhs.online.nhsonline.registration.PaycassoService
import com.nhs.online.nhsonline.services.ConfigurationService
import com.nhs.online.nhsonline.services.logging.LoggingService
import com.nhs.online.nhsonline.services.logging.VolleyQueueProvider
import com.nhs.online.nhsonline.services.NotificationsService
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.services.logging.ILoggingService
import com.nhs.online.nhsonline.support.*
import com.nhs.online.nhsonline.support.intentHandlers.DefaultIntentHandler
import com.nhs.online.nhsonline.support.intentHandlers.FirebaseMessagingIntentHandler
import com.nhs.online.nhsonline.support.intentHandlers.IntentHandlers
import com.nhs.online.nhsonline.support.intentHandlers.ViewIntentHandler
import com.nhs.online.nhsonline.utils.NotificationManagerCompat
import com.nhs.online.nhsonline.web.NhsWeb
import com.nhs.online.nhsonline.utils.UrlHelper
import com.nhs.online.nhsonline.web.UserAgentBuilder.buildUserAgentString
import com.nhs.online.nhsonline.webclients.CAMERA_STORAGE_REQUEST_CODE
import com.nhs.online.nhsonline.webclients.LOCATION_REQUEST_CODE
import com.nhs.online.nhsonline.webclients.UPLOAD_FILE_REQUEST_CODE
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.error_layout.*
import kotlinx.android.synthetic.main.header_layout.*
import kotlinx.android.synthetic.main.success_layout.*
import java.net.MalformedURLException
import java.net.URL
import java.util.logging.Level
import java.util.logging.Logger

private val TAG = MainActivity::class.java.simpleName

class MainActivity :
        IInteractor,
        AppCompatActivity(),
        LifeCycleObserverContext {
    private val logger = Logger.getLogger(TAG)
    private val openBrowserActivity = OpenUrlInBrowserActivity()
    private lateinit var biometricsInterface: BiometricsInterface
    private lateinit var biometricsInteractor: BiometricsInteractor
    private lateinit var connectionStateMonitor: ConnectionStateMonitor
    private var nhsWeb: NhsWeb? = null
    lateinit var appDialogs: AppDialogs
    private lateinit var appWebInterface: AppWebInterface
    private var lifeCycleObserver: LifeCycleObserver? = null
    lateinit var activityViewSwitcher: MainActivityViewSwitcher
    private lateinit var downloadHelper: FileDownloadHelper
    private lateinit var notificationsService: NotificationsService
    private lateinit var urlHelper: UrlHelper
    private lateinit var appPersistData: PersistData
    private lateinit var intentHandlers: IntentHandlers
    private lateinit var paycassoService: PaycassoService
    private lateinit var paycassoFlowFactory: PaycassoFlowFactory
    private lateinit var nhs111Uri: Uri

    private val headerViewSwitcherLoggedInHeaderIndex = 0
    private val headerViewSwitcherLoggedOutSymptomsHeaderIndex = 1

    private var knownServices = KnownServices()
    private lateinit var configServiceManager: ConfigurationServiceManager
    private lateinit var configService: ConfigurationService
    var configurationResponse: ConfigurationResponse = ConfigurationResponse()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        intentHandlers = IntentHandlers()
        intentHandlers.registerHandler(FirebaseMessagingIntentHandler(this))
        intentHandlers.registerHandler(ViewIntentHandler(this))
        intentHandlers.registerHandler(DefaultIntentHandler(this))

        if (resources.getString(R.string.secureFlag) != "disabled" && VERSION.SDK_INT < VERSION_CODES.O) {
            window.setFlags(WindowManager.LayoutParams.FLAG_SECURE,
                    WindowManager.LayoutParams.FLAG_SECURE)
        }
        urlHelper = UrlHelper(this)
        appPersistData = PersistData(this)

        setTheme(R.style.AppTheme)
        setContentView(R.layout.activity_main)
        setSupportActionBar(findViewById(R.id.logged_in_header))
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        nhs111Uri = Uri.parse(getString(R.string.nhs111URL))

        activityViewSwitcher = MainActivityViewSwitcher(this)
        appDialogs = AppDialogs(this)

        window.setFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE,
                WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE)

        configureWebView()
        appWebInterface = AppWebInterface(webview)
        downloadHelper = FileDownloadHelper(this)
        paycassoFlowFactory = PaycassoFlowFactory(logger)
        paycassoService = PaycassoService(paycassoFlowFactory.getFlow(this))

        val notificationManager = NotificationManagerCompat(this)
        notificationsService = NotificationsService(appWebInterface, FirebaseClient(), notificationManager)

        connectionStateMonitor = ConnectionStateMonitor(this)
        connectionStateMonitor.registerNetworkCallback()

        configService = ConfigurationService(
                this,
                resources.getString(R.string.baseApiURL) + resources.getString(R.string.configurationApiPath),
                this,
                ErrorMessageHandler(this.resources),
                HttpClient(),
                connectionStateMonitor
        )
        configServiceManager = ConfigurationServiceManager(this, configService)
        configurationResponse = configServiceManager.getConfigurationResponse()
        if (configurationResponse.callSuccessful) {
            knownServices = configurationResponse.knownServices!!
        }

        val loggingService = LoggingService(this, VolleyQueueProvider())
        initialiseNhsWeb(loggingService)
        initialiseBiometrics(loggingService)

        intentHandlers.handleIntent(intent, true, nhsWeb!!)

        if (configurationResponse.callSuccessful) {
            loadWelcomePage()
        } else {
            dismissSplashScreen()
        }

        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }

        backToAccountButton.setOnClickListener { onSuccessButton() }
        retryButton.setOnClickListener { onErrorRetryButton() }
        homeLogoIcon.setOnClickListener { onNhsOnlineLogoIconSelected() }
        myAccountIcon.setOnClickListener { onMyAccountIconSelected() }
        helpIcon.setOnClickListener { onHelpIconSelected() }
    }

    private fun initialiseNhsWeb(loggingService: ILoggingService) {
        nhsWeb = NhsWeb(this, this, webview, notificationsService, appWebInterface,
            knownServices, paycassoService, loggingService, connectionStateMonitor)

        menuBar.nhsWeb = nhsWeb

        setHelpUrl(resources.getString(R.string.helpURL))
    }

    private fun initialiseBiometrics(loggingService: ILoggingService) {
        biometricsInteractor = BiometricsInteractor(this, nhsWeb!!, this)
        biometricsInterface = BiometricsInterface(biometricsInteractor, this, appWebInterface, loggingService)
        configBiometricSetup(configurationResponse.fidoServerUrl)
    }

    private fun loadWelcomePage() {
        nhsWeb?.loadWelcomePage()
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

        webview.settings.userAgentString = buildUserAgentString(userAgent)
    }

    override val url: String?
        get() = webview.url

    private fun onErrorRetryButton() {
        logger.info("${this::class.java.simpleName}: Entering OnErrorRetryButton")

        if (!connectionStateMonitor.isConnectedToNetwork) {
            logger.info(
                    "${this::class.java.simpleName}: Leaving OnErrorRetryButton as presently no network access")
            return
        }

        if (!configurationResponse.callSuccessful) {
            configurationResponse = configServiceManager.getConfigurationResponse()
            if (configurationResponse.callSuccessful) {
                knownServices = configurationResponse.knownServices!!

                val loggingService = LoggingService(this, VolleyQueueProvider())

                initialiseNhsWeb(loggingService)
                initialiseBiometrics(loggingService)
                loadWelcomePage()
            } else {
                logger.info(
                        "${this::class.java.simpleName}: Leaving OnErrorRetryButton as getConfigurationResponse didn't work")
                return
            }
        }

        clearMenuBarItem()
        showProgressDialog()
        nhsWeb?.reloadCurrentUrl()
        dismissProgressDialog()
    }

    override fun onStart() {
        logger.info("Entering OnStart")
        super.onStart()

        if (configurationResponse.callSuccessful) {
            if (lifeCycleObserver == null) {
                lifeCycleObserver = LifeCycleObserver(this,
                        appWebInterface, knownServices)
            }
            lifeCycleObserver?.onMoveToForeground()
        }
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
        val fileUploadCallback = nhsWeb?.getFileUploadCallback()

        logger.log(Level.WARNING,
                "${this::class.java.simpleName}: Entering onActivityResult with request code: $requestCode")

        try {
            if (resultCode == Activity.RESULT_OK) {
                if (requestCode == UPLOAD_FILE_REQUEST_CODE) {

                    val uploadedFileLocation = nhsWeb?.getUploadedFileLocation()

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
                    nhsWeb?.resetFileUploadCallback()
                }
            } else {
                if (requestCode == UPLOAD_FILE_REQUEST_CODE) {
                    if (fileUploadCallback == null) return

                    fileUploadCallback.onReceiveValue(null)
                    nhsWeb?.resetFileUploadCallback()
                }
            }
        } catch (exception: Exception) {
            logger.log(Level.SEVERE, "Unexpected error in onActivityResult", exception)
        }
    }

    override fun onNewIntent(intent: Intent?) {
        super.onNewIntent(intent)
        intent?.let {
            intentHandlers.handleIntent(intent, false, nhsWeb!!)
        }
    }

    fun configBiometricSetup(fidoServerUrl: String) {
        if (!biometricsInterface.isFingerprintServiceInitialised()) {
            biometricsInterface.initializeFingerprintService(fidoServerUrl)
        }
    }

    override fun onOptionsItemSelected(menuItem: MenuItem): Boolean {
        return nhsWeb!!.onSlimHeaderBackButtonPressed()
    }

    private fun onMenuSelected(menuBarItem: MenuBarItem) {
        logger.info("Entering onMenuSelected")
        val path: String

        when (menuBarItem.id) {
            R.id.advice -> {
                path = resources.getString(R.string.advicePath)
            }
            R.id.yourHealth -> {
                path = resources.getString(R.string.healthRecordsPath)
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
        nhsWeb?.stopLoading()
        nhsWeb?.loadUrl(path)
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
        nhsWeb?.loadUrl(url)
    }

    private fun onNhsOnlineLogoIconSelected() {
        nhsWeb?.loadWelcomePage()
        menuBar.deselectActiveItem()
    }

    private fun onMyAccountIconSelected() {
        nhsWeb?.loadUrl(resources.getString(R.string.myAccountPath))
        menuBar.deselectActiveItem()
    }

    private fun onHelpIconSelected() {
        nhsWeb?.loadUrlInChromeTab(nhsWeb!!.getHelpLocation())
    }

    override fun setHelpUrl(url: String) {
        logger.info("Entering setHelpUrl")
        nhsWeb?.setHelpLocation(url)
    }

    override fun setRetryPath(url: String) {
        logger.info("Entering setRetryUrl")
        nhsWeb?.setReloadPath(url)

        logger.info("New retry URL: ${nhsWeb?.reloadUrl}")
    }

    override fun onBackPressed() {
        logger.info("${this::class.java.simpleName}: Entering onBackPressed")

        val path = getCurrentPath(webview.url)

        nhsWeb?.let {
            when {
                it.isUserLoggedIn -> showExitDialog()
                path == "/" + resources.getString(R.string.checkYourSymptoms) ->
                    it.onbackButtonPressedOnLoggedOutUnsecurePage()
                path == "/" + resources.getString(R.string.loginBiometricsError) ->
                    it.onbackButtonPressedOnLoggedOutUnsecurePage()
                it.shouldReloadHomepageOnBackReturn(it.reloadUrl) ->
                    it.reloadHomepageOnBackReturn()
                else -> this.finishAndRemoveTask()
            }
            return
        }
        this.finishAndRemoveTask()
    }

    override fun onPause() {
        if (VERSION.SDK_INT >= VERSION_CODES.O) {
            window.addFlags(WindowManager.LayoutParams.FLAG_SECURE)
        }
        super.onPause()
    }

    override fun onResume() {
        super.onResume()

        if (VERSION.SDK_INT >= VERSION_CODES.O) {
            window.clearFlags(WindowManager.LayoutParams.FLAG_SECURE)
        }

        if (configurationResponse.callSuccessful) {
            nhsWeb?.reloadLoginUrl()
        }
    }

    override fun ensureSupportedVersion() {
        if (!configurationResponse.isSupportedVersion && !appDialogs.isUpgradeDialogActive()) {
            appDialogs.showVersionUpgradeDialog()
        }
    }

    private fun showExitDialog() {
        appDialogs.showExitDialog {
            nhsWeb?.loadUrl(resources.getString(R.string.baseURL) + resources.getString(R.string.logoutPath))
        }
    }

    override fun showExtendSessionDialogue() {
        logger.info("Entering showExtendSessionDialogue")

        val sessionExtendCallback = { appWebInterface.extendSession() }
        val logoutCallback = { appWebInterface.logout() }
        appDialogs.showExtendSessionDialogue(sessionExtendCallback, logoutCallback)
    }

    override fun showLeavingPageWarningDialogue() {
        logger.info("Entering showLeavingPageWarningDialogue")

        val stayOnPageCallback = { appWebInterface.stayOnPage() }
        val leavePageCallback = { appWebInterface.leavePage() }
        appDialogs.showLeavingPageWarningDialogue(stayOnPageCallback, leavePageCallback)
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

    override fun dismissSplashScreen() {
        splashLayout.visibility = GONE
    }

    override fun selectNavigationMenuActive(navigationMenuId: Int) {
        if (menuBar.visibility == VISIBLE)
            menuBar.switchActiveMenuItemTo(navigationMenuId)
    }

    private fun showDownloadDocumentFailureError() {
        Log.d(Application.TAG, "Download document failed")
        showErrorScreen(ErrorMessage(this.resources, ErrorType.DownloadDocumentError))
    }

    override fun showUnavailabilityError(errorMessage: ErrorMessage) {
        showErrorScreen(errorMessage)
    }

    private fun showErrorScreen(errorMessage: ErrorMessage) {
        try {
            if (errorHeader !== null && errorTextView !== null) {
                if (errorMessage.header.isEmpty()) {
                    errorHeader.visibility = GONE
                }
                errorHeader.text = errorMessage.header

                errorTextView.setServiceError(errorMessage.title, errorMessage.message)
                errorTextView.makeLinks(Pair(nhs111Uri.host.toString(), View.OnClickListener {
                    if (canOpenUrlInWebView()) {
                        nhsWeb?.loadUrl(nhs111Uri.toString())
                    } else {
                        openUrlInBrowserActivity(nhs111Uri.toString())
                    }
                }))
                errorTextView.contentDescription = errorMessage.title + ". " +
                        errorMessage.accessibleMessage

                appDialogs.dismissVersionUpgradeDialog()
                showErrorScreen()
            }
        } catch (e: Exception) {
            logger.log(Level.WARNING, "${this::class.java.simpleName}: Unable to show error page $e")
        }
    }

    private fun canOpenUrlInWebView(): Boolean {
        return nhsWeb != null && configurationResponse.callSuccessful && connectionStateMonitor.isConnectedToNetwork
    }

    private fun showErrorScreen() {
        hideBlankScreen()
        activityViewSwitcher.switchTo(ActivityView.ERROR)
        nhsWeb?.requiresFullPageLoad = true
    }

    override fun showWebviewScreen() {
        logger.info("Entering showWebViewScreen")

        nhsWeb?.applicationState?.unBlock()

        if (configurationResponse.callSuccessful) {
            activityViewSwitcher.switchTo(ActivityView.WEBVIEW)
            hideBlankScreen()

            if (nhsWeb!!.isUserLoggedIn) {
                nhsWeb!!.requiresFullPageLoad = false
            }
        }
    }

    override fun setWebViewVisible() = activityViewSwitcher.switchTo(ActivityView.WEBVIEW)

    override fun onRequestPermissionsResult(
            requestCode: Int,
            permissions: Array<out String>, grantResults: IntArray
    ) {
        when (requestCode) {
            LOCATION_REQUEST_CODE -> nhsWeb?.handleWebClientLocationResult(grantResults)
            CAMERA_STORAGE_REQUEST_CODE -> {
                logger.info("Checking Camera Storage Request Code: $CAMERA_STORAGE_REQUEST_CODE")
                nhsWeb?.handleCameraFilePermissionResult(grantResults)
            }
            STORAGE_REQUEST_CODE -> {
                logger.info("Storage permission granted, starting download")
                handleDownloadPermissionResult(grantResults)
            }
        }
    }

    override fun setMenuBarItem(index: Int) {
        when (index) {
            MenuTab.Advice.tabIndex -> menuBar.switchActiveMenuItemTo(R.id.advice)
            MenuTab.Appointments.tabIndex -> menuBar.switchActiveMenuItemTo(R.id.appointments)
            MenuTab.Prescriptions.tabIndex -> menuBar.switchActiveMenuItemTo(R.id.prescriptions)
            MenuTab.MyRecord.tabIndex -> menuBar.switchActiveMenuItemTo(R.id.yourHealth)
            MenuTab.More.tabIndex -> menuBar.switchActiveMenuItemTo(R.id.more)
        }
        enableMenuBar();
    }

    override fun clearMenuBarItem() {
        menuBar.deselectActiveItem()
    }

    fun enableMenuBar() {
        nhsWeb?.applicationState?.unBlock()
    }

    override fun announcePageTitle(title: String?) {
        title?.let { nhsWeb?.announceForAccessibility(it) }
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

    override fun hideHeaderAndMenu() {
        logger.info("Entering hideHeaderAndMenu")
        header_view_switcher.visibility = GONE
        menuBar.visibility = GONE
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

    override fun showHeaderSlim(headerIcon: HeaderIcon) {
        logger.info("Entering showHeaderSlim")
        header_view_switcher.visibility = VISIBLE
        header_view_switcher.displayedChild = headerViewSwitcherLoggedOutSymptomsHeaderIndex

        setupToolbar(R.id.symptoms_toolbar, true)?.apply {
            when (headerIcon) {
                HeaderIcon.Back -> setHomeAsUpIndicator(R.drawable.back_arrow)
                HeaderIcon.Close -> setHomeAsUpIndicator(R.drawable.ic_nhsapp_close)
            }
        }
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
        viewSwitcher.visibility = VISIBLE
        blankScreen.visibility = GONE
    }

    override fun pageLoadComplete() {
        logger.info("Entering pageLoadComplete")
        nhsWeb?.applicationState?.unBlock()
    }

    override fun dismissSessionExtensionDialog() {
        appDialogs.dismissExtendSessionDialog()
    }

    override fun fetchBiometricSpec() {
        if (biometricsInterface.isFingerprintServiceInitialised()) {
            appWebInterface.biometricSpec(BiometricConstants.BIOMETRIC_TYPE,
                    biometricsInterface.isFingerprintRegistered)
        }
    }

    override fun showBiometricLoginIfEnabled(forceStart: Boolean): Boolean {
        if (!configurationResponse.callSuccessful || !configurationResponse.isSupportedVersion) {
            return false
        }

        webview?.url?.let { currentUrl ->
            val url = URL(currentUrl)

            if (url.query == null || !url.query.contains(resources.getString(R.string.fidoAuthQueryKey))) {
                try {
                    return biometricsInterface.showBiometricLoginIfEnabled(forceStart)
                } catch (fidoException: FidoInvalidSignatureException) {
                    appWebInterface.biometricLoginFailure()
                }
            }
        }

        return false
    }

    override fun dismissBiometricDialog() {
        biometricsInterface.dismissBiometricDialog()
    }

    override fun displayBiometricLoginErrorOccurrence() {
        biometricsInterface.notifyLoginErrorOccurrence()
    }

    override fun canDisplayBiometricLogin(): Boolean {
        return biometricsInterface.isFingerprintRegistered
    }

    private fun onSuccessButton() {
        activityViewSwitcher.switchTo(ActivityView.WEBVIEW)
        nhsWeb?.reloadCurrentUrl()
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

    private fun setupToolbar(id: Int, isHomeEnabled: Boolean): ActionBar? {
        setSupportActionBar(findViewById(id))
        return supportActionBar?.apply {
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

    override fun getActivity(): FragmentActivity = this

    override fun updateBiometricRegistration(accessToken: String) {

        biometricsInteractor.dismissBiometricNotification()
        biometricsInterface.requestBiometricsRegistrationStateChange(accessToken)
    }

    override fun dismissAllDialogues() {
        appDialogs.dismissAll()
    }

    override fun dismissPageLeaveWarningDialogue() {
        appDialogs.dismissShowLeavingWarningDialog()
    }

    override fun openUrlInBrowserActivity(url: String) {
        openBrowserActivity.start(this, url, this)
    }

    override fun showInternetConnectionError() {
        nhsWeb!!.showNoConnectionError()
    }
}
