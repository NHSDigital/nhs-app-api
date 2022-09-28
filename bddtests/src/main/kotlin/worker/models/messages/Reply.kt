package worker.models.messages

data class Reply(var options: List<ReplyOption>? = null,
                 var response: String? = null,
                 var responseSentDateTime: String? = null)
