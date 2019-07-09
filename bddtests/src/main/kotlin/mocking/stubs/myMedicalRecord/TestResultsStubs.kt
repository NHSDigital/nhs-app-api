package mocking.stubs.myMedicalRecord

import mocking.MockingClient
import mocking.data.myrecord.TestResultsData.Companion.getTestResultsForEmis
import mocking.emis.testResults.EmisTestResultsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class TestResultsStubs(private val mockingClient: MockingClient) {

    companion object {
       private const val EMIS_RESULT_COUNT = 6
    }

    fun generateEMISStubs() {

        val testResultsLoader =
                getTestResultsForEmis(EMIS_RESULT_COUNT)
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
            mockingClient.forEmis { scenario.getResponse(myRecord.testResultsRequest(scenario.forMatcher)) }
        }
    }
}