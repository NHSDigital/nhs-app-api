package com.nhs.online.nhsonline

import android.graphics.Bitmap
import android.os.Handler
import android.webkit.WebView
import android.webkit.WebViewClient
import com.nhs.online.nhsonline.interfaces.IInteractor
import java.net.URL
import java.util.logging.Logger

private const val DELAY_PROGRESS_SHOW_TIME = 500L
private const val REQUEST_TIMEOUT = 10 * 1000L

class WebClientInterceptor(private val uiInteractor: IInteractor, serviceUrls: Array<String>) :
        WebViewClient() {
    private val serviceUrls = serviceUrls

    companion object {
        val logger = Logger.getLogger(WebClientInterceptor::class.java.simpleName)!!
    }


    private val handler = Handler()
    private var shouldShowErrorPage = false

    @Suppress("OverridingDeprecatedMember")
    override fun shouldOverrideUrlLoading(view: WebView?, request: String?): Boolean {
        val requestUrl = request ?: ""
        if (shouldInterceptUrl(requestUrl)) {
            uiInteractor.selectSymptomsMenuActive()
        }
        return false
    }

    override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {
        cancelTrackingWebRequestResponse()
        if (shouldInterceptUrl(url)) {
            trackWebRequestResponse(view)
        }

        shouldShowErrorPage = false
        super.onPageStarted(view, url, favicon)
    }

    override fun onPageFinished(view: WebView?, url: String?) {
        if (shouldInterceptUrl(url)) {
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
        if (shouldInterceptUrl(failingUrl)) {
            shouldShowErrorPage = true
            cancelTrackingWebRequestResponse()
            uiInteractor.showUnavailabilityError()
            logger.info("NHS 111 unavailable")
        }
    }

    private fun shouldInterceptUrl(url2: String?): Boolean {
        if (url2.isNullOrEmpty())
            return false

        val currentUrl = URL(url2)
        serviceUrls.forEach { url ->
            if (URL(url).host == currentUrl.host) {
                return true
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
            uiInteractor.showUnavailabilityError()
            logger.info("NHS 111 unavailable")
        }

        handler.postDelayed(showDialogFn, DELAY_PROGRESS_SHOW_TIME)
        handler.postDelayed(expireRequestFn, REQUEST_TIMEOUT)

    }

    private fun cancelTrackingWebRequestResponse() {
        uiInteractor.dismissProgressDialog()
        handler.removeCallbacksAndMessages(null)
    }

}