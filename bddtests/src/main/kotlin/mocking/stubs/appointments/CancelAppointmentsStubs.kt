package mocking.stubs.appointments

import constants.Supplier
import mocking.MockingClient
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.TppStubsPatientFactory.Companion.goodPatientTPP
import mocking.stubs.appointments.AppointmentMatchers.Companion.cancellationReasonMatcher
import mocking.stubs.appointments.AppointmentMatchers.Companion.cancellationSlotMatcher
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import utils.getOrFail

class CancelAppointmentsStubs(private val mockingClient: MockingClient, private val patient: Patient ?= null) {

    fun generateStubs(supplier: Supplier) {
        when (supplier) {
            Supplier.EMIS -> generateEMISStubs()
            Supplier.TPP -> generateTPPStubs()
            else -> throw IllegalArgumentException("$supplier not implemented")
        }
    }

    private fun generateEMISStubs() {
        val mapCancelAppointmentStubs =
                InputResponse<String, ICancelAppointmentsBuilder>()
                        .addResponse(cancellationReasonMatcher) { builder
                            ->
                            builder.respondWithSuccess()
                        }

        mapCancelAppointmentStubs.listResponse().forEach { scenario ->
            val facade = CancelAppointmentSlotFacade(patient!!.userPatientLinkToken,
                    cancellationSlotMatcher,
                    scenario.forMatcher)
            mockingClient.forEmis.mock { scenario.getResponse(appointments.cancelAppointmentRequest(patient, facade)) }
        }
    }

    private fun generateTPPStubs() {
        val mapCancelAppointmentStubs =
                InputResponse<String, ICancelAppointmentsBuilder>()
                        .addResponse(cancellationReasonMatcher) { builder
                            ->
                            builder.respondWithSuccess()
                        }

        mapCancelAppointmentStubs.listResponse().forEach { scenario ->
            val facade = CancelAppointmentSlotFacade(goodPatientTPP.userPatientLinkToken,
                    SerenitySessionSlotId.APPOINTMENTONE.getOrFail(),
                    scenario.forMatcher)
            mockingClient.forTpp.mock {
                scenario.getResponse(appointments.cancelAppointmentRequest(goodPatientTPP, facade))
            }
        }
    }
}