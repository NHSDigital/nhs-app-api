package features.appointments.data

import addDays
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import mocking.MockingClient
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*

open class AppointmentsBookingData {

    companion object {

        const val pastFromDate = "2017-12-24T14:00:00"
        const val pastToDate = "2017-12-30T14:00:00"
        val dateTimeFormat = DateTimeFormatter.ofPattern(backendDateTimeFormatWithoutTimezone)!!
        val mockingClient = MockingClient.instance

        val defaultSessionStartDateRaw = tomorrowMidnight()
        val defaultSessionEndDateRaw = threeWeeksTomorrowMidnight()

        val defaultSessionStartDate = defaultSessionStartDateRaw.format(dateTimeFormat)!!
        val defaultSessionEndDate = defaultSessionEndDateRaw.format(dateTimeFormat)!!


        private fun tomorrowMidnight() = midnightDayInTheFuture(1)
        private fun threeWeeksTomorrowMidnight() = midnightDayInTheFuture(22)

        private fun midnightDayInTheFuture(daysToAdd: Int): LocalDateTime {
            val baseTime = Calendar.getInstance(TimeZone.getTimeZone("UTC"))
            val dayInTheFuture = baseTime.addDays(daysToAdd)
            return toSpecificTimeZone(setAsTime(dayInTheFuture))
        }

        private fun toSpecificTimeZone(calendar: Calendar): LocalDateTime {
            val timeZone = TimeZone.getTimeZone("Europe/London")
            return LocalDateTime.ofInstant(calendar.toInstant(), timeZone.toZoneId())
        }

        private fun setAsTime(calendar: Calendar, hour: Int = 0, minute: Int = 0, second: Int = 0): Calendar {
            calendar.set(Calendar.HOUR_OF_DAY, hour)
            calendar.set(Calendar.MINUTE, minute)
            calendar.set(Calendar.SECOND, second)

            return calendar
        }
    }
}
