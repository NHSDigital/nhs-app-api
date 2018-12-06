package com.nhs.online.nhsonline.webclients

import android.graphics.Bitmap
import android.os.Handler
import android.content.Context
import android.util.Log
import android.webkit.*
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.ActivityInterface
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.Reachability
import com.nhs.online.nhsonline.services.KnownServices
import java.io.InputStream
import java.net.URL
import java.util.logging.Logger

private const val DELAY_PROGRESS_SHOW_TIME_MILLISECONDS = 500L
private const val REQUEST_TIMEOUT_MILLISECONDS = 20 * 1000L
private const val WOFF2 = "woff2"

class WebClientInterceptor(
    private val uiInteractor: IInteractor,
    private val knownServices: KnownServices,
    private val activities: List<ActivityInterface>,
    private val context: Context
) : WebViewClient() {

    companion object {
        val logger = Logger.getLogger(WebClientInterceptor::class.java.simpleName)!!
    }

    private val handler = Handler()
    private var noConnectionHandled = false
    private var shouldShowErrorPage = false

    @Suppress("OverridingDeprecatedMember")
    override fun shouldOverrideUrlLoading(view: WebView, url: String): Boolean {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering shouldOverrideUrlLoading")
        if (URL(url).host?.equals(URL(context.getString(R.string.dataPreferencesBaseUrl)).host)!!) {
            view.loadUrl(url)
            return false
        }

        if (url == context.getString(R.string.organDonation)) {
            view.loadUrl(context.getString(R.string.organDonationNative))
            return false
        }

        activities.forEach { activity ->
            if (activity.canStart(view.context, url)) {
                activity.start(view.context, url)
                return true
            }
        }
        return false
    }

    override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onPageStarted > url $url")
        uiInteractor.setReloadUrl(url)
        cancelTrackingWebRequestResponse()

        if (!isConnectedToInternet()) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onPageStarted > no internet")

            stopLoadingWebviewAndShowNoConnectionError(view)
            noConnectionHandled = true
            return
        }

        updateHeaderAndNavMenu(url)

        if (hasMissingQueryString(url)) {
            view?.stopLoading()
            url?.let { uiInteractor.loadPage(it) }
            return
        }

        if (shouldHandleUnavailability(url)) {
            trackWebRequestResponse(view, url)
        }

        shouldShowErrorPage = false

        super.onPageStarted(view, url, favicon)
    }

    override fun onLoadResource(view: WebView?, url: String?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLoadResource > url $url")
        if (!isConnectedToInternet()) {
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


    @Suppress("OverridingDeprecatedMember")
    override fun onReceivedError(
        view: WebView?,
        errorCode: Int,
        description: String?,
        failingUrl: String?
    ) {
        if (shouldHandleUnavailability(failingUrl)) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onReceivedError > failingUrl $failingUrl")

            cancelTrackingWebRequestResponse()
            handleUnavailability(failingUrl, errorCode)
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
            }

            uiInteractor.showWebviewScreen()
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

    fun isConnectedToInternet(): Boolean {
        Log.d(Application.TAG, "${this::class.java.simpleName}: isConnectedToInternet")
        return Reachability.isConnectedToNetwork(context)
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
        uiInteractor.setHeaderText(context.resources.getString(R.string.connection_error_header))

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
                    context.resources.getString(R.string.organ_donation_register_header) -> uiInteractor.selectNavigationMenuActive(
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

        val unavailabilityErrorMessage = getUnavailabilityErrorMessageForService(failingUrl)

        uiInteractor.showUnavailabilityError(unavailabilityErrorMessage)

        logger.info("Failing Url: $failingUrl with error code: $errorCode")
    }

    private fun getUnavailabilityErrorMessageForService(failingUrl: String?): ErrorMessage {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering getUnavailabilityErrorMessageForService > failingUrl $failingUrl")
        val serviceInfo = knownServices.findMatchingServiceInfo(failingUrl.toString())
        return serviceInfo?.errorMessage ?: knownServices.getServiceUnavailabilityError()
    }
}