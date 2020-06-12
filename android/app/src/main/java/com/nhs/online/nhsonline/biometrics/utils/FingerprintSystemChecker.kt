package com.nhs.online.nhsonline.biometrics.utils

import android.app.Activity
import android.os.Build
import android.support.annotation.RequiresApi
import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.support.AlertHelper


class FingerprintSystemChecker(
    private val fingerprintManager: FingerprintManagerCompat,
    val context: Activity,
    interactor: IInteractor
) {

    private val alertHelper: AlertHelper = AlertHelper(context, interactor)

    @RequiresApi(23)
    fun preRegistrationCheck(): Boolean {
        return checkIfAndroidMOrAbove() && checkIfHardwareSupported() && checkIfFingerprintsExist()
    }

    @RequiresApi(23)
    fun preLoginCheck(): Boolean {
        if (!checkIfFingerprintsExist()) {
            showInvalidFingerprintDialog()
            return false
        }
        return true
    }

    fun showInvalidFingerprintDialog() {
        alertHelper.showDialog(
            context.resources.getString(R.string.invalid_fingerprint_dialog_header),
            context.resources.getString(R.string.invalid_fingerprint_dialog_message)
        )
    }

    @RequiresApi(23)
    fun checkIfFingerprintsExist(): Boolean {
        return fingerprintManager.hasEnrolledFingerprints()
    }


    @RequiresApi(23)
    fun checkIfHardwareSupported(): Boolean {
        return fingerprintManager.isHardwareDetected
    }

    companion object {
        fun checkIfAndroidMOrAbove(): Boolean {
            return (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M)
        }
    }
}