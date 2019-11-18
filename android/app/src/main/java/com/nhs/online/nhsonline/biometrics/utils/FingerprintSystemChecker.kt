package com.nhs.online.nhsonline.biometrics.utils

import android.app.Activity
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
    
    fun preRegistrationCheck(): Boolean {

        if (!checkIfHardwareSupported()) {
            alertHelper.showDialog(
                context.resources.getString(R.string.hardware_not_supported_header),
                context.resources.getString(R.string.hardware_not_supported_message))
            return false
        }

        if (!checkIfFingerprintsExist()) {
            alertHelper.showDialog(
                context.resources.getString(R.string.fingerprints_not_enabled_header),
                context.resources.getString(R.string.fingerprints_not_enabled_message),
                context.resources.getString(R.string.biometricsHelpURL))
            return false
        }
        return true
    }

    fun preLoginCheck(): Boolean {

        if (!checkIfHardwareSupported()) {
            alertHelper.showDialog(
                context.resources.getString(R.string.hardware_not_supported_header),
                context.resources.getString(R.string.hardware_not_supported_message))
            return false
        }

        if (!checkIfFingerprintsExist()) {
            alertHelper.showDialog(
                context.resources.getString(R.string.invalid_fingerprint_dialog_header),
                context.resources.getString(R.string.invalid_fingerprint_dialog_message))
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

    fun checkIfFingerprintsExist(): Boolean {
        return fingerprintManager.hasEnrolledFingerprints()
    }

    fun checkIfHardwareSupported(): Boolean {
        return fingerprintManager.isHardwareDetected
    }
}