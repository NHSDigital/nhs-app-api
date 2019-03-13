package mocking.data.appointments

import mocking.emis.models.SlotTypeStatus
import mocking.stubs.appointments.AppointmentsSlotsExampleBuilder
import mocking.stubs.appointments.IdValue
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.AppointmentDate
import java.time.DayOfWeek.MONDAY
import java.time.LocalDateTime

class AppointmentsSlotsExample {

    fun getGenericExample(appointmentSessionFacade: ArrayList<AppointmentSessionFacade>? = null):
            AppointmentSlotsResponseFacade {

        if (appointmentSessionFacade == null){
            return getGenericExampleBuilder().build()
        }

        return getGenericExampleBuilder(appointmentSessionFacade)
                .build()
    }

    private fun getGenericExampleBuilder(appointmentSessionFacade: ArrayList<AppointmentSessionFacade>? =  null)
            : AppointmentsSlotsExampleBuilder {

        val generateAppointmentData = GenerateAppointmentData()
        val appointmentSessions: ArrayList<AppointmentSessionFacade>

        val filter = generateAppointmentData.generateFilter(slotType,
                staffDrWho.value, locationLeeds.value,
                arrayListOf(startDateAppointment1, startDateAppointment2))

        if (appointmentSessionFacade == null) {
            val appointmentSession1 = generateAppointmentData.generateAppointmentSession(
                    clinicSessionType, locationLeeds, staffDrWho,
                    arrayListOf(startDateAppointment1))
            val appointmentSession2 = generateAppointmentData.generateAppointmentSession(
                    clinicSessionType, locationSheffield, staffDrScott,
                    arrayListOf(startDateAppointment2))

            appointmentSessions = arrayListOf(appointmentSession1, appointmentSession2)
        } else {
            appointmentSessions = appointmentSessionFacade
        }

        return AppointmentsSlotsExampleBuilderWithExpectations()
                .appointmentSessions(appointmentSessions)
                .filterValues(filter)
                .appointmentTypesList(arrayListOf(slotType))
                .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                .locationsList(arrayListOf(locationLeeds.value, locationSheffield.value))
    }

