package worker.models.messages

data class SingleMessageFacade(
        val sender: String,
        val body: String,
        val read: Boolean,
        val sentTime: String
)
