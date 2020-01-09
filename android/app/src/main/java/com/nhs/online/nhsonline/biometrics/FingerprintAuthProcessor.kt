package com.nhs.online.nhsonline.biometrics

import android.support.v4.hardware.fingerprint.FingerprintManagerCompat

interface FingerprintAuthProcessor {

    fun processAuthentication(cryptObj: FingerprintManagerCompat.CryptoObject): Int

    fun cancel()

    fun error()
}
