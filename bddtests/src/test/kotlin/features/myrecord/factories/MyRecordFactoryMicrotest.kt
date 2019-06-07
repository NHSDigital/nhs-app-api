package features.myrecord.factories

import models.Patient
import mocking.data.myrecord.MicrotestMyRecordData


class MyRecordFactoryMicrotest: MyRecordFactory() {
    override fun disabled(patient: Patient) {
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }

        mockingClient.forMicrotest {
            myRecord.myRecordRequest(patient).respondWithSuccess(MicrotestMyRecordData.getEmptyMicrotestMyRecord())
        }
    }

    override fun enabledWithData(patient: Patient, numAllergies: Int) {
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }

        mockingClient.forMicrotest {
            myRecord.myRecordRequest(patient)
                    .respondWithSuccess(
                            MicrotestMyRecordData.getPopulatedMicrotestMyRecord(numAllergies)
                    )
        }
    }
}