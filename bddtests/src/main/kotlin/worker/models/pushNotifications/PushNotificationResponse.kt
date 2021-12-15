package worker.models.pushNotifications

data class PushNotificationResponse(val notificationId:String,
                                    val trackingId:String,
                                    val scheduled: Boolean)
