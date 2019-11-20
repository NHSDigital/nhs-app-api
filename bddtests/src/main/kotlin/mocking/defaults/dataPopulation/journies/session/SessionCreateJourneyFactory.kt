package mocking.defaults.dataPopulation.journies.session

import mocking.MockingClient
import models.Patient
import org.junit.Assert
import utils.GlobalSerenityHelpers
import utils.isTrueOrFalse
import utils.set

abstract class SessionCreateJourneyFactory {

    fun createFor(patient: Patient, defaultPracticeSettings: Boolean = true, alternativeUser:Boolean = false) {
        if (!GlobalSerenityHelpers.USER_SESSION_CREATED.isTrueOrFalse() || alternativeUser) {
            createFor(patient, defaultPracticeSettings)
            GlobalSerenityHelpers.USER_SESSION_CREATED.set(true)
        }
    }

    protected abstract fun createFor(patient: Patient, defaultPracticeSettings: Boolean)

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