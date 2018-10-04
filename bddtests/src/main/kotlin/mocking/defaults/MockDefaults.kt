package mocking.defaults

import mocking.MockingClient
import mocking.emis.EmisConfiguration
import config.Config
import mocking.MockingConfiguration
import mocking.vision.models.VisionUserSession

import models.Patient

class MockDefaults {

    companion object {
        const val DEFAULT_END_USER_SESSION_ID: String = "7YjG1LYkOkSY1iAcXGG8ZU"
        const val DEFAULT_CONNECTION_TOKEN: String = "28681a98-e280-4038-af63-d5ad39f2833c"
        const val DEFAULT_ODS_CODE: String = "A29928"
        const val DEFAULT_ODS_CODE_VISION: String = "X00100"


        fun createMockingClient(config: Config): MockingClient {
            val emisConfig = EmisConfiguration(config.emisApplicationId, "2.1.0.0")

            return MockingClient(MockingConfiguration(config.wiremockUrl, emisConfig))
        }

        val patient = Patient.getDefault("EMIS")

        val visionUserSession = VisionUserSession(
                Patient.aderynCanon.rosuAccountId,
                Patient.aderynCanon.apiKey,
                Patient.aderynCanon.odsCode,
                Patient.aderynCanon.patientId)
    }
}
