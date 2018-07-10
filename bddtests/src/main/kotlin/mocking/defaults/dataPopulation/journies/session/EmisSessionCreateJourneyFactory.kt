package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.emis.models.AssociationType
import models.Patient

class EmisSessionCreateJourneyFactory(val client: MockingClient) {

    fun createFor(patient: Patient) {
        client
                .forEmis {
                    endUserSessionRequest()
                            .respondWithSuccess(patient.endUserSessionId)
                }

        client
                .forEmis {
                    sessionRequest(patient)
                            .respondWithSuccess(patient, AssociationType.Self)
                }
    }
}