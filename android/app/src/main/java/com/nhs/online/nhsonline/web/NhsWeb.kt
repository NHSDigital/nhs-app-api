package com.nhs.online.nhsonline.web

import android.app.Activity
import android.util.Log
import android.webkit.CookieManager
import android.webkit.WebView
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.OpenUrlInBrowserActivity
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.Reachability
import com.nhs.online.nhsonline.services.KnownService
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.services.UrlLoader
import com.nhs.online.nhsonline.support.PersistData
import com.nhs.online.nhsonline.webclients.ChromeClientLocationHandler
import com.nhs.online.nhsonline.webclients.WebClientInterceptor
import com.nhs.online.nhsonline.webinterfaces.WebAppInterface
import java.net.URL

private val TAG = NhsWeb::class.java.simpleName
private const val NATIVE_APP = "nativeApp"

class NhsWeb(
    private val activity: Activity,
    private val uiInteractor: IInteractor,
    private val webView: WebView
) {
    private val knownServices = KnownServices(activity)
    private val openBrowserActivity =
        OpenUrlInBrowserActivity(activity.resources.getStringArray(R.array.nativeAppHosts))
    private val urlLoader =
        UrlLoader(webView, knownServices, activity.getString(R.string.baseURL))
    private val chromeClient = ChromeClientLocationHandler(activity)
    private val appPersistData = PersistData(activity)
    private var originalWebViewZoom = 0

    var isUserLoggedIn = false
    var useAbsoluteUri
        get() = urlLoader.usingAbsoluteUri
        set(value) {
            urlLoader.usingAbsoluteUri = value
        }

    init {
        val webInterceptor =
            WebClientInterceptor(uiInteractor, this, activity, knownServices)
        webView.webViewClient = webInterceptor

        val webInterface = WebAppInterface(activity, uiInteractor, this)
        webView.addJavascriptInterface(webInterface, NATIVE_APP)

        webView.webChromeClient = chromeClient
        setupBetaCookie()
    }

    fun loadWelcomePage() = loadUrl(readResourceString(R.string.baseURL))

    fun loadUrl(path: String) {
        var knownService = knownServices.findNHSAppInternalServiceInfoByPath(path)
        if (knownService == null) {
            knownService = knownServices.findMatchingServiceInfo(path)
        }
        if (!Reachability.isConnectedToNetwork(activity)) {
            handleConnectionError(path, knownService)
            return
        }
        knownService?.header?.let { nativeHeader ->
            uiInteractor.setHeaderText(nativeHeader)
        }
        uiInteractor.setWebViewVisible()
        urlLoader.loadUrl(path)
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

    fun loadThrottlingCarousel() {
        originalWebViewZoom = webView.settings.textZoom
        webView.settings.textZoom = 100
        webView.loadUrl(readResourceString(R.string.throttleCarouselPath))
    }

    fun onThrottlingCarouselComplete() {
        webView.settings.textZoom = originalWebViewZoom
        appPersistData.storeBoolean(activity.getString(R.string.haveShownThrottlingCarouselBefore),
            true)
        loadWelcomePage()
    }

    fun onWebLoggedIn() {
        Log.d(TAG, "Entering loggedIn")
        if (isUserLoggedIn) return
        useAbsoluteUri = false
        isUserLoggedIn = true

        uiInteractor.showMenuBar()
        uiInteractor.showHeader()
    }

    fun onWebLoggedOut() {
        uiInteractor.showWebviewScreen()
        Log.d(TAG, "Entering loggedOut")
        useAbsoluteUri = true
        isUserLoggedIn = false

        uiInteractor.dismissSessionExtensionDialog()
        uiInteractor.showBiometricLoginIfEnabled()
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

    fun handleWebClientLocationResult(grantResults: IntArray) {
        chromeClient.handleLocationPersionResult(grantResults)
    }

    fun setReloadUrl(url: String?) {
        if (!knownServices.shouldURLOpenExternally(URL(url))) {
            urlLoader.reloadUrl = url
        }
    }

    fun loadUrlInChromeTab(urlString: String) {
        openBrowserActivity.start(activity, urlString)
    }

    fun openInChromeTabIfApplicable(url: String): Boolean {
        if (openBrowserActivity.canStart(activity, url)) {
            loadUrlInChromeTab(url)
            return true
        }
        return false
    }

    fun reloadCurrentUrl() = urlLoader.reloadRequest()

    fun announceForAccessibility(text: String) = webView.announceForAccessibility(text)

    private fun handleConnectionError(path: String, knownService: KnownService.Info?) {
        urlLoader.reloadUrl = urlLoader.produceValidUrl(path)
        val errorMessage =
            knownService?.errorMessage ?: knownServices.getServiceUnavailabilityError()
        webView.stopLoading()
        uiInteractor.setHeaderText(readResourceString(R.string.connection_error_header))
        uiInteractor.showUnavailabilityError(errorMessage)

        Log.d(TAG, "Failing Url: ${urlLoader.reloadUrl}")
        return
    }

    private fun setupBetaCookie() {
        CookieManager.getInstance().removeAllCookies(null)

        val persistedBetaCookie = appPersistData.getBetaCookie()
        if (!persistedBetaCookie.isNullOrBlank()) {
            CookieManager.getInstance().setCookie(readResourceString(R.string.cookieDomain),
                "$persistedBetaCookie; max-age=${60 * 60 * 24 * 365}")
        }
    }

    private fun readResourceString(resourceId: Int): String {
        return activity.getString(resourceId)
    }
}