package mocking.stubs.myMedicalRecord

import mocking.MockingClient
import mocking.data.myrecord.TestResultsData
import mocking.emis.testResults.EmisTestResultsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.StubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class TestResultsStubs(private val mockingClient: MockingClient) {

    companion object {
       private const val EMIS_RESULT_COUNT = 6
    }

    fun generateEMISStubs() {

        val testResultsLoader = TestResultsData.getTestResultsForEmis(EMIS_RESULT_COUNT)
        val mapEMISTestResultsStubs =
                InputResponse<Patient, EmisTestResultsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(testResultsLoader)
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithExceptionWhenNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(testResultsLoader)
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))
                        }

        mapEMISTestResultsStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(testResultsRequest(scenario.forMatcher)) }
        }
    }
}