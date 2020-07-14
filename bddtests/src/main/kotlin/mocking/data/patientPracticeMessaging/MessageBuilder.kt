package mocking.data.patientPracticeMessaging

import constants.DateTimeFormats
import mocking.tpp.models.Message
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

class MessageBuilder(private val messageId: String,
                     private val conversationId: String,
                     private val date: ZonedDateTime) {

    private var read: String = "y"
    private var binaryDataId: String? = null
    private var incoming: String = "n"

    fun isRead(value: String): MessageBuilder {
        read = value
        return this
    }

    fun binaryDataId(value: String?): MessageBuilder {
        binaryDataId = value
        return this
    }

    fun isIncoming(value: String): MessageBuilder {
        incoming = value
        return this
    }

    fun build(): Message {
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
}
