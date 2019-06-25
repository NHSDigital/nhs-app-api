package com.nhs.online.nhsonline.biometrics.utils

import android.annotation.TargetApi
import android.os.Build
import android.support.v4.app.FragmentActivity
import android.util.Log
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.BiometricConstants
import com.nhs.online.nhsonline.biometrics.BiometricState
import com.nhs.online.nhsonline.fido.uaf.fp.createFingerprintAuthenticationDialogFragment
import com.nhs.online.nhsonline.fido.uaf.fp.FingerprintAuthProcessor
import com.nhs.online.nhsonline.fido.uaf.fp.FingerprintContent

private val TAG = FingerprintDialog::class.java.simpleName
@TargetApi(Build.VERSION_CODES.M)
class FingerprintDialog(
        private val activity: FragmentActivity,
        private val biometricState: BiometricState,
        private val signingHelper: SigningHelper
) {
    fun showFingerprintAuthDialog(
            fingerprintAuthProcessor: FingerprintAuthProcessor,
            fingerprintContent: FingerprintContent
    ) {
        val fingerprintAuthFragment = createFingerprintAuthenticationDialogFragment(signingHelper, fingerprintContent, fingerprintAuthProcessor)

        Log.d(TAG, "Showing fingerprint fragment: $fingerprintAuthFragment")
        fingerprintAuthFragment.show(activity.supportFragmentManager, BiometricConstants.DIALOG_FRAGMENT_TAG)
    }

    fun generateFingerprintContent(forRegistration: Boolean): FingerprintContent {
        val title = when {
            forRegistration -> activity.getString(R.string.enable_fingerprint)
            biometricState.hasLoginError -> activity.getString(R.string.fingerprint_login_error_title)
            else -> activity.getString(R.string.log_in)
        }

        val description = when {
            forRegistration -> activity.getString(R.string.fingerprint_description_register)
            biometricState.hasLoginError -> activity.getString(R.string.fingerprint_login_error_description)
            else -> activity.getString(R.string.fingerprint_description_login)
        }

        val cancelText = activity.getString(R.string.cancel)
        return FingerprintContent(title, description, cancelText)
    }
}