package features.myrecord.factories

import models.Patient
import mocking.data.myrecord.MicrotestMyRecordData
import mocking.microtest.myRecord.MyRecordModuleCounts
import mocking.microtest.myRecord.TestResultOptions
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

    override fun enabledWithData(
            patient: Patient, myRecordModuleCounts: MyRecordModuleCounts, testResultOptions: TestResultOptions) {
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }

        mockingClient.forMicrotest {
            myRecord.myRecordRequest(patient)
                    .respondWithSuccess(
                            MicrotestMyRecordData.getPopulatedMicrotestMyRecord(myRecordModuleCounts, testResultOptions)
                    )
        }
    }

    override fun enabledWithAllRecords(patient: Patient){
        mockingClient.forMicrotest {
            demographics.demographicsRequest(patient).respondWithSuccess()
        }

        mockingClient.forMicrotest {
            myRecord.myRecordRequest(patient)
                    .respondWithSuccess(MicrotestMyRecordData.getPopulatedMicrotestMyRecord(
                            MyRecordModuleCounts(), TestResultOptions())
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