package worker.models.messages

data class MessagesSummaryFacade(val sender:String,
                                 val unreadCount:Int,
                                 val messages: List<SingleMessageFacade>,
                                 val lastMessageTime: String? = null)


