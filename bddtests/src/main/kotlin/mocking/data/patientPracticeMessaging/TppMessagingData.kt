package mocking.data.patientPracticeMessaging

import constants.DateTimeFormats
import mocking.emis.models.PatientPracticeMessagingSerenityHelpers
import mocking.tpp.models.Message
import mocking.tpp.models.MessagesViewReply
import utils.setIfNotAlreadySet
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

object TppMessagingData {
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
}