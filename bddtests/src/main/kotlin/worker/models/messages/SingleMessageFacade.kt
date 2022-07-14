package worker.models.messages

data class SingleMessageFacade(
    val id: String,
    val sender: String,
    val body: String,
    val read: Boolean,
    val sentTime: String,
    val version: Int,
    val senderContext: SenderContext?
)
