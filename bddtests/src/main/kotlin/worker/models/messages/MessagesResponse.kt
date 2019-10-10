package worker.models.messages

data class MessagesResponse(val sender:String,
                            val unreadCount:Int,
                            val messages: Array<MessagesResponseMessage>)


