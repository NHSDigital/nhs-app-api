package com.nhs.online.nhsonline.webclients

import android.content.Context
import android.graphics.Bitmap
import android.net.ConnectivityManager
import android.os.Handler
import android.util.Log
import android.webkit.WebView
import android.webkit.WebViewClient
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.UnsecureInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.support.schemehandlers.SchemeHandlers

private const val DELAY_PROGRESS_SHOW_TIME = 500L
private const val REQUEST_TIMEOUT = 10 * 1000L

class UnsecureWebClient(
    private val uiInteractor: UnsecureInteractor,
    private val knownServices: KnownServices,
    private val context: Context,
    private val schemeHandlers: SchemeHandlers
) : WebViewClient() {
    private val errorMessageHandler = ErrorMessageHandler(context)
    private val handler = Handler()
    private var noConnectionHandled = false
    private var shouldShowErrorPage = false

    override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onPageStarted")
        uiInteractor.setReloadUrl(url)
        updateHeaderText(url)

        if (!isConnectedToNetwork) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onPageStarted > no internet")
            stopLoadingWebviewAndShowNoConnectionError(view)
            noConnectionHandled = true
            return
        }
        if (shouldHandleUnavailability(url)) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onPageStarted > shouldHandleUnavailability true")
            trackWebRequestResponse(view, url)
        }

        shouldShowErrorPage = false
        super.onPageStarted(view, url, favicon)
    }

    private fun updateHeaderText(url: String?) {
        if (url != null && !url.isBlank()) {
            val serviceInfo = knownServices.findMatchingServiceInfo(url)
            serviceInfo?.header?.let { header ->
                uiInteractor.setHeaderText(header)
            }
        }
    }

    override fun onLoadResource(view: WebView?, url: String?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLoadResource > url $url")
        if (!isConnectedToNetwork) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onLoadResource > no internet")
            if (!noConnectionHandled) {
                Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Entering onLoadResource > no internet > noConnectionHandled false")
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
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering onReceivedError > errorCode: $errorCode description: $description failingUrl: $failingUrl")
        if (shouldHandleUnavailability(failingUrl)) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onReceivedError > shouldHandleUnavailability")
            cancelTrackingWebRequestResponse()
            handleUnavailability(failingUrl, errorCode)
        }
    }

    override fun onPageFinished(view: WebView?, url: String?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onPageFinished")
        if (shouldHandleUnavailability(url)) {
            cancelTrackingWebRequestResponse()
        }

        if (!shouldShowErrorPage) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onPageFinished > showWebView")
            uiInteractor.showWebviewScreen()
        }

        super.onPageFinished(view, url)
    }

    private fun shouldHandleUnavailability(urlString: String?): Boolean {
        if (urlString == null) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering shouldHandleUnavailability > false")
            return false
        }
        val matchingKnownServiceInfo =
            knownServices.findMatchingServiceInfo(urlString)
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering shouldHandleUnavailability > ${matchingKnownServiceInfo != null}")
        return matchingKnownServiceInfo != null
    }

    private fun stopLoadingWebviewAndShowNoConnectionError(view: WebView?) {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering stopLoadingWebviewAndShowNoConnectionError")
        handleUnavailability(view?.url)
        view?.stopLoading()
    }

    override fun onPageCommitVisible(view: WebView?, url: String?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onPageCommitVisible")
        if (shouldHandleUnavailability(url)) {
            uiInteractor.dismissProgressDialog()
        }

        if (!shouldShowErrorPage) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering onPageCommitVisible > shouldShowErrorPage")
            uiInteractor.showWebviewScreen()
        }
        super.onPageCommitVisible(view, url)
    }

    private fun trackWebRequestResponse(view: WebView?, url: String?) {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering trackWebRequestResponse > url $url")
        val showDialogFn = { uiInteractor.showProgressDialog() }

        val expireRequestFn = {
            view?.stopLoading()
            uiInteractor.dismissProgressDialog()
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Entering trackWebRequestResponse > expireRequestFn")
            handleUnavailability(url)
        }

        handler.postDelayed(showDialogFn,
            DELAY_PROGRESS_SHOW_TIME)
        handler.postDelayed(expireRequestFn, REQUEST_TIMEOUT)
    }

    private fun cancelTrackingWebRequestResponse() {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering cancelTrackingWebRequestResponse")
        uiInteractor.dismissProgressDialog()
        handler.removeCallbacksAndMessages(null)
    }

    private fun handleUnavailability(failingUrl: String?, errorCode: Int? = null) {
        Log.d(Application.TAG,
            "${this::class.java.simpleName}: Entering handleUnavailability > failingUrl: $failingUrl errorCode: $errorCode")
        shouldShowErrorPage = true

        shouldShowErrorPage = true
        val pageHeader: String
        val errorMessage: ErrorMessage
        when(ConnectionStateMonitor.isConnectedToNetwork) {
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
    }

    @Suppress("OverridingDeprecatedMember")
    override fun shouldOverrideUrlLoading(view: WebView, url: String): Boolean {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering shouldOverrideUrlLoading")
        if(schemeHandlers.handleUrl(url))
            return true

        return false
    }
}