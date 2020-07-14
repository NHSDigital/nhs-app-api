package features.myrecord.factories

import mocking.data.myrecord.ProblemsData
import models.Patient
import worker.models.myrecord.ProblemItem

class ProblemsFactoryEmis : ProblemsFactory(){

    override fun disabled(patient: Patient) {

        mockingClient.forEmis.mock {
            myRecord.problemsRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.problemsRequest(patient)
                    .respondWithSuccess(ProblemsData.getDefaultProblemModel())
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.problemsRequest(patient)
                    .respondWithSuccess(ProblemsData.getProblemsData())
        }
    }

    override fun errorRetrieving(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.problemsRequest(patient)
                    .respondWithNonDataAccessException()
        }
    }

    override fun badDataResponse(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.problemsRequest(patient)
                    .respondWithCorruptedContent("Bad Data")
        }
    }

    override fun noAccess(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.problemsRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun getExpectedProblems(): List<ProblemItem> {
        throw UnsupportedOperationException()
    }

    override fun secondProblemHasNoDate(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.problemsRequest(patient)
                    .respondWithSuccess(ProblemsData.getEmisProblemRecordsWhereTheSecondRecordHasNoEffectiveDate())
        }
    }
}
