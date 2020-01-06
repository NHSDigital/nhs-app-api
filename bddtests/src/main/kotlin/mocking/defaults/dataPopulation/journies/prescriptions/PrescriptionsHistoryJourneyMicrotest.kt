package mocking.defaults.dataPopulation.journies.prescriptions

import mocking.data.prescriptions.MicrotestPrescriptionLoader
import models.Patient
import models.prescriptions.PrescriptionLoaderConfiguration

class PrescriptionsHistoryJourneyMicrotest : PrescriptionsHistoryJourney() {

    override fun createFor(patient: Patient) {
        val prescriptionLoaderConfig = PrescriptionLoaderConfiguration(
                noPrescriptions = 1, noCourses = 1, noRepeats = 1
        )
        MicrotestPrescriptionLoader.loadData(prescriptionLoaderConfig)
        client
                .forMicrotest {
                    prescriptions.getPrescriptionHistoryRequest(patient)
                            .respondWithSuccess(MicrotestPrescriptionLoader.data)
                }
    }
}