package com.nhs.online.nhsonline.fido.uaf.fp

import android.support.v4.hardware.fingerprint.FingerprintManagerCompat

interface FingerprintAuthProcessor {

    fun processAuthentication(cryptObj: FingerprintManagerCompat.CryptoObject): Int

    fun cancel()

}
