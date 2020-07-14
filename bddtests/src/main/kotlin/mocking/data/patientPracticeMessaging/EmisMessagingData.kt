package mocking.data.patientPracticeMessaging

import constants.DateTimeFormats
import mocking.patientPracticeMessaging.ConversationDeletedResponse
import mocking.patientPracticeMessaging.MessageDetails
import mocking.patientPracticeMessaging.MessageReadStatusUpdateResponse
import mocking.patientPracticeMessaging.MessageRecipientsResponseModel
import mocking.patientPracticeMessaging.MessageReply
import mocking.patientPracticeMessaging.MessageResponseModel
import mocking.patientPracticeMessaging.MessagesResponseModel
import mocking.patientPracticeMessaging.PatientMessageSummary
import mocking.patientPracticeMessaging.Recipient
import mocking.patientPracticeMessaging.RecipientResponse
import mocking.patientPracticeMessaging.PatientPracticeMessagingSerenityHelpers
import mocking.patientPracticeMessaging.DateAndFormat
import utils.set
import utils.setIfNotAlreadySet
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.util.*

object EmisMessagingData {
    private const val MESSAGE_ID_OFFSET = 10

    private val RECIPIENT_RESPONSE_1 = RecipientResponse("Dr. Dolittle", "1234-12345678-1234-1234-1")
    private val RECIPIENT_RESPONSE_2 = RecipientResponse("Dr. NHS Online", "1234-12345678-1234-1234-2")
    private val RECIPIENT_1 = Recipient("Dr. Dolittle", "1234-12345678-1234-1234-1")
    private val RECIPIENT_2 = Recipient("Dr. NHS Online", "1234-12345678-1234-1234-2")

    fun getDefaultMessagesData(replyCount: Int = 0, hasUnreadReplies: Boolean = false): MessagesResponseModel {
        PatientPracticeMessagingSerenityHelpers
                .INITIAL_FROM_GP
                .setIfNotAlreadySet( false )
        val messages = mutableListOf<PatientMessageSummary>()
        val expectedDates = mutableListOf<String>()
        val todaysDate = ZonedDateTime.now(ZoneId.of("Europe/London")).withMinute(TWELVE)

        val inboxDates = mutableListOf(
                DateAndFormat(todaysDate,
                        MessageDateFormat.INBOX_TIME_12_HR),
                DateAndFormat(todaysDate.withHour(TWELVE).withMinute(ZERO),
                        MessageDateFormat.INBOX_MIDDAY),
                DateAndFormat(todaysDate.withHour(ZERO).withMinute(ZERO),
                        MessageDateFormat.INBOX_MIDNIGHT),
                DateAndFormat(todaysDate.minusDays(ONE),
                        MessageDateFormat.INBOX_YESTERDAY),
                DateAndFormat(todaysDate.minusDays(TWO),
                        MessageDateFormat.INBOX_LAST_WEEK),
                DateAndFormat(todaysDate.minusDays(SEVEN),
                        MessageDateFormat.INBOX_BEFORE_LAST_WEEK)
        )

        inboxDates.forEachIndexed(fun (index, dateObject) {
            val messageId = "${index + MESSAGE_ID_OFFSET}"

            expectedDates.add(DateHelpers().getExpectedFormattedMessageDate(dateObject.date, dateObject.format))
            messages.add(PatientMessageSummary(
                messageId,
                "GP Practice information $messageId",
                dateObject.date.format(DateTimeFormatter.ofPattern(DateTimeFormats.emisMessageDateTimeFormat)),
                mutableListOf(RECIPIENT_1),
                replyCount,
                hasUnreadReplies
            ))
        })

        PatientPracticeMessagingSerenityHelpers.INITIAL_MESSAGE_ID.set("$MESSAGE_ID_OFFSET")

        PatientPracticeMessagingSerenityHelpers.EXPECTED_INBOX_MESSAGE_DATES.setIfNotAlreadySet(expectedDates)
        return MessagesResponseModel(messages)
    }

