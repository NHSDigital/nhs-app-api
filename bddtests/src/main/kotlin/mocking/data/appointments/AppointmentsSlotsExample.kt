package mocking.data.appointments

import mocking.emis.models.SlotTypeStatus
import mocking.stubs.appointments.AppointmentSessionFacadeBuilder
import mocking.stubs.appointments.IdValue
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.AppointmentDate
import java.time.DayOfWeek.MONDAY
import java.time.LocalDateTime

private const val DAY_AFTER_TOMORROW = 2L
private const val DAYS_IN_WEEK = 7L
private const val MONTHS_IN_HALF_YEAR = 6L

private const val DEFAULT_START_VALUE = 0
private const val DEFAULT_TIME_HOUR = 14
private const val DEFAULT_TIME_MIN = 10
private const val ALTERNATE_DEFAULT_TIME_HOUR = 15
private const val ALTERNATE_DEFAULT_TIME_MIN = 20
private const val END_OF_DAY_TIME_HOUR = 23
private const val END_OF_DAY_TIME_MIN = 45

private const val DEFAULT_MINUTE_VALUE = 15
private const val HOURLY_INCREMENT = 2

private const val CANCELLATION_CUTOFF: Long = 30

private const val DEFAULT_SLOT_TYPE = "Slot"
private const val CLINIC_SESSION_TYPE = "Clinic"
private const val TELEPHONE_SESSION_TYPE = "Telephone"

private const val STARTING_LOCATION_ID = 1
private const val STARTING_CLINICIAN_ID = 101

class AppointmentsSlotsExample {

    private val currentTime = LocalDateTime.now()
    private var currentDateToAdd = currentTime
    val remainingDatesForThisWeek = setWeek()
    val datesForNextWeek = setWeek()

    private val tomorrowDate = LocalDateTime.now().plusDays(1)

    private var nextLocationId = STARTING_LOCATION_ID
    private var nextStaffId = STARTING_CLINICIAN_ID

    private val startDateAppointment1 = AppointmentDate(
            tomorrowDate,
            DEFAULT_TIME_HOUR,
            DEFAULT_TIME_MIN,
            sessionName = CLINIC_SESSION_TYPE
    )
    private val startDateAppointment2 = AppointmentDate(
            tomorrowDate,
            ALTERNATE_DEFAULT_TIME_HOUR,
            ALTERNATE_DEFAULT_TIME_MIN,
            sessionName = CLINIC_SESSION_TYPE
    )

    private val telephoneStartDateAppointment = AppointmentDate(
            tomorrowDate,
            DEFAULT_TIME_HOUR,
            DEFAULT_TIME_MIN,
            sessionName = TELEPHONE_SESSION_TYPE
    )

    private val historicalDate = AppointmentDate(LocalDateTime.now().minusMonths(MONTHS_IN_HALF_YEAR),
            DEFAULT_START_VALUE, DEFAULT_MINUTE_VALUE, sessionName = CLINIC_SESSION_TYPE)

    private val locationLeeds = IdValue(nextLocationId++, "Leeds")
    private val locationSheffield = IdValue(nextLocationId++, "Sheffield")
    private val staffDrWho = IdValue(nextStaffId++, "Dr. Who")
    private val staffDrScott = IdValue(nextStaffId++, "Dr. Scott")

    private val generateAppointmentData = GenerateAppointmentData()

    fun getGenericExample(appointmentSessionFacade: ArrayList<AppointmentSessionFacade>? = null):
            AppointmentSlotsResponseFacade {

        if (appointmentSessionFacade == null) {
            return getGenericExampleBuilder().build()
        }

        return getGenericExampleBuilder(appointmentSessionFacade)
                .build()
    }

