package features.myrecord.factories

import constants.ErrorResponseCodeTpp
import mocking.data.myrecord.TestResultsData
import mocking.tpp.models.Error
import models.Patient
import worker.models.myrecord.TestResultItem
import java.time.OffsetDateTime


private const val NUMBER_OF_TEST_RESULTS_EQUALS_ONE = 1
private const val NUMBER_OF_TEST_RESULTS_EQUALS_TWO = 2
private const val NUMBER_OF_TEST_RESULTS_EQUALS_THREE = 3
private const val START_DATE_FOR_RANGE_ONE = 179L
private const val END_DATE_FOR_RANGE_ONE = 120L
private const val START_DATE_FOR_RANGE_TWO = 119L
private const val END_DATE_FOR_RANGE_TWO = 60L
private const val START_DATE_FOR_RANGE_THREE = 59L
class TestResultsFactoryTpp : TestResultsFactory(){

    override fun disabled(patient: Patient) {
        val today = OffsetDateTime.now()

        val startDate = today.minusDays(START_DATE_FOR_RANGE_ONE)
        val endDate = today.minusDays(END_DATE_FOR_RANGE_ONE)

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                            "Requested record access is disabled by the practice",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        val today = OffsetDateTime.now()

        var startDate = today.minusDays(START_DATE_FOR_RANGE_ONE)
        var endDate = today.minusDays(END_DATE_FOR_RANGE_ONE)

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithSuccess(TestResultsData.getDefaultTppTestResultsData())
        }

        startDate = today.minusDays(START_DATE_FOR_RANGE_TWO)
        endDate = today.minusDays(END_DATE_FOR_RANGE_TWO)

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithSuccess(TestResultsData.getDefaultTppTestResultsData())
        }

        startDate = today.minusDays(START_DATE_FOR_RANGE_THREE)
        endDate = today

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithSuccess(TestResultsData.getDefaultTppTestResultsData())
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        val today = OffsetDateTime.now()
        var startDate = today.minusDays(START_DATE_FOR_RANGE_ONE)
        var endDate = today.minusDays(END_DATE_FOR_RANGE_ONE)

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithSuccess(TestResultsData
                            .getMultipleTestResultsForTpp(NUMBER_OF_TEST_RESULTS_EQUALS_ONE))
        }

        startDate = today.minusDays(START_DATE_FOR_RANGE_TWO)
        endDate = today.minusDays(END_DATE_FOR_RANGE_TWO)

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithSuccess(TestResultsData
                            .getMultipleTestResultsForTpp(NUMBER_OF_TEST_RESULTS_EQUALS_TWO))
        }

        startDate = today.minusDays(START_DATE_FOR_RANGE_THREE)
        endDate = today

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithSuccess(TestResultsData
                            .getMultipleTestResultsForTpp(NUMBER_OF_TEST_RESULTS_EQUALS_THREE))
        }
    }

    override fun errorRetrieving(patient: Patient) {
       val today = OffsetDateTime.now()

        val startDate = today.minusDays(START_DATE_FOR_RANGE_ONE)
        val endDate = today.minusDays(END_DATE_FOR_RANGE_ONE)

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithServiceNotAvailableException()
        }
    }

    override fun noAccess(patient: Patient) {
        val today = OffsetDateTime.now()

        val startDate = today.minusDays(START_DATE_FOR_RANGE_ONE)
        val endDate = today.minusDays(END_DATE_FOR_RANGE_ONE)

        mockingClient.forTpp {
            myRecord.testResultsViewRequest(patient.tppUserSession!!, startDate, endDate)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                            "You don&apos;t have access to this online service. " +
                                    "You can request access to this service at Kainos GP Demo Unit by " +
                                    "clicking Manage Online Services in the Account section.",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }


    override fun getExpectedTestResults(): List<TestResultItem> {
        throw UnsupportedOperationException("Not yet implemented")
    }
}

