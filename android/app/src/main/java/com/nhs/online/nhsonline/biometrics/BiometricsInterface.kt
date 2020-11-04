package com.nhs.online.nhsonline.biometrics

import android.util.Log
import com.nhs.online.nhsonline.biometrics.utils.BiometricConstants
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSystemChecker
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.services.logging.ILoggingService
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface

private val TAG = BiometricsInterface::class.java.simpleName

class BiometricsInterface(private val biometricsInteractor: BiometricsInteractor,
                          private val interactor: IInteractor,
                          private val appWebInterface: AppWebInterface,
                          private val logger: ILoggingService) {
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
            FingerprintService.createIfDeviceSupported(biometricsInteractor, fidoServerUrl, interactor, appWebInterface, logger)
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

            if (!FingerprintSystemChecker.checkIfAndroidMOrAbove()) {
                Log.e(TAG, "Biometric registration failure: Incompatible Android OS verion");
            } else {
                Log.e(TAG, "Biometric registration failure: No fingerprint(s) found");
            }

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

        logger.logError("Biometric registration failure: Unable to create fingerprint service instance");

        return false
    }

    fun isFingerprintServiceInitialised() = fingerprintService != null

    fun cancelAllProgressingTasks() = fingerprintService?.cancelAllProgressingTasks()

    fun doFingerprintsExist() = fingerprintService?.doFingerprintsExist() ?: false

    fun showBiometricLoginIfEnabled(forceStart: Boolean = false) = fingerprintService?.showBiometricLoginIfEnabled(forceStart) ?: false

    fun notifyLoginErrorOccurrence() = fingerprintService?.notifyLoginErrorOccurrence()

    fun dismissBiometricDialog() = fingerprintService?.dismissBiometricDialog()
}
