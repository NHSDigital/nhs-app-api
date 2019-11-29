package mocking.defaults.dataPopulation.journies.session

import constants.Supplier
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

        private val map: (HashMap<Supplier, (MockingClient) -> SessionCreateJourneyFactory>) by lazy { map() }

        private fun map(): HashMap<Supplier, (MockingClient) -> SessionCreateJourneyFactory> {
            return hashMapOf(
                    Supplier.EMIS to { client -> EmisSessionCreateJourneyFactory(client) },
                    Supplier.TPP to { client -> TppSessionCreateJourneyFactory(client) },
                    Supplier.VISION to { client -> VisionSessionCreateJourneyFactory(client) },
                    Supplier.MICROTEST to { client -> MicrotestSessionCreateJourneyFactory(client) }
            )
        }

        fun getForSupplier(gpSystem: Supplier, mockingClient: MockingClient): SessionCreateJourneyFactory {
            if (!map.containsKey(gpSystem)) {
                Assert.fail("GP system '$gpSystem' is not set up.")
            }
            return map.getValue(gpSystem).invoke(mockingClient)
        }
    }
}