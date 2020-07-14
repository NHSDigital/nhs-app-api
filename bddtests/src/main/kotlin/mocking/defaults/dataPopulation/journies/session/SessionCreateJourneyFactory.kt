package mocking.defaults.dataPopulation.journies.session

import constants.Supplier
import mocking.MockingClient
import mocking.SupplierSpecificFactory
import models.Patient
import utils.GlobalSerenityHelpers
import utils.isTrueOrFalse
import utils.set
import java.util.*

abstract class SessionCreateJourneyFactory {

    protected val client = MockingClient.instance

    fun createFor(patient: Patient, defaultPracticeSettings: Boolean = true, alternativeUser:Boolean = false) {
        if (!GlobalSerenityHelpers.USER_SESSION_CREATED.isTrueOrFalse() || alternativeUser) {
            createFor(patient, defaultPracticeSettings)
            GlobalSerenityHelpers.USER_SESSION_CREATED.set(true)
        }
    }

    protected abstract fun createFor(patient: Patient, defaultPracticeSettings: Boolean)

    companion object : SupplierSpecificFactory<SessionCreateJourneyFactory>() {

        override val map: HashMap<Supplier, (() -> (SessionCreateJourneyFactory))> by lazy {
            hashMapOf(
                    Supplier.EMIS to { EmisSessionCreateJourneyFactory() },
                    Supplier.TPP to {  TppSessionCreateJourneyFactory() },
                    Supplier.VISION to { VisionSessionCreateJourneyFactory() },
                    Supplier.MICROTEST to { MicrotestSessionCreateJourneyFactory() }
            )
        }
    }
}
