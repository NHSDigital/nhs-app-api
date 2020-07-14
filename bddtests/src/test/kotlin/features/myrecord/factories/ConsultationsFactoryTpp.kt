package features.myrecord.factories

import constants.ErrorResponseCodeTpp
import mocking.data.myrecord.TppDcrData
import mocking.tpp.models.Error
import models.Patient
import worker.models.myrecord.ConsultationItem

class ConsultationsFactoryTpp: ConsultationsFactory() {

    override fun disabled(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                            "You don&apos;t have access to this online service. " +
                                    "You can request access to this service at Kainos GP Demo Unit by " +
                                    "clicking Manage Online Services in the Account section.",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }

    }

    override fun enabledWithRecords(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppDcrData.getMultipleDcrEventsForTpp())
        }
    }

    override fun getExpectedConsultations(): List<ConsultationItem> {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppDcrData.getDefaultTppDcrData())

        }
    }

    override fun errorRetrieving(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithServiceNotAvailableException()
        }
    }

    override fun recordWithBadConsultationsData(patient: Patient) {
        mockingClient.forTpp.mock {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithCorruptedContent()
        }
    }
}
