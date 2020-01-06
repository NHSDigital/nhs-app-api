package mocking.defaults.dataPopulation.journies.prescriptions

import mocking.data.prescriptions.VisionPrescriptionLoader
import mocking.defaults.VisionMockDefaults
import models.Patient
import models.prescriptions.PrescriptionLoaderConfiguration

class PrescriptionsHistoryJourneyVision: PrescriptionsHistoryJourney() {

    override fun createFor(patient: Patient) {
        val prescriptionLoaderConfig = PrescriptionLoaderConfiguration(
                noPrescriptions = 1, noCourses = 1, noRepeats = 1
        )

        VisionPrescriptionLoader.loadData(prescriptionLoaderConfig)
        client
                .forVision {
                    prescriptions.getPrescriptionHistoryRequest(VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithSuccess(VisionPrescriptionLoader.data)
                }
    }
}