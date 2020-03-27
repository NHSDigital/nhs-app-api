package models

data class ExpectedMessage(
        var id: String,
        var conversationId: String? = null,
        var subject: String? = null,
        var messageText: String? = null,
        var lastMessageDateTime: String,
        var recipient: String,
        var sender: String? = null,
        var attachmentId: String? = null,
        var hasUnreadReplies: Boolean? = null,
        var unreadCount: Int? = null
)