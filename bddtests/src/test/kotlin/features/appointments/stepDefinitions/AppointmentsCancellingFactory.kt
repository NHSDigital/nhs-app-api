package features.appointments.stepDefinitions

import mocking.IAppointmentMappingBuilder
import mocking.ICancelAppointmentsBuilder
import mocking.MockingClient
import mocking.models.Mapping
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.models.appointments.CancelAppointmentRequest

abstract class AppointmentsCancellingFactory() {

    val mockingClient = MockingClient.instance

    fun defaultAppointmentCancellingSetupWithResult(builder: (ICancelAppointmentsBuilder) -> Mapping) {
        var patient = getDefaultPatient()
        var request = defaultRequest(patient)
        sendRequestViaMockingClient { builder(cancelAppointmentRequest(patient, request)) }
        setAppointmentToBeCancelled(request)
    }

    abstract fun defaultRequest(patient: Patient,
                                appointmentId: Int? = null,
                                cancellationReason: String? = null): CancelAppointmentSlotFacade

    abstract fun getDefaultPatient(): Patient

    fun setupRequestAndResponse(request: CancelAppointmentSlotFacade,
                                response: (IAppointmentMappingBuilder.() -> Mapping)? = null) {

        if (response != null) {
            sendRequestViaMockingClient(response)
        }
        setAppointmentToBeCancelled(request)
    }

    protected abstract fun sendRequestViaMockingClient(resolver: IAppointmentMappingBuilder.() -> Mapping)

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

        val map: HashMap<String, AppointmentsCancellingFactory> =
                hashMapOf(
                        "EMIS" to AppointmentsCancellingFactoryEmis(),
                        "TPP" to AppointmentsCancellingFactoryTpp())

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