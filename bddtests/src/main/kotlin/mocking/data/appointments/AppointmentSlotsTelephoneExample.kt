package mocking.data.appointments

import mocking.emis.models.SlotTypeStatus
import mocking.stubs.appointments.AppointmentSessionFacadeBuilder
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.AppointmentDate
import java.time.LocalDateTime

const val TELEPHONE_SESSION_TYPE = "Telephone"

class AppointmentSlotsTelephoneExample : AppointmentsSlotsExample() {

    private val generateAppointmentData = GenerateAppointmentData()

    private val tomorrowDate = LocalDateTime.now().plusDays(1)

    private val telephoneStartDateAppointment = AppointmentDate(
            tomorrowDate,
            DEFAULT_TIME_HOUR,
            DEFAULT_TIME_MIN)
    private val startDateAppointment2 = AppointmentDate(
            tomorrowDate,
            ALTERNATE_DEFAULT_TIME_HOUR,
            ALTERNATE_DEFAULT_TIME_MIN
    )

    fun getGenericTelephoneExample(appointmentSessionFacade: ArrayList<AppointmentSessionFacade>? = null):
            AppointmentSlotsResponseFacade {

        if (appointmentSessionFacade == null) {
            val genericExample = getGenericExampleBuilder().build()
            genericExample.sessions.forEach { session ->
                session.slots.forEach { slot ->
                    slot.channel = SlotTypeStatus.Telephone
                    slot.telephoneNumber = "0123456789"
                }
            }
            return genericExample
        }

        return getGenericExampleBuilder(appointmentSessionFacade)
                .build()
    }

    fun getHistoricalTelephoneAppointmentSession(): AppointmentSessionFacade {
        return generateAppointmentData.generateAppointmentSession(
                sessionDetails = defaultSessionDetails,
                slotTypes = arrayListOf(slotTypeDefault),
                dates = arrayListOf(historicalDate),
                staff = staffDrWho
        )
    }

    fun slotExampleIncludingTelephoneAppointments(telephoneNumber: String): AppointmentSlotsResponseFacade {
        val appointment1 = generateAppointmentData.generateAppointmentSession(
                sessionDetails = AppointmentSessionFacadeBuilder()
                        .sessionType(TELEPHONE_SESSION_TYPE)
                        .locationId(locationLeeds.locationId)
                        .staffDetails(staffDrWho.staffDetailsid),
                slotTypes = arrayListOf(slotTypeDefault),
                dates = arrayListOf(telephoneStartDateAppointment),
                staff = staffDrWho,
                //keep this to still have radio buttons show
                telephoneNumber = telephoneNumber
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
}
