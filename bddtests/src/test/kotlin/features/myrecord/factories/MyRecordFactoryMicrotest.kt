package features.myrecord.factories

import models.Patient
import mocking.data.myrecord.MicrotestMyRecordData
import mocking.microtest.myRecord.MyRecordModuleCounts
import org.apache.http.HttpStatus


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

    override fun enabledWithData(patient: Patient, myRecordModuleCounts: MyRecordModuleCounts) {
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }

        mockingClient.forMicrotest {
            myRecord.myRecordRequest(patient)
                    .respondWithSuccess(
                            MicrotestMyRecordData.getPopulatedMicrotestMyRecord(myRecordModuleCounts)
                    )
        }
    }

    override fun enabledWithAllRecords(patient: Patient){
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }

        mockingClient.forMicrotest {
            myRecord.myRecordRequest(patient)
                    .respondWithSuccess(
                            MicrotestMyRecordData.getPopulatedMicrotestMyRecord(MyRecordModuleCounts())
                    )
        }
    }

    override fun respondWithForbidden(patient: Patient) {
        mockingClient
                .forMicrotest {
                    myRecord.myRecordRequest(patient)
                            .respondWith(HttpStatus.SC_FORBIDDEN, resolve = {})
                }
    }
}