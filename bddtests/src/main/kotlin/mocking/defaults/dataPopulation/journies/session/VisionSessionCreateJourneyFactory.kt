package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.models.AssociationType
import mocking.vision.VisionConstants
import mocking.vision.models.*
import models.Patient

class VisionSessionCreateJourneyFactory(val client: MockingClient) {

    fun createFor(patient: Patient) {
        client
                .forVision {
                    getConfigurationRequest(
                                    MockDefaults.visionUserSession,
                            MockDefaults.visionGetConfiguration)
                            .respondWithSuccess(MockDefaults.visionConfigurationResponse)
                }
    }
}