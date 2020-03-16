package mocking.data.patientPracticeMessaging

import constants.DateTimeFormats
import mocking.sharedModels.PatientPracticeMessagingSerenityHelpers
import mocking.sharedModels.Recipient
import mocking.tpp.models.Item
import mocking.tpp.models.Message
import mocking.tpp.models.MessageRecipientsReply
import mocking.tpp.models.MessagesViewReply
import utils.setIfNotAlreadySet
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

object TppMessagingData {
    private val RECIPIENT_1 = Recipient("Dr. Dolittle", "1234-12345678-1234-1234-1")
    private val RECIPIENT_2 = Recipient("Dr. NHS Online", "1234-12345678-1234-1234-2")

    fun getDefaultTppMessages(hasUnread: Boolean = false): MessagesViewReply {
        val messages = mutableListOf<Message>()
        val expectedDates = mutableListOf<String>()
        val todaysDate = ZonedDateTime.now(ZoneId.of("Europe/London")).withMinute(TWELVE)

        val inboxDates = mutableListOf(
                todaysDate,
                todaysDate.withHour(TWELVE).withMinute(ZERO),
                todaysDate.withHour(ZERO).withMinute(ZERO),
                todaysDate.minusDays(ONE),
                todaysDate.minusDays(TWO),
                todaysDate.minusDays(SEVEN)
        )
        val inboxDateFormats = mutableListOf(
                MessageDateFormat.INBOX_TIME_12_HR,
                MessageDateFormat.INBOX_MIDDAY,
                MessageDateFormat.INBOX_MIDNIGHT,
                MessageDateFormat.INBOX_YESTERDAY,
                MessageDateFormat.INBOX_LAST_WEEK,
                MessageDateFormat.INBOX_BEFORE_LAST_WEEK
        )

        inboxDates.forEachIndexed(fun (index, date) {
            val read = if (hasUnread) "n" else "y"
            expectedDates.add(DateHelpers().getExpectedFormattedInboxMessageDate(date, inboxDateFormats[index]))
            messages.add(Message(
                    messageId = index.toString(),
                    conversationId = index.toString(),
                    messageText = "GP Practice information $index",
                    sent = date.format(DateTimeFormatter.ofPattern(DateTimeFormats.tppDateTimeFormat)),
                    sender = "Gp Test Practice",
                    recipient = "Recipient1",
                    read = read
             ))
        })

        PatientPracticeMessagingSerenityHelpers.EXPECTED_INBOX_MESSAGE_DATES.setIfNotAlreadySet(expectedDates)
        return MessagesViewReply(Message = messages)
    }

    fun getDefaultTppRecipients() : MessageRecipientsReply {
        val items = mutableListOf<Item>();
        items.add(Item (
                id = RECIPIENT_1.recipientIdentifier!!,
                value = RECIPIENT_1.name!!
        ))
        items.add(Item (
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

    fun getTppEmptyRecipients() : MessageRecipientsReply {

        return MessageRecipientsReply(
                Item = mutableListOf()
        )
    }
}