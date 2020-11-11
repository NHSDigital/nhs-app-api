package com.nhs.online.nhsonline.web

import android.os.Build.*
import com.nhs.online.nhsonline.BuildConfig.VERSION_NAME

object UserAgentBuilder {
    private fun buildKvp(key: String, value: String) : String {
        return "nhsapp-$key/$value"
    }

    fun buildUserAgentString(userAgent: String): String {
        val nhsAndroidUserAgent = buildKvp("android", VERSION_NAME)

        val manufacturer = buildKvp("manufacturer", MANUFACTURER)
        val model = buildKvp("model", MODEL)

        val os = buildKvp("os", VERSION.RELEASE)
        val architecture = buildKvp("architecture", SUPPORTED_ABIS.joinToString(","))

        return "$userAgent $nhsAndroidUserAgent $manufacturer $model $os $architecture"
    }
}
