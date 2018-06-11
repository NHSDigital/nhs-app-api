package mocking.defaults.dataPopulation.journies.prescriptions

import mocking.MockingClient
import models.Patient

class PrescriptionsHistoryJourney(private val client: MockingClient) {

    fun createFor(patient: Patient) {

        client
                .forEmis {
                    prescriptionsRequest(patient)
                            .respondWithSuccess(PrescriptionsData.loadPrescriptionsData(10,10,10))
                }
    }

}