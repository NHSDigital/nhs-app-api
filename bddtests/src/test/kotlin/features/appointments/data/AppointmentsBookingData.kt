package features.appointments.data

import addDays
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.models.*
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import java.text.SimpleDateFormat
import java.time.format.DateTimeFormatter
import java.util.*
import kotlin.collections.ArrayList

open class AppointmentsBookingData {

    companion object {

        val backendDateTimeFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        val pastFromDate = "2017-12-24T14:00:00"
        val pastToDate = "2017-12-30T14:00:00"
        private val numberOfDaysOfSlotsToRetrieve = 28
        private val requestedFromDateDaysAfterToday = 14
        val explicitFromDate = sometimeDayInTheFuture(requestedFromDateDaysAfterToday)
        private val requestedToDateDaysAfterToday = 40
        val explicitToDate = sometimeDayInTheFuture(requestedToDateDaysAfterToday)
        val defaultFromDateIfExplicitToDate = midnightDayInTheFuture(requestedToDateDaysAfterToday - numberOfDaysOfSlotsToRetrieve)
        val defaultToDateIfExplicitFromDate = midnightDayInTheFuture(requestedFromDateDaysAfterToday + numberOfDaysOfSlotsToRetrieve + 1)
        val dateTimeFormat = DateTimeFormatter.ofPattern(backendDateTimeFormatWithoutTimezone)!!

        val mockingClient = MockingClient.instance
        val patient = MockDefaults.patient
        val tppPatient = MockDefaults.patientTpp

        val defaultSessionStartDate = tomorrowMidnight()
        val defaultSessionEndDate = threeWeeksTomorrowMidnight()

        private val defaultDuration = 30
        private val numberOfMinutesInWorkingDay = 540
        private val maximumNumberOfSlotsInDay = numberOfMinutesInWorkingDay / defaultDuration

        val defaultTppLocations = arrayListOf("Sheffield","Leeds")
        val defaultTppClinicians = arrayListOf("Bob","Steve")

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

        fun generateEmisAppointmentSlots(numberOfDaysInFuture: Int = 1, startingId: Int = 303): ArrayList<AppointmentSlotFacade> {
            val generatedAppointmentSlots = arrayListOf<AppointmentSlotFacade>()
            for (slotsCreated in 0 until (2 * maximumNumberOfSlotsInDay) step 2) {
                val baseSlotId = (numberOfDaysInFuture * 1000) + startingId + slotsCreated
                generatedAppointmentSlots.add(
                        AppointmentSlotFacade(
                                slotId = baseSlotId,
                                startTime = sometimeDayInTheFuture(numberOfDaysInFuture, slotsCreated / 2 * 30),
                                endTime = sometimeDayInTheFuture(numberOfDaysInFuture, (slotsCreated / 2 * 30) + defaultDuration),
                                slotTypeName = "Immunisations"
                        )
                )
                generatedAppointmentSlots.add(
                        AppointmentSlotFacade(
                                slotId = baseSlotId + 1,
                                startTime = sometimeDayInTheFuture(numberOfDaysInFuture, slotsCreated / 2 * 30),
                                endTime = sometimeDayInTheFuture(numberOfDaysInFuture, (slotsCreated / 2 * 30) + defaultDuration),
                                slotTypeName = "Back"
                        )
                )
            }
            return generatedAppointmentSlots
        }

        fun generateTppSessions(
                numberOfDaysSessionsToCreate: Int = 1,
                startingId: Int = 203,
                locations: ArrayList<String> = defaultTppLocations,
                clinicians: ArrayList<String> = defaultTppClinicians
        ): ArrayList<AppointmentSessionFacade> {
            val generatedSessions = arrayListOf<AppointmentSessionFacade>()
            var currentId = startingId

            for (daysCreated in 0 until numberOfDaysSessionsToCreate) {
                for (location in locations) {
                    for(clinician in clinicians){
                        generatedSessions.add(
                                AppointmentSessionFacade(
                                        sometimeDayInTheFuture(daysCreated + 1),
                                        currentId,
                                        "Clinic",
                                        clinician,
                                        location
                                )
                        )
                        currentId++
                        generatedSessions.add(
                                AppointmentSessionFacade(
                                        sometimeDayInTheFuture(daysCreated + 1),
                                        currentId,
                                        "Walk-in",
                                        clinician,
                                        location
                                )
                        )
                        currentId++
                    }
                }
            }

            return generatedSessions
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
                appointmentSlots: ArrayList<ArrayList<AppointmentSlotFacade>> = arrayListOf(
                        generateEmisAppointmentSlots(startingId = 320),
                        generateEmisAppointmentSlots(startingId = 340),
                        generateEmisAppointmentSlots(startingId = 360),
                        generateEmisAppointmentSlots(startingId = 380),
                        generateEmisAppointmentSlots(startingId = 400),
                        generateEmisAppointmentSlots(startingId = 420)
                )
        ): ArrayList<AppointmentSessionFacade> {
            val generatedAppointmentSessions = arrayListOf<AppointmentSessionFacade>()
            var appointmentSlotIndex = 0
            for (session in sessions) {
                if (appointmentSlots.size == appointmentSlotIndex) break
                generatedAppointmentSessions.add(
                        AppointmentSessionFacade(
                                sessionDate = session.startDate,
                                sessionId = session.sessionId,
                                sessionType = session.sessionName,
                                slots = appointmentSlots[appointmentSlotIndex]
                        )
                )
                appointmentSlotIndex++
            }
            return generatedAppointmentSessions
        }

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

        fun generateTppAppointmentSlots(sessionId: Int, numberOfDaysInFuture: Int = 1): ArrayList<AppointmentSlotFacade> {
            val generatedAppointmentSlots = arrayListOf<AppointmentSlotFacade>()
            for (slotsCreated in 0 until (2 * maximumNumberOfSlotsInDay) step 2) {
                generatedAppointmentSlots.add(
                        AppointmentSlotFacade(
                                slotId = sessionId,
                                startTime = sometimeDayInTheFuture(numberOfDaysInFuture, slotsCreated / 2 * 30),
                                endTime = sometimeDayInTheFuture(numberOfDaysInFuture, (slotsCreated / 2 * 30) + defaultDuration),
                                slotTypeName = "Immunisations"
                        )
                )
                generatedAppointmentSlots.add(
                        AppointmentSlotFacade(
                                slotId = sessionId,
                                startTime = sometimeDayInTheFuture(numberOfDaysInFuture, slotsCreated / 2 * 30),
                                endTime = sometimeDayInTheFuture(numberOfDaysInFuture, (slotsCreated / 2 * 30) + defaultDuration),
                                slotTypeName = "Back"
                        )
                )
            }
            return generatedAppointmentSlots
        }

        private fun tomorrowMidnight() = midnightDayInTheFuture(1)


        private fun threeWeeksTomorrowMidnight() = midnightDayInTheFuture(22)

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

        private fun midnightDayInTheFuture(daysToAdd: Int): String {
            val baseTime = Calendar.getInstance()
            val dayInTheFuture = baseTime.addDays(daysToAdd)
            return setAsTime(dayInTheFuture)
        }

        private fun setAsTime(calendar: Calendar, hour: Int = 0, minute: Int = 0, second: Int = 0): String {
            calendar.set(Calendar.HOUR_OF_DAY, hour)
            calendar.set(Calendar.MINUTE, minute)
            calendar.set(Calendar.SECOND, second)
            return backendDateTimeFormat.format(calendar.time)
        }
    }
}
