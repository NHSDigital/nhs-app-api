package com.nhs.online.nhsonline.biometrics

abstract class FingerprintAuthCallback(val uafMessage: String) : FingerprintAuthProcessor {
    override fun cancel() = Unit
}