package mocking.stubs.myMedicalRecord.tpp

import mocking.MockingClient
import mocking.stubs.InputResponse
import mocking.stubs.TppStubsPatientFactory.Companion.goodPatientTPP
import mocking.tpp.data.TEST_RESULT_ID
import mocking.tpp.data.TestResultsDataTpp
import mocking.tpp.testResultDetail.TppTestResultDetailBuilder
import worker.models.demographics.TppUserSession

class ViewDetailedTestResultsStubsTpp(private val mockingClient :MockingClient) {
    fun generateTPPStubs() {

        val testResultsLoader = TestResultsDataTpp().detailedTestResultData
        val mapTppTestResultsStubs =
                InputResponse<TppUserSession, TppTestResultDetailBuilder>()
                        .addResponse(goodPatientTPP.tppUserSession!!) { builder
                            ->
                            builder.respondWithSuccess(testResultsLoader)
                        }

        mapTppTestResultsStubs.listResponse().forEach { scenario ->
            mockingClient.forTpp { scenario.getResponse(myRecord.testResultsDetailRequest(scenario.forMatcher,
                    TEST_RESULT_ID)) }
        }
    }
}