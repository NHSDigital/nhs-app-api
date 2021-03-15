package worker.models.messages

data class MessageRequest(var sender: String,
                          var body: String,
                          var version: Int,
                          var communicationId: String? = null,
                          var transmissionId: String? = null,
                          var senderContext: SenderContext? = null)
