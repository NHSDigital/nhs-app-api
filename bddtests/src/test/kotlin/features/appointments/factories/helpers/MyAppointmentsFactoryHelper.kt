package features.appointments.factories.helpers

import constants.DateTimeFormats
import java.text.SimpleDateFormat
import java.util.*

class MyAppointmentsFactoryHelper {

    companion object {

        val timeZone = TimeZone.getTimeZone("Europe/London")
        val gpDateTimeFormat = createBackendDateTimeFormatWithoutTimezone()

        fun slotDateFormat(dateTimeToConvert: Date): String {
            val slotDateFormat = SimpleDateFormat(DateTimeFormats.frontendDateFormat)
            slotDateFormat.timeZone = timeZone
            return slotDateFormat.format(dateTimeToConvert)
        }

        fun slotTimeFormat(dateTimeToConvert: Date): String {
            val slotTimeFormat = SimpleDateFormat(DateTimeFormats.frontendTimeFormat)
            slotTimeFormat.timeZone = timeZone
            return slotTimeFormat.format(dateTimeToConvert).toLowerCase()
        }

        private fun createBackendDateTimeFormatWithoutTimezone(): SimpleDateFormat {
            val sdf = SimpleDateFormat(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
            sdf.timeZone = timeZone
            return sdf
        }
    }
}
