package mocking.data.patientPracticeMessaging

import constants.DateTimeFormats
import mocking.patientPracticeMessaging.MessageDetails
import mocking.patientPracticeMessaging.MessageReply
import mocking.patientPracticeMessaging.Recipient
import mocking.tpp.models.Message
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

class MessageHelpers {

    fun createMessage(messageId: String,
                      conversationId: String,
                      date: ZonedDateTime,
                      read: String = "y",
                      binaryDataId: String? = null,
                      incoming: String = "n"): Message {
        return Message(
                messageId = messageId,
                conversationId = conversationId,
                messageText = "GP Practice information 0",
                sent = date.format(DateTimeFormatter.ofPattern(DateTimeFormats
                        .tppDateTimeFormat)),
                sender = "Gp Test Practice",
                recipient = "Recipient1",
                read = read,
                binaryDataId = binaryDataId,
                incoming = incoming
        )
    }

    fun createMessageReply(sender: String,
                           sentDateTime: String,
                           isUnread: Boolean?,
                           replyContent: String,
                           outboundMessage: Boolean,
                           isLegacy: Boolean? = null): MessageReply {
        return MessageReply(
                sender = sender,
                sentDateTime = sentDateTime,
                isUnread = isUnread,
                replyContent = replyContent,
                outboundMessage = outboundMessage,
                isLegacy = isLegacy
        )
    }

    fun createMessageDetails(messageId: String,
                             recipients: List<Recipient>,
                             messageReplies: List<MessageReply> = arrayListOf(),
                             content: String,
                             sentDateTime: String,
                             subject: String? = null,
                             clientApplicationName: String? = null): MessageDetails {
        return MessageDetails(
                messageId = messageId,
                recipients = recipients,
                messageReplies = messageReplies,
                content = content,
                sentDateTime = sentDateTime,
                subject = subject,
                clientApplicationName = clientApplicationName)
    }
}