package worker.models.userDevices

import worker.models.messages.SenderContext

data class NotificationSendRequest(
        var title:String,
        var subtitle: String,
        var body: String?,
        var url: String,
        var senderContext: SenderContext? = null
)
