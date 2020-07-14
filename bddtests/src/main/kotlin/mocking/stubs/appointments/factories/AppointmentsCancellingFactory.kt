package mocking.stubs.appointments.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import utils.ProxySerenityHelpers
import worker.models.appointments.CancelAppointmentRequest

class AppointmentsCancellingFactory(gpSystem: Supplier) : AppointmentsFactory(gpSystem) {

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
                response(cancelAppointmentRequest(ProxySerenityHelpers.getPatientOrProxy(), request))
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

        override val map: HashMap<Supplier, (() -> AppointmentsCancellingFactory)> by lazy {
            hashMapOf(
                    Supplier.EMIS to { AppointmentsCancellingFactory(Supplier.EMIS) },
                    Supplier.TPP to { AppointmentsCancellingFactory(Supplier.TPP) },
                    Supplier.VISION to { AppointmentsCancellingFactory(Supplier.VISION) },
                    Supplier.MICROTEST to { AppointmentsCancellingFactory(Supplier.MICROTEST) })
        }

        val defaultApptCancellingReason = "Cancel an appointment."
        val defaultApptCancellingSlotId = 1
    }
}
