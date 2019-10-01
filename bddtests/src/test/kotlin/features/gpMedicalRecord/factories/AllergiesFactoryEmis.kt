package features.gpMedicalRecord.factories

import mocking.data.myrecord.AllergiesData
import models.Patient
import worker.models.myrecord.AllergyItem

class AllergiesFactoryEmis: AllergiesFactory() {

    override fun disabled(patient: Patient) {
        mockingClient.forEmis {
            myRecord.allergiesRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithRecords(patient: Patient, count: Int) {
        mockingClient.forEmis {
            myRecord.allergiesRequest(patient)
                    .respondWithSuccess(AllergiesData.getEmisAllergiesData(count))
        }
    }

    override fun getExpectedAllergies(): List<AllergyItem> {
        throw UnsupportedOperationException("Not yet implemented")
    }
}