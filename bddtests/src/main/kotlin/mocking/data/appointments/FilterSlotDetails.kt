package mocking.data.appointments

import constants.DateTimeFormats
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.format.DateTimeFormatter

class FilterSlotDetails(private var dateAsLocalDateTime: LocalDateTime,
                        hoursOverride: Int? = null,
                        minutesOverride: Int? = null) {

    init {
        hoursOverride?.let { this.dateAsLocalDateTime = this.dateAsLocalDateTime.withHour(it) }
        minutesOverride?.let { this.dateAsLocalDateTime = this.dateAsLocalDateTime.withMinute(it) }
    }

    val dateAsUIString = convertDateToUIString()
    val timeAsUIString = convertTimeToUIString()
    var sessionName: String? = null
    val dateTimeAsBackendString = convertDateToBackendString()

    fun sessionName(sessionName: String): FilterSlotDetails {
        this.sessionName = sessionName
        return this
    }

    private fun convertDateToUIString(): String {
        val dateFormatter = DateTimeFormatter.ofPattern(DateTimeFormats.frontendDateFormat)
        return this.dateAsLocalDateTime.format(dateFormatter)
    }

    private fun convertTimeToUIString(): String {
        val dateFormatter = DateTimeFormatter.ofPattern(DateTimeFormats.frontendTimeFormat)
        return this.dateAsLocalDateTime.format(dateFormatter).toLowerCase()
    }

    private fun convertDateToBackendString(): String {
        val dateFormatter = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
        val zonedDateTime = dateAsLocalDateTime.atZone(ZoneId.of("Europe/London"))
        return zonedDateTime.format(dateFormatter)
    }
}
