package features.myrecord.factories

import mocking.data.myrecord.AllergiesData
import models.Patient

class AllergiesFactoryEmis: AllergiesFactory(){
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
}