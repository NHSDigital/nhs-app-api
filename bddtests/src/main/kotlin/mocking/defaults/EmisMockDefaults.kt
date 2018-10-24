package mocking.defaults

import config.Config
import mocking.MockingClient
import mocking.MockingConfiguration
import mocking.emis.EmisConfiguration
import models.Patient

class EmisMockDefaults{
    companion object {

        const val DEFAULT_ODS_CODE_EMIS: String = "A29928"
        const val DEFAULT_CONNECTION_TOKEN: String = "28681a98-e280-4038-af63-d5ad39f2833c"

        val patientEmis = Patient.getDefault("EMIS")

        fun createMockingClient(config: Config): MockingClient {
            val emisConfig = EmisConfiguration(config.emisApplicationId, "2.1.0.0")

            return MockingClient(MockingConfiguration(config.wiremockUrl, emisConfig))
        }
    }
}