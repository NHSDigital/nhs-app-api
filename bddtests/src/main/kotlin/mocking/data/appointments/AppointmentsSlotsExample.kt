package mocking.data.appointments

import mocking.emis.models.SlotTypeStatus
import mocking.stubs.appointments.AppointmentSessionFacadeBuilder
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.metadata.LocationFacade
import mockingFacade.appointments.metadata.SlotTypeFacade
import mockingFacade.appointments.metadata.StaffDetailsFacade
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
const val HOURLY_INCREMENT = 2

private const val CANCELLATION_CUTOFF: Long = 30

const val DEFAULT_SLOT_TYPE = "Slot"
const val CLINIC_SESSION_TYPE = "Clinic"
const val TELEPHONE_SESSION_TYPE = "Telephone"

private const val STARTING_LOCATION_ID = 1
private const val STARTING_CLINICIAN_ID = 1
private const val STARTING_SLOT_TYPE_ID = 1
private const val FOUR_WEEKS_IN_DAYS = DAYS_IN_WEEK * 4

open class AppointmentsSlotsExample {

    protected val currentTime = LocalDateTime.now()!!
    private var currentDateToAdd = currentTime
    private val remainingDatesForThisWeek = setWeek()
    val datesForNextWeek = setWeek()

    private val tomorrowDate = LocalDateTime.now().plusDays(1)
    private val nextWeekDate = LocalDateTime.now().plusDays( DAYS_IN_WEEK)
    private val nextFourWeeksDate = LocalDateTime.now().plusDays( FOUR_WEEKS_IN_DAYS)

    private var nextLocationId = STARTING_LOCATION_ID
    private var nextStaffId = STARTING_CLINICIAN_ID
    protected var nextSlotTypeId = STARTING_SLOT_TYPE_ID

    private val startDateAppointment1 = AppointmentDate(
            tomorrowDate,
            DEFAULT_TIME_HOUR,
            DEFAULT_TIME_MIN
    )
    private val startDateAppointment2 = AppointmentDate(
            tomorrowDate,
            ALTERNATE_DEFAULT_TIME_HOUR,
            ALTERNATE_DEFAULT_TIME_MIN
    )
    private val startDateNextWeekAppointment1 = AppointmentDate(
            nextWeekDate,
            DEFAULT_TIME_HOUR,
            DEFAULT_TIME_MIN
    )
    private val startDateNextFourWeeksAppointment1 = AppointmentDate(
            nextFourWeeksDate,
            DEFAULT_TIME_HOUR,
            DEFAULT_TIME_MIN
    )
    private val telephoneStartDateAppointment = AppointmentDate(
            tomorrowDate,
            DEFAULT_TIME_HOUR,
            DEFAULT_TIME_MIN
    )

    private val historicalDate = AppointmentDate(LocalDateTime.now().minusMonths(MONTHS_IN_HALF_YEAR),
            DEFAULT_START_VALUE, DEFAULT_MINUTE_VALUE)

    protected val locationLeeds = LocationFacade(nextLocationId++, "Leeds")
    protected val locationSheffield = LocationFacade(nextLocationId++, "Sheffield")
    protected val staffDrWho = StaffDetailsFacade(nextStaffId++, "Dr. Who")
    protected val staffDrScott = StaffDetailsFacade(nextStaffId++, "Dr. Scott")
    protected val slotTypeDefault = SlotTypeFacade(nextSlotTypeId++, DEFAULT_SLOT_TYPE)

    private val defaultSessionDetails = AppointmentSessionFacadeBuilder()
            .sessionType(CLINIC_SESSION_TYPE)
            .locationId(locationLeeds.locationId)
            .staffDetails(staffDrWho.staffDetailsid)

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
        var appointmentsSlotsExampleBuilder: AppointmentsSlotsExampleBuilder
                = AppointmentsSlotsExampleBuilderWithExpectations()

        if (appointmentSessionFacade == null) {
            val appointmentSession1 = generateAppointmentData.generateAppointmentSession(
                    sessionDetails = defaultSessionDetails,
                    slotTypes = arrayListOf(slotTypeDefault),
                    dates = arrayListOf(startDateAppointment1),
                    staff = staffDrWho
            )
            val appointmentSession2 = generateAppointmentData.generateAppointmentSession(
                    sessionDetails = AppointmentSessionFacadeBuilder()
                            .sessionType(CLINIC_SESSION_TYPE)
                            .locationId(locationSheffield.locationId)
                            .staffDetails(staffDrScott.staffDetailsid),
                    slotTypes = arrayListOf(slotTypeDefault),
                    dates = arrayListOf(startDateAppointment2),
                    staff = staffDrScott
            )

            appointmentSessions = arrayListOf(appointmentSession1, appointmentSession2)

            appointmentsSlotsExampleBuilder = appointmentsSlotsExampleBuilder
                    .locationsList(listOf(locationLeeds, locationSheffield))
                    .appointmentTypesList(listOf(slotTypeDefault))
                    .cliniciansList(listOf(staffDrWho, staffDrScott))
                    .appointmentSessions(appointmentSessions)
        } else {
            appointmentSessions = appointmentSessionFacade

            val locationsList = listOf(locationLeeds)
            val slotTypeList = listOf(slotTypeDefault)
            val clinicianList = listOf(staffDrWho)

            appointmentsSlotsExampleBuilder = appointmentsSlotsExampleBuilder
                    .locationsList(locationsList)
                    .appointmentTypesList(slotTypeList)
                    .cliniciansList(clinicianList)
                    .appointmentSessions(appointmentSessions)
        }

