package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import models.Patient

class SessionJournies(private val client: MockingClient) {
    fun create() {
        val factory = EmisSessionCreateJourneyFactory(client)

        factory.createFor(Patient.alanCook)
        factory.createFor(Patient.jackJackson)
        factory.createFor(Patient.paulSmith)
        factory.createFor(Patient.montelFrye)
    }
}