package features.myrecord.factories

import mocking.data.myrecord.MedicationsData
import models.Patient

class MedicationsFactoryEmis: MedicationsFactory(){
    override fun getExpectedMedications(): worker.models.myrecord.MedicationsData {
        throw UnsupportedOperationException()
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forEmis {
            myRecord.medicationsRequest(patient)
                    .respondWithSuccess(MedicationsData.getEmisDefaultMedicationsModel())
        }
    }

    override fun respondWithBadData(patient: Patient) {
        mockingClient.forEmis {
            myRecord.medicationsRequest(patient)
                    .respondWithCorruptedContent("Bad Data")
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        mockingClient.forEmis {
            myRecord.medicationsRequest(patient).respondWithSuccess(
                    MedicationsData.getEmisMedicationData())
        }
    }
}