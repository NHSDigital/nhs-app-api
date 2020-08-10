package com.nhs.online.nhsonline.biometrics

import android.util.Log
import com.nhs.online.fidoclient.exceptions.GenericFidoException
import com.nhs.online.nhsonline.biometrics.utils.BiometricState
import com.nhs.online.nhsonline.biometrics.utils.BiometricCleanupHelper
import com.nhs.online.nhsonline.biometrics.utils.BiometricConstants
import com.nhs.online.nhsonline.biometrics.utils.FingerprintCookieService
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSharedPreferences
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface

private val TAG = DeRegistrationService::class.java.simpleName

class DeRegistrationService(
    private val biometricCleanupHelper: BiometricCleanupHelper,
    private val preferencesService: FingerprintSharedPreferences,
    private val biometricState: BiometricState,
    private val biometricAsyncHandler: BiometricAsyncHandler,
    private val appWebInterface: AppWebInterface
        ) {
    fun deRegisterBiometrics(accessToken: String) {
        biometricState.registrationStateChangeInProgress = true

        try {
            val appId = preferencesService.readStringFromSharedPref(BiometricConstants.APP_ID)
            val keyId = preferencesService.readStringFromSharedPref(BiometricConstants.KEY_ID)

            biometricAsyncHandler.sendDeRegistrationOperation(appId, keyId, accessToken) {
                biometricCleanupHelper.removeFidoData()
                appWebInterface.biometricCompletion(
                    BiometricConstants.DEREGISTER,
                    BiometricConstants.SUCCESS,
                    "")

            }
        } catch (fidoException: GenericFidoException) {
            Log.d(TAG, "De-registration call failed due to fido exception", fidoException)
            dispatchError()

        } catch (exception: Exception) {
            Log.d(TAG, "De-registration call failed", exception)
            dispatchError()

        }

        biometricState.registrationStateChangeInProgress = false
    }

    private fun dispatchError(){
        appWebInterface.biometricCompletion(
            BiometricConstants.DEREGISTER,
            BiometricConstants.FAILURE,
            BiometricConstants.CANNOT_CHANGE_CODE)
    }
}
