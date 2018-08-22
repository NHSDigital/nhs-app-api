package com.nhs.online.nhsonline.webclients

import android.content.Context
import android.graphics.Bitmap
import android.net.ConnectivityManager
import android.os.Handler
import android.webkit.WebView
import android.webkit.WebViewClient
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.SymptomsActivity
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.interfaces.UnsecureInteractor
import com.nhs.online.nhsonline.services.KnownServices

private const val DELAY_PROGRESS_SHOW_TIME = 500L
private const val REQUEST_TIMEOUT = 10 * 1000L

class UnsecureWebClient(
        private val uiInteractor: UnsecureInteractor,
        private val knownServices: KnownServices,
        private val context: Context
) : WebViewClient() {

    private val handler = Handler()
    private var noConnectionHandled = false
    private var shouldShowErrorPage = false

    override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {
        uiInteractor.setReloadUrl(url)
        val urlPath = context.resources.getString(R.string.baseURL)+context.resources.getString(R.string.checkYourSymptoms)+context.resources.getString(R.string.nhsOnlineRequiredQueries)
        when(url) {
            context.resources.getString(R.string.nhs111) -> uiInteractor.setHeaderText(context.resources.getString(R.string.nhs_111_header))
            context.resources.getString(R.string.conditions) -> uiInteractor.setHeaderText(context.resources.getString(R.string.conditions_header))
            urlPath -> uiInteractor.setHeaderText(context.resources.getString(R.string.symptoms_header))
        }
        if (!isConnectedToInternet()) {
            stopLoadingWebviewAndShowNoConnectionError(view)
            noConnectionHandled = true
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
                stopLoadingWebviewAndShowNoConnectionError(view)
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

        super.onPageFinished(view, url)
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

    private fun isConnectedToInternet(): Boolean {
        val cm = context.getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager
        return cm.activeNetworkInfo?.isConnectedOrConnecting == true
    }


    private fun stopLoadingWebviewAndShowNoConnectionError(view: WebView?) {
        handleUnavailability(view?.url)
        view?.stopLoading()
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

    private fun trackWebRequestResponse(view: WebView?, url: String?) {
        val showDialogFn = { uiInteractor.showProgressDialog() }

        val expireRequestFn = {
            view?.stopLoading()
            uiInteractor.dismissProgressDialog()
            handleUnavailability(url)
        }

        handler.postDelayed(showDialogFn,
                DELAY_PROGRESS_SHOW_TIME)
        handler.postDelayed(expireRequestFn, REQUEST_TIMEOUT)
    }

    private fun cancelTrackingWebRequestResponse() {
        uiInteractor.dismissProgressDialog()
        handler.removeCallbacksAndMessages(null)
    }

    private fun handleUnavailability(failingUrl: String?, errorCode: Int? = null) {
        shouldShowErrorPage = true

        val unavailabilityErrorMessage = getUnavailabilityErrorMessageForService(failingUrl)

        uiInteractor.showUnavailabilityError(unavailabilityErrorMessage)

        WebClientInterceptor.logger.info("Failing Url: $failingUrl with error code: $errorCode")
    }

    private fun getUnavailabilityErrorMessageForService(failingUrl: String?): ErrorMessage {
        val service = knownServices.findMatchingKnownService(failingUrl.toString())
        return service?.unavailabilityErrorMessage ?: knownServices.getServiceUnavailabilityError()
    }

}