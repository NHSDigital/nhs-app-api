package com.nhs.online.nhsonline.support

import android.util.Log
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.interfaces.IVolleyCallback
import com.nhs.online.nhsonline.services.ConfigurationService
import kotlinx.android.synthetic.main.activity_main.*
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.data.ErrorMessage


class LifeCycleObserver(
    private var context: MainActivity,
    private var appWebInterface: AppWebInterface,
    private var knownServices: KnownServices,
    private var configurationService: ConfigurationService
) {

    fun onMoveToForeground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToForeground")
        updateUI()
        checkAndHandleConfiguration()
    }

    fun onMoveToBackground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToBackground")
        context.showBlankScreen()
    }

    fun updateUI() {
        val currentUrl: String? = context.webview.url
        currentUrl?.let {
            if (currentUrl.contains("auth-return")) return

            val knownServiceInfo = knownServices.findMatchingServiceInfo(it)

            if (knownServiceInfo != null && knownServiceInfo.shouldValidateSession) {
                Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToForeground > isKnownService > ${knownServiceInfo.baseUrl} and shouldValidateSession")
                appWebInterface.validateSession()
            } else {
                context.hideBlankScreen()
            }
        }
    }

    fun checkAndHandleConfiguration () {
        configurationService.isValidConfiguration(object : IVolleyCallback {
            override fun onSuccess(isValid: Boolean) {
                context.isSuccessfulConfigCheck = true
                if (!isValid) {
                    context.showVersionUpgradeDialog()
                } else {
                    context.hideVersionUpgradeDialog()
                }
            }

            override fun onError(errorMessage: ErrorMessage) {
                context.isSuccessfulConfigCheck = false
                context.showUnavailabilityError(errorMessage)
            }
        })
    }
}