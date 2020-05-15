package mocking.stubs.appointments

import constants.Supplier
import mocking.MockingClient
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.timeoutPatientEMIS
import mocking.stubs.TppStubsPatientFactory
import models.Patient
import java.time.Duration

class AppointmentSlotsStubs(private val mockingClient: MockingClient) {

    private val appointmentSlotsExample = AppointmentsSlotsExample()

    fun generateStubs(supplier: Supplier){
        when(supplier){
            Supplier.EMIS -> generateEMISStubs()
            Supplier.TPP -> generateTPPStubs()
            else -> throw IllegalArgumentException("$supplier not implemented")
        }
    }

    private fun generateEMISStubs() {
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
            mockingClient.forEmis.mock {
                scenario.getResponse(appointments.appointmentSlotsRequest(scenario.forMatcher)) }
            mockingClient.forEmis.mock {
                scenario.getResponse(appointments.appointmentSlotsMetaRequest(scenario.forMatcher))
            }
        }
    }

    private fun generateTPPStubs() {
        val facade = appointmentSlotsExample.multipleSlotsOneLocation()
        val mapAppointmentSlotsStubs =
                InputResponse<Patient, IAppointmentSlotsBuilder>()
                        .addResponse(TppStubsPatientFactory.goodPatientTPP) { builder
                            ->
                            builder.respondWithSuccess(facade)
                        }

        mapAppointmentSlotsStubs.listResponse().forEach { scenario ->
            mockingClient.forTpp.mock {
                scenario.getResponse(appointments.appointmentSlotsRequest(scenario.forMatcher)) }
        }
    }
}
