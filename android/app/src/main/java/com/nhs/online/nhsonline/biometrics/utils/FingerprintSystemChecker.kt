package com.nhs.online.nhsonline.biometrics.utils

import android.content.Context
import android.os.Build
import android.support.annotation.RequiresApi
import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.support.AlertHelper


class FingerprintSystemChecker(
    private val fingerprintManager: FingerprintManagerCompat,
    val context: Context,
    interactor: IInteractor
) {

    private val alertHelper: AlertHelper = AlertHelper(context, interactor)

    @RequiresApi(23)
    fun preRegistrationCheck(): Boolean {
        if (!checkIfAndroidMOrAbove()) {
            alertHelper.showDialog(
                context.resources.getString(R.string.version_not_supported_header),
                context.resources.getString(R.string.version_not_supported_message))
            return false
        }

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

    @RequiresApi(23)
    fun preLoginCheck(): Boolean {
        if (!checkIfAndroidMOrAbove()) {
            alertHelper.showDialog(
                context.resources.getString(R.string.version_not_supported_header),
                context.resources.getString(R.string.version_not_supported_message))
            return false
        }

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

        fun showCurrentOSNotSupportDialog(context: Context, interactor: IInteractor) {
            if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
                AlertHelper(context, interactor).showDialog(
                    context.getString(R.string.version_not_supported_header),
                    context.getString(R.string.version_not_supported_message))
            }
        }
    }
}