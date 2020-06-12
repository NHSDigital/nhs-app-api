package com.nhs.online.nhsonline.biometrics.utils

import android.annotation.TargetApi
import android.os.Build
import android.support.v4.app.FragmentActivity
import android.util.Log
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.FingerprintAuthProcessor
import com.nhs.online.nhsonline.biometrics.FingerprintAuthenticationDialogFragment
import com.nhs.online.nhsonline.biometrics.createFingerprintAuthenticationDialogFragment

private val TAG = FingerprintDialog::class.java.simpleName
@TargetApi(Build.VERSION_CODES.M)
class FingerprintDialog(
    private val activity: FragmentActivity,
    private val biometricState: BiometricState,
    private val signingHelper: SigningHelper
) {
    private var fingerprintAuthFragment: FingerprintAuthenticationDialogFragment? = null

    fun showFingerprintAuthDialog(
        fingerprintAuthProcessor: FingerprintAuthProcessor,
        fingerprintContent: FingerprintContent
    ) {
        fingerprintAuthFragment =
            createFingerprintAuthenticationDialogFragment(
                signingHelper,
                fingerprintContent,
                fingerprintAuthProcessor)

        Log.d(TAG, "Showing fingerprint fragment: $fingerprintAuthFragment")
        fingerprintAuthFragment?.show(activity.supportFragmentManager, BiometricConstants.DIALOG_FRAGMENT_TAG)
    }

    fun dismissFingerprintAuthDialog() {
        fingerprintAuthFragment?.dismissAllowingStateLoss()
        fingerprintAuthFragment = null
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

        return FingerprintContent(title,
            description,
            cancelText)
    }
}