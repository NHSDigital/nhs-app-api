package mocking.defaults.dataPopulation.journies.prescriptions

import constants.Supplier
import mocking.MockingClient
import mocking.SupplierSpecificFactory
import models.Patient
import java.util.*

abstract class PrescriptionsHistoryJourney  {

    protected val client = MockingClient.instance

    abstract fun createFor(patient: Patient)

    companion object : SupplierSpecificFactory<PrescriptionsHistoryJourney>() {

        override val map: HashMap<Supplier, (() -> (PrescriptionsHistoryJourney))> by lazy {
            hashMapOf(
                    Supplier.EMIS to { PrescriptionsHistoryJourneyEmis() },
                    Supplier.TPP to { throw NotImplementedError("Not implemented") },
                    Supplier.VISION to { PrescriptionsHistoryJourneyVision() },
                    Supplier.MICROTEST to { PrescriptionsHistoryJourneyMicrotest() }
            )
        }
    }
}
