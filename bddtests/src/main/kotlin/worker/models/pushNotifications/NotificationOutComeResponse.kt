package worker.models.pushNotifications

data class NotificationOutComeResponse(val state:String,
                                       val enqueueTime:String,
                                       val startTime: String,
                                       val endTime: String,
                                       val pnsErrorDetailsUri: String,
                                       val platformOutcomes:List<PlatformOutCome>
                                    )
