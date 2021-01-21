package com.nhs.online.nhsonline.webclients

import android.content.Context
import android.content.res.Resources
import android.graphics.Bitmap
import android.net.http.SslError
import android.os.Build
import android.os.Handler
import android.util.Log
import android.view.accessibility.AccessibilityEvent
import android.webkit.*
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.services.logging.ILoggingService
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.support.schemehandlers.SchemeHandlers
import com.nhs.online.nhsonline.utils.UrlHelper
import com.nhs.online.nhsonline.web.NhsWeb
import java.io.InputStream
import java.net.MalformedURLException
import java.net.URL

private const val WOFF2 = "woff2"
private val TAG = WebClientInterceptor::class.java.simpleName

class WebClientInterceptor(
        private val resources: Resources,
        private val uiInteractor: IInteractor,
        private val nhsWeb: NhsWeb,
        private val context: Context,
        private val knownServices: KnownServices,
        private val schemeHandlers: SchemeHandlers,
        private val loggingService: ILoggingService,
        private val connectionStateMonitor: ConnectionStateMonitor
) : WebViewClient() {

    private val errorMessageHandler = ErrorMessageHandler(context.resources)
    private val handler = Handler()
    private var noConnectionHandled = false
    private var shouldShowErrorPage = false
    private val urlHelper = UrlHelper(context)
    private val requestTimeout = context.resources
            .getInteger(R.integer.webClientRequestTimeoutMillis)
            .toLong()

    @Suppress("OverridingDeprecatedMember")
    override fun shouldOverrideUrlLoading(view: WebView, url: String): Boolean {
        Log.d(TAG, "Entering shouldOverrideUrlLoading > url $url")

        // Scheme handlers need to be executed first in order for the app to safely handle mailto: and tel: links
        if (schemeHandlers.handleUrl(url)) {
            return true
        }

        if (urlHasAppScheme(url)) {
            val sanitizedUrl = ensureSupportedScheme(url)
            Log.d(TAG, "Overriding url $url to $sanitizedUrl")

            uiInteractor.hideHeaderSlim()
            view.stopLoading()
            nhsWeb.requiresFullPageLoad = true
            sanitizedUrl?.let { uiInteractor.loadPage(it) }
            return true
        }

        showKnownServicesSpinner(view.url, url)

        try {
            if (URL(url).host?.equals(URL(context.getString(R.string.dataPreferencesBaseUrl)).host)!!) {
                view.loadUrl(url)
                return true
            }
        } catch (exception: MalformedURLException) {
            return false
        }

        val openedInChromeTab = nhsWeb.loadUrlInChromeTab(url)
        if (openedInChromeTab)
            return true

        return false
    }

    override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {
        Log.d(TAG, "Entering onPageStarted > url $url")

        val sanitizedUrl = when {
            urlHasAppScheme(url) -> {
                uiInteractor.hideHeaderSlim()
                ensureSupportedScheme(url)
            }
            else -> url
        }

        Log.d(TAG, "Sanitized url $sanitizedUrl")

        nhsWeb.reloadUrl = sanitizedUrl
        cancelTrackingWebRequestResponse()

        if (!connectionStateMonitor.isConnectedToNetwork) {
            Log.d(TAG, "Entering onPageStarted > no internet")

            stopLoadingWebviewAndShowNoConnectionError(view)
            noConnectionHandled = true
            return
        }

        var knownService = knownServices.findMatchingKnownService(URL(sanitizedUrl))
        if (knownService != null) {
            updateNavMenu(knownService.menuTab)
            nhsWeb.javaScriptInteractionMode = knownService.javaScriptInteractionMode
        }

        if (shouldHandleUnavailability(sanitizedUrl)) {
            trackWebRequestResponse(view, sanitizedUrl)
        }

        shouldShowErrorPage = false

        super.onPageStarted(view, sanitizedUrl, favicon)
    }

    override fun onLoadResource(view: WebView?, url: String?) {
        Log.d(TAG, "Entering onLoadResource > url $url")

        if (!connectionStateMonitor.isConnectedToNetwork) {
            Log.d(TAG, "Entering onLoadResource > isConnectedToInternet")
            if (!noConnectionHandled) {
                Log.d(TAG, "Entering onLoadResource > isConnectedToInternet > noConnectionHandled false")
                cancelTrackingWebRequestResponse()
            }
            return
        }
        noConnectionHandled = false

        super.onLoadResource(view, url)
    }

    override fun doUpdateVisitedHistory(view: WebView?, url: String?, isReload: Boolean) {
        if (!shouldShowErrorPage) {
            var knownService = knownServices.findMatchingKnownService(URL(url))
            knownService?.let {
                nhsWeb.headerStrategy.apply(knownService.integrationLevel)
            }

        }

        super.doUpdateVisitedHistory(view, url, isReload)
    }

    override fun onReceivedSslError(view: WebView?, handler: SslErrorHandler?, error: SslError?) {
        Log.d(TAG, "Entering onReceivedSslError")

        if (canHandleUnavailability(view)) {
            handleUnavailability(view?.url, ERROR_CONNECT)
        }
    }

    override fun onReceivedHttpError(view: WebView?, request: WebResourceRequest?, errorResponse: WebResourceResponse?) {
        Log.d(TAG, "Entering onReceivedHttpError")
        loggingService.logError("Failed HTTP Call from webview. url:${request?.url} httpResponseCode:${errorResponse?.statusCode}")

        if (canHandleUnavailability(view) && !isNHSApi(request)) {
            cancelTrackingWebRequestResponse()
            handleUnavailability(view?.url, ERROR_CONNECT)
        }
    }

    override fun onReceivedError(view: WebView?, request: WebResourceRequest?, error: WebResourceError?) {
        Log.d(TAG, "Entering onReceivedError")

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            loggingService.logError("Failed HTTP Call from webview. url:${request?.url} AndroidError:${error?.errorCode}")
        }

        if (isNhsDigitalAnalyticsHost(request?.url?.host)) {
            if (!connectionStateMonitor.isConnectedToNetwork) {
                stopLoadingWebviewAndShowNoConnectionError(view)
            }
            return
        }

        if ((isNHSAppDomain(request?.url?.host)) &&
            canHandleUnavailability(view) &&
            shouldHandleUnavailability(view?.url)) {

            cancelTrackingWebRequestResponse()
            if (error != null) {
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                    handleUnavailability(view?.url, error.errorCode)
                }
            }
        } else {
            Log.d(TAG, "In onReceivedError > Skipping unavailability handling > failed url ${request?.url}")
        }
    }

    @Suppress("OverridingDeprecatedMember")
    override fun onReceivedError(
            view: WebView?,
            errorCode: Int,
            description: String?,
            failingUrl: String?
    ) {
        loggingService.logError("Failed HTTP Call from webview. url:${failingUrl} AndroidError:${errorCode}")

        if (shouldHandleUnavailability(failingUrl)) {
            Log.d(TAG, "Entering onReceivedError > failingUrl $failingUrl > Error Description: $description")
            cancelTrackingWebRequestResponse()
            if (canHandleUnavailability(view)) {
                handleUnavailability(failingUrl, errorCode)
            }
        }
    }

    override fun shouldInterceptRequest(
            view: WebView?,
            request: WebResourceRequest?
    ): WebResourceResponse? {
        if (request == null) {
            return null
        }

        val requestUrl = request.url.toString()

        Log.d(TAG, "Request to Load $requestUrl")

        if (requestUrl.endsWith(WOFF2, ignoreCase = true)) {
            val fonts = context.resources.getStringArray(R.array.fonts)

            val fontInRequest = fonts.find { requestUrl.contains(it, ignoreCase = true) }

            fontInRequest?.let {
                Log.d(TAG, "Loading local font for: $fontInRequest")
                return loadBundledFont(fontInRequest)
            }
        }

        return super.shouldInterceptRequest(view, request)
    }

    private fun loadBundledFont(font: String): WebResourceResponse {
        val woff2MimeType = "application/font-woff2"

        val headers = mutableMapOf<String, String>()
        headers["Access-Control-Allow-Origin"] = "*"

        val inputStream: InputStream = context.assets.open("fonts/$font")
        return WebResourceResponse(woff2MimeType, WOFF2, 200, "OK", headers, inputStream)
    }

    override fun onPageFinished(view: WebView?, url: String?) {
        Log.d(TAG, "Entering onPageFinished > url $url")

        if (url.isNullOrBlank()) {
            Log.d(TAG, "Exiting onPageFinished > url was null or blank")
            super.onPageFinished(view, url)

            return
        }

        val urlString = url!!

        if (shouldHandleUnavailability(urlString)) {
            cancelTrackingWebRequestResponse()
        }

        if (urlString.contains(resources.getString(R.string.authRedirectPath))) {
            view?.sendAccessibilityEvent(AccessibilityEvent.TYPE_VIEW_ACCESSIBILITY_FOCUSED)
        }

        view?.isFocusable = true
        view?.requestFocus()
        if (!shouldShowErrorPage) {
            uiInteractor.showWebviewScreen()
        }

        super.onPageFinished(view, urlString)
        hideKnownServicesSpinner(urlString)

        // TODO: Needs a better way of handling dismissal of the progress dialog
        //       Currently leaves the app too tied to a specific path from Login
        //       but is needed to dismiss the progress dialog without a delay before the page transition to login
        if (!urlString.contains(context.resources.getString(R.string.fido_auth_response)) &&
                !(urlString.contains(URL(context.resources.getString(R.string.nhsLoginSuffix)).host) &&
                        urlString.contains(context.resources.getString(R.string.login_auth_code_path)))) {
            uiInteractor.dismissProgressDialog()
        }
    }

    override fun onPageCommitVisible(view: WebView?, url: String?) {
        Log.d(TAG, "Entering onPageCommitVisible and url: $url")

        if (!shouldShowErrorPage) {
            if (!urlHelper.isSameHostAndSchemeAsHomeUrl(url)) {
                uiInteractor.announcePageTitle(view?.title)
                uiInteractor.dismissBiometricDialog()
            }
        }
        uiInteractor.dismissSplashScreen()
        super.onPageCommitVisible(view, url)
    }

    private fun shouldHandleUnavailability(urlString: String?): Boolean {
        if (urlString == null) {
            Log.d(TAG, "Entering shouldHandleUnavailability > url $urlString > false")
            return false
        }
        val matchingKnownService =
                knownServices.findMatchingKnownService(URL(urlString))

        Log.d(TAG, "Entering shouldHandleUnavailability > url $urlString > ${matchingKnownService != null}")

        return matchingKnownService != null
    }

    private fun canHandleUnavailability(view: WebView?): Boolean {
        return view?.url === null || isNHSAppPage(view)
    }

    private fun isNHSAppDomain(value: String?): Boolean {
        return value == context.getString(R.string.baseHost)
    }

    private fun isNhsDigitalAnalyticsHost(value: String?): Boolean {
        return value == context.getString(R.string.nhsDigitalAnalyticsHost)
    }

    private fun isNHSAppPage(view: WebView?): Boolean {
        val url = URL(view?.url)
        return isNHSAppDomain(url.host)
    }

    private fun isNHSApi(request: WebResourceRequest?): Boolean {
        return request?.url?.host == URL(context.getString(R.string.baseApiURL)).host
    }

    fun stopLoadingWebviewAndShowNoConnectionError(view: WebView?) {
        Log.d(TAG, "Entering stopLoadingWebviewAndShowNoConnectionError > ${view?.url}")
        handleUnavailability(view?.url)
        view?.stopLoading()
    }

    private fun updateNavMenu(menuTab: MenuTab) {
        uiInteractor.selectNavigationMenuActive(menuTab.tabIndex)
    }

    private fun trackWebRequestResponse(view: WebView?, url: String?) {
        Log.d(TAG, "Entering trackWebRequestResponse")

        val expireRequestFn = {
            view?.stopLoading()
            uiInteractor.dismissProgressDialog()
            Log.d(TAG, "Entering trackWebRequestResponse > expireRequestFn")
            handleUnavailability(url)
        }

        handler.postDelayed(expireRequestFn, requestTimeout)
    }

    private fun cancelTrackingWebRequestResponse() {
        Log.d(TAG, "Entering cancelTrackingWebRequestResponse")
        handler.removeCallbacksAndMessages(null)
    }

    private fun handleUnavailability(failingUrl: String?, errorCode: Int? = null) {
        Log.d(TAG, "Entering handleUnavailability > failingUrl $failingUrl")

        shouldShowErrorPage = true
        val errorMessage = when (connectionStateMonitor.isConnectedToNetwork) {
            true -> {
                errorMessageHandler.getErrorMessage(ErrorType.ServiceUnavailable)
            }
            false -> {
                errorMessageHandler.getErrorMessage(ErrorType.NoConnection)
            }
        }

        Log.d(TAG, "HandleUnavailability -> ErrorMessage Type:  ${errorMessage.title}")
        uiInteractor.showUnavailabilityError(errorMessage)
        uiInteractor.dismissBiometricDialog()
        uiInteractor.dismissProgressDialog()

        Log.i(TAG, "Failing Url: $failingUrl with error code: $errorCode")
    }

    private fun ensureSupportedScheme(url: String?): String? {
        val appScheme = context.getString(R.string.appScheme)
        val baseScheme = context.getString(R.string.baseScheme)
        val appSchemePattern = "^$appScheme://".toRegex()

        return url?.replace(appSchemePattern, "$baseScheme://")
    }

    private fun urlHasAppScheme(url: String?): Boolean {
        val appScheme = context.getString(R.string.appScheme)

        return url?.startsWith("$appScheme://") ?: false
    }

    private fun showKnownServicesSpinner(currentUrl: String, newUrl: String) {
        if (!URL(currentUrl).host.equals(URL(newUrl).host, true)) {
            val matchingKnownService =
                    knownServices.findMatchingKnownService(URL(newUrl))
            if (matchingKnownService != null && matchingKnownService.showSpinner) {
                uiInteractor.showProgressDialog()
            }
        }
    }

    private fun hideKnownServicesSpinner(url: String?) {
        val matchingKnownService =
                knownServices.findMatchingKnownService(URL(url))
        if (matchingKnownService != null && matchingKnownService.showSpinner) {
            uiInteractor.dismissProgressDialog()
        }
    }
}
