package com.nhs.online.nhsonline.web

import android.app.Activity
import android.net.Uri
import android.util.Log
import android.webkit.CookieManager
import android.webkit.URLUtil
import android.webkit.WebView
import com.google.gson.Gson
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.data.PaycassoData
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.registration.PaycassoCallbackResponse
import com.nhs.online.nhsonline.registration.PaycassoService
import com.nhs.online.nhsonline.services.logging.ILoggingService
import com.nhs.online.nhsonline.services.NotificationsService
import com.nhs.online.nhsonline.services.SettingsService
import com.nhs.online.nhsonline.services.UrlLoader
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.knownservices.enums.ViewMode
import com.nhs.online.nhsonline.support.AddToCalendarHandler
import com.nhs.online.nhsonline.support.ApplicationState
import com.nhs.online.nhsonline.support.PersistData
import com.nhs.online.nhsonline.support.schemehandlers.MailToSchemeHandler
import com.nhs.online.nhsonline.support.schemehandlers.SchemeHandlers
import com.nhs.online.nhsonline.support.schemehandlers.TelSchemeHandler
import com.nhs.online.nhsonline.webinterfaces.WebAppInterfacePrivate
import com.nhs.online.nhsonline.webinterfaces.WebAppInterfaceThirdParty
import com.nhs.online.nhsonline.support.uiinteraction.IHeaderStrategy
import com.nhs.online.nhsonline.support.uiinteraction.LoggedInHeaderStrategy
import com.nhs.online.nhsonline.support.uiinteraction.LoggedOutHeaderStrategy
import com.nhs.online.nhsonline.utils.UrlHelper
import com.nhs.online.nhsonline.webclients.ChromeClientLocationHandler
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.webinterfaces.WebAppInterfaceNhsLogin
import java.net.MalformedURLException
import java.net.URL
import java.util.Locale
import java.io.File

private val TAG = NhsWeb::class.java.simpleName
private const val NATIVE_APP_PRIVATE = "nativeApp"
private const val NATIVE_APP_THIRDPARTY = "nhsappNative"
private const val NATIVE_APP_LOGIN = "nativeNhsLogin"


