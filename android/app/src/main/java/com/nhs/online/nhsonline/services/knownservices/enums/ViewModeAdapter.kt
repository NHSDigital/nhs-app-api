package com.nhs.online.nhsonline.services.knownservices.enums

import android.util.Log
import com.squareup.moshi.FromJson
import java.lang.IllegalArgumentException

private val TAG =  ViewModeAdapter::class.java.simpleName

class ViewModeAdapter {
    @FromJson fun fromJson(value: String): ViewMode {
        return try {
            ViewMode.valueOf(value)
        } catch(e: IllegalArgumentException) {
            Log.v(TAG, "An unknown ViewMode enum has been encountered: $value")
            ViewMode.Unknown
        }
    }
}