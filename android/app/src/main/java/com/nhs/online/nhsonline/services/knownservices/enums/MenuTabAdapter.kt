package com.nhs.online.nhsonline.services.knownservices.enums

import android.util.Log
import com.squareup.moshi.FromJson
import java.lang.IllegalArgumentException

private val TAG =  MenuTabAdapter::class.java.simpleName

class MenuTabAdapter {
    @FromJson fun fromJson(value: String): MenuTab {
        return try {
            MenuTab.valueOf(value)
        } catch(e: IllegalArgumentException) {
            Log.i(TAG, "An unknown MenuTab enum has been encountered: $value")
            MenuTab.Unknown
        }
    }
}