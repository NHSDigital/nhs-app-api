package com.nhs.online.nhsonline.support

import android.util.Log
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import java.net.URL

class LifeCycleObserver(
        private val context: LifeCycleObserverContext,
        private val appWebInterface: AppWebInterface,
        private val knownServices: KnownServices
) {

    fun onMoveToForeground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToForeground")

        if (!validateUserSession()) {
            context.hideBlankScreen()
        }

        context.ensureSupportedVersion()
    }

    fun onMoveToBackground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToBackground")
        context.showBlankScreen()
    }

    private fun validateUserSession(): Boolean {
        val currentUrl: String? = context.url
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
}
