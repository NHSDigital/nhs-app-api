package com.nhs.online.nhsonline.web

import android.app.Activity
import android.util.Log
import android.webkit.CookieManager
import android.webkit.URLUtil
import android.webkit.WebView
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
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
import com.nhs.online.nhsonline.utils.UrlHelper
import com.nhs.online.nhsonline.webclients.ChromeClientLocationHandler
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import java.net.MalformedURLException
import java.net.URL
import java.util.Locale
import java.io.File

private val TAG = NhsWeb::class.java.simpleName
private const val NATIVE_APP_PRIVATE = "nativeApp"
private const val NATIVE_APP_THIRDPARTY = "nhsappNative"


class NhsWeb(
        private val activity: Activity,
        private val uiInteractor: IInteractor,
        private val webView: WebView,
        private val notificationsService: NotificationsService,
        appWebInterface: AppWebInterface,
        private val knownServices: KnownServices,
        private val nhsLoginLoggedInPaths: List<String>
) {
    private val openBrowserActivity = OpenUrlInBrowserActivity()
    private val urlLoader = UrlLoader(webView, activity.getString(R.string.baseURL), appWebInterface)
    private val urlHelper = UrlHelper(activity)
    private val chromeClient = ChromeClientLocationHandler(activity)
    private val appPersistData = PersistData(activity)
    private val errorMessageHandler = ErrorMessageHandler(activity.resources)
    private val settingsService = SettingsService(activity)
    private val cacheDir = File(activity.filesDir.parent + "/cache")
    private val appWebViewDir = File(activity.filesDir.parent + "/app_webview")

    var applicationState =
            ApplicationState(readResourceString(R.string.menuTimeoutSeconds).toLong())
    var isUserLoggedIn = false
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

        val webInterceptor =
                WebClientInterceptor(uiInteractor, this, activity, knownServices, schemeHandlers, nhsLoginLoggedInPaths)

        val addToCalendarHandler = AddToCalendarHandler(activity)

        val webAppInterfacePrivate = WebAppInterfacePrivate(activity, this, uiInteractor, settingsService)
        val webAppInterfaceThirdParty = WebAppInterfaceThirdParty(activity, this, addToCalendarHandler)
        webView.addJavascriptInterface(webAppInterfacePrivate, NATIVE_APP_PRIVATE)
        webView.addJavascriptInterface(webAppInterfaceThirdParty, NATIVE_APP_THIRDPARTY)

        webView.webViewClient = webInterceptor
        webView.webChromeClient = chromeClient
        clearSessionCookies()
        trimCachedFiles()
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

        if (!isConnectedToNetwork) {
            showNoConnectionError()
            return
        }

        val url = if (!appPersistData.getPersistedLink().isNullOrBlank()) {
            loadPersistedLink()!!
        } else {
            urlLoader.produceValidUrl(path)
        }

        reloadUrl = url

        val loginUrl = readResourceString(R.string.baseURL) + readResourceString(R.string.loginPath)
        if (path.startsWith(loginUrl)) {
            requiresFullPageLoad = true
            isUserLoggedIn = false
        }
        urlLoader.loadUrl(url, requiresFullPageLoad)
    }

    private fun loadPersistedLink(): String? {
        val url = appPersistData.getPersistedLink().toString()
        if (!url.isNullOrBlank() && URLUtil.isValidUrl(url)) {
            appPersistData.storePersistedLink("")
            return url
        }
        return null
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

        trimCachedFiles()
    }

    private fun trimCachedFiles() {
        val contentsToDelete =
                when (appWebViewDir.listFiles()) {
                    null -> arrayOf(cacheDir)
                    else -> appWebViewDir.listFiles().plus(cacheDir)
                }

        contentsToDelete.forEach { file ->
            try {
                if (file.isDirectory) {
                    file.deleteRecursively()
                } else {
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

        openBrowserActivity.start(activity, url, uiInteractor)
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
}