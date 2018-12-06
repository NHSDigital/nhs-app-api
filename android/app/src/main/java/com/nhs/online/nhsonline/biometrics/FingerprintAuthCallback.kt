package com.nhs.online.nhsonline.biometrics

import com.nhs.online.nhsonline.fido.uaf.fp.FingerprintAuthProcessor

abstract class FingerprintAuthCallback(val uafMessage: String) : FingerprintAuthProcessor {
    override fun cancel() = Unit
}