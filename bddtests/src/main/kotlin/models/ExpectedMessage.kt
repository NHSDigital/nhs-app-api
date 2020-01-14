package models

data class ExpectedMessage(
        var id: Int,
        var subject: String,
        var lastMessageDateTime: String,
        var recipient: String,
        var hasUnreadReplies: Boolean
)