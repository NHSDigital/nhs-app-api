package features.appointments.data

import constants.AppointmentDateTimeFormat
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

class DateTimeWrapper(private var dateAsLocalDateTime: LocalDateTime, hoursOverride: Int? = null, minutesOverride: Int? = null) {

    init {
        hoursOverride?.let { this.dateAsLocalDateTime = this.dateAsLocalDateTime.withHour(it) }
        minutesOverride?.let { this.dateAsLocalDateTime = this.dateAsLocalDateTime.withMinute(it) }
    }

    val dateAsUIString = convertDateToUIString()
    val timeAsUIString = convertTimeToUIString()
    val dateTimeAsBackendString = convertDateToBackendString()

    private fun convertDateToUIString(): String {
        val dateFormatter = DateTimeFormatter.ofPattern(AppointmentDateTimeFormat.frontendDateFormat)
        return this.dateAsLocalDateTime.format(dateFormatter)
    }

    private fun convertTimeToUIString(): String {
        val dateFormatter = DateTimeFormatter.ofPattern(AppointmentDateTimeFormat.frontendTimeFormat)
        return this.dateAsLocalDateTime.format(dateFormatter).toLowerCase()
    }

    private fun convertDateToBackendString(): String {
        val dateFormatter = DateTimeFormatter.ofPattern(AppointmentDateTimeFormat.backendDateTimeFormatWithoutTimezone)
        return this.dateAsLocalDateTime.format(dateFormatter)
    }
}
