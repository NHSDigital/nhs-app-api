package features.im1Appointments.factories

import mocking.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.models.Mapping
import mocking.stubs.appointments.factories.AppointmentsFactory
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import worker.models.appointments.CancelAppointmentRequest

class AppointmentsCancellingFactory(gpSystem: String) : AppointmentsFactory(gpSystem) {

    fun defaultRequest(patient: Patient,
                                appointmentId: Int? = null,
                                cancellationReason: String? = null): CancelAppointmentSlotFacade {
        return CancelAppointmentSlotFacade(
                patient.userPatientLinkToken,
                appointmentId ?: defaultApptCancellingSlotId,
                cancellationReason ?: defaultApptCancellingReason
        )
    }

    fun setupRequestAndResponse(request: CancelAppointmentSlotFacade,
                                response: ((ICancelAppointmentsBuilder) -> Mapping)? = null) {

        if (response != null) {
            appointmentMapper.requestMapping {
                response(cancelAppointmentRequest(patient, request))
            }
        }
        setAppointmentToBeCancelled(request)
    }

    private fun setAppointmentToBeCancelled(toBeCancelled: CancelAppointmentSlotFacade) {
        Serenity.setSessionVariable("AppointmentToCancel").to(getAppointmentCancelRequest(toBeCancelled))
    }

    private fun getAppointmentCancelRequest(cancelApptSlot: CancelAppointmentSlotFacade): CancelAppointmentRequest {
        return CancelAppointmentRequest(
                cancelApptSlot.slotId.toString(),
                cancelApptSlot.cancellationReason
        )
    }

    companion object : SupplierSpecificFactory<AppointmentsCancellingFactory>() {

        override val map: HashMap<String, (() -> AppointmentsCancellingFactory)> by lazy {
            hashMapOf(
                    "EMIS" to { AppointmentsCancellingFactory("EMIS") },
                    "TPP" to { AppointmentsCancellingFactory("TPP") },
                    "VISION" to { AppointmentsCancellingFactory("VISION") },
                    "MICROTEST" to { AppointmentsCancellingFactory("MICROTEST")})
        }

        val defaultApptCancellingReason = "Cancel an appointment."
        val defaultApptCancellingSlotId = 1
    }
}