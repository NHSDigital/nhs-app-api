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

        return BookAppointmentSlotFacade(
                patient.userPatientLinkToken,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason,
                telephoneNumber = telephoneNumber)
    }

    override fun getExpectedUiRepresentationOfSlot(
            slot: AppointmentSlotFacade, session: AppointmentSessionFacade
    ): Slot {
        val startDate = gpDateTimeFormat.parse(slot.startTime)
        val date = slotDateFormat(startDate)
        val time = slotTimeFormat(startDate)

        return Slot(
                date = date,
                time = time,
                sessionName = null,
                slotType = appointmentSlotsFactoryHelper.getSlotTypeNameFromId(slot),
                location = appointmentSlotsFactoryHelper.getLocationNameFromId(session),
                clinicians = setOf(appointmentSlotsFactoryHelper.getClinicianNamesFromIds(session).first()),
                id = slot.slotId
        )
    }
}