package features.appointments.data

import addDays
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mocking.emis.models.SessionType
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import java.text.SimpleDateFormat
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.format.DateTimeFormatter
import java.util.*


open class AppointmentsBookingData {

    companion object {

        val timeZone = TimeZone.getTimeZone("Europe/London")
        val backendDateTimeFormat = createBackendDateTimeFormatWithoutTimezone()
        val pastFromDate = "2017-12-24T14:00:00"
        val pastToDate = "2017-12-30T14:00:00"
        private val requestedFromDateDaysAfterToday = 14
        val explicitFromDate = sometimeDayInTheFuture(requestedFromDateDaysAfterToday)
        private val requestedToDateDaysAfterToday = 40
        val explicitToDate = sometimeDayInTheFuture(requestedToDateDaysAfterToday)
        val dateTimeFormat = DateTimeFormatter.ofPattern(backendDateTimeFormatWithoutTimezone)!!

        val mockingClient = MockingClient.instance
        val patient = MockDefaults.patient
        val tppPatient = MockDefaults.patientTpp


        val defaultSessionStartDateRaw = tomorrowMidnight()
        val defaultSessionEndDateRaw = threeWeeksTomorrowMidnight()

        val defaultSessionStartDate =defaultSessionStartDateRaw.format(dateTimeFormat)
        val defaultSessionEndDate =defaultSessionEndDateRaw.format(dateTimeFormat)

        private val defaultDuration = 30
        private val numberOfMinutesInWorkingDay = 540

        val defaultEmisMetaSlotLocations = arrayListOf(
                Location(
                        1,
                        "Sheffield"
                ),
                Location(
                        2,
                        "Leeds"
                )
        )

        val defaultEmisMetaSlotSessionHolders = arrayListOf(
                SessionHolder(
                        101,
                        "Bob"
                ),
                SessionHolder(
                        102,
                        "Steve"
                )
        )

        val defaultEmisMetaSlotSessions = arrayListOf(
                Session(
                        "Nurse Clinic",
                        201,
                        1,
                        defaultDuration,
                        SessionType.Timed,
                        1,
                        arrayListOf(101),
                        sometimeTomorrow(),
                        sometimeTomorrow(numberOfMinutesInWorkingDay)
                ),
                Session(
                        "Physio",
                        202,
                        2,
                        defaultDuration,
                        SessionType.Timed,
                        1,
                        arrayListOf(102),
                        sometimeDayAfterTomorrow(),
                        sometimeDayAfterTomorrow(numberOfMinutesInWorkingDay)
                )
        )

        val defaultEmisAppointmentSessions = arrayListOf(
                AppointmentSessionFacade(
                        sessionDate = getDefaultEmisAppointmentSlots()[0].startTime,
                        sessionId = 201,
                        slots = arrayListOf(getDefaultEmisAppointmentSlots()[0]),
                        sessionType = defaultEmisMetaSlotSessions[0].sessionName,
                        location = defaultEmisMetaSlotLocations[0].locationName,
                        staffDetails = defaultEmisMetaSlotSessionHolders[0].displayName
                ),
                AppointmentSessionFacade(
                        sessionDate = getDefaultEmisAppointmentSlots()[1].startTime,
                        sessionId = 202,
                        slots = arrayListOf(getDefaultEmisAppointmentSlots()[1]),
                        sessionType = defaultEmisMetaSlotSessions[1].sessionName,
                        location = defaultEmisMetaSlotLocations[1].locationName,
                        staffDetails = defaultEmisMetaSlotSessionHolders[1].displayName
                )
        )

        private fun createBackendDateTimeFormatWithoutTimezone(): SimpleDateFormat {
            val sdf = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
            sdf.timeZone = timeZone

            return sdf
        }

        fun getDefaultEmisAppointmentSlots() = arrayListOf(
                AppointmentSlotFacade(
                        slotId = 301,
                        startTime = sometimeTomorrow(),
                        endTime = sometimeTomorrow(defaultDuration),
                        sessionTypeName = defaultEmisMetaSlotSessions[0].sessionName,
                        slotTypeName = "Immunisations"
                ),
                AppointmentSlotFacade(
                        slotId = 302,
                        startTime = sometimeDayAfterTomorrow(),
                        endTime = sometimeDayAfterTomorrow(defaultDuration),
                        sessionTypeName = defaultEmisMetaSlotSessions[1].sessionName,
                        slotTypeName = "Back"
                )
        )

        fun getDefaultTppAppointmentSessions(sessionId: Int = 1, sessionType: String = "Clinic") = arrayListOf(
                AppointmentSessionFacade(
                        sessionId = sessionId,
                        sessionType = "Clinic",
                        staffDetails = "Dr. Who",
                        location = "Leeds",
                        slots = generateDefaultTppSlots(sessionId, sessionType)
                )
        )

        private fun generateDefaultTppSlots(sessionId: Int, sessionType: String) = arrayListOf(
                AppointmentSlotFacade(
                        startTime = "2018-08-01T11:00:00",
                        endTime = "2018-08-01T11:10:00",
                        sessionTypeName = sessionType,
                        slotTypeName = "Slot",
                        slotId = sessionId
                )
        )

        private fun tomorrowMidnight() = midnightDayInTheFuture(1)
        private fun threeWeeksTomorrowMidnight() = midnightDayInTheFuture(22)

        private fun sometimeTomorrow(minutesToAdd: Int = 0): String {
            val baseTime = Calendar.getInstance(timeZone)
            val tomorrow = baseTime.addDays(1)
            return backendDateTimeFormat.format(setAsTime(tomorrow, 14, minutesToAdd).time)
        }

        private fun sometimeDayAfterTomorrow(minutesToAdd: Int = 0): String {
            return sometimeDayInTheFuture(2, minutesToAdd)
        }

        private fun sometimeDayInTheFuture(daysToAdd: Int = 2, minutesToAdd: Int = 0): String {
            val baseTime = Calendar.getInstance(timeZone)
            val futureDay = baseTime.addDays(daysToAdd)
            return backendDateTimeFormat.format(setAsTime(futureDay, 9, minutesToAdd).time)
        }

        private fun midnightDayInTheFuture(daysToAdd: Int): LocalDateTime {
            val baseTime = Calendar.getInstance(timeZone)
            val dayInTheFuture = baseTime.addDays(daysToAdd)
            return toLocalDateTime(setAsTime(dayInTheFuture))
        }

        fun toLocalDateTime(calendar: Calendar): LocalDateTime {
            val tz = calendar.timeZone
            val zid = if (tz == null) ZoneId.systemDefault() else tz.toZoneId()
            return LocalDateTime.ofInstant(calendar.toInstant(), zid)
        }

        fun setAsTime(calendar: Calendar, hour: Int = 0, minute: Int = 0, second: Int = 0): Calendar {
            calendar.set(Calendar.HOUR_OF_DAY, hour)
            calendar.set(Calendar.MINUTE, minute)
            calendar.set(Calendar.SECOND, second)

            return calendar
        }

        fun adjustForTimeZone(localDateTime: LocalDateTime):LocalDateTime {

            val ldtZoned = localDateTime.atZone(ZoneId.systemDefault())
            val adjusted=localDateTime.plusSeconds(ldtZoned.offset!!.totalSeconds.toLong())
            return adjusted
        }
    }
}
