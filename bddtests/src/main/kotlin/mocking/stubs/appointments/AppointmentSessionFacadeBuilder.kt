package mocking.stubs.appointments

import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.StaffDetailsFacade
import models.Channel
import java.util.*

class AppointmentSessionFacadeBuilder {

    private val sessionDetails = "Clinic - Slot, Clinician: %s"

    private var session = AppointmentSessionFacade()

    fun sessionId(value: Int): AppointmentSessionFacadeBuilder {
        session.sessionId = value
        return this
    }

    fun sessionType(value: String): AppointmentSessionFacadeBuilder {
        session.sessionType = value
        return this
    }

    fun staffDetails(staff: IdValue): AppointmentSessionFacadeBuilder {
        return staffDetails(arrayListOf(staff))
    }

    fun staffDetails(staff: ArrayList<IdValue>): AppointmentSessionFacadeBuilder {
        session.staffDetails = (staff.map { clinician -> StaffDetailsFacade(clinician.value, clinician.id) })
        session.sessionDetails =
                String.format(
                        sessionDetails,
                        staff.joinToString { clinician -> clinician.value }
                )
        return this
    }

    fun location(location: IdValue): AppointmentSessionFacadeBuilder {
        session.location = location.value
        session.locationid = location.id
        return this
    }

    fun slots(value: (AppointmentSlotFacadeArrayBuilder.() -> AppointmentSlotFacadeArrayBuilder))
            : AppointmentSessionFacadeBuilder {

        val builder = AppointmentSlotFacadeArrayBuilder()
        val thing = value.invoke(builder)
        session.slots = thing.build()
        return this
    }

    fun build(): AppointmentSessionFacade {
        return session
    }
}

class AppointmentSlotFacadeArrayBuilder {

    private var nextSlotId = 1
    private var appointmentSlots: ArrayList<AppointmentSlotFacade> = arrayListOf()

    fun addAppointment(appointment: AppointmentSlotFacadeBuilder.() -> AppointmentSlotFacadeBuilder)
            : AppointmentSlotFacadeArrayBuilder {
        val builder = AppointmentSlotFacadeBuilder().slotId(nextSlotId++)
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
    private var startDate: String = ""
    private var endDate: String = ""
    private var slotTypeName: String = "Slot"
    private var slotInThePast: Boolean = false
    private var channel: Channel = Channel.Unknown

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

    fun slotTypeName(value: String): AppointmentSlotFacadeBuilder {
        slotTypeName = value
        return this
    }

    fun setSlotInThePast(): AppointmentSlotFacadeBuilder {
        slotInThePast = true
        return this
    }

    fun channel(value: Channel): AppointmentSlotFacadeBuilder {
        channel = value
        return this
    }

    fun build(): AppointmentSlotFacade {
        return AppointmentSlotFacade(
                slotId = slotId,
                startTime = startDate,
                endTime = endDate,
                slotTypeName = slotTypeName,
                slotInThePast = slotInThePast,
                channel = channel

        )
    }
}

data class IdValue(val id: Int, val value: String)