        val filter = generateAppointmentData.generateFilter(
                DEFAULT_SLOT_TYPE,
                locationLeeds.locationName,
                staffDrWho.staffName,
                appointmentsSlotsExampleBuilder.build()
        )

        return appointmentsSlotsExampleBuilder
                .filterValues(filter)
    }


    fun getHistoricalAppointmentSession(): AppointmentSessionFacade {
        return generateAppointmentData.generateAppointmentSession(
                sessionDetails = defaultSessionDetails,
                slotTypes = arrayListOf(slotTypeDefault),
                dates = arrayListOf(historicalDate),
                staff = staffDrWho
        )
    }

    fun getExampleWithPastAppointment(): AppointmentSessionFacade {
        val pastAppointmentDate = arrayListOf(AppointmentDate(
                currentTime.minusMinutes(DEFAULT_MINUTE_VALUE.toLong()),
                DEFAULT_START_VALUE,
                DEFAULT_MINUTE_VALUE
        ))

        return generateAppointmentData.generateAppointmentSession(
                sessionDetails = defaultSessionDetails,
                slotTypes = arrayListOf(slotTypeDefault),
                dates = pastAppointmentDate,
                staff = staffDrWho
        )
    }

    fun getExampleWithAppointmentWithinCutoffTime(): AppointmentSessionFacade {
        val cutOffTimeAppointment = arrayListOf(
                AppointmentDate(
                        LocalDateTime.now().plusMinutes(CANCELLATION_CUTOFF)
                )
        )

        return generateAppointmentData.generateAppointmentSession(
                sessionDetails = defaultSessionDetails,
                slotTypes = arrayListOf(slotTypeDefault),
                dates = cutOffTimeAppointment,
                staff = staffDrWho
        )
    }

    fun slotExampleIncludingTelephoneAppointments(): AppointmentSlotsResponseFacade {
        val appointment1 = generateAppointmentData.generateAppointmentSession(
                sessionDetails = AppointmentSessionFacadeBuilder()
                        .sessionType(TELEPHONE_SESSION_TYPE)
                        .locationId(locationLeeds.locationId)
                        .staffDetails(staffDrWho.staffDetailsid),
                slotTypes = arrayListOf(slotTypeDefault),
                dates = arrayListOf(telephoneStartDateAppointment),
                staff = staffDrWho,
                channel = SlotTypeStatus.Telephone
        )

        val appointment2 = generateAppointmentData.generateAppointmentSession(
                sessionDetails = AppointmentSessionFacadeBuilder()
                        .sessionType(CLINIC_SESSION_TYPE)
                        .locationId(locationLeeds.locationId)
                        .staffDetails(staffDrScott.staffDetailsid),
                slotTypes = arrayListOf(slotTypeDefault),
                dates = arrayListOf(startDateAppointment2),
                staff = staffDrScott
        )

        val appointments = arrayListOf(appointment1, appointment2)

        val appointmentsSlotsExampleBuilder = AppointmentsSlotsExampleBuilderWithExpectations()
                .locationsList(listOf(locationLeeds))
                .appointmentTypesList(listOf(slotTypeDefault))
                .cliniciansList(listOf(staffDrWho, staffDrScott))
                .appointmentSessions(appointments)

        val filter = generateAppointmentData.generateFilter(
                type = DEFAULT_SLOT_TYPE,
                appointmentsSlotsResponse = appointmentsSlotsExampleBuilder.build()
        )

        return appointmentsSlotsExampleBuilder
                .filterValues(filter)
                .build()
    }

    fun singleSlotExample(dates: ArrayList<AppointmentDate> = arrayListOf(startDateAppointment1)):
            AppointmentSlotsResponseFacade {

        val locations = generateAppointmentData.generateLocationDetails(arrayListOf(locationLeeds.locationName))
        val types = generateAppointmentData.generateSlotTypes(arrayListOf(DEFAULT_SLOT_TYPE))
        val clinicians = generateAppointmentData.generateStaffDetails(arrayListOf(staffDrWho.staffName))

        val appointmentSessions = generateAppointmentData.generateAppointments(
                locationNames = locations,
                typesNames = types,
                staffNames = clinicians,
                dates = dates
        )

        val appointmentsSlotsExampleBuilder = AppointmentsSlotsExampleBuilderWithExpectations()
                .locationsList(locations)
                .appointmentTypesList(types)
                .cliniciansList(clinicians)
                .appointmentSessions(appointmentSessions)

        val filter = generateAppointmentData.generateFilter(
                type = DEFAULT_SLOT_TYPE,
                location = locationLeeds.locationName,
                doctor = staffDrWho.staffName,
                appointmentsSlotsResponse = appointmentsSlotsExampleBuilder.build()
        )

        return appointmentsSlotsExampleBuilder
                .locationsList(locations)
                .appointmentTypesList(types)
                .cliniciansList(clinicians)
                .filterValues(filter)
                .build()
    }

    fun multipleSlotsOneLocation(): AppointmentSlotsResponseFacade {
        val locations = generateAppointmentData.generateLocationDetails(arrayListOf(locationLeeds.locationName))
        val types = generateAppointmentData.generateSlotTypes(arrayListOf(DEFAULT_SLOT_TYPE))
        val clinicians = generateAppointmentData.generateStaffDetails(
                arrayListOf(staffDrWho.staffName, staffDrScott.staffName)
        )

        val appointmentSessions = generateAppointmentData.generateAppointments(
                locationNames = locations,
                typesNames = types,
                staffNames = clinicians,
                dates = arrayListOf(
                        startDateAppointment1,
                        startDateNextWeekAppointment1,
                        startDateNextFourWeeksAppointment1)
        )

        val appointmentsSlotsExampleBuilder = AppointmentsSlotsExampleBuilderWithExpectations()
                .locationsList(locations)
                .appointmentTypesList(types)
                .cliniciansList(clinicians)
                .appointmentSessions(appointmentSessions)

        val filter = generateAppointmentData.generateFilter(DEFAULT_SLOT_TYPE, locationLeeds.locationName,
                staffDrWho.staffName, appointmentsSlotsExampleBuilder.build())

        return appointmentsSlotsExampleBuilder
                .locationsList(locations)
                .appointmentTypesList(types)
                .cliniciansList(clinicians)
                .filterValues(filter)
                .build()
    }

    fun multipleSlotsOneTime(): AppointmentSlotsResponseFacade {
        val dates: ArrayList<AppointmentDate> = arrayListOf(startDateAppointment1)

        val locations = generateAppointmentData.generateLocationDetails(arrayListOf(locationLeeds.locationName))
        val types = generateAppointmentData.generateSlotTypes(arrayListOf(DEFAULT_SLOT_TYPE))
        val clinicians = generateAppointmentData.generateStaffDetails(
                arrayListOf(staffDrScott.staffName, staffDrWho.staffName)
        )

        val appointmentSessions = generateAppointmentData.generateAppointments(
                locationNames = locations,
                typesNames = types,
                staffNames = clinicians,
                dates = dates)

        val appointmentsSlotsExampleBuilder = AppointmentsSlotsExampleBuilderWithExpectations()
                .locationsList(locations)
                .appointmentTypesList(types)
                .cliniciansList(clinicians)
                .appointmentSessions(appointmentSessions)

        val filter = generateAppointmentData.generateFilter(type = DEFAULT_SLOT_TYPE,
                appointmentsSlotsResponse = appointmentsSlotsExampleBuilder.build())

        return appointmentsSlotsExampleBuilder
                .locationsList(locations)
                .appointmentTypesList(types)
                .cliniciansList(clinicians)
                .filterValues(filter)
                .build()
    }

    fun slotForDayAfterTomorrow(): AppointmentSlotsResponseFacade {
        val dayAfterTomorrow = LocalDateTime.now().plusDays(DAY_AFTER_TOMORROW)
        val date = arrayListOf(AppointmentDate(dayAfterTomorrow, DEFAULT_START_VALUE + HOURLY_INCREMENT,
                DEFAULT_START_VALUE))
        return singleSlotExample(date)
    }

    fun slotForEndOfToday(): AppointmentSlotsResponseFacade {
        val date = arrayListOf(AppointmentDate(LocalDateTime.now(), END_OF_DAY_TIME_HOUR, END_OF_DAY_TIME_MIN))
        return singleSlotExample(date)
    }

    fun slotForThisTimeNextWeek(): AppointmentSlotsResponseFacade {
        val aWeekToday = LocalDateTime.now().plusDays(DAYS_IN_WEEK)
        val date = AppointmentDate(aWeekToday, DEFAULT_TIME_HOUR, DEFAULT_TIME_MIN)
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
