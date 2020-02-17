package com.nhs.online.nhsonline.biometrics

import android.content.res.Resources
import android.support.v4.app.FragmentActivity
import android.util.Log
import com.nhs.online.fidoclient.interfaces.IBiometricsInteractor
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.support.LifeCycleObserverContext
import com.nhs.online.nhsonline.web.NhsWeb

class BiometricsInteractor(private val resources: Resources,
                           private val mainInteractor: IInteractor,
                           private val nhsWeb:NhsWeb,
                           private val lifeCycleObserverContext: LifeCycleObserverContext) : IBiometricsInteractor {

    fun dismissNotifications() {
        nhsWeb.onBiometricOptionChanged()
    }

    override fun dismissProgressDialog() {
        mainInteractor.dismissProgressDialog()
    }

    override fun getActivity(): FragmentActivity = mainInteractor.getActivity()

    override fun loadBiometricLoginPage(url: String) {
        var contextUrl = lifeCycleObserverContext.url!!

        contextUrl += when(contextUrl.contains("?")) {
            true -> "&$url"
            false -> "?$url"
        }

        nhsWeb.requiresFullPageLoad = true
        nhsWeb.loadUrl(contextUrl)
    }

    override fun showBiometricDeviceError() {
        val biometricDeviceErrorMessage = ErrorMessage(resources, ErrorType.BiometricDeviceFailure)
        Log.d(Application.TAG, "Biometric device failed")
        mainInteractor.showUnavailabilityError( biometricDeviceErrorMessage )
    }

    override fun showBiometricRegistrationError() {
        val biometricDeviceErrorMessage = ErrorMessage(resources, ErrorType.BiometricRegistrationFailure)
        Log.d(Application.TAG, "Biometric registration failed")
        mainInteractor.showUnavailabilityError( biometricDeviceErrorMessage )
    }

    override fun showBiometricsOnDeRegistrationSuccessMessage() {
        mainInteractor.setSuccessViewMessage(R.string.fingerprint_de_registration_success_dialog_message)
        mainInteractor.switchToFingerprintSuccessView()
    }

    override fun showBiometricsOnRegistrationSuccessMessage() {
        mainInteractor.setSuccessViewMessage(R.string.fingerprint_registration_success_dialog_message)
        mainInteractor.switchToFingerprintSuccessView()
    }

    override fun showProgressDialog() {
        mainInteractor.showProgressDialog()
    }

    override fun toggleBiometricSwitch(isChecked: Boolean) {
        if (isChecked) {
            mainInteractor.toggleBiometricSwitchOn()
        } else {
            mainInteractor.toggleBiometricSwitchOff()
        }
    }
}