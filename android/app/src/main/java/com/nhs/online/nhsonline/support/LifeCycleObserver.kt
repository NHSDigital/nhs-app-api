package com.nhs.online.nhsonline.support

import android.util.Log
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.activities.MainActivity
import kotlinx.android.synthetic.main.activity_main.*
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import java.net.URL

class LifeCycleObserver(
        private val context: MainActivity,
        private val appWebInterface: AppWebInterface,
        private val knownServices: KnownServices
) {

    fun onMoveToForeground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToForeground")

        if (!validateUserSession() && !isCurrentWebViewUrlFromCID()) {
            context.hideBlankScreen()
        }

        if (!context.configurationResponse.isSupportedVersion && !context.appDialogs.isUpgradeDialogActive()) {
            context.appDialogs.showVersionUpgradeDialog()
        }
    }

    fun onMoveToBackground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToBackground")
        context.showBlankScreen()
    }

    private fun validateUserSession(): Boolean {
        val currentUrl: String? = context.webview.url
        currentUrl?.let {
            val matchingKnownService = knownServices.findMatchingKnownService(URL(it))

            if (matchingKnownService != null && matchingKnownService.validateSession) {
                Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Entering onMoveToForeground > isKnownService > $currentUrl and shouldValidateSession")
                appWebInterface.validateSession { context.hideBlankScreen() }
                return true
            }
        }
        return false
    }

    private fun isCurrentWebViewUrlFromCID(): Boolean {
        context.webview.url?.let { url: String ->
            if (url.contains(context.getString(R.string.authRedirectPath)) || url.contains(
                    context.getString(R.string.fidoAuthQueryKey))) return true
        }
        return false
    }
}
