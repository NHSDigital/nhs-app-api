package mocking.defaults.dataPopulation.journies.prescriptions

import mocking.MockingClient
import models.Patient

class RepeatPrescriptionsJourney(private val client: MockingClient) {


    fun createFor(patient: Patient) {
        client
                .forEmis {
                    repeatPrescriptionSubmissionRequest(patient)
                            .respondWithCreated()
                }
    }

}