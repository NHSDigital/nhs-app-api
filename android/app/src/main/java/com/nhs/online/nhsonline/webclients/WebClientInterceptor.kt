package com.nhs.online.nhsonline.webclients

import android.graphics.Bitmap
import android.os.Handler
import android.webkit.WebView
import android.webkit.WebViewClient
import android.content.Context
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.browseractivities.ActivityInterface
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.Reachability
import com.nhs.online.nhsonline.services.KnownService
import com.nhs.online.nhsonline.services.KnownServices
import java.net.URL
import java.util.logging.Logger

private const val DELAY_PROGRESS_SHOW_TIME_MILLISECONDS = 500L
private const val REQUEST_TIMEOUT_MILLISECONDS = 20 * 1000L 

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
        if (URL(url).host?.equals(URL(context.getString(R.string.dataPreferencesBaseUrl)).host)!!) {
            view.loadUrl(url)
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
        uiInteractor.setReloadUrl(url)
        cancelTrackingWebRequestResponse()

        if (!isConnectedToInternet()) {
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
        if (!isConnectedToInternet()) {
            if (!noConnectionHandled) {
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
            cancelTrackingWebRequestResponse()
            handleUnavailability(failingUrl, errorCode)
        }
    }

    override fun onPageFinished(view: WebView?, url: String?) {
        if (shouldHandleUnavailability(url)) {
            cancelTrackingWebRequestResponse()
        }

        if (!shouldShowErrorPage) {
            uiInteractor.showWebviewScreen()
        }

        super.onPageFinished(view, url)
    }

    override fun onPageCommitVisible(view: WebView?, url: String?) {
        if(shouldHandleUnavailability(url)){
            uiInteractor.dismissProgressDialog()
        }

        if (!shouldShowErrorPage) {
            uiInteractor.showWebviewScreen()
        }
        super.onPageCommitVisible(view, url)
    }

    private fun shouldHandleUnavailability(urlString: String?): Boolean {
        if (urlString != null) {
            val matchingKnownService =
                knownServices.findMatchingKnownService(urlString)

            if (matchingKnownService != null) {
                return true
            }
        }

        return false
    }

    fun isConnectedToInternet(): Boolean {
        return Reachability.isConnectedToNetwork(context)
    }

    private fun hasMissingQueryString(url: String?): Boolean {
        if (url == null)
            return false
        return knownServices.findMatchingKnownService(url)?.hasMissingQueryString(url) ?: false
    }

    fun stopLoadingWebviewAndShowNoConnectionError(view: WebView?) {
        handleUnavailability(view?.url)
        view?.stopLoading()
        uiInteractor.setHeaderText(context.resources.getString(R.string.connection_error_header))

    }

    private fun updateHeaderAndNavMenu(url: String?) {
        var service: KnownService?
        url?.let {
            service = knownServices.findMatchingInternalService(it)
            if (service == null) {
                service = knownServices.findMatchingKnownService(it)
            }

            val header = service?.nativeHeader
            val headerDescription = service?.nativeHeaderDescription
            if (header != null) {
                when (header) {
                    context.resources.getString(R.string.nhs_111_header) -> uiInteractor.selectNavigationMenuActive(R.id.symptoms)
                    context.resources.getString(R.string.symptoms_header) -> uiInteractor.selectNavigationMenuActive(R.id.symptoms)
                    context.resources.getString(R.string.appointments_header) -> uiInteractor.selectNavigationMenuActive(R.id.appointments)
                    context.resources.getString(R.string.prescriptions_header) -> uiInteractor.selectNavigationMenuActive(R.id.prescriptions)
                    context.resources.getString(R.string.my_record_header) -> uiInteractor.selectNavigationMenuActive(R.id.myRecord)
                    context.resources.getString(R.string.organ_donation_register_header) -> uiInteractor.selectNavigationMenuActive(R.id.more)
                }

                uiInteractor.setHeaderText(header, headerDescription)
            }
        }
    }

    private fun trackWebRequestResponse(view: WebView?, url: String?) {
        val showDialogFn = { uiInteractor.showProgressDialog() }

        val expireRequestFn = {
            view?.stopLoading()
            uiInteractor.dismissProgressDialog()
            handleUnavailability(url)
        }

        handler.postDelayed(showDialogFn,
            DELAY_PROGRESS_SHOW_TIME_MILLISECONDS)
        handler.postDelayed(expireRequestFn, REQUEST_TIMEOUT_MILLISECONDS)

    }

    private fun cancelTrackingWebRequestResponse() {
        uiInteractor.dismissProgressDialog()
        handler.removeCallbacksAndMessages(null)
    }

    private fun handleUnavailability(failingUrl: String?, errorCode: Int? = null) {
        shouldShowErrorPage = true

        val unavailabilityErrorMessage = getUnavailabilityErrorMessageForService(failingUrl)

        uiInteractor.showUnavailabilityError(unavailabilityErrorMessage)

        logger.info("Failing Url: $failingUrl with error code: $errorCode")
    }

    private fun getUnavailabilityErrorMessageForService(failingUrl: String?): ErrorMessage {
        val service = knownServices.findMatchingKnownService(failingUrl.toString())
        return service?.unavailabilityErrorMessage ?: knownServices.getServiceUnavailabilityError()
    }
}