    private fun createdMessageDetails(
            sentDate: ZonedDateTime,
            replyDates: List<ZonedDateTime>,
            replyDateFormats: List<MessageDateFormat>,
            isUnread: Boolean,
            isLegacy: Boolean): MessageDetails {
        val replies = mutableListOf<MessageReply>()
        val expectedDates = mutableListOf<String>()

        replyDates.forEachIndexed(fun (index, date) {
            expectedDates.add(DateHelpers().getExpectedFormattedMessageDate(date, replyDateFormats[index]))
            replies.add(MessageReply(
                isLegacy,
                isUnread,
                "Your blood test results have been updated.",
                RECIPIENT_1.name!!,
                date.format(DateTimeFormatter.ofPattern(DateTimeFormats.emisMessageDateTimeFormat)),
                    true
            ))
        })

        if (isUnread) {
            PatientPracticeMessagingSerenityHelpers
                .EXPECTED_UNREAD_MESSAGE_REPLY_DATES
                .setIfNotAlreadySet(expectedDates)
        } else {
            PatientPracticeMessagingSerenityHelpers
                .EXPECTED_READ_MESSAGE_REPLY_DATES
                .setIfNotAlreadySet(expectedDates)
        }

        PatientPracticeMessagingSerenityHelpers
            .EXPECTED_MESSAGE_SENT_DATE
            .setIfNotAlreadySet(
                DateHelpers().getExpectedFormattedMessageDate(sentDate, MessageDateFormat.DETAILS_BEFORE_YESTERDAY))

        return MessageDetails(
                "1",
                "GP Practice Information",
                mutableListOf(RECIPIENT_1),
                replies, "When will my blood test results be ready?",
                sentDate.format(DateTimeFormatter.ofPattern(DateTimeFormats.emisMessageDateTimeFormat)),
                "NHS App Messaging")
    }

    fun getDefaultMessageDetailsWithReplies(
        isUnread: Boolean = false,
        isLegacy: Boolean = false
    ): MessageResponseModel {
        PatientPracticeMessagingSerenityHelpers
                .INITIAL_FROM_GP
                .setIfNotAlreadySet( false )
        val todaysDate = ZonedDateTime.now(ZoneId.of("Europe/London")).withHour(ZERO).withMinute(TWELVE)
        val initialMessageDate = todaysDate.minusDays(THREE)

        val replyDates = mutableListOf(
            todaysDate.withHour(THIRTEEN).minusDays(TWO),
            todaysDate.withHour(ZERO).withMinute(ZERO).minusDays(ONE),
            todaysDate.withHour(TWELVE).withMinute(ZERO).minusDays(ONE),
            todaysDate.minusDays(ONE),
            todaysDate.withHour(ZERO).withMinute(ZERO),
            todaysDate.withHour(TWELVE).withMinute(ZERO),
            todaysDate
        )

        val replyDateFormats = mutableListOf(
            MessageDateFormat.DETAILS_BEFORE_YESTERDAY,
            MessageDateFormat.DETAILS_YESTERDAY_AT_MIDNIGHT,
            MessageDateFormat.DETAILS_YESTERDAY_AT_MIDDAY,
            MessageDateFormat.DETAILS_YESTERDAY,
            MessageDateFormat.DETAILS_TODAY_AT_MIDNIGHT,
            MessageDateFormat.DETAILS_TODAY_AT_MIDDAY,
            MessageDateFormat.DETAILS_TODAY
        )

        val messageDetails =
                createdMessageDetails(initialMessageDate, replyDates, replyDateFormats, isUnread, isLegacy)

        return MessageResponseModel(messageDetails)
    }

    fun getDefaultMessageRecipients(): MessageRecipientsResponseModel {
        PatientPracticeMessagingSerenityHelpers
            .AVAILABLE_RECIPIENTS
            .setIfNotAlreadySet(arrayListOf(RECIPIENT_1, RECIPIENT_2))

        return MessageRecipientsResponseModel(
            arrayListOf(RECIPIENT_RESPONSE_1, RECIPIENT_RESPONSE_2, RECIPIENT_RESPONSE_1)
        )
    }

    fun getEmptyRecipients() : MessageRecipientsResponseModel {
        return MessageRecipientsResponseModel()
    }

    fun getUpdatedResponse(): MessageReadStatusUpdateResponse {
        return MessageReadStatusUpdateResponse("Updated")
    }

    fun getDeleteResponse(): ConversationDeletedResponse {
        return ConversationDeletedResponse(true)
    }
}
