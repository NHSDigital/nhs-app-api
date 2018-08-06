package features.appointments.factories

import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient

class AppointmentsCancellingFactoryTpp : AppointmentsCancellingFactory("TPP") {

    override fun defaultRequest(patient: Patient,
                                appointmentId: Int?,
                                cancellationReason: String?): CancelAppointmentSlotFacade {

        return CancelAppointmentSlotFacade(
                patient.userPatientLinkToken,
                appointmentId ?: defaultApptCancellingSlotId,
                cancellationReason ?: defaultApptCancellingReason
        )
    }
}