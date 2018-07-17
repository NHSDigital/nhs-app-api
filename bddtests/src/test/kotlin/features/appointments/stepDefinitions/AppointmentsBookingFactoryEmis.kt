package features.appointments.stepDefinitions

import mocking.IAppointmentMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import worker.models.appointments.BookAppointmentSlotRequest

class AppointmentsBookingFactoryEmis : AppointmentsBookingFactory() {

    override fun getDefaultPatient(): Patient {
        return Patient.getDefault("EMIS")
    }

    override fun sendRequestViaMockingClient(resolver: IAppointmentMappingBuilder.() -> Mapping) {
        mockingClient.forEmis { resolver() }
    }

    override fun defaultAppointmentRequest(patient: Patient,
                                           slotId: Int?,
                                           bookingReason: String?): BookAppointmentSlotFacade {

        return BookAppointmentSlotFacade(
                patient.patientId,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason
        )
    }
}