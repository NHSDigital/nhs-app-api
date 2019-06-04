package features.myrecord.factories

import models.Patient

class MyRecordFactoryMicrotest: MyRecordFactory() {
    override fun disabled(patient: Patient) {
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }

    }
}