package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import models.patients.EmisPatients
import models.patients.TppPatients
import models.patients.VisionPatients

class SessionJournies(private val client: MockingClient) {
    fun create() {
        val cidFactory = CitizenIdSessionCreateJourney(client)
        val emisFactory = EmisSessionCreateJourneyFactory(client)
        val tppFactory = TppSessionCreateJourneyFactory(client)
        val visionFactory = VisionSessionCreateJourneyFactory(client)

        cidFactory.createFor(EmisPatients.alanCook)
        emisFactory.createFor(EmisPatients.alanCook)
        cidFactory.createFor(EmisPatients.jackJackson)
        emisFactory.createFor(EmisPatients.jackJackson)
        cidFactory.createFor(EmisPatients.paulSmith)
        emisFactory.createFor(EmisPatients.paulSmith)
        cidFactory.createFor(EmisPatients.montelFrye)
        emisFactory.createFor(EmisPatients.montelFrye)

        cidFactory.createFor(TppPatients.kevinBarry)
        tppFactory.createFor(TppPatients.kevinBarry)

        visionFactory.createFor(VisionPatients.aderynCanon)
    }
}
