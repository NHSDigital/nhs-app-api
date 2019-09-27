package worker.models.myrecord

import java.time.LocalDate
import java.time.format.DateTimeFormatter
import java.time.format.DateTimeParseException
import kotlin.Comparable

data class Date (val value: String, val datePart: String) : Comparable<Date> {

    override fun compareTo(other: Date): Int {
        var result = 0

        try {
            val thisDate =  LocalDate.parse(value, DateTimeFormatter. ISO_DATE)
            val otherDate =  LocalDate.parse(other.value, DateTimeFormatter. ISO_DATE)

            if (thisDate < otherDate) {
                result =  -1
            } else if (thisDate > otherDate) {
                result = 1
            }
        } catch (e: DateTimeParseException) {
            result = -1
        }
        return result
    }
}
