package com.nhs.online.nhsonline.webclients

import android.graphics.Bitmap
import android.os.Handler
import android.webkit.WebView
import android.webkit.WebViewClient
import com.nhs.online.nhsonline.activity.ActivityInterface
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.services.KnownServices
import java.net.URL
import java.util.logging.Logger

private const val DELAY_PROGRESS_SHOW_TIME = 500L
private const val REQUEST_TIMEOUT = 10 * 1000L

class WebClientInterceptor(
    private val uiInteractor: IInteractor,
    private val knownServices: KnownServices,
    private val activities: List<ActivityInterface>
) : WebViewClient() {

    companion object {
        val logger = Logger.getLogger(WebClientInterceptor::class.java.simpleName)!!
    }

    private val handler = Handler()
    private var shouldShowErrorPage = false

    @Suppress("OverridingDeprecatedMember")
    override fun shouldOverrideUrlLoading(view: WebView, url: String): Boolean {
        activities.forEach { activity ->
            if (activity.canStart(view.context, url)) {
                activity.start(view.context, url)
                return true
            }
        }

        val matchingKnownService = knownServices.findMatchingKnownService(url)
        if (matchingKnownService != null) {
            if (knownServices.isTheService(matchingKnownService,
                    KnownServices.ServiceName.NHS111)) {
                uiInteractor.selectSymptomsMenuActive()
            }

            if (knownServices.isTheService(matchingKnownService,
                            KnownServices.ServiceName.ORGAN_DONATION)) {
                uiInteractor.selectMoreMenuActive()
            }

            if (matchingKnownService.hasMissingQueryString(url)) {
                val urlWithMissingQueryStrings = matchingKnownService.addMissingQueryStrings(url)
                view.loadUrl(urlWithMissingQueryStrings)
                return true
            }
        }

        if (shouldHandleUnavailability(url)) {
            uiInteractor.selectSymptomsMenuActive()
        }

        return false
    }

    override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {
        cancelTrackingWebRequestResponse()
        if (shouldHandleUnavailability(url)) {
            trackWebRequestResponse(view)
        }

        shouldShowErrorPage = false
        super.onPageStarted(view, url, favicon)
    }

    override fun onPageFinished(view: WebView?, url: String?) {
        if (shouldHandleUnavailability(url)) {
            cancelTrackingWebRequestResponse()
            uiInteractor.dismissProgressDialog()
        }

        if (!shouldShowErrorPage) {
            uiInteractor.showWebviewScreen()
        }
        super.onPageFinished(view, url)
    }


    @Suppress("OverridingDeprecatedMember")
    override fun onReceivedError(
        view: WebView?,
        errorCode: Int,
        description: String?,
        failingUrl: String?
    ) {
        if (shouldHandleUnavailability(failingUrl)) {
            shouldShowErrorPage = true
            cancelTrackingWebRequestResponse()

            val unavailabilityErrorMessage = getUnavailabilityErrorMessageForService(failingUrl)
            uiInteractor.showUnavailabilityError(unavailabilityErrorMessage)
            logger.info(unavailabilityErrorMessage)
        }
    }

    private fun shouldHandleUnavailability(urlString: String?): Boolean {
        if (urlString != null) {
            val matchingKnownService =
                knownServices.findMatchingKnownService(urlString)
            if (matchingKnownService != null) {
                return matchingKnownService.shouldHandleUnavailability
            }
        }

        return false
    }

    private fun trackWebRequestResponse(view: WebView?) {
        val showDialogFn = { uiInteractor.showProgressDialog() }

        val expireRequestFn = {
            shouldShowErrorPage = true
            view?.stopLoading()
            uiInteractor.dismissProgressDialog()

            val failingUrl: String? = (URL(view?.url)).host
            val unavailabilityErrorMessage = getUnavailabilityErrorMessageForService(failingUrl)
            uiInteractor.showUnavailabilityError(unavailabilityErrorMessage)
            logger.info(unavailabilityErrorMessage)
        }

        handler.postDelayed(showDialogFn,
            DELAY_PROGRESS_SHOW_TIME)
        handler.postDelayed(expireRequestFn, REQUEST_TIMEOUT)

    }

    private fun cancelTrackingWebRequestResponse() {
        uiInteractor.dismissProgressDialog()
        handler.removeCallbacksAndMessages(null)
    }

    private fun getUnavailabilityErrorMessageForService(failingUrl: String?) : String? {
        val service = knownServices.findMatchingKnownService(failingUrl.toString())
        return service?.unavailabilityErrorMessage
    }
}