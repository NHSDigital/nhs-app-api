package mocking.data.patientPracticeMessaging

import constants.DateTimeFormats
import mocking.sharedModels.PatientPracticeMessagingSerenityHelpers
import mocking.sharedModels.ConversationDeletedResponse
import mocking.sharedModels.MessageDetails
import mocking.sharedModels.MessageReadStatusUpdateResponse
import mocking.sharedModels.MessageRecipientsResponseModel
import mocking.sharedModels.MessageReply
import mocking.sharedModels.MessageResponseModel
import mocking.sharedModels.MessagesResponseModel
import mocking.sharedModels.PatientMessageSummary
import mocking.sharedModels.Recipient
import utils.setIfNotAlreadySet
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.util.*

object EmisMessagingData {

    private val RECIPIENT_1 = Recipient("Dr. Dolittle", "1234-12345678-1234-1234-1")
    private val RECIPIENT_2 = Recipient("Dr. NHS Online", "1234-12345678-1234-1234-2")


    private fun getExpectedFormattedIndividualMessageDate(date: ZonedDateTime, format: MessageDateFormat): String {
        val formattedDate: String

        when (format) {
            MessageDateFormat.DETAILS_TODAY -> {
                formattedDate = DateTimeFormatter
                        .ofPattern("'Sent today at '${DateTimeFormats.frontendTimeFormat}", Locale.UK)
                        .format(date)
            }
            MessageDateFormat.DETAILS_TODAY_AT_MIDDAY -> { formattedDate = "Sent today at midday" }
            MessageDateFormat.DETAILS_TODAY_AT_MIDNIGHT -> { formattedDate = "Sent today at midnight" }
            MessageDateFormat.DETAILS_YESTERDAY -> {
                formattedDate = DateTimeFormatter
                        .ofPattern("'Sent yesterday at '${DateTimeFormats.frontendTimeFormat}", Locale.UK)
                        .format(date)
            }
            MessageDateFormat.DETAILS_YESTERDAY_AT_MIDDAY -> { formattedDate = "Sent yesterday at midday" }
            MessageDateFormat.DETAILS_YESTERDAY_AT_MIDNIGHT -> { formattedDate = "Sent yesterday at midnight" }
            else -> {
                formattedDate = DateTimeFormatter
                        .ofPattern("'Sent '${DateTimeFormats.frontendBasicDateFormat}'" +
                                " at '${DateTimeFormats.frontendTimeFormat}", Locale.UK)
                        .format(date)
            }
        }

        return formattedDate.replace("AM", "am").replace("PM", "pm")
    }

    fun getDefaultMessagesData(replyCount: Int = 0, hasUnreadReplies: Boolean = false): MessagesResponseModel {
        val messages = mutableListOf<PatientMessageSummary>()
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
            expectedDates.add(DateHelpers().getExpectedFormattedInboxMessageDate(date, inboxDateFormats[index]))
            messages.add(PatientMessageSummary(
                index.toString(),
                "GP Practice information $index",
                date.format(DateTimeFormatter.ofPattern(DateTimeFormats.emisMessageDateTimeFormat)),
                mutableListOf(RECIPIENT_1),
                replyCount,
                hasUnreadReplies
            ))
        })

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
            expectedDates.add(getExpectedFormattedIndividualMessageDate(date, replyDateFormats[index]))
            replies.add(MessageReply(
                isLegacy,
                isUnread,
                "Your blood test results have been updated.",
                RECIPIENT_1.name!!,
                date.format(DateTimeFormatter.ofPattern(DateTimeFormats.emisMessageDateTimeFormat))
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
                getExpectedFormattedIndividualMessageDate(sentDate, MessageDateFormat.DETAILS_BEFORE_YESTERDAY))

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

        val messageDetails = createdMessageDetails(initialMessageDate, replyDates, replyDateFormats, isUnread, isLegacy)

        return MessageResponseModel(messageDetails)
    }

    fun getDefaultMessageRecipients(): MessageRecipientsResponseModel {
        PatientPracticeMessagingSerenityHelpers
            .AVAILABLE_RECIPIENTS
            .setIfNotAlreadySet(arrayListOf(RECIPIENT_1, RECIPIENT_2))

        return MessageRecipientsResponseModel(
            arrayListOf(RECIPIENT_1, RECIPIENT_2, RECIPIENT_1)
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