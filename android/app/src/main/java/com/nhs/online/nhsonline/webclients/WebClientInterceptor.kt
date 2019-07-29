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
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.support.schemehandlers.SchemeHandlers
import com.nhs.online.nhsonline.web.NhsWeb
import java.io.InputStream
import java.net.MalformedURLException
import java.net.URL
import java.util.logging.Logger


private const val DELAY_PROGRESS_SHOW_TIME_MILLISECONDS = 500L
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

        val openedInChromeTab = nhsWeb.openInChromeTabIfApplicable(url)
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

        if (hasMissingQueryString(sanitizedUrl)) {
            view?.stopLoading()
            nhsWeb.requiresFullPageLoad = true
            sanitizedUrl?.let { uiInteractor.loadPage(it) }
            return
        }

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

        if (isNHSAppPage(view)) {
            handleUnavailability(view?.url, WebViewClient.ERROR_CONNECT)
        }
    }

    override fun onReceivedHttpError(view: WebView?, request: WebResourceRequest?, errorResponse: WebResourceResponse?) {
        Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onReceivedHttpError")
        if (isNHSAppPage(view) && !isNHSApi(request)) {
            cancelTrackingWebRequestResponse()
            handleUnavailability(view?.url, WebViewClient.ERROR_CONNECT)
        }
    }

    override fun onReceivedError(view: WebView?, request: WebResourceRequest?, error: WebResourceError?) {
        Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onReceivedError")
        if (isNHSAppPage(view) && shouldHandleUnavailability(view?.url)) {
            cancelTrackingWebRequestResponse()
            if (error != null) {
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                    handleUnavailability(view?.url, error.errorCode)
                }
            }
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
            if (isNHSAppPage(view)) {
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
        if (shouldHandleUnavailability(url)) {
            if (knownServices.isLoginUrlWithSourceQuery(url))
                uiInteractor.showBiometricLoginIfEnabled()
            uiInteractor.dismissProgressDialog()
        }

        if (!shouldShowErrorPage) {
            if (url == context.resources.getString(R.string.baseURL) +
                context.resources.getString(R.string.nhsOnlineRequiredQueries)) {
                Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Entering onPageCommitVisible > is Home page")
                uiInteractor.showHeader()
                uiInteractor.showMenuBar()
            } else if (urlHasRequiresSlimHeader(url)) {
                uiInteractor.showHeaderSlim()
            }

            if (!knownServices.isUrlHostSameAsHomeUrlHost(url))
                uiInteractor.announcePageTitle(view?.title)
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
            knownServices.findMatchingKnownService(urlString)

        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering shouldHandleUnavailability > url $urlString > ${matchingKnownService != null}")

        return matchingKnownService != null
    }

    private fun isNHSAppPage(view: WebView?): Boolean {
        val url = URL(view?.url)
        return url?.host == context.getString(R.string.baseHost)
    }

    private fun isNHSApi(request: WebResourceRequest?): Boolean {
        return request?.url?.host == URL(context.getString(R.string.baseApiURL))?.host
    }

    private fun hasMissingQueryString(url: String?): Boolean {
        if (url == null)
            return false
        return knownServices.findMatchingKnownService(url)?.hasMissingQueryString(url) ?: false
    }

    fun stopLoadingWebviewAndShowNoConnectionError(view: WebView?) {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering stopLoadingWebviewAndShowNoConnectionError > ${view?.url}")
        handleUnavailability(view?.url)
        view?.stopLoading()
    }

    private fun updateHeaderAndNavMenu(url: String?) {
        url?.let {
            val serviceInfo = knownServices.findMatchingServiceInfo(it)

            val header = serviceInfo?.header
            val headerDescription = serviceInfo?.nativeHeaderDescription
            if (header != null) {
                when (header) {
                    context.resources.getString(R.string.nhs_111_header),
                    context.resources.getString(R.string.conditions_header),
                    context.resources.getString(R.string.symptoms_header) -> uiInteractor.selectNavigationMenuActive(
                        R.id.symptoms)
                    context.resources.getString(R.string.appointments_header) -> uiInteractor.selectNavigationMenuActive(
                        R.id.appointments)
                    context.resources.getString(R.string.prescriptions_header) -> uiInteractor.selectNavigationMenuActive(
                        R.id.prescriptions)
                    context.resources.getString(R.string.my_record_header) -> uiInteractor.selectNavigationMenuActive(
                        R.id.myRecord)
                    context.resources.getString(R.string.organ_donation_header) -> uiInteractor.selectNavigationMenuActive(
                        R.id.more)
                }

                uiInteractor.setHeaderText(header, headerDescription)
            }
        }
    }

    private fun trackWebRequestResponse(view: WebView?, url: String?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering trackWebRequestResponse")

        val showDialogFn = { uiInteractor.showProgressDialog() }

        val expireRequestFn = {
            view?.stopLoading()
            uiInteractor.dismissProgressDialog()
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering trackWebRequestResponse > expireRequestFn")
            handleUnavailability(url)
        }

        handler.postDelayed(showDialogFn,
            DELAY_PROGRESS_SHOW_TIME_MILLISECONDS)
        handler.postDelayed(expireRequestFn, REQUEST_TIMEOUT_MILLISECONDS)

    }

    private fun cancelTrackingWebRequestResponse() {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering cancelTrackingWebRequestResponse")
        uiInteractor.dismissProgressDialog()
        handler.removeCallbacksAndMessages(null)
    }

    private fun handleUnavailability(failingUrl: String?, errorCode: Int? = null) {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering handleUnavailability > failingUrl $failingUrl")

        shouldShowErrorPage = true
        val pageHeader: String
        val errorMessage: ErrorMessage
        when (isConnectedToNetwork) {
            true -> {
                errorMessage = errorMessageHandler.getErrorMessage(ErrorType.ServiceUnavailable)
                pageHeader = context.resources.getString(R.string.service_unavailable)
            }
            false -> {
                errorMessage = errorMessageHandler.getErrorMessage(ErrorType.NoConnection)
                pageHeader = context.resources.getString(R.string.connection_error_header)
            }
        }

        Log.d(Application.TAG,
            "${this::class.java.simpleName}: HandleUnavailability -> ErrorMessage Type:  ${errorMessage.title}")
        uiInteractor.showUnavailabilityError(errorMessage)
        uiInteractor.setHeaderText(pageHeader)

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