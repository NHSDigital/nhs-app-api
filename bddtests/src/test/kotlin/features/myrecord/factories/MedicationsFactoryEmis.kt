package features.myrecord.factories

import mocking.data.myrecord.MedicationsData
import models.Patient

class MedicationsFactoryEmis: MedicationsFactory(){

    override fun enabledWithBlankRecord(patient: Patient) {
        mockingClient.forEmis {
            myRecord.medicationsRequest(patient)
                    .respondWithSuccess(MedicationsData.getEmisDefaultMedicationsModel())
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        mockingClient.forEmis {
            myRecord.medicationsRequest(patient).respondWithSuccess(
                    MedicationsData.getEmisMedicationData())
        }
    }
}