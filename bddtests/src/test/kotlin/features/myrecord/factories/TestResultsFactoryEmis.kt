package features.myrecord.factories

import mocking.data.myrecord.TestResultsData
import models.Patient
import worker.models.myrecord.TestResultItem

private const val NUMBER_OF_TEST_RESULTS_EQUALS_SIX = 6
class TestResultsFactoryEmis : TestResultsFactory(){
    override fun disabled(patient: Patient) {
        mockingClient.forEmis {
            myRecord.testResultsRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forEmis {
            myRecord.testResultsRequest(patient)
                    .respondWithSuccess(TestResultsData.getDefaultTestResultsModel())
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        mockingClient.forEmis {
            myRecord.testResultsRequest(patient)
                    .respondWithSuccess(TestResultsData.getTestResultsForEmis(NUMBER_OF_TEST_RESULTS_EQUALS_SIX))
        }
    }

    override fun errorRetrieving(patient: Patient) {
        mockingClient.forEmis {
            myRecord.testResultsRequest(patient)
                    .respondWithNonDataAccessException()
        }
    }

    override fun respondWithACorruptedResponse(patient: Patient){
        mockingClient.forEmis {
            myRecord.testResultsRequest(patient)
                    .respondWithCorruptedContent("Bad Data")
        }
    }

    override fun noAccess(patient: Patient) {

        mockingClient.forEmis {
            myRecord.testResultsRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun getExpectedTestResults(): List<TestResultItem> {
        throw UnsupportedOperationException("Not yet implemented")
    }
}