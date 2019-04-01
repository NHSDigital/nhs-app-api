package utils

import constants.DateTimeFormats
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

private const val NUMBER_OF_SECONDS_IN_A_MINUTE = 60

class TimeConverter {
    companion object {
        fun setDuration(startTime: String, endTime: String?): String {
            val format = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
            val startTimeAsLocalDateTime = ZonedDateTime.of(
                LocalDateTime.parse(startTime, format), ZoneId.of
                    ("Europe/London"))
            val endTimeAsLocalDateTime = ZonedDateTime.of(
                LocalDateTime.parse(endTime, format), ZoneId.of
                    ("Europe/London"))
            return (
                    (endTimeAsLocalDateTime.toEpochSecond() - startTimeAsLocalDateTime.toEpochSecond())
                            / NUMBER_OF_SECONDS_IN_A_MINUTE
                    ).toString() + " Minutes"
        }
    }
}