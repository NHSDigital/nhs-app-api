package mocking.stubs.appointments.factories

import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient

class AppointmentsBookingFactoryEmis : AppointmentsBookingFactory("EMIS") {

    override fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int?,
                                           bookingReason: String?): BookAppointmentSlotFacade {

        return BookAppointmentSlotFacade(
                patient.userPatientLinkToken,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason)
    }

    override fun telephoneAppointmentRequest(patient: Patient,
                                             slotId: Int?,
                                             bookingReason: String?,
                                             telephoneNumber: String?,
                                             telephoneContactType: String?): BookAppointmentSlotFacade {

        return BookAppointmentSlotFacade(
                    patient.userPatientLinkToken,
                    slotId ?: defaultApptBookingSlotId,
                    bookingReason ?: defaultApptBookingReason,
                    telephoneNumber = telephoneNumber,
                    telephoneContactType = telephoneContactType)
    }
}