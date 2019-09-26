package worker.models.messages

data class MessageRequest(var sender: String,
                          var body: String,
                          var version: Int)