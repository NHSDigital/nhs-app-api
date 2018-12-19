package com.nhs.online.nhsonline.support

import android.util.Log
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.interfaces.IVolleyCallback
import com.nhs.online.nhsonline.services.ConfigurationService
import kotlinx.android.synthetic.main.activity_main.*
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.services.ConfigurationResponse
import com.scottyab.rootbeer.RootBeer


class LifeCycleObserver(
    private var context: MainActivity,
    private var appWebInterface: AppWebInterface,
    private var knownServices: KnownServices,
    private var configurationService: ConfigurationService,
    private var rootBeerService: RootBeer
) {

    private val isRootCheckEnabled = context.resources.getString(R.string.isRootCheckEnabled).toBoolean()

    fun onMoveToForeground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToForeground")

        val isDeviceRooted = checkForRooting()
        if (isDeviceRooted) {
            context.showRootedDeviceDialog()
            return
        }

        updateUI()
        checkAndHandleConfiguration()
    }

    fun onMoveToBackground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToBackground")
        context.showBlankScreen()
    }

    private fun checkForRooting() : Boolean {
        rootBeerService.setLogging(true)

        if (!isRootCheckEnabled) {
            Log.d(Application.TAG, "${this::class.java.simpleName}: Root check is disabled")
        }
        else if (rootBeerService.isRootedWithoutBusyBoxCheck) {
            Log.e(Application.TAG, "${this::class.java.simpleName}: Detected that device is rooted")
            return true
        }

        return false
    }

    private fun updateUI() {
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

    private fun checkAndHandleConfiguration () {
        configurationService.getConfiguration(object : IVolleyCallback {
            override fun onSuccess(configurationResponse: ConfigurationResponse) {

                if(!context.isSuccessfulConfigCheck) {
                    context.isSuccessfulConfigCheck = true

                    if(configurationResponse.isThrottlingEnabled) {
                        context.loadThrottlingCarousel()
                    } else {
                        context.loadAuthReturnOrWelcomePage()
                    }
                }

                if (!configurationResponse.isValidConfiguration) {
                    context.showVersionUpgradeDialog()
                } else {
                    context.hideVersionUpgradeDialog()
                }
            }

            override fun onError(errorMessage: ErrorMessage) {
                context.loadAuthReturnOrWelcomePage()
                context.isSuccessfulConfigCheck = false
                context.showUnavailabilityError(errorMessage)
            }
        })
    }
}
