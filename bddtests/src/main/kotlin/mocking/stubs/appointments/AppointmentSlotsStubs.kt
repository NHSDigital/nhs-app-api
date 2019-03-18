package mocking.stubs.appointments

import mocking.MockingClient
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.StubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class AppointmentSlotsStubs(private val mockingClient: MockingClient) {

    private val appointmentSlotsExample = AppointmentsSlotsExample()

    fun generateEMISStubs() {
        val facade = appointmentSlotsExample.multipleSlotsOneLocation()
        val mapAppointmentSlotsStubs =
                InputResponse<Patient, IAppointmentSlotsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(facade)
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithGPErrorWhenNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.withDelay(Duration.ofSeconds(TIMEOUT_DELAY))
                                    .respondWithSuccess(facade)
                        }

        mapAppointmentSlotsStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(appointments.appointmentSlotsRequest(scenario.forMatcher)) }
            mockingClient.forEmis {
                scenario.getResponse(appointments.appointmentSlotsMetaRequest(scenario.forMatcher))
            }
        }
    }
}
