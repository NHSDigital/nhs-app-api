package features.myrecord.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import worker.models.myrecord.EncounterItem

abstract class EncountersFactory {

    abstract fun getExpectedEncounters(): List<EncounterItem>

    companion object : SupplierSpecificFactory<EncountersFactory>() {

        override val map: HashMap<Supplier, (() -> EncountersFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.MICROTEST to { EncountersFactoryMicrotest() as EncountersFactory }
                    )
                }
    }
}
