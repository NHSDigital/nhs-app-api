package worker.models.pushNotifications

data class PushNotificationResponse(val notificationId:String,
                                    val scheduled:Boolean,
                                    val hubPath:String)
