package features.myrecord.factories

import mocking.data.myrecord.ConsultationsData
import models.Patient
import worker.models.myrecord.ConsultationItem

class ConsultationsFactoryEmis: ConsultationsFactory() {

    override fun disabled(patient: Patient) {
        mockingClient.forEmis {
            myRecord.consultationsRequest(patient).respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        mockingClient.forEmis {
            myRecord.consultationsRequest(patient)
                    .respondWithSuccess(ConsultationsData.getMultipleConsultationRecords())
        }
    }

    override fun getExpectedConsultations(): List<ConsultationItem> {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forEmis {
            myRecord.consultationsRequest(patient)
                    .respondWithSuccess(ConsultationsData.getDefaultConsultationsData())
        }
    }

    override fun errorRetrieving(patient: Patient) {
        mockingClient.forEmis {
            myRecord.consultationsRequest(patient).respondWithNonDataAccessException()
        }
    }

    override fun recordWithBadConsultationsData(patient: Patient) {
        mockingClient.forEmis {
            myRecord.consultationsRequest(patient)
                    .respondWithCorruptedContent("Bad Data")
        }
    }
}


