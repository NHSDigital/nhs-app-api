package features.myrecord.factories

import constants.Supplier
import mocking.MockingClient
import mocking.SupplierSpecificFactory
import worker.models.myrecord.EncounterItem

abstract class EncountersFactory {

    abstract fun getExpectedEncounters(): List<EncounterItem>

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<EncountersFactory>() {

        override val map: HashMap<Supplier, (() -> EncountersFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.MICROTEST to { EncountersFactoryMicrotest() as EncountersFactory }
                    )
                }
    }
}