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
import models.Patient
import java.time.Duration

class ViewAppointmentsStubs(private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
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
}