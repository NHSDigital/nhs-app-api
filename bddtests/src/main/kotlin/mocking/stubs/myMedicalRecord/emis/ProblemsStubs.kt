package mocking.stubs.myMedicalRecord.emis

import mocking.MockingClient
import mocking.data.myrecord.ProblemsData
import mocking.emis.problems.EmisProblemsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class ProblemsStubs(private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val problemsDataLoader = ProblemsData.getProblemsData()
        val mapEMISProblemsRequestStubs =
                InputResponse<Patient, EmisProblemsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(problemsDataLoader)
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithExceptionWhenNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(problemsDataLoader)
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))
                        }

        mapEMISProblemsRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(myRecord.problemsRequest(scenario.forMatcher)) }
        }

    }
}