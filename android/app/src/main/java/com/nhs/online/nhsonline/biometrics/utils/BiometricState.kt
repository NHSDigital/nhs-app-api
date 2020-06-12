package com.nhs.online.nhsonline.biometrics.utils

import android.util.Log
import com.nhs.online.fidoclient.uaf.crypto.FidoKeystore

private val TAG = BiometricState::class.java.simpleName

class BiometricState(
        private val preferencesService: FingerprintSharedPreferences,
        private val fidoKeystore: FidoKeystore,
        private val fingerprintSystemChecker: FingerprintSystemChecker
) {
    var registered: Boolean = isUserFidoRegistered()
    var registrationStateChangeInProgress: Boolean = false
    var hasLoginError: Boolean = false

    private fun isUserFidoRegistered(): Boolean {
        val fidoUsername = preferencesService.getFidoUsername()
        if (fidoUsername.isEmpty()) {
            return false
        }

        if (!preferencesService.getFingerprintRegisteredState()) {
            return false
        }

        try {
            if (this.fidoKeystore.getKeyPair(fidoUsername) != null) {
                return true
            }
        } catch (e: Exception) {
            Log.i(TAG, "No Key info found")
        }

        fingerprintSystemChecker.showInvalidFingerprintDialog()
        return false
    }
}