package com.nhs.online.nhsonline.biometrics.utils

import android.annotation.TargetApi
import android.os.Build
import android.security.keystore.KeyPermanentlyInvalidatedException
import com.nhs.online.nhsonline.fido.uaf.crypto.FidoKeystore
import com.nhs.online.nhsonline.support.BiometricsInvalidSignatureException
import java.security.Signature

@TargetApi(Build.VERSION_CODES.M)
class SigningHelper(
        private val fidoKeystore: FidoKeystore,
        private val preferencesService: FingerprintSharedPreferences
) {
    fun initSignature(): Signature {
        try {
            val signature = Signature.getInstance("SHA256withECDSA")
            val privateKey =
                    fidoKeystore.getPrivateKey(preferencesService.getFidoUsername())

            signature.initSign(privateKey)

            return signature
        } catch (e: KeyPermanentlyInvalidatedException) {
            throw BiometricsInvalidSignatureException("Biometric authentication revoked.", e)
        }
    }
}