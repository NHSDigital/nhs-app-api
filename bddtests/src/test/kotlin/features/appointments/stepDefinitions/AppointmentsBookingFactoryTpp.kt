package features.appointments.stepDefinitions

import mocking.IAppointmentMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import worker.models.appointments.BookAppointmentSlotRequest
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

class AppointmentsBookingFactoryTpp : AppointmentsBookingFactory() {

    override fun getDefaultPatient(): Patient {
        return Patient.getDefault("TPP")
    }

    override fun sendRequestViaMockingClient(resolver: IAppointmentMappingBuilder.() -> Mapping) {
        mockingClient.forTpp { resolver() }
    }

    override fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int?,
                                           bookingReason: String?): BookAppointmentSlotFacade {
        var dateFormatter = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss")

        return BookAppointmentSlotFacade(
                patient.patientId,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason,
                startTime = LocalDateTime.now().plusDays(1).format(dateFormatter),
                endTime = LocalDateTime.now().plusDays(1).plusMinutes(30).format(dateFormatter)
        )
    }
}