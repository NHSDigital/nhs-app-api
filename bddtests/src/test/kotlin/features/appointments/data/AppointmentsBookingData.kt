package features.appointments.data

import addDays
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.models.*
import java.text.SimpleDateFormat
import java.time.format.DateTimeFormatter
import java.util.*
import kotlin.collections.ArrayList

open class AppointmentsBookingData {

    val pastFromDate = "2017-12-24T14:00:00"
    val pastToDate = "2017-12-30T14:00:00"
    val explicitFromDate = "2018-12-24T14:00:00"
    val explicitToDate = "2018-12-30T14:00:00"
    val defaultFromDateIfExplicitToDate = "2018-12-16T00:00:00"
    val defaultToDateIfExplicitFromDate = "2019-01-22T00:00:00"
    val dateTimeFormat = DateTimeFormatter.ofPattern(backendDateTimeFormatWithoutTimezone)!!

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    val backendDateTimeFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
    val defaultSessionStartDate = tomorrowMidnight()
    val defaultSessionEndDate = threeWeeksTomorrowMidnight()

    private val defaultDuration = 30
    private val numberOfMinutesInWorkingDay = 540
    private val maximumNumberOfSlotsInDay = numberOfMinutesInWorkingDay / defaultDuration

    val defaultEmisAppointmentSlots = arrayListOf(
            AppointmentSlot(
                    slotId = 301,
                    startTime = sometimeTomorrow(),
                    endTime = sometimeTomorrow(defaultDuration),
                    slotTypeName = "Immunisations",
                    slotTypeStatus = SlotTypeStatus.Visit
            ),
            AppointmentSlot(
                    slotId = 302,
                    startTime = sometimeDayAfterTomorrow(),
                    endTime = sometimeDayAfterTomorrow(defaultDuration),
                    slotTypeName = "Back",
                    slotTypeStatus = SlotTypeStatus.Practice
            )
    )

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
                    defaultEmisAppointmentSlots[0].startTime,
                    defaultEmisAppointmentSlots[0].endTime
            ),
            Session(
                    "Physio",
                    202,
                    2,
                    defaultDuration,
                    SessionType.Timed,
                    1,
                    arrayListOf(102),
                    defaultEmisAppointmentSlots[1].startTime,
                    defaultEmisAppointmentSlots[1].endTime
            )
    )

    val defaultEmisAppointmentSessions = arrayListOf(
            AppointmentSession(
                    sessionDate = defaultEmisAppointmentSlots[0].startTime,
                    sessionId = 201,
                    slots = arrayListOf(defaultEmisAppointmentSlots[0])
            ),
            AppointmentSession(
                    sessionDate = defaultEmisAppointmentSlots[1].startTime,
                    sessionId = 202,
                    slots = arrayListOf(defaultEmisAppointmentSlots[1])
            )
    )

    fun generateEmisAppointmentSlots(numberOfDaysInFuture: Int = 1, startingId: Int = 303): ArrayList<AppointmentSlot> {
        val generatedAppointmentSlots = arrayListOf<AppointmentSlot>()
        for (slotsCreated in 0 until (2 * maximumNumberOfSlotsInDay) step 2) {
            val baseSlotId = (numberOfDaysInFuture * 1000) + startingId + slotsCreated
            generatedAppointmentSlots.add(
                    AppointmentSlot(
                            slotId = baseSlotId,
                            startTime = sometimeDayInTheFuture(numberOfDaysInFuture, slotsCreated / 2 * 30),
                            endTime = sometimeDayInTheFuture(numberOfDaysInFuture, (slotsCreated / 2 * 30) + defaultDuration),
                            slotTypeName = "Immunisations",
                            slotTypeStatus = SlotTypeStatus.Practice
                    )
            )
            generatedAppointmentSlots.add(
                    AppointmentSlot(
                            slotId = baseSlotId + 1,
                            startTime = sometimeDayInTheFuture(numberOfDaysInFuture, slotsCreated / 2 * 30),
                            endTime = sometimeDayInTheFuture(numberOfDaysInFuture, (slotsCreated / 2 * 30) + defaultDuration),
                            slotTypeName = "Back",
                            slotTypeStatus = SlotTypeStatus.Practice
                    )
            )
        }
        return generatedAppointmentSlots
    }

    fun generateEmisSessions(
            numberOfDaysSessionsToCreate: Int = 1,
            startingId: Int = 203,
            locations: ArrayList<Location> = defaultEmisMetaSlotLocations,
            clinicians: ArrayList<SessionHolder> = defaultEmisMetaSlotSessionHolders
    ): ArrayList<Session> {
        val generatedSessions = arrayListOf<Session>()
        var currentId = startingId
        val allClinicians = arrayListOf<Int>()
        for (clinician in clinicians) {
            allClinicians.add(clinician.clinicianId)
        }
        for (daysCreated in 0 until numberOfDaysSessionsToCreate) {
            for (location in locations) {
                for (clinician in clinicians) {
                    generatedSessions.add(
                            Session(
                                    "Clinic",
                                    currentId,
                                    location.locationId,
                                    defaultDuration,
                                    SessionType.Timed,
                                    maximumNumberOfSlotsInDay,
                                    arrayListOf(clinician.clinicianId),
                                    sometimeDayInTheFuture(daysCreated + 1),
                                    sometimeDayInTheFuture(daysCreated + 1, numberOfMinutesInWorkingDay)
                            )
                    )
                    currentId++
                    generatedSessions.add(
                            Session(
                                    "Walk-in",
                                    currentId,
                                    location.locationId,
                                    defaultDuration,
                                    SessionType.Timed,
                                    maximumNumberOfSlotsInDay,
                                    arrayListOf(clinician.clinicianId),
                                    sometimeDayInTheFuture(daysCreated + 1),
                                    sometimeDayInTheFuture(daysCreated + 1, numberOfMinutesInWorkingDay)
                            )
                    )
                    currentId++
                }
                generatedSessions.add(
                        Session(
                                "Clinic",
                                currentId,
                                location.locationId,
                                defaultDuration,
                                SessionType.Timed,
                                maximumNumberOfSlotsInDay,
                                allClinicians,
                                sometimeDayInTheFuture(daysCreated + 1),
                                sometimeDayInTheFuture(daysCreated + 1, numberOfMinutesInWorkingDay)
                        )
                )
                currentId++
                generatedSessions.add(
                        Session(
                                "Walk-in",
                                currentId,
                                location.locationId,
                                defaultDuration,
                                SessionType.Timed,
                                maximumNumberOfSlotsInDay,
                                allClinicians,
                                sometimeDayInTheFuture(daysCreated + 1),
                                sometimeDayInTheFuture(daysCreated + 1, numberOfMinutesInWorkingDay)
                        )
                )
                currentId++
            }
        }
        return generatedSessions
    }

    fun generateEmisAppointmentSessions(
            sessions: ArrayList<Session> = generateEmisSessions(),
            appointmentSlots: ArrayList<ArrayList<AppointmentSlot>> = arrayListOf(
                    generateEmisAppointmentSlots(startingId = 320),
                    generateEmisAppointmentSlots(startingId = 340),
                    generateEmisAppointmentSlots(startingId = 360),
                    generateEmisAppointmentSlots(startingId = 380),
                    generateEmisAppointmentSlots(startingId = 400),
                    generateEmisAppointmentSlots(startingId = 420)
            )
    ): ArrayList<AppointmentSession> {
        val generatedAppointmentSessions = arrayListOf<AppointmentSession>()
        var appointmentSlotIndex = 0
        for (session in sessions) {
            if (appointmentSlots.size == appointmentSlotIndex) break
            generatedAppointmentSessions.add(
                    AppointmentSession(
                            sessionDate = session.startDate,
                            sessionId = session.sessionId,
                            slots = appointmentSlots[appointmentSlotIndex]
                    )
            )
            appointmentSlotIndex++
        }
        return generatedAppointmentSessions
    }

    private fun tomorrowMidnight(): String {
        val baseTime = Calendar.getInstance()
        val tomorrow = baseTime.addDays(1)
        return setAsTime(tomorrow)
    }

    private fun threeWeeksTomorrowMidnight(): String {
        val baseTime = Calendar.getInstance()
        val aWeekTomorrow = baseTime.addDays(22)
        return setAsTime(aWeekTomorrow)
    }

    private fun sometimeTomorrow(minutesToAdd: Int = 0): String {
        val baseTime = Calendar.getInstance()
        val tomorrow = baseTime.addDays(1)
        return setAsTime(tomorrow, 14, minutesToAdd)
    }

    private fun sometimeDayAfterTomorrow(minutesToAdd: Int = 0): String {
        return sometimeDayInTheFuture(2, minutesToAdd)
    }

    private fun sometimeDayInTheFuture(daysToAdd: Int = 2, minutesToAdd: Int = 0): String {
        val baseTime = Calendar.getInstance()
        val futureDay = baseTime.addDays(daysToAdd)
        return setAsTime(futureDay, 9, minutesToAdd)
    }

    private fun setAsTime(calendar: Calendar, hour: Int = 0, minute: Int = 0, second: Int = 0): String {
        calendar.set(Calendar.HOUR_OF_DAY, hour)
        calendar.set(Calendar.MINUTE, minute)
        calendar.set(Calendar.SECOND, second)
        return backendDateTimeFormat.format(calendar.time)
    }
}
