package com.nhs.online.nhsonline.utils

import android.content.Context
import android.support.v4.app.NotificationManagerCompat

class NotificationManagerCompat(private val context: Context) {
    fun areNotificationsEnabled(): Boolean {
        return NotificationManagerCompat.from(context).areNotificationsEnabled()
    }
}