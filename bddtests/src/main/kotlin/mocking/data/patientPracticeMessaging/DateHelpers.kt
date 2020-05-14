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

    private val dateFormatting = mapOf(
            MessageDateFormat.INBOX_TIME_12_HR to DateTimeFormats.frontendTimeFormat,
            MessageDateFormat.DETAILS_TODAY to "'Sent today at '${DateTimeFormats.frontendTimeFormat}",
            MessageDateFormat.INBOX_MIDDAY to "'Midday'",
            MessageDateFormat.DETAILS_TODAY_AT_MIDDAY to "'Sent today at midday'",
            MessageDateFormat.INBOX_MIDNIGHT to "'Midnight'",
            MessageDateFormat.DETAILS_TODAY_AT_MIDNIGHT to "'Sent today at midnight'",
            MessageDateFormat.INBOX_YESTERDAY to "'Yesterday'",
            MessageDateFormat.DETAILS_YESTERDAY to "'Sent yesterday at '${DateTimeFormats.frontendTimeFormat}",
            MessageDateFormat.INBOX_LAST_WEEK to DateTimeFormats.fullDayOfWeek,
            MessageDateFormat.INBOX_BEFORE_LAST_WEEK to DateTimeFormats.frontendBasicDateFormat,
            MessageDateFormat.DETAILS_YESTERDAY_AT_MIDDAY to "'Sent yesterday at midday'",
            MessageDateFormat.DETAILS_YESTERDAY_AT_MIDNIGHT to "'Sent yesterday at midnight'")

    fun getExpectedFormattedMessageDate(date: ZonedDateTime, format: MessageDateFormat): String {
        val dateFormat = getFormat(format)
        val formattedDate = DateTimeFormatter
                .ofPattern(dateFormat, Locale.UK)
                .format(date)
        return formattedDate.replace("AM", "am").replace("PM", "pm")
    }

    private fun getFormat(format: MessageDateFormat): String {
        if (dateFormatting.containsKey(format)) {
            return dateFormatting[format]!!
        }
        return "'Sent '${DateTimeFormats.frontendBasicDateFormat}'" +
                " at '${DateTimeFormats.frontendTimeFormat}"
    }
}