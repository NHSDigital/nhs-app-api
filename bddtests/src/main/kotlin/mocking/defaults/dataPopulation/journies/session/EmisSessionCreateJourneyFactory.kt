package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.emis.models.AssociationType
import models.Patient

class EmisSessionCreateJourneyFactory(val client: MockingClient) : SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient) {
        createEndUserSessionRequest(patient)

        client.forEmis {
            authentication.sessionRequest(patient).respondWithSuccess(patient, AssociationType.Self)
        }
    }

    private fun createEndUserSessionRequest(patient: Patient) {
        client.forEmis {
            authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId)
        }
    }
}
