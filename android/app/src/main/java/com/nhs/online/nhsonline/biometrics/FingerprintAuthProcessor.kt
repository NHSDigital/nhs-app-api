package com.nhs.online.nhsonline.biometrics

import androidx.core.hardware.fingerprint.FingerprintManagerCompat

interface FingerprintAuthProcessor {

    fun processAuthentication(cryptObj: FingerprintManagerCompat.CryptoObject): Int

    fun cancel()

    fun error()
}
