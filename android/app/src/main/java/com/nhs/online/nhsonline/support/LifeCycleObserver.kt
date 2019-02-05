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
import com.nhs.online.nhsonline.web.NhsWeb
import com.scottyab.rootbeer.RootBeer


class LifeCycleObserver(
    private val context: MainActivity,
    private val appWebInterface: AppWebInterface,
    private val nhsWeb: NhsWeb,
    private val rootBeerService: RootBeer,
    private val appDialogs: AppDialogs
) {
    private val knownServices = KnownServices(context)
    private val configurationService = ConfigurationService(context)
    private val isRootCheckEnabled =
        context.resources.getString(R.string.isRootCheckEnabled).toBoolean()

    fun onMoveToForeground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToForeground")

        val isDeviceRooted = checkForRooting()
        if (isDeviceRooted) {
            appDialogs.showRootedDeviceDialog()
            return
        }

        val validating = validateUserSession()
        if (!validating && !isCurrentWebViewUrlFromCID())
            context.hideBlankScreen()

        if (!context.isSuccessfulConfigCheck)
            checkAndHandleConfiguration()
    }

    fun onMoveToBackground() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onMoveToBackground")
        context.showBlankScreen()
    }

    private fun checkForRooting(): Boolean {
        rootBeerService.setLogging(true)

        if (!isRootCheckEnabled) {
            Log.d(Application.TAG, "${this::class.java.simpleName}: Root check is disabled")
        } else if (rootBeerService.isRootedWithoutBusyBoxCheck) {
            Log.e(Application.TAG, "${this::class.java.simpleName}: Detected that device is rooted")
            return true
        }

        return false
    }

    private fun validateUserSession(): Boolean {
        val currentUrl: String? = context.webview.url
        currentUrl?.let {
            val knownServiceInfo = knownServices.findMatchingServiceInfo(it)

            if (knownServiceInfo != null && knownServiceInfo.shouldValidateSession) {
                Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Entering onMoveToForeground > isKnownService > ${knownServiceInfo.baseUrl} and shouldValidateSession")
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

    fun checkAndHandleConfiguration() {
        if (configurationService.isInProgress) return

        if (appDialogs.isUpgradeDialogActive()) return

        context.showProgressDialog()
        configurationService.getConfiguration(object : IVolleyCallback {
            override fun onSuccess(configurationResponse: ConfigurationResponse) {
                context.dismissProgressDialog()

                context.isSuccessfulConfigCheck = true
                if (!configurationResponse.isValidConfiguration) {
                    appDialogs.showVersionUpgradeDialog()
                }

                context.configBiometricSetup(configurationResponse.fidoServerUrl)

                val throttlingPageLoaded = PersistData(context)
                    .readBoolean(context.getString(R.string.haveShownThrottlingCarouselBefore))

                if (configurationResponse.isThrottlingEnabled && !throttlingPageLoaded) {
                    nhsWeb.loadThrottlingCarousel()
                } else {
                    nhsWeb.loadWelcomePage()
                }
            }

            override fun onError(errorMessage: ErrorMessage) {
                context.dismissProgressDialog()

                context.isSuccessfulConfigCheck = false
                context.showUnavailabilityError(errorMessage)

            }
        })
    }
}
