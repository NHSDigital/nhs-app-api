package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import models.Patient
import org.junit.Assert

abstract class SessionCreateJourneyFactory {

    abstract fun createFor(patient: Patient)
    abstract fun createForWithLinkedAccounts(patient: Patient)

    companion object {

        private val map: (HashMap<String, (MockingClient) -> SessionCreateJourneyFactory>) by lazy { map() }

        private fun map(): HashMap<String, (MockingClient) -> SessionCreateJourneyFactory> {
            return hashMapOf(
                    "EMIS" to { client -> EmisSessionCreateJourneyFactory(client) },
                    "TPP" to { client -> TppSessionCreateJourneyFactory(client) },
                    "VISION" to { client -> VisionSessionCreateJourneyFactory(client) },
                    "MICROTEST" to { client -> MicrotestSessionCreateJourneyFactory(client) }
            )
        }

        fun getForSupplier(gpSystem: String, mockingClient: MockingClient): SessionCreateJourneyFactory {
            if (!map.containsKey(gpSystem.toUpperCase())) {
                Assert.fail("GP system '$gpSystem' is not set up.")
            }
            return map.getValue(gpSystem).invoke(mockingClient)
        }
    }
}