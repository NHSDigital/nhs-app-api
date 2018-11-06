package mocking.data.appointments

import constants.DateTimeFormats.Companion.backendDateTimeFormatWithTimezone
import mocking.MockingClient
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

open class AppointmentsBookingData {

    companion object {
        private const val tomorrow = 1
        private const val threeWeeks = 22

        const val pastFromDate = "2017-12-24T14:00:00"
        const val pastToDate = "2017-12-30T14:00:00"
        val dateTimeFormat = DateTimeFormatter.ofPattern(backendDateTimeFormatWithTimezone)!!
        val mockingClient = MockingClient.instance

        val defaultSessionStartDateRaw = tomorrowMidnight()
        val defaultSessionEndDateRaw = threeWeeksTomorrowMidnight()

        val defaultSessionStartDate = defaultSessionStartDateRaw.format(dateTimeFormat)!!
        val defaultSessionEndDate = defaultSessionEndDateRaw.format(dateTimeFormat)!!

        private fun tomorrowMidnight() = midnightDayInTheFuture(tomorrow)
        private fun threeWeeksTomorrowMidnight() = midnightDayInTheFuture(threeWeeks)

        private fun midnightDayInTheFuture(daysToAdd: Int): ZonedDateTime {

            val baseTime = ZonedDateTime.now(ZoneId.of("Europe/London"))
            val dayInTheFuture = baseTime.plusDays(daysToAdd.toLong())
            return setToMidnight(dayInTheFuture)
        }

        private fun setToMidnight(date: ZonedDateTime): ZonedDateTime {
            return date.withHour(0).withMinute(0).withSecond(0)
        }
    }
}
