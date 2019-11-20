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
 *
 * Modified heavily (including conversion to Kotlin) by NHS App
 */

package com.nhs.online.nhsonline.biometrics

import android.os.Build
import android.os.Bundle
import android.support.annotation.RequiresApi
import android.support.v4.app.DialogFragment
import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import android.view.KeyEvent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.utils.SigningHelper
import kotlinx.android.synthetic.main.fingerprint_dialog_container.*

@RequiresApi(Build.VERSION_CODES.M)
fun createFingerprintAuthenticationDialogFragment(signingHelper: SigningHelper,
                                                  fingerprintContent: FingerprintContent,
                                                  fingerprintAuthProcessor: FingerprintAuthProcessor) : FingerprintAuthenticationDialogFragment {
    val fragment = FingerprintAuthenticationDialogFragment()
    val args = Bundle()
    fragment.arguments = args
    fragment.cryptoObject = FingerprintManagerCompat.CryptoObject(signingHelper.initSignature())
    fragment.fingerprintContent = fingerprintContent
    fragment.fingerprintAuthProcessor = fingerprintAuthProcessor
    return fragment
}

/**
 * A dialog which uses fingerprint APIs to authenticate the user, and falls back to password
 * authentication if fingerprint is not available.
 */
@RequiresApi(Build.VERSION_CODES.M)
class FingerprintAuthenticationDialogFragment : DialogFragment(),
    TextView.OnEditorActionListener,
        FingerprintUiHelper.Callback {
    var cryptoObject: FingerprintManagerCompat.CryptoObject? = null
    var fingerprintUiHelper: FingerprintUiHelper? = null
    var fingerprintContent: FingerprintContent? = null
    var fingerprintAuthProcessor: FingerprintAuthProcessor? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        // Do not create a new Fragment when the Activity is re-created such as orientation changes.
        retainInstance = true
        setStyle(STYLE_NORMAL, android.R.style.Theme_Material_Light_Dialog)

    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        dialog.setTitle(fingerprintContent?.title)
        dialog.setCanceledOnTouchOutside(false)
        return inflater.inflate(R.layout.fingerprint_dialog_container, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        fingerprintDescription.text = fingerprintContent?.description
        cancelButton.text = fingerprintContent?.cancelText

        cancelButton.setOnClickListener {
            fingerprintAuthProcessor?.cancel()
            dismissAllowingStateLoss()
        }

        initiateFingerprintUiHelper()
    }

    override fun onResume() {
        super.onResume()
        if (cryptoObject != null) {
            fingerprintUiHelper?.startListening(cryptoObject!!)
        } else {
            fingerprintAuthProcessor?.cancel()
            dismissAllowingStateLoss()
        }
    }

    override fun onPause() {
        super.onPause()
        fingerprintUiHelper?.stopListening()
    }

    private fun initiateFingerprintUiHelper() {
        context?.let {
            val fingerprintManager = FingerprintManagerCompat.from(it)
            if (fingerprintManager.isHardwareDetected && fingerprintManager.hasEnrolledFingerprints()) {
                fingerprintUiHelper = FingerprintUiHelper(fingerprintManager, fingerprintIcon,
                        fingerprintStatus, this)
                return
            }
        }
        dismissAllowingStateLoss()
    }

    override fun onEditorAction(v: TextView, actionId: Int, event: KeyEvent?): Boolean {
        return false
    }

    override fun onAuthenticated(cryptoObject: FingerprintManagerCompat.CryptoObject) {
        fingerprintAuthProcessor?.processAuthentication(cryptoObject)
        dismissAllowingStateLoss()
    }

    override fun onError() {
        if (this.isAdded) {
            dismissAllowingStateLoss()
        }
        fingerprintUiHelper?.stopListening()
    }
}
