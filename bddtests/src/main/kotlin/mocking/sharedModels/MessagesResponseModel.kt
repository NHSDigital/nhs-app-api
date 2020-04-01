package mocking.sharedModels

data class MessagesResponseModel(
        var messages: List<PatientMessageSummary> = arrayListOf()
)

data class PatientMessageSummary (
        var messageId: String,
        var subject: String,
        var sentDateTime: String?,
        var recipients: List<Recipient>,
        var replyCount: Int,
        var hasUnreadReplies: Boolean
)

data class MessageResponseModel(
        var Message : MessageDetails
)

data class MessageDetails (
        var messageId: String,
        var subject: String,
        var recipients: List<Recipient>,
        var messageReplies: List<MessageReply> = arrayListOf(),
        var content: String,
        var sentDateTime: String,
        var clientApplicationName: String
)

data class MessageReply(
        var isLegacy: Boolean,
        var isUnread: Boolean,
        var replyContent: String,
        var sender: String,
        var sentDateTime: String
)

data class MessageRecipientsResponseModel(
        var MessageRecipients: List<RecipientResponse> = arrayListOf()
)

data class Recipient (
        var name: String?,
        var recipientIdentifier: String? = null
)

data class RecipientResponse (
        var name: String?,
        var recipientGuid: String? = null
)

data class MessageReadStatusUpdateResponse (
        var MessageReadStateUpdateStatus: String
)

data class ConversationDeletedResponse (
        var IsDeleted: Boolean
)