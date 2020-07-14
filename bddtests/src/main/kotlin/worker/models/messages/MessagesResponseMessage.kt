package worker.models.messages

data class MessagesResponseMessage(
        val id: String,
        val sender: String,
        val version: Int,
        val body: String,
        val read: String?,
        val sentTime: String
)
