package worker.models.messages

data class SenderFacade(val name:String,
                        val unreadCount:String,
                        val messages: List<SingleMessageFacade>)


