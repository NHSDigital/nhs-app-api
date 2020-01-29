package com.nhs.online.nhsonline.web

import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.util.Log
import android.webkit.CookieManager
import android.webkit.WebView
import com.nhs.online.nhsonline.services.NotificationsService
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.services.SettingsService
import com.nhs.online.nhsonline.services.UrlLoader
import com.nhs.online.nhsonline.support.ApplicationState
import com.nhs.online.nhsonline.support.schemehandlers.SchemeHandlers
import com.nhs.online.nhsonline.support.PersistData
import com.nhs.online.nhsonline.support.schemehandlers.MailToSchemeHandler
import com.nhs.online.nhsonline.webclients.ChromeClientLocationHandler
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.webinterfaces.WebAppInterface
import java.net.MalformedURLException
import java.net.URL


private val TAG = NhsWeb::class.java.simpleName
private const val NATIVE_APP = "nativeApp"

class NhsWeb(
        private val activity: Activity,
        private val uiInteractor: IInteractor,
        private val webView: WebView,
        private val notificationsService: NotificationsService,
        appWebInterface: AppWebInterface
) {
    private val knownServices = KnownServices(activity)
    private val openBrowserActivity =
            OpenUrlInBrowserActivity(activity.resources.getStringArray(R.array.nativeAppHosts))
    private val urlLoader =
            UrlLoader(webView, activity.getString(R.string.baseURL), appWebInterface)
    private val chromeClient = ChromeClientLocationHandler(activity)
    private val appPersistData = PersistData(activity)
    private val errorMessageHandler = ErrorMessageHandler(activity)
    private val settingsService = SettingsService(activity)

    var applicationState =
            ApplicationState(readResourceString(R.string.menuTimeoutSeconds).toLong())
    var isUserLoggedIn = false
    var requiresFullPageLoad = true
    var reloadUrl: String? = null
        set(value) {
            if (value != null && !knownServices.shouldURLOpenExternally(URL(value))) {
                field = value
            }
        }

    init {
        val schemeHandlers = SchemeHandlers()
        schemeHandlers.registerHandler(MailToSchemeHandler(activity))

        val webInterceptor =
            WebClientInterceptor(uiInteractor, this, activity, knownServices, schemeHandlers)
        webView.webViewClient = webInterceptor

        val webInterface = WebAppInterface(activity, uiInteractor, this, settingsService)
        webView.addJavascriptInterface(webInterface, NATIVE_APP)

        webView.webChromeClient = chromeClient
        clearSessionCookies()
    }

    fun loadWelcomePage() = loadUrl(readResourceString(R.string.baseURL))

    fun loadUrl(path: String) {
        Log.d(TAG, "Entering loadUrl")

        val hasFidoLoginError =
                path.contains(activity.resources.getString(R.string.authRedirectPath)) &&
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
        val url = urlLoader.produceValidUrl(path)
        reloadUrl = url

        val loginUrl = readResourceString(R.string.baseURL) + readResourceString(R.string.loginPath)
        if (path.startsWith(loginUrl)) {
            requiresFullPageLoad = true
            isUserLoggedIn = false
        }
        urlLoader.loadUrl(url, requiresFullPageLoad)
    }

    fun loadTelephoneUrl(url: String) {
        val intent = Intent(Intent.ACTION_DIAL)
        intent.setData(Uri.parse(url))
        activity.startActivity(intent)
    }

    fun setReloadPath(path: String) {
        if(path.isNullOrBlank()){
            return
        }

        reloadUrl = readResourceString(R.string.baseURL) + path.removePrefix("/")
    }

    fun isCheckSymptomsUnsecureURL(url: String?): Boolean {
        if (url == null) {
            return false
        }
        val unsecuredKnownServiceInfo = knownServices.findMatchingServiceInfo(url)
        unsecuredKnownServiceInfo?.header?.let { nativeHeader ->
            return when (nativeHeader) {
                activity.getString(R.string.nhs_111_header),
                activity.getString(R.string.conditions_header) -> true
                else -> false
            }
        }
        return false
    }

    fun isLoginPath(): Boolean{
        if(webView.url == null){
            return false
        }
        return (webView.url.contains(readResourceString(R.string.loginPath)))
    }

    fun reloadLoginUrl() {
        val loginUrl =
                readResourceString(R.string.baseURL) + readResourceString(R.string.loginPath)
        val currentUrl = webView.url ?: ""

        val isFidoLoginUrl = currentUrl.contains(loginUrl) &&
                currentUrl.contains(readResourceString(R.string.fidoAuthQueryKey))

        if (currentUrl.isEmpty() || isFidoLoginUrl) {
            loadUrl(loginUrl)
        } else if (webView.url.toLowerCase().startsWith(loginUrl.toLowerCase())) {
            webView.reload()
        }
    }

    fun onSlimHeaderBackButtonPressed(): Boolean {
        when {
            shouldReloadHomepageOnBackReturn(reloadUrl) -> {
                reloadHomepageOnBackReturn()
            }
            webView.canGoBack() -> webView.goBack()
            isCheckSymptomsUnsecureURL(reloadUrl) ->
                loadUrl(readResourceString(R.string.checkYourSymptoms))
            else -> activity.onBackPressed()
        }
        return true
    }

    fun onbackButtonPressedOnCheckSymptomsUnsecurePage() {
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
    }

    fun onBetaCookieStoreRequest() {
        val cookies: String? = CookieManager.getInstance()
                .getCookie(activity.resources.getString(R.string.cookieDomain))
                ?.takeIf { it.contains("BetaCookie=") }
        if (cookies != null) {
            val betaCookie = cookies.split("; ").first { it.startsWith("BetaCookie=") }
            appPersistData.storeBetaCookie(betaCookie)
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

    fun loadUrlInChromeTab(urlString: String) {
        openBrowserActivity.start(activity, urlString, uiInteractor)
    }

    fun openInChromeTabIfApplicable(url: String): Boolean {
        if (openBrowserActivity.canStart(activity, url)) {
            loadUrlInChromeTab(url)
            return true
        }
        return false
    }

    fun reloadCurrentUrl() {
        knownServices.getPostRequestReloadUrl(reloadUrl.orEmpty())?.let { urlLoader.reloadRequest(it) }
                ?: urlLoader.reloadRequest(reloadUrl)
    }

    fun getFileUploadCallback() = chromeClient.getFileUploadCallback()

    fun resetFileUploadCallback() = chromeClient.resetFileUploadCallback()

    fun getUploadedFileLocation() = chromeClient.getUploadedFileLocation()

    fun announceForAccessibility(text: String) = webView.announceForAccessibility(text)

    fun showNoConnectionError() {
        Log.d(TAG, "Entering ShowNoConnectionError")
        webView.stopLoading()
        uiInteractor.setHeaderText(readResourceString(R.string.connection_error_header))
        uiInteractor.showUnavailabilityError(errorMessageHandler.getErrorMessage(ErrorType.NoConnection))
        return
    }

    fun clearSessionCookies() {
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

    fun readResourceString(resourceId: Int): String {
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
}