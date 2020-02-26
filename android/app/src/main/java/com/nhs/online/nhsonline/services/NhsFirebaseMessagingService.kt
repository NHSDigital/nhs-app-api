package com.nhs.online.nhsonline.services

import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.PendingIntent
import android.content.Context
import android.content.Intent
import android.os.Build
import android.support.v4.app.NotificationCompat
import android.support.v4.app.NotificationManagerCompat
import android.support.v4.content.ContextCompat
import com.google.firebase.messaging.FirebaseMessagingService
import com.google.firebase.messaging.RemoteMessage
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.MainActivity

class NhsFirebaseMessagingService : FirebaseMessagingService() {

    companion object {
        private const val CHANNEL_ID: String = "101"
    }

    override fun onMessageReceived(remoteMessage: RemoteMessage) {
        remoteMessage.notification.let {
            createNotificationChannel("NHS Notifications", "Notifications from the NHS.")
            val builder = buildNotification(remoteMessage)

            val notificationId = (System.currentTimeMillis() / 1000).toInt()
            sendNotification(notificationId, builder)
        }
    }

    private fun buildIntent(remoteMessage: RemoteMessage): PendingIntent {
        val requestCode = System.currentTimeMillis().toInt()

        val intent = Intent(this, MainActivity::class.java).apply {
            flags = Intent.FLAG_ACTIVITY_NEW_TASK
            remoteMessage.data.let {data -> putExtra("url", data.get("url")) }
        }
        return PendingIntent.getActivity(this, requestCode, intent, 0)
    }

    private fun createNotificationChannel(channelName: String, channelDescription: String) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            val importance = NotificationManager.IMPORTANCE_DEFAULT
            val channel = NotificationChannel(CHANNEL_ID, channelName, importance).apply {
                description = channelDescription
            }

            val notificationManager: NotificationManager =
                    getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager
            notificationManager.createNotificationChannel(channel)
        }
    }

    private fun buildNotification(remoteMessage: RemoteMessage): NotificationCompat.Builder {
        val pendingIntent = buildIntent(remoteMessage)

        return NotificationCompat.Builder(this, CHANNEL_ID)
                .setSmallIcon(R.drawable.icon_drawable_nhs_logo)
                .setColor(ContextCompat.getColor(this, R.color.colorPrimary))
                .setContentTitle(remoteMessage.notification?.title)
                .setContentText(remoteMessage.notification?.body)
                .setStyle(NotificationCompat.BigTextStyle()
                        .bigText(remoteMessage.notification?.body))
                .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                .setContentIntent(pendingIntent)
                .setAutoCancel(true)
    }

    private fun sendNotification(notificationId: Int, builder: NotificationCompat.Builder) {
        with(NotificationManagerCompat.from(this)) {
            notify(Build.PRODUCT ,notificationId, builder.build())
        }
    }
}