class NhsWeb(
        private val activity: Activity,
        private val uiInteractor: IInteractor,
        private val webView: WebView,
        private val notificationsService: NotificationsService,
        appWebInterface: AppWebInterface,
        private val knownServices: KnownServices,
        private val paycassoService: PaycassoService,
        private val loggingService: ILoggingService,
        private val connectionStateMonitor: ConnectionStateMonitor
) {
    private val urlLoader = UrlLoader(webView, activity.getString(R.string.baseURL), appWebInterface)
    private val urlHelper = UrlHelper(activity)
    private val chromeClient = ChromeClientLocationHandler(activity)
    private val appPersistData = PersistData(activity)
    private val errorMessageHandler = ErrorMessageHandler(activity.resources)
    private val settingsService = SettingsService(activity)
    private val cacheDir = File(activity.filesDir.parent + "/cache")
    private val appWebViewDir = File(activity.filesDir.parent + "/app_webview")
    private val authRedirectPath = readResourceString(R.string.authRedirectPath)
    private val fidoAuthQueryKey = readResourceString(R.string.fidoAuthQueryKey)
    private val nhs111Host = Uri.parse(readResourceString(R.string.nhs111URL)).host.toString()

    var headerStrategy: IHeaderStrategy = LoggedOutHeaderStrategy(uiInteractor)

    var applicationState =
            ApplicationState(readResourceString(R.string.menuTimeoutSeconds).toLong())
    var isUserLoggedIn = false
    set(value) {
        field = value
        headerStrategy = if (field) LoggedInHeaderStrategy(uiInteractor) else LoggedOutHeaderStrategy(uiInteractor)
    }
    var requiresFullPageLoad = true
    var javaScriptInteractionMode = JavaScriptInteractionMode.None
    var reloadUrl: String? = null
        set(value) {
            value?.let { url ->
                val ks = knownServices.findMatchingKnownService(URL(url))
                if (ks?.viewMode == ViewMode.WebView || ks == null) {
                    field = url
                }
            }
        }

    init {
        val schemeHandlers = SchemeHandlers()
        schemeHandlers.registerHandler(MailToSchemeHandler(activity))
        schemeHandlers.registerHandler(TelSchemeHandler(activity))

        val webInterceptor = WebClientInterceptor(uiInteractor, this,
                activity, knownServices, schemeHandlers, loggingService, connectionStateMonitor)

        val addToCalendarHandler = AddToCalendarHandler(activity, loggingService)

        val webAppInterfacePrivate = WebAppInterfacePrivate(activity, this, uiInteractor, settingsService, addToCalendarHandler)
        val webAppInterfaceThirdParty = WebAppInterfaceThirdParty(activity, this, addToCalendarHandler)
        val webAppInterfaceNhsLogin = WebAppInterfaceNhsLogin(activity, this)
        webView.addJavascriptInterface(webAppInterfacePrivate, NATIVE_APP_PRIVATE)
        webView.addJavascriptInterface(webAppInterfaceThirdParty, NATIVE_APP_THIRDPARTY)
        webView.addJavascriptInterface(webAppInterfaceNhsLogin, NATIVE_APP_LOGIN)

        webView.webViewClient = webInterceptor
        webView.webChromeClient = chromeClient
        clearSessionCookies()

        trimCachedFiles(buildCachedFiles())
    }

    fun loadWelcomePage() = loadUrl(readResourceString(R.string.baseURL))

    fun loadUrl(path: String) {
        Log.d(TAG, "Entering loadUrl")

        val hasFidoLoginError = path.contains(activity.resources.getString(R.string.authRedirectPath)) &&
                uiInteractor.canDisplayBiometricLogin() &&
                path.contains(activity.resources.getString(R.string.redirectErrorQueryParam))
        if (hasFidoLoginError) {
            Log.d(TAG, "Fido login error response url: $path")
            uiInteractor.displayBiometricLoginErrorOccurrence()
            loadWelcomePage()
            return
        }

        if (!connectionStateMonitor.isConnectedToNetwork) {
            showNoConnectionError()
            return
        }

        val url: String = if (shouldIgnorePersistedLink(path)) {
            urlLoader.produceValidUrl(path)
        } else {
            getPersistedLink()
        }
        appPersistData.clearPersistedLink()

        reloadUrl = url

        val loginUrl = readResourceString(R.string.baseURL) + readResourceString(R.string.loginPath)
        if (path.startsWith(loginUrl)) {
            requiresFullPageLoad = true
            isUserLoggedIn = false
        }
        urlLoader.loadUrl(url, requiresFullPageLoad)
    }

    private fun shouldIgnorePersistedLink(path: String): Boolean {
        return path.contains(fidoAuthQueryKey) ||
                path.contains(authRedirectPath) ||
                path.contains(nhs111Host) ||
                appPersistData.getPersistedLink().isNullOrBlank()
    }

    private fun getPersistedLink(): String {
        val url = appPersistData.getPersistedLink().toString()
        if (URLUtil.isValidUrl(url)) {
            return url
        }
        return readResourceString(R.string.baseURL)
    }

    fun setReloadPath(path: String) {
        if (path.isBlank()) {
            return
        }

        reloadUrl = readResourceString(R.string.baseURL) + path.removePrefix("/")
    }

    fun reloadLoginUrl() {
        val loginUrl =
                readResourceString(R.string.baseURL) + readResourceString(R.string.loginPath)
        val currentUrl = webView.url ?: ""

        val isFidoLoginUrl = currentUrl.contains(loginUrl) &&
                currentUrl.contains(readResourceString(R.string.fidoAuthQueryKey))

        if (currentUrl.isEmpty() || isFidoLoginUrl) {
            loadUrl(loginUrl)
        } else if (webView.url.toLowerCase(Locale.ROOT).startsWith(loginUrl.toLowerCase(Locale.ROOT))) {
            webView.reload()
        }
    }

    fun onSlimHeaderBackButtonPressed(): Boolean {
        when {
            shouldReloadHomepageOnBackReturn(reloadUrl) -> {
                reloadHomepageOnBackReturn()
            }
            webView.canGoBack() -> webView.goBack()
            else -> activity.onBackPressed()
        }
        return true
    }

    fun onbackButtonPressedOnLoggedOutUnsecurePage() {
        when {
            webView.canGoBack() -> webView.goBack()
            else -> reloadHomepageOnBackReturn()
        }
    }

    fun onWebLoggedIn() {
        Log.d(TAG, "Entering loggedIn")
        if (isUserLoggedIn) return
        requiresFullPageLoad = false
        isUserLoggedIn = true

        uiInteractor.showMenuBar()
        uiInteractor.showHeader()
        uiInteractor.clearMenuBarItem()
    }

    fun onWebLoggedOut() {
        uiInteractor.showWebviewScreen()
        Log.d(TAG, "Entering loggedOut")
        requiresFullPageLoad = true
        isUserLoggedIn = false


        webView.settings.builtInZoomControls = false

        uiInteractor.dismissSessionExtensionDialog()

        trimCachedFiles(buildCachedFiles())
    }

    private fun buildCachedFiles(): Array<File> {
        return when (appWebViewDir.listFiles()) {
            null -> arrayOf(cacheDir)
            else -> appWebViewDir.listFiles().plus(cacheDir)
        }
    }

    private fun trimCachedFiles(contentsToDelete: Array<File>) {

        contentsToDelete.forEach { file ->
            try {
                if (file.isDirectory) {
                    trimCachedFiles(file.listFiles())
                } else if (!file.name.toLowerCase().contains("cookie")) {
                    file.delete()
                }
            } catch (e: Exception) {
                Log.w(TAG, "Error deleting  ${file.toString()}", e)
            }
        }
    }

    fun onBiometricOptionChanged() {
        val cookies: String? = CookieManager.getInstance()
                .getCookie(activity.resources.getString(R.string.cookieDomain))
                ?.takeIf { it.contains("HideBiometricBanner=") }
        if (cookies.isNullOrBlank()) {
            CookieManager.getInstance().setCookie(readResourceString(R.string.cookieDomain),
                    "HideBiometricBanner=true; max-age=${60 * 60 * 24 * 365 * 5}")
        }
    }

    fun handleWebClientLocationResult(grantResults: IntArray) {
        chromeClient.handleLocationPermissionResult(grantResults)
    }

    fun handleCameraFilePermissionResult(grantResults: IntArray) {
        chromeClient.handleCameraFilePermissionResult(grantResults)
    }

    fun loadUrlInChromeTab(url: String): Boolean {
        if (knownServices.findMatchingKnownService(URL(url))?.viewMode == ViewMode.WebView) {
            Log.d(TAG, "Should not open url: $url in chrome tab")
            return false
        }

        uiInteractor.openUrlInBrowserActivity(url)
        return true
    }

    fun reloadCurrentUrl() {
        urlHelper.getPostRequestReloadUrl(reloadUrl.orEmpty())?.let { urlLoader.reloadRequest(it) }
                ?: urlLoader.reloadRequest(reloadUrl)
    }

    fun getFileUploadCallback() = chromeClient.getFileUploadCallback()

    fun resetFileUploadCallback() = chromeClient.resetFileUploadCallback()

    fun getUploadedFileLocation() = chromeClient.getUploadedFileLocation()

    fun announceForAccessibility(text: String) = webView.announceForAccessibility(text)

    fun showNoConnectionError() {
        Log.d(TAG, "Entering ShowNoConnectionError")
        webView.stopLoading()
        uiInteractor.showUnavailabilityError(errorMessageHandler.getErrorMessage(ErrorType.NoConnection))
        uiInteractor.dismissSplashScreen()
        uiInteractor.dismissProgressDialog()
        uiInteractor.dismissBiometricDialog()
        return
    }

    private fun clearSessionCookies() {
        val host = readResourceString(R.string.baseHost)
        val allHosts = host.split('.')
                .foldRight(listOf<String>()) { e, accumulator -> accumulator + if (accumulator.any()) "$e.${accumulator.last()}" else e }
        for (String in allHosts) {
            clearCookie("nhso.session", "https://$String")
            clearCookie("NHSO-Session-Id", "https://.$String")
            clearCookie("nhso.terms", "https://$String")
            clearCookie("nhso.auth", "https://$String")
        }
        CookieManager.getInstance().flush()
    }

    private fun readResourceString(resourceId: Int): String {
        return activity.getString(resourceId)
    }

    fun shouldReloadHomepageOnBackReturn(urlString: String?): Boolean {
        val sitesFromResource: Array<String>? =
                activity.resources.getStringArray(R.array.nativeReloadOnBackUrls)

        if (urlString == null || sitesFromResource == null) return false

        return try {
            sitesFromResource.any { urlString.contains(URL(it).host) }
        } catch (exception: MalformedURLException) {
            false
        }
    }

    fun setHelpLocation(url: String? = readResourceString(R.string.helpURL)) {
        appPersistData.storeHelpUrl(url.toString())
    }

    fun getHelpLocation(): String {
        return appPersistData.getHelpUrl().toString()
    }

    fun stopLoading() {
        webView.stopLoading()
    }

    fun reloadHomepageOnBackReturn() {
        loadWelcomePage()
        showBlankScreen()
    }

    fun showBlankScreen() {
        uiInteractor.showBlankScreen()
    }

    fun requestPnsToken(trigger: String) {
        notificationsService.registerForPushNotifications(trigger)
    }

    private fun clearCookie(cookieName: String, domain: String) {
        CookieManager.getInstance()
                .setCookie(domain, "$cookieName=; Expires=Sat, 1 Jan 2000 00:00:01 UTC;")
    }

    fun getNotificationsStatus() {
        notificationsService.getNotificationsStatus()
    }

    fun goToPage(page: String) {
        val pageUrl = urlHelper.createRedirectToPageUrl(page)
        urlLoader.loadUrl(url = pageUrl.toString(), requiresFullPageLoad = true)
    }

    val onFailure: (PaycassoCallbackResponse) -> Unit = {
            response ->
        run {
            val responseString = Gson().toJson(response)

            webView.evaluateJavascript(
                "window.authentication.paycassoOnFailure(${responseString})", null)
        }
    }

    private val onSuccess: (PaycassoCallbackResponse) -> Unit = {
            response ->
        run {
            val responseString = Gson().toJson(response)

            webView.evaluateJavascript(
                "window.authentication.paycassoOnSuccess(${responseString})", null)
        }
    }


    fun startPaycasso(paycassoData: PaycassoData) {
        paycassoService.start(paycassoData,
            onSuccess,
            onFailure)
    }
}
