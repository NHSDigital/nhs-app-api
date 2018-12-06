package com.nhs.online.nhsonline.biometrics

import android.util.Log
import com.nhs.online.nhsonline.biometrics.utils.BiometricCleanupHelper
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSharedPreferences
import com.nhs.online.nhsonline.support.GenericBiometricException

private val TAG = DeRegistrationService::class.java.simpleName

class DeRegistrationService(
        private val biometricsInteractor: IBiometricsInteractor,
        private val biometricCleanupHelper: BiometricCleanupHelper,
        private val preferencesService: FingerprintSharedPreferences,
        private val biometricState: BiometricState,
        private val biometricAsyncHandler: BiometricAsyncHandler
) {
    fun deRegisterBiometrics() {
        biometricState.registrationStateChangeInProgress = true

        biometricsInteractor.showProgressDialog()

        try {
            val appId = preferencesService.readStringFromSharedPref(BiometricConstants.APP_ID)
            val keyId = preferencesService.readStringFromSharedPref(BiometricConstants.KEY_ID)

            biometricAsyncHandler.sendDeRegistrationOperation(appId, keyId) {
                biometricCleanupHelper.removeFidoData()
                biometricsInteractor.dismissProgressDialog()
                biometricsInteractor.showBiometricsOnDeRegistrationSuccessMessage()

                biometricState.registrationStateChangeInProgress = false
            }
        } catch (e: GenericBiometricException) {
            Log.d(TAG, "De-registration call failed", e)
        }
    }
}