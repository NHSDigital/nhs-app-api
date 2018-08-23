package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.defaults.MockDefaults
import models.Patient

class VisionSessionCreateJourneyFactory(val client: MockingClient): SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient) {
        client
                .forVision {
                    getConfigurationRequest(
                                    MockDefaults.visionUserSession,
                            MockDefaults.visionGetConfiguration)
                            .respondWithSuccess(MockDefaults.visionConfigurationResponse)
                }
    }
}