package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import models.Patient

class SessionJournies(private val client: MockingClient) {
    fun create() {
        val cidFactory = CitizenIdSessionCreateJourney(client)
        val emisFactory = EmisSessionCreateJourneyFactory(client)
        val tppFactory = TppSessionCreateJourneyFactory(client)
        val visionFactory = VisionSessionCreateJourneyFactory(client)

        cidFactory.createFor(Patient.alanCook)
        emisFactory.createFor(Patient.alanCook)
        cidFactory.createFor(Patient.jackJackson)
        emisFactory.createFor(Patient.jackJackson)
        cidFactory.createFor(Patient.paulSmith)
        emisFactory.createFor(Patient.paulSmith)
        cidFactory.createFor(Patient.montelFrye)
        emisFactory.createFor(Patient.montelFrye)

        tppFactory.createFor(Patient.kevinBarry)

        visionFactory.createFor(Patient.aderynCanon)
    }
}
