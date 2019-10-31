package com.nhs.online.nhsonline.services

import android.content.Context
import android.content.Intent
import android.net.Uri
import android.provider.Settings
import com.nhs.online.nhsonline.BuildConfig

class SettingsService(private val context: Context) {

    fun openSettings() {
        context.startActivity(Intent().apply {
            action = Settings.ACTION_APPLICATION_DETAILS_SETTINGS
            data = Uri.fromParts("package", BuildConfig.APPLICATION_ID , null)
        })
    }
}