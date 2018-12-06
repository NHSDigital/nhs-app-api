package com.nhs.online.nhsonline.biometrics.utils

import android.util.Log
import com.nhs.online.nhsonline.biometrics.BiometricConstants
import com.nhs.online.nhsonline.biometrics.BiometricState
import com.nhs.online.nhsonline.biometrics.IBiometricsInteractor
import com.nhs.online.nhsonline.fido.uaf.crypto.FidoKeystore
import com.nhs.online.nhsonline.support.GenericBiometricException
import java.security.KeyStoreException

private val TAG = BiometricCleanupHelper::class.java.simpleName

class BiometricCleanupHelper(
        private val biometricsInteractor: IBiometricsInteractor,
        private val biometricState: BiometricState,
        private val fidoKeystore: FidoKeystore,
        private val preferencesService: FingerprintSharedPreferences
) {
    fun removeFidoData() {
        try {
            Log.d(TAG, "Attempting to delete FIDO Credentials")

            biometricsInteractor.toggleBiometricSwitch(false)

            this.fidoKeystore.deleteKey(this.preferencesService.getFidoUsername())
            this.preferencesService.deleteFidoData()

            biometricState.registered = false
            Log.i(TAG, "Successfully deleted FIDO Credentials")
        } catch (e: KeyStoreException) {
            Log.d(TAG, "Delete invalid credentials failed", e)
        } catch (e: GenericBiometricException) {
            Log.d(TAG, "Delete invalid credentials failed", e)
        }
    }
}