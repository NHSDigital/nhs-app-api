package com.nhs.online.nhsonline.support

import android.content.Context
import android.preference.PreferenceManager


open class AppSharedPref(context: Context) {
    private val sharedPref = PreferenceManager.getDefaultSharedPreferences(context)

    fun readString(key: String, defaultValue: String? = null): String? {
        return sharedPref.getString(key, defaultValue)
    }

    fun storeString(key: String, value: String) {
        sharedPref.edit().apply {
            putString(key, value)
            apply()
        }
    }

    fun storeBoolean(key: String, value: Boolean) {
        sharedPref.edit().apply {
            putBoolean(key, value)
            apply()
        }
    }

    fun readBoolean(key: String, defaultValue: Boolean = false): Boolean {
        return sharedPref.getBoolean(key, defaultValue)
    }

    fun removeData(key: String) {
        sharedPref.edit().apply {
            remove(key)
            apply()
        }
    }
}