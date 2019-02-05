package com.nhs.online.nhsonline.biometrics

import com.nhs.online.nhsonline.biometrics.utils.FingerprintSystemChecker


class BiometricsInterface(private val biometricsInteractor: IBiometricsInteractor) {
    private var fingerprintService: FingerprintService? = null
    var isFingerprintRegistered: Boolean
        get() = fingerprintService?.biometricState?.registered ?: false
        set(value) {
            fingerprintService?.biometricState?.registered = value
        }


    fun initializeFingerprintService(
        fidoServerUrl: String
    ): Boolean {
        val fingerprintService =
            FingerprintService.createIfDeviceSupported(biometricsInteractor, fidoServerUrl)
                    ?: return false
        this.fingerprintService = fingerprintService
        return true
    }

    fun requestBiometricsRegistrationStateChange(): Boolean {
        fingerprintService?.let {
            if (it.biometricState.registrationStateChangeInProgress) {
                biometricsInteractor.toggleBiometricSwitch(isFingerprintRegistered)
                return false
            }

            if (it.biometricState.registered)
                it.deRegisterBiometrics()
            else
                it.startFidoRegistration()

            return true
        }
        if (FingerprintSystemChecker.checkIfAndroidMOrAbove()) {
            biometricsInteractor.showBiometricRegistrationError()
        } else {
            FingerprintSystemChecker.showCurrentOSNotSupportDialog(biometricsInteractor.getActivity())
        }

        return false
    }

    fun isFingerprintServiceInitialised() = fingerprintService != null

    fun cancelAllProgressingTasks() = fingerprintService?.cancelAllProgressingTasks()

    fun showBiometricLoginIfEnabled() = fingerprintService?.showBiometricLoginIfEnabled() ?: false

    fun notifyLoginErrorOccurrence() = fingerprintService?.notifyLoginErrorOccurrence()
}