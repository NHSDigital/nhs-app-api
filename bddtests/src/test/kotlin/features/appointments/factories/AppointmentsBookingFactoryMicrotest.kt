package features.appointments.factories

import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import models.Slot

class AppointmentsBookingFactoryMicrotest : AppointmentsBookingFactory("MICROTEST") {

    override fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int?,
                                           bookingReason: String?): BookAppointmentSlotFacade {

        return BookAppointmentSlotFacade(
                slotId = slotId ?: defaultApptBookingSlotId,
                bookingReason = bookingReason ?: defaultApptBookingReason)
    }

    override fun telephoneAppointmentRequest(patient: Patient,
                                             slotId: Int?,
                                             bookingReason: String?,
                                             telephoneNumber: String?,
                                             telephoneContactType: String?): BookAppointmentSlotFacade {

        throw NotImplementedError("Telephone appointment not available")
    }

    override fun getExpectedUiRepresentationOfSlot(
            slot: AppointmentSlotFacade, session: AppointmentSessionFacade
    ): Slot {
        val startDate = gpDateTimeFormat.parse(slot.startTime)
        val date = slotDateFormat(startDate)
        val time = slotTimeFormat(startDate)
        val location = session.location
        return Slot(
                date = date,
                time = time,
                sessionName = null,
                slotType = slot.slotTypeName!!,
                location = location!!,
                clinicians = setOf(session.staffDetails.first().staffName!!),
                id = slot.slotId
        )
    }
}