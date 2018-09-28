package utils

import java.time.LocalDate
import java.time.format.DateTimeFormatter
import java.util.*

class DateConverter
{
    companion object {
        fun convertDateToDateTimeFormat(dateString: String, fromFormat: String, toFormat: String): String {
            val date = LocalDate.parse(dateString, DateTimeFormatter.ofPattern(fromFormat, Locale.ENGLISH))
            return date.atStartOfDay().format(DateTimeFormatter.ofPattern(toFormat, Locale.ENGLISH));
        }
    }
}