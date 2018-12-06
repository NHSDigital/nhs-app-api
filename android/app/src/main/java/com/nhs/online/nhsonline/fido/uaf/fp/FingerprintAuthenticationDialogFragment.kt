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

package com.nhs.online.nhsonline.fido.uaf.fp

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
import kotlinx.android.synthetic.main.fingerprint_dialog_container.*

/**
 * A dialog which uses fingerprint APIs to authenticate the user, and falls back to password
 * authentication if fingerprint is not available.
 */
@RequiresApi(Build.VERSION_CODES.M)
class FingerprintAuthenticationDialogFragment : DialogFragment(),
    TextView.OnEditorActionListener,
    FingerprintUiHelper.Callback {
    private lateinit var cryptoObject: FingerprintManagerCompat.CryptoObject
    private lateinit var fingerprintUiHelper: FingerprintUiHelper
    private lateinit var fingerprintContent: FingerprintContent
    var fingerprintAuthProcessor: FingerprintAuthProcessor? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        // Do not create a new Fragment when the Activity is re-created such as orientation changes.
        retainInstance = true
        setStyle(DialogFragment.STYLE_NORMAL, android.R.style.Theme_Material_Light_Dialog)
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        dialog.setTitle(fingerprintContent.title)
        dialog.setCanceledOnTouchOutside(false)
        return inflater.inflate(R.layout.fingerprint_dialog_container, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        fingerprintDescription.text = fingerprintContent.description
        cancelButton.text = fingerprintContent.cancelText

        cancelButton.setOnClickListener {
            fingerprintAuthProcessor?.cancel()
            dismiss()
        }

        initiateFingerprintUiHelper()
    }

    override fun onResume() {
        super.onResume()
        fingerprintUiHelper.startListening(cryptoObject)
    }

    override fun onPause() {
        super.onPause()
        fingerprintUiHelper.stopListening()
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
        dismiss()
    }

    fun setFingerprintContent(fingerprintContent: FingerprintContent) {
        this.fingerprintContent = fingerprintContent
    }

    fun setCryptoObject(cryptoObject: FingerprintManagerCompat.CryptoObject) {
        this.cryptoObject = cryptoObject
    }

    override fun onEditorAction(v: TextView, actionId: Int, event: KeyEvent?): Boolean {
        return false
    }

    override fun onAuthenticated(cryptoObject: FingerprintManagerCompat.CryptoObject) {
        fingerprintAuthProcessor?.processAuthentication(cryptoObject)
        dismiss()
    }

    override fun onError() {
        dismiss()
        fingerprintUiHelper.stopListening()
    }
}