    private fun getGenericExampleBuilder(appointmentSessionFacade: ArrayList<AppointmentSessionFacade>? = null)
            : AppointmentsSlotsExampleBuilder {

        val appointmentSessions: ArrayList<AppointmentSessionFacade>

        if (appointmentSessionFacade == null) {
            val appointmentSession1 = generateAppointmentData.generateAppointmentSession(
                    sessionDetails = AppointmentSessionFacadeBuilder()
                            .sessionType(CLINIC_SESSION_TYPE)
                            .location(locationLeeds)
                            .staffDetails(staffDrWho),
                    slotTypes = arrayListOf(DEFAULT_SLOT_TYPE),
                    dates = arrayListOf(startDateAppointment1))
            val appointmentSession2 = generateAppointmentData.generateAppointmentSession(
                    sessionDetails = AppointmentSessionFacadeBuilder()
                            .sessionType(CLINIC_SESSION_TYPE)
                            .location(locationSheffield)
                            .staffDetails(staffDrScott),
                    slotTypes = arrayListOf(DEFAULT_SLOT_TYPE),
                    dates = arrayListOf(startDateAppointment2)
            )

            appointmentSessions = arrayListOf(appointmentSession1, appointmentSession2)
        } else {
            appointmentSessions = appointmentSessionFacade
        }

        val filter = generateAppointmentData.generateFilter(
                DEFAULT_SLOT_TYPE,
                locationLeeds.value,
                staffDrWho.value,
                appointmentSessions
        )

        return AppointmentsSlotsExampleBuilderWithExpectations()
                .appointmentSessions(appointmentSessions)
                .filterValues(filter)
    }


    fun getHistoricalAppointmentSession(): AppointmentSessionFacade {
        return generateAppointmentData.generateAppointmentSession(
                sessionDetails = AppointmentSessionFacadeBuilder()
                        .sessionType(CLINIC_SESSION_TYPE)
                        .location(locationLeeds)
                        .staffDetails(staffDrWho),
                slotTypes = arrayListOf(DEFAULT_SLOT_TYPE),
                dates = arrayListOf(historicalDate))
    }

    fun getExampleWithPastAppointment(): AppointmentSessionFacade {
        val pastAppointmentDate = arrayListOf(AppointmentDate(
                currentTime.minusMinutes(DEFAULT_MINUTE_VALUE.toLong()),
                DEFAULT_START_VALUE,
                DEFAULT_MINUTE_VALUE,
                sessionName = CLINIC_SESSION_TYPE
        ))

        return generateAppointmentData.generateAppointmentSession(
                sessionDetails = AppointmentSessionFacadeBuilder()
                        .sessionType(CLINIC_SESSION_TYPE)
                        .location(locationLeeds)
                        .staffDetails(staffDrWho),
                slotTypes = arrayListOf(DEFAULT_SLOT_TYPE),
                dates = pastAppointmentDate
        )
    }

    fun getExampleWithAppointmentWithinCutoffTime(): AppointmentSessionFacade {
        val cutOffTimeAppointment = arrayListOf(
                AppointmentDate(
                        LocalDateTime.now().plusMinutes(CANCELLATION_CUTOFF),
                        sessionName = CLINIC_SESSION_TYPE
                )
        )

        return generateAppointmentData.generateAppointmentSession(
                sessionDetails = AppointmentSessionFacadeBuilder()
                        .sessionType(CLINIC_SESSION_TYPE)
                        .location(locationLeeds)
                        .staffDetails(staffDrWho),
                slotTypes = arrayListOf(DEFAULT_SLOT_TYPE),
                dates = cutOffTimeAppointment
        )
    }

    fun slotExampleIncludingTelephoneAppointments(): AppointmentSlotsResponseFacade {
        val appointment1 = generateAppointmentData.generateAppointmentSession(
                sessionDetails = AppointmentSessionFacadeBuilder()
                        .sessionType(TELEPHONE_SESSION_TYPE)
                        .location(locationLeeds)
                        .staffDetails(staffDrWho),
                slotTypes = arrayListOf(DEFAULT_SLOT_TYPE),
                dates = arrayListOf(telephoneStartDateAppointment),
                channel = SlotTypeStatus.Telephone
        )

        val appointment2 = generateAppointmentData.generateAppointmentSession(
                sessionDetails = AppointmentSessionFacadeBuilder()
                        .sessionType(CLINIC_SESSION_TYPE)
                        .location(locationLeeds)
                        .staffDetails(staffDrScott),
                slotTypes = arrayListOf(DEFAULT_SLOT_TYPE),
                dates = arrayListOf(startDateAppointment2)
        )

        val appointments = arrayListOf(appointment1, appointment2)

        val filter = generateAppointmentData.generateFilter(
                type = DEFAULT_SLOT_TYPE,
                globalSessions = appointments
        )

        return AppointmentsSlotsExampleBuilderWithExpectations()
                .appointmentSessions(appointments)
                .filterValues(filter)
                .build()
    }

