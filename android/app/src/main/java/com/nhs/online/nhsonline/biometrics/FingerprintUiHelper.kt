/*
 * Copyright (C) 2017 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License
 */

package com.nhs.online.nhsonline.biometrics

import android.annotation.TargetApi
import android.os.Build
import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import android.support.v4.os.CancellationSignal
import android.widget.ImageView
import android.widget.TextView
import com.nhs.online.nhsonline.R

/**
 * Small helper class to manage text/icon around fingerprint authentication UI.
 */
private const val ERROR_TIMEOUT_MILLIS: Long = 1600
private const val SUCCESS_DELAY_MILLIS: Long = 1300

@TargetApi(Build.VERSION_CODES.M)
class FingerprintUiHelper(
    private val fingerprintMgr: FingerprintManagerCompat,
    private val icon: ImageView,
    private val errorTextView: TextView,
    private val callback: Callback
) : FingerprintManagerCompat.AuthenticationCallback() {

    private var cancellationSignal: CancellationSignal? = null
    private var selfCancelled = false

    private val isFingerprintAuthAvailable: Boolean
        get() = fingerprintMgr.isHardwareDetected && fingerprintMgr.hasEnrolledFingerprints()

    private val resetErrorTextRunnable = Runnable {
        icon.setImageResource(R.drawable.ic_fp_40px)
        errorTextView.run {
            setTextColor(errorTextView.resources.getColor(R.color.hint_color, null))
            text = errorTextView.resources.getString(R.string.fingerprint_hint)
        }
    }

    fun startListening(cryptoObject: FingerprintManagerCompat.CryptoObject) {
        if (!isFingerprintAuthAvailable) return
        cancellationSignal = CancellationSignal()
        selfCancelled = false
        fingerprintMgr.authenticate(cryptoObject, 0, cancellationSignal, this, null)
        icon.setImageResource(R.drawable.ic_fp_40px)
    }

    fun stopListening() {
        cancellationSignal?.also {
            selfCancelled = true
            it.cancel()
        }
        cancellationSignal = null
    }

    override fun onAuthenticationError(errMsgId: Int, errString: CharSequence) {
        if (!selfCancelled) {
            showError(errString)
            icon.postDelayed({ callback.onError() }, ERROR_TIMEOUT_MILLIS)
        }
    }

    override fun onAuthenticationHelp(helpMsgId: Int, helpString: CharSequence) =
        showError(helpString)

    override fun onAuthenticationFailed() =
        showError(icon.resources.getString(R.string.fingerprint_not_recognized))

    override fun onAuthenticationSucceeded(result: FingerprintManagerCompat.AuthenticationResult) {
        errorTextView.run {
            removeCallbacks(resetErrorTextRunnable)
            setTextColor(errorTextView.resources.getColor(R.color.success_color, null))
            text = errorTextView.resources.getString(R.string.fingerprint_success)
        }
        icon.run {
            setImageResource(R.drawable.ic_fingerprint_success)
            postDelayed({ callback.onAuthenticated(result.cryptoObject) }, SUCCESS_DELAY_MILLIS)
        }
    }

    private fun showError(error: CharSequence) {
        icon.setImageResource(R.drawable.ic_fingerprint_error)
        errorTextView.run {
            text = error
            setTextColor(errorTextView.resources.getColor(R.color.warning_color, null))
            removeCallbacks(resetErrorTextRunnable)
            postDelayed(resetErrorTextRunnable, ERROR_TIMEOUT_MILLIS)
        }
    }

    interface Callback {
        fun onAuthenticated(cryptoObject: FingerprintManagerCompat.CryptoObject)
        fun onError()
    }
}
