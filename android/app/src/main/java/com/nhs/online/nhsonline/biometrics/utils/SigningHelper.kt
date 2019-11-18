package com.nhs.online.nhsonline.biometrics.utils

import android.security.keystore.KeyPermanentlyInvalidatedException
import com.nhs.online.fidoclient.exceptions.FidoInvalidSignatureException
import com.nhs.online.fidoclient.uaf.crypto.FidoKeystore
import java.security.Signature

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
            throw FidoInvalidSignatureException("Biometric authentication revoked.", e)
        }
    }
}