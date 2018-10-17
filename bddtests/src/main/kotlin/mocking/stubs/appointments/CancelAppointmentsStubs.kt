package mocking.stubs.appointments

import mocking.MockingClient
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.appointments.AppointmentMatchers.Companion.cancellationReasonMatcher
import mocking.stubs.appointments.AppointmentMatchers.Companion.cancellationSlotMatcher
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient

class CancelAppointmentsStubs(private val patient: Patient,
                              private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val mapCancelAppointmentStubs =
                InputResponse<String, ICancelAppointmentsBuilder>()
                        .addResponse(cancellationReasonMatcher) { builder
                            -> builder.respondWithSuccess() }

        mapCancelAppointmentStubs.listResponse().forEach { scenario ->
            var facade = CancelAppointmentSlotFacade(patient.userPatientLinkToken,
                                                     cancellationSlotMatcher,
                                                     scenario.forMatcher)
            mockingClient.forEmis { scenario.getResponse(appointments.cancelAppointmentRequest(patient, facade)) }
        }
    }
}