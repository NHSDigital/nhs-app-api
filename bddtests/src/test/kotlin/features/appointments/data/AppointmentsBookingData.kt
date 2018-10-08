package features.appointments.data

import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import mocking.MockingClient
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

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

        private fun midnightDayInTheFuture(daysToAdd: Int): ZonedDateTime {

            val baseTime = ZonedDateTime.now(ZoneId.of("UTC"))
            val dayInTheFuture = baseTime.plusDays(daysToAdd.toLong())
            val midnightDate = setToMidnight(dayInTheFuture)
            return midnightDate
        }

        private fun setToMidnight(date: ZonedDateTime): ZonedDateTime {
            return date.withHour(0).withMinute(0).withSecond(0)
        }
    }
}
