package features.appointments.factories

import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient

class AppointmentsBookingBackendFactoryEmis : AppointmentsBookingBackendFactory("EMIS") {

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