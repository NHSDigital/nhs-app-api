package com.nhs.online.nhsonline.biometrics

import com.nhs.online.nhsonline.biometrics.utils.BiometricConstants
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSystemChecker
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface


class BiometricsInterface(private val biometricsInteractor: BiometricsInteractor, private val interactor: IInteractor,
                          private val appWebInterface: AppWebInterface) {
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
            FingerprintService.createIfDeviceSupported(biometricsInteractor, fidoServerUrl, interactor, appWebInterface)
                    ?: return false
        this.fingerprintService = fingerprintService
        return true
    }

    fun requestBiometricsRegistrationStateChange(accessToken: String): Boolean {

        if (!FingerprintSystemChecker.checkIfAndroidMOrAbove() || !doFingerprintsExist()) {
            appWebInterface.biometricCompletion(
                BiometricConstants.REGISTER,
                BiometricConstants.FAILURE,
                BiometricConstants.CANNOT_FIND_CODE)
            return false
        }

        fingerprintService?.let {
            if (it.biometricState.registered){
                it.deRegisterBiometrics(accessToken)
            } else {
                it.startFidoRegistration(accessToken)
            }
            return true
        }

        return false
    }

    fun isFingerprintServiceInitialised() = fingerprintService != null

    fun cancelAllProgressingTasks() = fingerprintService?.cancelAllProgressingTasks()

    fun doFingerprintsExist() = fingerprintService?.doFingerprintsExist() ?: false

    fun showBiometricLoginIfEnabled(forceStart: Boolean = false) = fingerprintService?.showBiometricLoginIfEnabled(forceStart) ?: false

    fun notifyLoginErrorOccurrence() = fingerprintService?.notifyLoginErrorOccurrence()

    fun dismissBiometricDialog() = fingerprintService?.dismissBiometricDialog()
}
