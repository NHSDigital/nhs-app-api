package com.nhs.online.nhsonline.services.knownservices.enums

import android.util.Log
import com.squareup.moshi.FromJson

private val TAG =  JavaScriptInteractionModeAdapter::class.java.simpleName

class JavaScriptInteractionModeAdapter {
    @FromJson fun fromJson(value: String): JavaScriptInteractionMode {
        return try {
            JavaScriptInteractionMode.valueOf(value)
        } catch(e: IllegalArgumentException) {
            Log.i(TAG, "An unknown JavaScriptInteractionMode enum has been encountered: $value")
            JavaScriptInteractionMode.Unknown
        }
    }
}