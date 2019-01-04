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
                bookingReason ?: defaultApptBookingReason,
                null,
                null)
    }

    override fun telephoneAppointmentRequest(patient: Patient,
                                             slotId: Int?,
                                             bookingReason: String?,
                                             telephoneNumber: String?,
                                             telephoneContactType: String?): BookAppointmentSlotFacade {

        return if (telephoneNumber.isNullOrEmpty()) {
            BookAppointmentSlotFacade(
                    patient.userPatientLinkToken,
                    slotId ?: defaultApptBookingSlotId,
                    bookingReason ?: defaultApptBookingReason)
        } else {
            BookAppointmentSlotFacade(
                    patient.userPatientLinkToken,
                    slotId ?: defaultApptBookingSlotId,
                    bookingReason ?: defaultApptBookingReason,
                    null,
                    null,
                    telephoneNumber,
                    telephoneContactType ?: defaultTelephoneContactType)
        }
    }
}