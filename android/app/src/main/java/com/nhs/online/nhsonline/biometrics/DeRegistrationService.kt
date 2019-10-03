package com.nhs.online.nhsonline.biometrics

import android.util.Log
import com.nhs.online.fidoclient.exceptions.GenericFidoException
import com.nhs.online.fidoclient.interfaces.IBiometricsInteractor
import com.nhs.online.nhsonline.biometrics.utils.BiometricCleanupHelper
import com.nhs.online.nhsonline.biometrics.utils.FingerprintCookieService
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSharedPreferences

private val TAG = DeRegistrationService::class.java.simpleName

class DeRegistrationService(
        private val biometricsInteractor: IBiometricsInteractor,
        private val biometricCleanupHelper: BiometricCleanupHelper,
        private val preferencesService: FingerprintSharedPreferences,
        private val biometricState: BiometricState,
        private val biometricAsyncHandler: BiometricAsyncHandler,
        private val cookieService: FingerprintCookieService
        ) {
    fun deRegisterBiometrics() {
        biometricState.registrationStateChangeInProgress = true

        biometricsInteractor.showProgressDialog()

        try {
            val appId = preferencesService.readStringFromSharedPref(BiometricConstants.APP_ID)
            val keyId = preferencesService.readStringFromSharedPref(BiometricConstants.KEY_ID)

            val accessToken = cookieService.getAccessTokenFromCookie()

            biometricAsyncHandler.sendDeRegistrationOperation(appId, keyId, accessToken) {
                biometricCleanupHelper.removeFidoData()
                biometricsInteractor.dismissProgressDialog()
                biometricsInteractor.showBiometricsOnDeRegistrationSuccessMessage()

            }
        } catch (e: GenericFidoException) {
            Log.d(TAG, "De-registration call failed", e)
        }

        biometricState.registrationStateChangeInProgress = false
    }
}