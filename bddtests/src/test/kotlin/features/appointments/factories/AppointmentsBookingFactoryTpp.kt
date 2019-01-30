package features.appointments.factories

import constants.DateTimeFormats
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

private const val TIME_TO_ADD_IN_MINUTES = 30L

class AppointmentsBookingFactoryTpp : AppointmentsBookingFactory("TPP") {

    override fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int?,
                                           bookingReason: String?): BookAppointmentSlotFacade {
        val dateFormatter = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)

        return BookAppointmentSlotFacade(
                patient.patientId,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason,
                startTime = LocalDateTime.now().plusDays(1).format(dateFormatter),
                endTime = LocalDateTime.now().plusDays(1).plusMinutes(TIME_TO_ADD_IN_MINUTES).format(dateFormatter)
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
