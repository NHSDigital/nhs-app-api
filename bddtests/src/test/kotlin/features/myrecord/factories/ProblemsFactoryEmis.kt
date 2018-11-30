package features.myrecord.factories

import mocking.data.myrecord.ProblemsData
import models.Patient

class ProblemsFactoryEmis : ProblemsFactory(){

    override fun disabled(patient: Patient) {

        mockingClient.forEmis {
            myRecord.problemsRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forEmis {
            myRecord.problemsRequest(patient)
                    .respondWithSuccess(ProblemsData.getDefaultProblemModel())
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        mockingClient.forEmis {
            myRecord.problemsRequest(patient)
                    .respondWithSuccess(ProblemsData.getProblemsData())
        }
    }

    override fun errorRetrieving(patient: Patient) {
        mockingClient.forEmis {
            myRecord.problemsRequest(patient)
                    .respondWithNonDataAccessException()
        }
    }

    override fun noAccess(patient: Patient) {
        mockingClient.forEmis {
            myRecord.problemsRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }
}