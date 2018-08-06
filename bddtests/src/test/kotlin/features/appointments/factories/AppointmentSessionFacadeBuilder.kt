package features.appointments.factories

import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import java.util.ArrayList

class AppointmentSessionFacadeBuilder {

    var session = AppointmentSessionFacade(
            sessionId = 1,
            sessionType = "Clinic",
            staffDetails = "Dr. Who",
            location = "Leeds",
            slots = arrayListOf()
    )

    fun sessionId(value: Int): AppointmentSessionFacadeBuilder {
        session.sessionId = value
        return this
    }

    fun sessionType(value: String): AppointmentSessionFacadeBuilder {
        session.sessionType = value
        return this
    }

    fun staffDetails(staff:IdValue): AppointmentSessionFacadeBuilder {
        session.staffDetails = staff.value
        session.staffDetailsid = staff.id
        return this
    }

    fun location(location: IdValue): AppointmentSessionFacadeBuilder {
        session.location = location.value
        session.locationid = location.id
        return this
    }

    fun slots(value: (AppointmentSlotFacadeArrayBuilder.()-> AppointmentSlotFacadeArrayBuilder)): AppointmentSessionFacadeBuilder {

        var builder = AppointmentSlotFacadeArrayBuilder()
        var thing = value.invoke(builder)
        session.slots = thing.build()
        return this
    }

    fun build(): AppointmentSessionFacade {
        return session
    }
}
class AppointmentSlotFacadeArrayBuilder {

    var nextSlotId = 1

    var appointmentSlots: ArrayList<AppointmentSlotFacade> = arrayListOf()

    fun addAppointment(appointment: AppointmentSlotFacadeBuilder.()-> AppointmentSlotFacadeBuilder): AppointmentSlotFacadeArrayBuilder
    {
        var builder = AppointmentSlotFacadeBuilder().slotId(nextSlotId++)
        var appointmentBuilder= appointment.invoke(builder)
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

    fun build(): AppointmentSlotFacade {
        return AppointmentSlotFacade(
                slotId = slotId,
                startTime = startDate,
                endTime = endDate,
                slotTypeName = slotTypeName
        )
    }
}

data class IdValue(val id : Int, val value: String)