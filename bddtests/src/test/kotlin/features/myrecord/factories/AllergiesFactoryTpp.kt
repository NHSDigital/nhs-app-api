package features.myrecord.factories

import constants.ErrorResponseCodeTpp
import mocking.data.myrecord.AllergiesData
import mocking.tpp.models.Error
import models.Patient
import worker.models.myrecord.AllergyItem

class AllergiesFactoryTpp: AllergiesFactory() {

    override fun disabled(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                            "Requested record access is disabled by the practice",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))

        }
    }

    override fun enabledWithRecords(patient: Patient, count: Int) {

        mockingClient.forTpp.mock {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithSuccess(AllergiesData.getTppAllergiesData(count))
        }
    }

    override fun respondWithCorruptedContent(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.viewPatientOverviewPost(patient.tppUserSession!!)
                    .respondWithCorruptedContent()
        }
    }

    override fun getExpectedAllergies(): List<AllergyItem> {
        throw UnsupportedOperationException("Not yet implemented")
    }
}