    companion object {
        private const val dayAfterTomorrow = 2L
        private const val daysInWeek = 7L
        private const val defaultStartValue = 0
        private const val appointmentTimeHour = 14
        private const val appointmentTimeMin = 10

        private const val endOfDayTimeHour = 23
        private const val endOfDayTimeMin = 45
        private const val defaultMinuteValue = 15

        private const val hourlyIncrement = 2

        private const val cancellationCutOff: Long = 30

        private val currentTime = LocalDateTime.now()
        private var currentDateToAdd = currentTime
        val remainingDatesForThisWeek = setWeek()
        val datesForNextWeek = setWeek()

        private val tomorrowDate = LocalDateTime.now().plusDays(1)

        private const val slotType = "Slot"
        private const val clinicSessionType = "Clinic"

        private const val telephoneSessionType = "Telephone"

        private val startDateAppointment1 = AppointmentDate(tomorrowDate, 14, 0,
                sessionName = clinicSessionType)
        private val startDateAppointment2 = AppointmentDate(tomorrowDate, 15, 20,
                sessionName = clinicSessionType)

        private val telephoneStartDateAppointment1 = AppointmentDate(tomorrowDate, 14, 0,
                sessionName = telephoneSessionType)

        private val telephoneStartDateAppointment2 = AppointmentDate(tomorrowDate, 15, 20,
                sessionName = telephoneSessionType)

        private val historicalDate = AppointmentDate(LocalDateTime.now().minusMonths(6),
                defaultStartValue, defaultMinuteValue, sessionName = clinicSessionType)

        private val locationLeeds = IdValue(1, "Leeds")
        private val locationSheffield = IdValue(2, "Sheffield")
        private val staffDrWho = IdValue(101, "Dr. Who")
        private val staffDrScott = IdValue(102, "Dr. Scott")

        private val generateAppointmentData = GenerateAppointmentData()


        fun getHistoricalAppointmentSession() : AppointmentSessionFacade{
            val generateAppointmentData = GenerateAppointmentData()
            return generateAppointmentData.generateAppointmentSession(
                    clinicSessionType,
                    locationLeeds,
                    staffDrWho,
                    arrayListOf(historicalDate))
        }

        fun getExampleWithPastAppointment(): AppointmentSessionFacade {
            val generateAppointmentData = GenerateAppointmentData()

            val pastAppointmentDate = arrayListOf(AppointmentDate(currentTime.minusMinutes(defaultMinuteValue.toLong()),
                    defaultStartValue, defaultMinuteValue, sessionName = clinicSessionType))

            return generateAppointmentData.generateAppointmentSession(clinicSessionType,
                    staffDrWho, locationLeeds,
                    pastAppointmentDate)
        }

        fun getExampleWithAppointmentWithinCutoffTime(): AppointmentSessionFacade {
            val generateAppointmentData = GenerateAppointmentData()
            val cutOffTimeAppointment = arrayListOf(AppointmentDate(LocalDateTime.now().plusMinutes(cancellationCutOff),
                    defaultStartValue, defaultMinuteValue, sessionName = clinicSessionType))

            return generateAppointmentData.generateAppointmentSession(
                    clinicSessionType, staffDrWho, locationLeeds, cutOffTimeAppointment)
        }

        fun slotExampleIncludingTelephoneAppointments(): AppointmentSlotsResponseFacade {

            val appointment1 = GenerateAppointmentData().generateAppointmentSession(
                    telephoneSessionType, locationLeeds, staffDrWho,
                    arrayListOf(telephoneStartDateAppointment1), SlotTypeStatus.Telephone)

            val appointment2 = GenerateAppointmentData().generateAppointmentSession(
                    telephoneSessionType, locationLeeds, staffDrScott,
                    arrayListOf(telephoneStartDateAppointment2), SlotTypeStatus.Telephone)


            val filter = GenerateAppointmentData().generateFilter(slotType, staffDrWho.value,
                    locationLeeds.value, arrayListOf(telephoneStartDateAppointment1))

            val appointments = arrayListOf(appointment1, appointment2)

            return AppointmentsSlotsExampleBuilderWithExpectations()
                    .appointmentSessions(appointments)
                    .filterValues(filter)
                    .appointmentTypesList(arrayListOf(slotType))
                    .cliniciansList(arrayListOf(staffDrWho.value, staffDrScott.value))
                    .locationsList(arrayListOf(locationLeeds.value))
                    .build()
        }

        fun singleSlotExample(dates: ArrayList<AppointmentDate> = arrayListOf(startDateAppointment1)):
                AppointmentSlotsResponseFacade {
            val filter = generateAppointmentData.generateFilter(slotType, staffDrWho.value,
                    locationLeeds.value, dates)

            return GenerateAppointmentData().generateAppointments(
                    arrayListOf(locationLeeds.value),
                    arrayListOf(slotType),
                    arrayListOf(staffDrWho.value), dates, filter)
        }

        fun multipleSlotsOneLocation(): AppointmentSlotsResponseFacade {
            val filter = generateAppointmentData.generateFilter(slotType, staffDrWho.value,
                    locationLeeds.value, arrayListOf(startDateAppointment1))

            return generateAppointmentData.generateAppointments(arrayListOf(locationLeeds.value),
                    arrayListOf(staffDrWho.value, staffDrScott.value),
                    arrayListOf(slotType),
                    arrayListOf(startDateAppointment1, startDateAppointment2), filter)
        }

        fun multipleSlotsOneTime(): AppointmentSlotsResponseFacade {
            val dates: ArrayList<AppointmentDate> = arrayListOf(startDateAppointment1)
            val filter = generateAppointmentData.generateFilter(type = slotType,
                    dateArray = arrayListOf(startDateAppointment1))

            return GenerateAppointmentData().generateAppointments(
                    arrayListOf(locationLeeds.value),
                    arrayListOf(slotType),
                    arrayListOf(staffDrScott.value, staffDrWho.value), dates, filter)
        }

        fun slotForDayAfterTomorrow(): AppointmentSlotsResponseFacade {
            val dayAfterTomorrow = LocalDateTime.now().plusDays(dayAfterTomorrow)
            val date = arrayListOf(AppointmentDate(dayAfterTomorrow, defaultStartValue + hourlyIncrement,
                    defaultStartValue,
                    sessionName = clinicSessionType))
            return singleSlotExample(date)
        }

        fun slotForEndOfToday(): AppointmentSlotsResponseFacade {
            val date = arrayListOf(AppointmentDate(LocalDateTime.now(), endOfDayTimeHour, endOfDayTimeMin,
                    sessionName = clinicSessionType))
            return singleSlotExample(date)
        }

        fun slotForThisTimeNextWeek(): AppointmentSlotsResponseFacade {
            val aWeekToday = LocalDateTime.now().plusDays(daysInWeek)
            val date = AppointmentDate(aWeekToday, appointmentTimeHour, appointmentTimeMin,
                    sessionName = clinicSessionType)
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
}
