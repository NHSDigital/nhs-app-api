package mocking.defaults.dataPopulation.journies.prescriptions

import mocking.MockingClient
import mocking.emis.models.PrescriptionRequestsGetResponse
import models.Patient

class PrescriptionsHistoryJourney(private val client: MockingClient) {

    fun createFor(patient: Patient) {

        val prescriptions = listOf(
                PrescriptionsData.loadPrescriptionsData(7,7,7),
                PrescriptionsData.loadPrescriptionsData(1,1,1, false),
                PrescriptionsData.loadPrescriptionsData(1,1,1, true, false),
                PrescriptionsData.loadPrescriptionsData(1,1,1, false, false)
        )

        client
                .forEmis {
                    prescriptionsRequest(patient)
                            .respondWithSuccess(
                                    PrescriptionsData.addResponses(prescriptions)
                            )
                }
    }

}