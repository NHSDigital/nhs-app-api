package features.appointments.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import worker.models.appointments.CancelAppointmentRequest

abstract class AppointmentsCancellingFactory(gpSystem: String):AppointmentsFactory(gpSystem) {

    abstract fun defaultRequest(patient: Patient,
                                appointmentId: Int? = null,
                                cancellationReason: String? = null): CancelAppointmentSlotFacade

    fun setupRequestAndResponse(request: CancelAppointmentSlotFacade,
                                response: (IAppointmentMappingBuilder.() -> Mapping)? = null) {

        if (response != null) {
           appointmentMapper.requestMapping { response()}
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

        override val map: HashMap<String, (()-> AppointmentsCancellingFactory)> by lazy {
                hashMapOf(
                        "EMIS" to {AppointmentsCancellingFactoryEmis()},
                        "TPP" to {AppointmentsCancellingFactoryTpp()})}

        val defaultApptCancellingReason = "Cancel an appointment."
        val defaultApptCancellingSlotId = 1
    }
}