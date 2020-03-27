package mocking.data.patientPracticeMessaging

import constants.DateTimeFormats
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.util.*

enum class MessageDateFormat {
    INBOX_MIDDAY,
    INBOX_MIDNIGHT,
    INBOX_YESTERDAY,
    INBOX_TIME_12_HR,
    INBOX_LAST_WEEK,
    INBOX_BEFORE_LAST_WEEK,
    DETAILS_TODAY,
    DETAILS_TODAY_AT_MIDDAY,
    DETAILS_TODAY_AT_MIDNIGHT,
    DETAILS_YESTERDAY,
    DETAILS_YESTERDAY_AT_MIDDAY,
    DETAILS_YESTERDAY_AT_MIDNIGHT,
    DETAILS_BEFORE_YESTERDAY
}

const val ZERO = 0
const val TWELVE = 12
const val THIRTEEN = 13
const val ONE = 1L
const val TWO = 2L
const val THREE = 3L
const val SEVEN = 7L

class DateHelpers {

    fun getExpectedFormattedInboxMessageDate(date: ZonedDateTime, format: MessageDateFormat): String {
        val formattedDate: String

        when (format) {
            MessageDateFormat.INBOX_TIME_12_HR -> {
                formattedDate = DateTimeFormatter
                        .ofPattern(DateTimeFormats.frontendTimeFormat, Locale.UK)
                        .format(date)
            }
            MessageDateFormat.INBOX_MIDDAY -> { formattedDate = "Midday" }
            MessageDateFormat.INBOX_MIDNIGHT -> { formattedDate = "Midnight" }
            MessageDateFormat.INBOX_YESTERDAY -> { formattedDate = "Yesterday" }
            MessageDateFormat.INBOX_LAST_WEEK -> {
                formattedDate = DateTimeFormatter
                        .ofPattern(DateTimeFormats.fullDayOfWeek, Locale.UK)
                        .format(date)
            }
            MessageDateFormat.INBOX_BEFORE_LAST_WEEK -> {
                formattedDate = DateTimeFormatter
                        .ofPattern(DateTimeFormats.frontendBasicDateFormat, Locale.UK)
                        .format(date)
            }
            else -> {
                formattedDate = DateTimeFormatter
                        .ofPattern("'Sent '${DateTimeFormats.frontendBasicDateFormat}'" +
                                           " at '${DateTimeFormats.frontendTimeFormat}", Locale.UK)
                        .format(date)
            }
        }

        return formattedDate.replace("AM", "am").replace("PM", "pm")
    }
}