package mocking.emis.patientPracticeMessaging

data class MessagesResponseModel(
        var messages: List<PatientMessageSummary> = arrayListOf()
)

data class MessageResponseModel(
        var Message : MessageDetails
)

data class PatientMessageSummary (
        var messageId: Int,
        var subject: String,
        var sentDateTime: String,
        var recipients: List<Recipient>,
        var replyCount: Int,
        var hasUnreadReplies: Boolean
)

data class Recipient (
        var name: String
)

data class MessageDetails (
        var messageid: String,
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