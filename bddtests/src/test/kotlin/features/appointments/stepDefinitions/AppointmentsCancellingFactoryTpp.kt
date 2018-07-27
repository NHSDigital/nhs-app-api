package features.appointments.stepDefinitions

import mocking.IAppointmentMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient

class AppointmentsCancellingFactoryTpp : AppointmentsCancellingFactory() {

    override fun getDefaultPatient(): Patient {
        return Patient.getDefault("TPP")
    }

    override fun sendRequestViaMockingClient(resolver: IAppointmentMappingBuilder.() -> Mapping) {
        mockingClient.forTpp { resolver() }
    }

    override fun defaultRequest(patient: Patient,
                                slotId: Int?,
                                cancelellationReason: String?): CancelAppointmentSlotFacade {

        return CancelAppointmentSlotFacade(
                patient.userPatientLinkToken,
                slotId ?: defaultApptCancellingSlotId,
                cancelellationReason ?: defaultApptCancellingReason
        )
    }
}