package worker.models.messages

data class SenderFacade(val name:String,
                        val unreadCount:Int,
                        val messages: List<SingleMessageFacade>)


