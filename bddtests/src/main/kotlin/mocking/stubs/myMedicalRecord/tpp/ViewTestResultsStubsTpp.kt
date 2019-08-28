package mocking.stubs.myMedicalRecord.tpp

import mocking.MockingClient
import mocking.stubs.InputResponse
import mocking.stubs.TppStubsPatientFactory.Companion.goodPatientTPP
import mocking.tpp.data.TestResultsDataTpp
import mocking.tpp.testResultsView.TppTestResultsViewBuilder
import worker.models.demographics.TppUserSession
import java.time.OffsetDateTime

//Difference between start and end date is 59 days
private const val START_DIFF_ONE = 59L
private const val END_DIFF_ONE = 0L
private const val START_DIFF_TWO = 119L
private const val END_DIFF_TWO = 60L
private const val START_DIFF_THREE = 179L
private const val END_DIFF_THREE = 120L

class ViewTestResultsStubsTpp(private val mockingClient : MockingClient) {
    private val startDate = OffsetDateTime.now()
    fun generateTPPStubs() {
        val testResultsLoader = TestResultsDataTpp().testResultData
        val mapTppTestResultsStubs =
                InputResponse<TppUserSession, TppTestResultsViewBuilder>()
                        .addResponse(goodPatientTPP.tppUserSession!!) { builder
                            ->
                            builder.respondWithSuccess(testResultsLoader)
                        }

        mapTppTestResultsStubs.listResponse().forEach { scenario ->
            mockingClient.forTpp { scenario.getResponse(myRecord.testResultsViewRequest(scenario.forMatcher,
                    startDate.minusDays(START_DIFF_ONE), startDate.minusDays(END_DIFF_ONE)))}

            mockingClient.forTpp { scenario.getResponse(myRecord.testResultsViewRequest(scenario.forMatcher,
                    startDate.minusDays(START_DIFF_TWO), startDate.minusDays(END_DIFF_TWO)))}

            mockingClient.forTpp { scenario.getResponse(myRecord.testResultsViewRequest(scenario.forMatcher,
                    startDate.minusDays(START_DIFF_THREE), startDate.minusDays(END_DIFF_THREE)))}
        }
    }
}
