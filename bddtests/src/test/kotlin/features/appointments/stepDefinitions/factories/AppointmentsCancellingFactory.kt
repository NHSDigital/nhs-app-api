package features.appointments.stepDefinitions.factories

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.models.appointments.CancelAppointmentRequest

abstract class AppointmentsCancellingFactory(gpSystem: String):AppointmentsFactory(gpSystem) {

    fun defaultAppointmentCancellingSetupWithResult(builder: (ICancelAppointmentsBuilder) -> Mapping) {
        var request = defaultRequest(patient)
        appointmentMapper.requestMapping { builder(cancelAppointmentRequest(patient, request)) }
        setAppointmentToBeCancelled(request)
    }

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

    companion object {

        private val map: HashMap<String, AppointmentsCancellingFactory> by lazy {
                hashMapOf(
                        "EMIS" to AppointmentsCancellingFactoryEmis(),
                        "TPP" to AppointmentsCancellingFactoryTpp())}

        fun getForSupplier(gpSystem: String): AppointmentsCancellingFactory {
            if(! map.containsKey(gpSystem))
            {
                Assert.fail("GP system '$gpSystem' is not set up.")
            }
            return map.getValue(gpSystem)
        }

        val defaultApptCancellingReason = "Cancel an appointment."
        val defaultApptCancellingSlotId = 1
    }
}