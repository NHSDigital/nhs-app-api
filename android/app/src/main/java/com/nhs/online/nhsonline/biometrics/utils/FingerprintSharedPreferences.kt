package com.nhs.online.nhsonline.biometrics.utils

import android.app.Activity
import android.content.SharedPreferences
import android.preference.PreferenceManager
import android.util.Log
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.BiometricConstants

private val TAG = FingerprintSharedPreferences::class.java.simpleName

class FingerprintSharedPreferences(var activity: Activity) {

    private val prefs: SharedPreferences =
        PreferenceManager.getDefaultSharedPreferences(activity.baseContext)

    fun getFidoUsername(): String {
        val fidoUsername = prefs.getString(activity.getString(R.string.fidoUsername), "")

        Log.i(TAG, "FIDO username extracted: $fidoUsername")

        return fidoUsername
    }

    fun saveFidoUsername(username: String) {
        storeInSharedPref(activity.getString(R.string.fidoUsername), username)
    }

    fun deleteFidoData() {
        removeFromSharedPref(activity.getString(R.string.fidoUsername))
        removeFromSharedPref(BiometricConstants.FIDO_REGISTERED)
        removeFromSharedPref(BiometricConstants.APP_ID)
        removeFromSharedPref(BiometricConstants.KEY_ID)
    }

    fun readStringFromSharedPref(key: String): String {
        return prefs.getString(key, null) ?: ""
    }

    fun storeInSharedPref(key: String, value: String) {
        prefs.edit().apply {
            putString(key, value)
            apply()
        }
    }

    fun getFingerprintRegisteredState(): Boolean {
        return prefs.getBoolean(BiometricConstants.FIDO_REGISTERED, false)
    }

    fun storeFingerprintState(registered: Boolean) {
        prefs.edit().apply {
            putBoolean(BiometricConstants.FIDO_REGISTERED, registered)
            apply()
        }
    }

    private fun removeFromSharedPref(key: String) {
        prefs.edit().apply {
            remove(key)
            apply()
        }
    }
}