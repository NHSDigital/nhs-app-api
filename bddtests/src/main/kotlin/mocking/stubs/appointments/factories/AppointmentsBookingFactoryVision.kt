package mocking.stubs.appointments.factories

import constants.Supplier
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient

class AppointmentsBookingFactoryVision : AppointmentsBookingFactory(Supplier.VISION) {

    override fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int?,
                                           bookingReason: String?): BookAppointmentSlotFacade {

        return BookAppointmentSlotFacade(
                patient.userPatientLinkToken,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason
        )
    }

    override fun telephoneAppointmentRequest(patient: Patient,
                                             slotId: Int?,
                                             bookingReason: String?,
                                             telephoneNumber: String?,
                                             telephoneContactType: String?): BookAppointmentSlotFacade {

        throw NotImplementedError("Telephone appointment not available")

    }
}
