package mocking.data.patientPracticeMessaging

import mocking.patientPracticeMessaging.MessageResponseModel
import mocking.patientPracticeMessaging.Recipient
import mocking.patientPracticeMessaging.PatientPracticeMessagingSerenityHelpers
import mocking.patientPracticeMessaging.DateAndFormat
import mocking.tpp.models.Item
import mocking.tpp.models.Message
import mocking.tpp.models.MessageRecipientsReply
import mocking.tpp.models.MessagesViewReply
import utils.set
import utils.setIfNotAlreadySet
import java.time.ZoneId
import java.time.ZonedDateTime
import java.util.*

object TppMessagingData {

    private val RECIPIENT_1 = Recipient("Dr. Dolittle", "1234-12345678-1234-1234-1")
    private val RECIPIENT_2 = Recipient("Dr. NHS Online", "1234-12345678-1234-1234-2")

    fun getDefaultTppMessages(hasUnread: Boolean = false, hasAttachment: Boolean = false): MessagesViewReply {
        PatientPracticeMessagingSerenityHelpers
                .INITIAL_FROM_GP
                .setIfNotAlreadySet( false )
        val messages = mutableListOf<Message>()
        val todaysDate = ZonedDateTime.now(ZoneId.of("UTC")).withMinute(TWELVE)

        val expectedDates = mutableListOf(
                DateAndFormat(todaysDate.minusDays(TWO),
                        MessageDateFormat.INBOX_TIME_12_HR)
        )

        val expectedReplyDates = mutableListOf(
                DateAndFormat(todaysDate.withHour(TWELVE).withMinute(ZERO),
                        MessageDateFormat.DETAILS_TODAY_AT_MIDDAY),
                DateAndFormat(todaysDate,
                        MessageDateFormat.DETAILS_TODAY),
                DateAndFormat(todaysDate.withHour(ZERO).withMinute(ZERO),
                        MessageDateFormat.DETAILS_TODAY_AT_MIDNIGHT),
                DateAndFormat(todaysDate.minusDays(ONE),
                        MessageDateFormat.DETAILS_YESTERDAY)
        )

        val conversationId = UUID.randomUUID().toString()

        val repliesDates = mutableListOf<String>()

        messages.add(MessageHelpers().createMessage(
                conversationId,
                conversationId,
                expectedDates[0].date,
                "y",
                incoming="y")
        )

        PatientPracticeMessagingSerenityHelpers.INITIAL_MESSAGE_ID.set(
                conversationId)

        expectedReplyDates.forEachIndexed(fun (_, dateObject) {
                val read = if (hasUnread) "n" else "y"

                repliesDates.add(DateHelpers().getExpectedFormattedMessageDate(dateObject.date, dateObject.format))

                messages.add(MessageHelpers().createMessage(
                        UUID.randomUUID().toString(),
                        conversationId,
                        dateObject.date,
                        read,
                        incoming ="n",
                        binaryDataId = if (hasAttachment) "123456433546" else null)
                )
                if (read === "y") {
                    PatientPracticeMessagingSerenityHelpers
                            .EXPECTED_READ_MESSAGE_REPLY_DATES
                            .set(repliesDates)
                } else {
                    PatientPracticeMessagingSerenityHelpers
                            .EXPECTED_UNREAD_MESSAGE_REPLY_DATES
                            .setIfNotAlreadySet(repliesDates)
                }
            })

        PatientPracticeMessagingSerenityHelpers.EXPECTED_INBOX_MESSAGE_DATES.set(
                    expectedDates)

        PatientPracticeMessagingSerenityHelpers.EXPECTED_REPLY_MESSAGE_DATES.set(
                expectedReplyDates)

        PatientPracticeMessagingSerenityHelpers
                .EXPECTED_MESSAGE_SENT_DATE
                .setIfNotAlreadySet(
                        DateHelpers().getExpectedFormattedMessageDate(
                                expectedDates[0].date, MessageDateFormat.DETAILS_BEFORE_YESTERDAY))

            return MessagesViewReply(Message = messages)
        }

    fun getTppMessagesInitialFromGp (): MessagesViewReply {
        val messages = mutableListOf<Message>()
        val todaysDate = ZonedDateTime.now(ZoneId.of("UTC")).withMinute(TWELVE)

        val expectedReplyDates = mutableListOf(
                DateAndFormat(todaysDate.minusDays(TWO),
                        MessageDateFormat.INBOX_LAST_WEEK)
        )

        val expectedInboxDates = mutableListOf(
                DateAndFormat(todaysDate.minusDays(TWO),
                              MessageDateFormat.INBOX_LAST_WEEK))

        val conversationId = UUID.randomUUID().toString()

        messages.add(MessageHelpers().createMessage(
                conversationId,
                conversationId,
                expectedInboxDates[0].date,
                "y",
                incoming = "n")
        )

        PatientPracticeMessagingSerenityHelpers.INITIAL_MESSAGE_ID.set(
                conversationId)

        PatientPracticeMessagingSerenityHelpers.EXPECTED_INBOX_MESSAGE_DATES.set(
                expectedInboxDates)

        PatientPracticeMessagingSerenityHelpers.EXPECTED_REPLY_MESSAGE_DATES.set(
                expectedReplyDates)

        PatientPracticeMessagingSerenityHelpers
                .EXPECTED_MESSAGE_SENT_DATE
                .setIfNotAlreadySet(
                        DateHelpers().getExpectedFormattedMessageDate(
                                expectedInboxDates[0].date, MessageDateFormat.DETAILS_BEFORE_YESTERDAY))

        PatientPracticeMessagingSerenityHelpers
                .INITIAL_FROM_GP
                .setIfNotAlreadySet( true )
        return MessagesViewReply(Message = messages)
    }

    fun getDefaultSelected(message: Message, replies: List<Message>): MessageResponseModel {
        val replyList = replies.filter{
            messageObject -> messageObject.conversationId != messageObject.messageId}
                .map { MessageHelpers().createMessageReply(
                        it.sender,
                        it.sent,
                        it.read !== "y",
                        it.messageText,
                        it.incoming == "y",
                        null
                ) }

        val messageDetails = MessageHelpers().createMessageDetails(
                message.messageId,
                listOf(Recipient(message.sender)),
                replyList,
                message.messageText,
                message.sent,
                null,
                null)

        return MessageResponseModel(messageDetails)
    }

    fun getDefaultTppRecipients(): MessageRecipientsReply {
        val items = mutableListOf<Item>()
        items.add(Item(
                id = RECIPIENT_1.recipientIdentifier!!,
                value = RECIPIENT_1.name!!
        ))
        items.add(Item(
                id = RECIPIENT_2.recipientIdentifier!!,
                value = RECIPIENT_2.name!!
        ))

        PatientPracticeMessagingSerenityHelpers
                .AVAILABLE_RECIPIENTS
                .setIfNotAlreadySet(arrayListOf(RECIPIENT_1, RECIPIENT_2))

        return MessageRecipientsReply(
                Item = items
        )
    }

    fun getTppEmptyRecipients(): MessageRecipientsReply {

        return MessageRecipientsReply(
                Item = mutableListOf()
        )
    }
}