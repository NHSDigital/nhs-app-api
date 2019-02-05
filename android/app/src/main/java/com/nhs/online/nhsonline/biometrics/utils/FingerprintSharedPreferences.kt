package com.nhs.online.nhsonline.biometrics.utils

import android.app.Activity
import android.util.Log
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.BiometricConstants
import com.nhs.online.nhsonline.support.AppSharedPref

private val TAG = FingerprintSharedPreferences::class.java.simpleName

class FingerprintSharedPreferences(val activity: Activity) : AppSharedPref(activity) {

    fun getFidoUsername(): String {
        val fidoUsername = readString(activity.getString(R.string.fidoUsername)) ?: ""
        Log.i(TAG, "FIDO username extracted: $fidoUsername")

        return fidoUsername
    }

    fun saveFidoUsername(username: String) {
        storeString(activity.getString(R.string.fidoUsername), username)
    }

    fun deleteFidoData() {
        removeData(activity.getString(R.string.fidoUsername))
        removeData(BiometricConstants.FIDO_REGISTERED)
        removeData(BiometricConstants.APP_ID)
        removeData(BiometricConstants.KEY_ID)
    }

    fun readStringFromSharedPref(key: String): String {
        return readString(key) ?: ""
    }

    fun getFingerprintRegisteredState(): Boolean {
        return readBoolean(BiometricConstants.FIDO_REGISTERED)
    }

    fun storeFingerprintState(registered: Boolean) {
        storeBoolean(BiometricConstants.FIDO_REGISTERED, registered)
    }
}