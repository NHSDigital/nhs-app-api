package features.myrecord.factories

import mocking.data.myrecord.MedicationsData
import models.Patient

class MedicationsFactoryEmis: MedicationsFactory(){

    override fun enabled(patient: Patient) {
        mockingClient.forEmis {
            myRecord.medicationsRequest(patient).respondWithSuccess(
                    MedicationsData.getEmisMedicationData())
        }
    }

    override fun enabledAndNoMedicationsMock(patient: Patient) {
        mockingClient.forEmis {
            myRecord.medicationsRequest(patient)
                    .respondWithSuccess(MedicationsData.getEmisDefaultMedicationsModel())
        }
    }
}