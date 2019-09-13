package com.nhs.online.nhsonline.services

import com.nhs.online.nhsonline.clients.FirebaseClient
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import java.util.logging.Level
import java.util.logging.Logger

class NotificationsService(
        private val appWebInterface: AppWebInterface,
        private val firebaseClient: FirebaseClient
) {
    private val logger = Logger.getLogger(NotificationsService::class.java.simpleName)

    fun registerForPushNotifications() {
        try {
            firebaseClient.instanceId.addOnSuccessListener { instanceIdResult ->
                val FCM_token = instanceIdResult.token
                logger.log(Level.INFO, "FCM Registration Token: $FCM_token")
                appWebInterface.notificationsAuthorised(FCM_token)
            }
        } catch (e: java.lang.Exception) {
            logger.log(Level.WARNING, "Failed to register for notifications", e)
            appWebInterface.notificationsUnauthorised()
        }
    }

}