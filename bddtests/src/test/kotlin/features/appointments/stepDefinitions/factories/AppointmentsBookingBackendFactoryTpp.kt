package features.appointments.stepDefinitions.factories

import constants.AppointmentDateTimeFormat
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

class AppointmentsBookingBackendFactoryTpp : AppointmentsBookingBackendFactory("TPP") {

    override fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int?,
                                           bookingReason: String?): BookAppointmentSlotFacade {
        var dateFormatter = DateTimeFormatter.ofPattern(AppointmentDateTimeFormat.backendDateTimeFormatWithoutTimezone)

        return BookAppointmentSlotFacade(
                patient.patientId,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason,
                startTime = LocalDateTime.now().plusDays(1).format(dateFormatter),
                endTime = LocalDateTime.now().plusDays(1).plusMinutes(30).format(dateFormatter)
        )
    }
}