package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.vision.VisionMockDefaults
import models.Patient

class VisionSessionCreateJourneyFactory(val client: MockingClient): SessionCreateJourneyFactory() {

    override fun createFor(patient: Patient) {
        client
                .forVision {
                    getConfigurationRequest(
                                    VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithSuccess(VisionMockDefaults.visionConfigurationResponse)
                }
    }
}
