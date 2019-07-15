package mocking.stubs.appointments

import mocking.emis.models.SlotTypeStatus
import mocking.emis.models.TelephoneAppointmentDetails
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import java.util.*

class AppointmentSessionFacadeBuilder {

    private var sessionId: Int? = null
    private var sessionType: String? = null
    private var staffDetails: List<Int> = arrayListOf()
    private var locationId: Int? = null
    private var slots: List<AppointmentSlotFacade> = arrayListOf()

    fun sessionId(value: Int): AppointmentSessionFacadeBuilder {
        sessionId = value
        return this
    }

    fun sessionType(value: String): AppointmentSessionFacadeBuilder {
        sessionType = value
        return this
    }

    fun staffDetails(staff: Int): AppointmentSessionFacadeBuilder {
        return staffDetails(arrayListOf(staff))
    }

    fun staffDetails(staff: List<Int>): AppointmentSessionFacadeBuilder {
        staffDetails = staff
        return this
    }

    fun locationId(location: Int): AppointmentSessionFacadeBuilder {
        locationId = location
        return this
    }

    fun slots(value: (AppointmentSlotFacadeArrayBuilder.() -> AppointmentSlotFacadeArrayBuilder))
            : AppointmentSessionFacadeBuilder {

        val builder = AppointmentSlotFacadeArrayBuilder()
        val thing = value.invoke(builder)
        slots = thing.build()
        return this
    }

    fun build(): AppointmentSessionFacade {
        return AppointmentSessionFacade(
                slots.firstOrNull()?.startTime,
                sessionId,
                sessionType,
                staffDetails,
                locationId,
                slots
        )
    }
}

class AppointmentSlotFacadeArrayBuilder {

    private var appointmentSlots: ArrayList<AppointmentSlotFacade> = arrayListOf()

    fun addAppointment(appointment: AppointmentSlotFacadeBuilder.() -> AppointmentSlotFacadeBuilder)
            : AppointmentSlotFacadeArrayBuilder {
        val builder = AppointmentSlotFacadeBuilder()
        val appointmentBuilder = appointment.invoke(builder)
        appointmentSlots.add(appointmentBuilder.build())
        return this
    }

    fun build(): ArrayList<AppointmentSlotFacade> {
        return appointmentSlots
    }
}


class AppointmentSlotFacadeBuilder {

    private var slotId: Int = 1
    private lateinit var startDate: String
    private lateinit var endDate: String
    private var slotTypeId: Int = 1
    private var slotInThePast: Boolean = false
    private lateinit var channel: SlotTypeStatus
    private lateinit var slotDetails: String
    private lateinit var telephoneNumber: String
    private lateinit var telephoneAppointmentDetails: TelephoneAppointmentDetails

    fun slotId(value: Int): AppointmentSlotFacadeBuilder {
        slotId = value
        return this
    }

    fun startDate(value: String): AppointmentSlotFacadeBuilder {
        startDate = value
        return this
    }

    fun endDate(value: String): AppointmentSlotFacadeBuilder {
        endDate = value
        return this
    }

    fun slotTypeId(value: Int): AppointmentSlotFacadeBuilder {
        slotTypeId = value
        return this
    }

    fun setSlotInThePast(value: Boolean): AppointmentSlotFacadeBuilder {
        slotInThePast = value
        return this
    }

    fun channel(value: SlotTypeStatus): AppointmentSlotFacadeBuilder {
        channel = value
        return this
    }

    fun telephoneNumber(value: String): AppointmentSlotFacadeBuilder {
        telephoneNumber = value
        return this
    }

    // Need to think if there's a way to dynamically set this
    fun slotDetails(slotTypeName: String, sessionName: String, clinician: String):
    AppointmentSlotFacadeBuilder {
        slotDetails = "$sessionName - $slotTypeName, Clinician: $clinician"
        return this
    }

    fun build(): AppointmentSlotFacade {
        return AppointmentSlotFacade(
                slotId = slotId,
                startTime = startDate,
                endTime = endDate,
                slotTypeId = slotTypeId,
                slotInThePast = slotInThePast,
                slotDetails = slotDetails,
                channel = channel,
                telephoneNumber = telephoneNumber
        )
    }
}
