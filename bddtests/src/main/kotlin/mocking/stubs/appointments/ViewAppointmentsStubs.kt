package mocking.stubs.appointments

import mocking.JSonXmlConverter
import mocking.MockingClient
import mocking.emis.data.EmisAppointmentData
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.timeoutPatientEMIS
import mocking.stubs.TppStubsPatientFactory
import mocking.tpp.data.TppAppointmentData
import models.Patient
import utils.set
import java.time.Duration

const val CANCEL_APPOINTMENT_SLOT_ID = 100

class ViewAppointmentsStubs(private val mockingClient: MockingClient) {

    fun generateStubs(supplier: String){
        when (supplier) {
            "EMIS" -> generateEMISStubs()
            "TPP" -> generateTPPStubs()
        }
    }

    private fun generateEMISStubs() {
        val appointmentsBody = JSonXmlConverter.toJsonWithUpperCamelCase(
                EmisAppointmentData.instance.createGetAppointmentsResponse())

        val mapViewAppointmentStubs =
                InputResponse<Patient, IMyAppointmentsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            -> builder.respondWithSuccess(appointmentsBody) }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            -> builder.respondWithGPErrorWhenNotEnabled() }

                        .addResponse(timeoutPatientEMIS) { builder
                            -> builder.respondWithSuccess(appointmentsBody)
                                        .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY)) }

        mapViewAppointmentStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis{ scenario.getResponse(appointments.viewMyAppointmentsRequest(scenario.forMatcher)) }
        }
    }

    private fun generateTPPStubs() {
        val appointmentsBody = TppAppointmentData.instance.createGetAppointmentsResponse()

        val mapViewAppointmentStubs =
                InputResponse<Patient, IMyAppointmentsBuilder>()
                        .addResponse(TppStubsPatientFactory.goodPatientTPP) { builder
                            ->
                            builder.respondWithSuccess(appointmentsBody) }

        mapViewAppointmentStubs.listResponse().forEach { scenario ->
            mockingClient.forTpp { scenario.getResponse(appointments.viewMyAppointmentsRequest(scenario.forMatcher,
                    IMyAppointmentsBuilder.AppointmentType.UPCOMING_ONLY)) }
            mockingClient.forTpp { scenario.getResponse(appointments.viewMyAppointmentsRequest(scenario.forMatcher,
                    IMyAppointmentsBuilder.AppointmentType.PAST_ONLY))}
        }
        SerenitySessionSlotId.APPOINTMENTONE.set(CANCEL_APPOINTMENT_SLOT_ID)
    }
}