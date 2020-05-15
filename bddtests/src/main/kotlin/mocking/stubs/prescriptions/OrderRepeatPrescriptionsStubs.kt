package mocking.stubs.prescriptions

import constants.Supplier
import mocking.MockingClient
import mocking.emis.prescriptionsSubmission.EmisPrescriptionsSubmissionBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.prescriptions.PrescriptionMatchers.Companion.courseInvalidMatcher
import mocking.stubs.prescriptions.PrescriptionMatchers.Companion.pendingPrescriptionRequestMatcher
import mocking.stubs.prescriptions.PrescriptionMatchers.Companion.prescriptionNotEnabledMatcher
import mocking.stubs.prescriptions.PrescriptionMatchers.Companion.prescriptionNotSubmittedMatcher
import mocking.stubs.prescriptions.PrescriptionMatchers.Companion.successMatcher
import mocking.stubs.prescriptions.PrescriptionMatchers.Companion.timeoutMatcher
import mocking.tpp.models.RequestMedicationReply
import mocking.tpp.prescriptionsSubmission.TppPrescriptionsSubmissionBuilder
import models.Patient
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.time.Duration

class OrderRepeatPrescriptionsStubs(private val patient: Patient,
                                    private val mockingClient: MockingClient,
                                    private val uuid: MutableList<String> ?= null) {

    fun generateStubs(supplier: Supplier){
        when (supplier){
            Supplier.EMIS -> generateEMISStubs()
            Supplier.TPP -> generateTPPStubs()
            else -> throw IllegalArgumentException("$supplier not implemented")
        }
    }

    private fun generateEMISStubs() {
        val mapEMISBookRepeatPrescriptionRequestStubs =
                InputResponse<String, EmisPrescriptionsSubmissionBuilder>()
                        .addResponse(successMatcher) { builder
                            ->
                            builder.respondWithCreated()
                                    .whenScenarioStateIs("Started")
                        }

                        .addResponse(prescriptionNotEnabledMatcher) { builder
                            ->
                            builder.respondWithPrescriptionsNotEnabled()
                                    .whenScenarioStateIs("Started")
                        }

                        .addResponse(timeoutMatcher) { builder
                            ->
                            builder.respondWithCreated()
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))
                                    .whenScenarioStateIs("Started")
                        }

                        .addResponse(courseInvalidMatcher) { builder
                            ->
                            builder.respondWithBadRequestErrorIndicatingACourseIsInvalid()
                                    .whenScenarioStateIs("Started")
                        }

                        .addResponse(pendingPrescriptionRequestMatcher) { builder
                            ->
                            builder.respondWithAlreadyAPendingRequestInTheLast30Days()
                                    .whenScenarioStateIs("Started")
                        }

                        .addResponse(prescriptionNotSubmittedMatcher) { builder
                            ->
                            builder.respondWithGenericInternalServerError()
                                    .whenScenarioStateIs("Started")
                        }

        mapEMISBookRepeatPrescriptionRequestStubs.listResponse().forEach { scenario ->
            val facade = PrescriptionSubmissionRequest(uuid!!, scenario.forMatcher)
            mockingClient.forEmis.mock {
                scenario.getResponse(prescriptions.repeatPrescriptionSubmissionRequest(patient, facade))
            }
        }
    }

   private fun generateTPPStubs() {
        val mapTPPBookRepeatPrescriptionRequestStubs =
                InputResponse<String, TppPrescriptionsSubmissionBuilder>()
                        .addResponse(successMatcher) { builder ->
                            builder.respondWithSuccess(RequestMedicationReply())
                        }

                        .addResponse(timeoutMatcher) { builder ->
                            builder.respondWithSuccess(RequestMedicationReply())
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))

                        }

        mapTPPBookRepeatPrescriptionRequestStubs.listResponse().forEach { scenario ->
            val messageResponse = RequestMedicationReply(message = scenario.forMatcher)
            mockingClient.forTpp.mock {
                scenario.getResponse(prescriptions.prescriptionSubmission(patient=patient, drugIds = null,
                        notes= messageResponse))
            }
        }
    }
}