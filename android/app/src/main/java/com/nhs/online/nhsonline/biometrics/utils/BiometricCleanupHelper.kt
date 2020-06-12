package com.nhs.online.nhsonline.biometrics.utils

import android.util.Log
import com.nhs.online.fidoclient.exceptions.GenericFidoException
import com.nhs.online.fidoclient.uaf.crypto.FidoKeystore
import java.security.KeyStoreException

private val TAG = BiometricCleanupHelper::class.java.simpleName

class BiometricCleanupHelper(
    private val biometricState: BiometricState,
    private val fidoKeystore: FidoKeystore,
    private val preferencesService: FingerprintSharedPreferences
) {
    fun removeFidoData() {
        try {
            Log.d(TAG, "Attempting to delete FIDO Credentials")

            this.fidoKeystore.deleteKey(this.preferencesService.getFidoUsername())
            this.preferencesService.deleteFidoData()

            biometricState.registered = false
            Log.i(TAG, "Successfully deleted FIDO Credentials")
        } catch (e: KeyStoreException) {
            Log.d(TAG, "Delete invalid credentials failed", e)
            throw e
        } catch (e: GenericFidoException) {
            Log.d(TAG, "Delete invalid credentials failed", e)
            throw e
        }
    }
}