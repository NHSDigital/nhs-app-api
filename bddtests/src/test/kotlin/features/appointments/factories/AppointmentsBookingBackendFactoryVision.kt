package features.appointments.factories

import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient

class AppointmentsBookingBackendFactoryVision : AppointmentsBookingBackendFactory("VISION") {

    override fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int?,
                                           bookingReason: String?): BookAppointmentSlotFacade {

        return BookAppointmentSlotFacade(
                patient.userPatientLinkToken,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason
        )
    }
}