package mocking.stubs.prescriptions

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
import models.Patient
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.time.Duration

class OrderRepeatPrescriptionsStubs(private val patient: Patient,
                                    private val uuid: MutableList<String>,
                                    private val mockingClient: MockingClient) {

    fun generateEMISStubs() {
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
            var facade = PrescriptionSubmissionRequest(uuid, scenario.forMatcher)
            mockingClient.forEmis { scenario.getResponse(repeatPrescriptionSubmissionRequest(patient, facade)) }
        }
    }
}