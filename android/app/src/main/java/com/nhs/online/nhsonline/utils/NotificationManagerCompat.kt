package com.nhs.online.nhsonline.utils

import android.content.Context
import androidx.core.app.NotificationManagerCompat

class NotificationManagerCompat(private val context: Context) {
    fun areNotificationsEnabled(): Boolean {
        return NotificationManagerCompat.from(context).areNotificationsEnabled()
    }
}
