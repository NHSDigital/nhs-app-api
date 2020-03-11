package com.nhs.online.nhsonline.webclients

import android.content.Context
import android.graphics.Bitmap
import android.net.http.SslError
import android.os.Handler
import android.os.Build
import android.util.Log
import android.webkit.WebResourceRequest
import android.webkit.WebResourceResponse
import android.webkit.WebView
import android.webkit.WebViewClient
import android.webkit.WebResourceError
import android.webkit.SslErrorHandler
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import com.nhs.online.nhsonline.support.schemehandlers.SchemeHandlers
import com.nhs.online.nhsonline.utils.UrlHelper
import com.nhs.online.nhsonline.web.NhsWeb
import java.io.InputStream
import java.net.MalformedURLException
import java.net.URL
import java.util.logging.Logger

private const val REQUEST_TIMEOUT_MILLISECONDS = 20 * 1000L
private const val WOFF2 = "woff2"

class WebClientInterceptor(
        private val uiInteractor: IInteractor,
        private val nhsWeb: NhsWeb,
        private val context: Context,
        private val knownServices: KnownServices,
        private val schemeHandlers: SchemeHandlers
) : WebViewClient() {

    companion object {
        val logger = Logger.getLogger(WebClientInterceptor::class.java.simpleName)!!
    }

    private val errorMessageHandler = ErrorMessageHandler(context)
    private val handler = Handler()
    private var noConnectionHandled = false
    private var shouldShowErrorPage = false
    private val urlHelper = UrlHelper(context)

    @Suppress("OverridingDeprecatedMember")
    override fun shouldOverrideUrlLoading(view: WebView, url: String): Boolean {
        Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering shouldOverrideUrlLoading > url $url")

        if (urlHasAppScheme(url)) {
            val sanitizedUrl = ensureSupportedScheme(url)
            Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Overriding url $url to $sanitizedUrl")

            uiInteractor.hideHeaderSlim()
            view.stopLoading()
            nhsWeb.requiresFullPageLoad = true
            sanitizedUrl?.let { uiInteractor.loadPage(it) }
            return true
        }

        if (schemeHandlers.handleUrl(url))
            return true

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
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onPageStarted > url $url")

        val sanitizedUrl = when {
            urlHasAppScheme(url) -> {
                uiInteractor.hideHeaderSlim()
                ensureSupportedScheme(url)
            }
            else -> url
        }

        Log.d(Application.TAG, "${this::class.java.simpleName}: Sanitized url $sanitizedUrl")

        nhsWeb.reloadUrl = sanitizedUrl
        cancelTrackingWebRequestResponse()

        if (!isConnectedToNetwork) {
            Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Entering onPageStarted > no internet")

            stopLoadingWebviewAndShowNoConnectionError(view)
            noConnectionHandled = true
            return
        }

        updateHeaderAndNavMenu(sanitizedUrl)

        if (shouldHandleUnavailability(sanitizedUrl)) {
            trackWebRequestResponse(view, sanitizedUrl)
        }

        shouldShowErrorPage = false

        super.onPageStarted(view, sanitizedUrl, favicon)
    }

    override fun onLoadResource(view: WebView?, url: String?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLoadResource > url $url")

        if (!isConnectedToNetwork) {
            Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Entering onLoadResource > isConnectedToInternet")
            if (!noConnectionHandled) {
                Log.d(Application.TAG,
                        "${this::class.java.simpleName}: Entering onLoadResource > isConnectedToInternet > noConnectionHandled false")
                cancelTrackingWebRequestResponse()
            }
            return
        }
        noConnectionHandled = false
        super.onLoadResource(view, url)
    }

    override fun onReceivedSslError(view: WebView?, handler: SslErrorHandler?, error: SslError?) {
        Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onReceivedSslError")

        if (canHandleUnavailability(view)) {
            handleUnavailability(view?.url, ERROR_CONNECT)
        }
    }

    override fun onReceivedHttpError(view: WebView?, request: WebResourceRequest?, errorResponse: WebResourceResponse?) {
        Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onReceivedHttpError")
        if (canHandleUnavailability(view) && !isNHSApi(request)) {
            cancelTrackingWebRequestResponse()
            handleUnavailability(view?.url, ERROR_CONNECT)
        }
    }

    override fun onReceivedError(view: WebView?, request: WebResourceRequest?, error: WebResourceError?) {
        Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onReceivedError")
        if (isNHSAppDomain(request?.url?.host) &&
                canHandleUnavailability(view) &&
                shouldHandleUnavailability(view?.url)) {

            cancelTrackingWebRequestResponse()
            if (error != null) {
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                    handleUnavailability(view?.url, error.errorCode)
                }
            }
        } else {
            Log.d(Application.TAG,
                    "${this::class.java.simpleName}: In onReceivedError > Skipping unavailability handling > failed url ${request?.url}")
        }
    }

    @Suppress("OverridingDeprecatedMember")
    override fun onReceivedError(
            view: WebView?,
            errorCode: Int,
            description: String?,
            failingUrl: String?
    ) {

        if (shouldHandleUnavailability(failingUrl)) {
            Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Entering onReceivedError > failingUrl $failingUrl > Error Description: $description")
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

        Log.d(Application.TAG, "${this::class.java.simpleName}: Request to Load $requestUrl")

        if (requestUrl.endsWith(WOFF2, ignoreCase = true)) {
            val fonts = context.resources.getStringArray(R.array.fonts)

            val fontInRequest = fonts.find { requestUrl.contains(it, ignoreCase = true) }

            fontInRequest?.let {
                Log.d(Application.TAG,
                        "${this::class.java.simpleName}: Loading local font for: $fontInRequest")
                return loadBundledFont(fontInRequest)
            }
        }

        return super.shouldInterceptRequest(view, request)
    }

    private fun loadBundledFont(font: String): WebResourceResponse {
        val woff2MimeType = "application/font-woff2"

        val inputStream: InputStream = context.assets.open("fonts/$font")
        return WebResourceResponse(woff2MimeType, WOFF2, inputStream)
    }

    override fun onPageFinished(view: WebView?, url: String?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onPageFinished")
        if (shouldHandleUnavailability(url)) {
            cancelTrackingWebRequestResponse()
        }

        view?.isFocusable = true
        view?.requestFocus()
        if (!shouldShowErrorPage) {
            uiInteractor.showWebviewScreen()
        }

        super.onPageFinished(view, url)
    }

    override fun onPageCommitVisible(view: WebView?, url: String?) {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering onPageCommitVisible and url: $url")

        if (!shouldShowErrorPage) {
            if (url == context.resources.getString(R.string.baseURL)) {
                Log.d(Application.TAG,
                        "${this::class.java.simpleName}: Entering onPageCommitVisible > is Home page")
                uiInteractor.showHeader()
                uiInteractor.showMenuBar()
            } else if (urlHasRequiresSlimHeader(url)) {
                uiInteractor.showHeaderSlim()
            }

            if (!urlHelper.isSameHostAndSchemeAsHomeUrl(url)) {
                uiInteractor.announcePageTitle(view?.title)
                uiInteractor.dismissBiometricDialog()
            }
        }
        super.onPageCommitVisible(view, url)
    }

    private fun shouldHandleUnavailability(urlString: String?): Boolean {
        if (urlString == null) {
            Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Entering shouldHandleUnavailability > url $urlString > false")
            return false
        }
        val matchingKnownService =
                knownServices.findMatchingKnownService(URL(urlString))

        Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering shouldHandleUnavailability > url $urlString > ${matchingKnownService != null}")

        return matchingKnownService != null
    }

    private fun canHandleUnavailability(view: WebView?): Boolean {
        return view?.url === null || isNHSAppPage(view)
    }

    private fun isNHSAppDomain(value: String?): Boolean {
        return value == context.getString(R.string.baseHost)
    }

    private fun isNHSAppPage(view: WebView?): Boolean {
        val url = URL(view?.url)
        return isNHSAppDomain(url.host)
    }

    private fun isNHSApi(request: WebResourceRequest?): Boolean {
        return request?.url?.host == URL(context.getString(R.string.baseApiURL)).host
    }

    fun stopLoadingWebviewAndShowNoConnectionError(view: WebView?) {
        Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering stopLoadingWebviewAndShowNoConnectionError > ${view?.url}")
        handleUnavailability(view?.url)
        view?.stopLoading()
    }

    private fun updateHeaderAndNavMenu(url: String?) {
        url?.let {
            val serviceInfo = knownServices.findMatchingKnownService(URL(url))

            serviceInfo?.let { info ->
                uiInteractor.selectNavigationMenuActive(info.menuTab.tabIndex)
            }
        }
    }

    private fun trackWebRequestResponse(view: WebView?, url: String?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering trackWebRequestResponse")

        val expireRequestFn = {
            view?.stopLoading()
            uiInteractor.dismissProgressDialog()
            Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Entering trackWebRequestResponse > expireRequestFn")
            handleUnavailability(url)
        }

        handler.postDelayed(expireRequestFn, REQUEST_TIMEOUT_MILLISECONDS)
    }

    private fun cancelTrackingWebRequestResponse() {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering cancelTrackingWebRequestResponse")
        handler.removeCallbacksAndMessages(null)
    }

    private fun handleUnavailability(failingUrl: String?, errorCode: Int? = null) {
        Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering handleUnavailability > failingUrl $failingUrl")

        shouldShowErrorPage = true
        val errorMessage = when (isConnectedToNetwork) {
            true -> {
                errorMessageHandler.getErrorMessage(ErrorType.ServiceUnavailable)
            }
            false -> {
                errorMessageHandler.getErrorMessage(ErrorType.NoConnection)
            }
        }

        Log.d(Application.TAG,
                "${this::class.java.simpleName}: HandleUnavailability -> ErrorMessage Type:  ${errorMessage.title}")
        uiInteractor.showUnavailabilityError(errorMessage)
        uiInteractor.dismissBiometricDialog()
        uiInteractor.dismissProgressDialog()

        logger.info("Failing Url: $failingUrl with error code: $errorCode")
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

    private fun urlHasRequiresSlimHeader(url: String?): Boolean {
        if (url == null) return false

        return try {
            url.contains(URL(context.resources.getString(R.string.nhsLoginSuffix)).host)
        } catch (exception: MalformedURLException) {
            false
        }
    }
}