    fun singleSlotExample(dates: ArrayList<AppointmentDate> = arrayListOf(startDateAppointment1)):
            AppointmentSlotsResponseFacade {
        val appointmentSessions = generateAppointmentData.generateAppointments(
                locationNames = arrayListOf(locationLeeds.value),
                typesArray = arrayListOf(DEFAULT_SLOT_TYPE),
                staffNames = arrayListOf(staffDrWho.value),
                dates = dates
        )

        val filter = generateAppointmentData.generateFilter(
                type = DEFAULT_SLOT_TYPE,
                location = locationLeeds.value,
                doctor = staffDrWho.value,
                globalSessions = appointmentSessions
        )

        return AppointmentsSlotsExampleBuilderWithExpectations()
                .appointmentSessions(appointmentSessions)
                .filterValues(filter)
                .build()
    }

    fun multipleSlotsOneLocation(): AppointmentSlotsResponseFacade {
        val appointmentSessions = generateAppointmentData.generateAppointments(
                locationNames = arrayListOf(locationLeeds.value),
                typesArray = arrayListOf(DEFAULT_SLOT_TYPE),
                staffNames = arrayListOf(staffDrWho.value, staffDrScott.value),
                dates = arrayListOf(startDateAppointment1, startDateAppointment2)
        )

        val filter = generateAppointmentData.generateFilter(DEFAULT_SLOT_TYPE, locationLeeds.value,
                staffDrWho.value, appointmentSessions)

        return AppointmentsSlotsExampleBuilderWithExpectations()
                .appointmentSessions(appointmentSessions)
                .filterValues(filter)
                .build()
    }

    fun multipleSlotsOneTime(): AppointmentSlotsResponseFacade {
        val dates: ArrayList<AppointmentDate> = arrayListOf(startDateAppointment1)

        val appointmentSessions = generateAppointmentData.generateAppointments(
                locationNames = arrayListOf(locationLeeds.value),
                typesArray = arrayListOf(DEFAULT_SLOT_TYPE),
                staffNames = arrayListOf(staffDrScott.value, staffDrWho.value), dates = dates)

        val filter = generateAppointmentData.generateFilter(type = DEFAULT_SLOT_TYPE,
                globalSessions = appointmentSessions)

        return AppointmentsSlotsExampleBuilderWithExpectations()
                .appointmentSessions(appointmentSessions)
                .filterValues(filter)
                .build()
    }

    fun slotForDayAfterTomorrow(): AppointmentSlotsResponseFacade {
        val dayAfterTomorrow = LocalDateTime.now().plusDays(DAY_AFTER_TOMORROW)
        val date = arrayListOf(AppointmentDate(dayAfterTomorrow, DEFAULT_START_VALUE + HOURLY_INCREMENT,
                DEFAULT_START_VALUE,
                sessionName = CLINIC_SESSION_TYPE))
        return singleSlotExample(date)
    }

    fun slotForEndOfToday(): AppointmentSlotsResponseFacade {
        val date = arrayListOf(AppointmentDate(LocalDateTime.now(), END_OF_DAY_TIME_HOUR, END_OF_DAY_TIME_MIN,
                sessionName = CLINIC_SESSION_TYPE))
        return singleSlotExample(date)
    }

    fun slotForThisTimeNextWeek(): AppointmentSlotsResponseFacade {
        val aWeekToday = LocalDateTime.now().plusDays(DAYS_IN_WEEK)
        val date = AppointmentDate(aWeekToday, DEFAULT_TIME_HOUR, DEFAULT_TIME_MIN,
                sessionName = CLINIC_SESSION_TYPE)
        return singleSlotExample(arrayListOf(date))
    }

    private fun setWeek(currentArray: ArrayList<String> = arrayListOf()): ArrayList<String> {
        val timeOfDay = FilterSlotDetails(currentDateToAdd)
        currentArray.add(timeOfDay.dateAsUIString)
        currentDateToAdd = currentDateToAdd.plusDays(1)
        return if (currentDateToAdd.dayOfWeek != MONDAY) {
            setWeek(currentArray)
        } else {
            currentArray
        }
    }
}
