package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import models.Patient

class MicrotestSessionCreateJourneyFactory(val client: MockingClient) : SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient) {
        // no session required for Microtest
    }
}
