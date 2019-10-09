package features.myrecord.factories

import mocking.MockingClient
import mocking.SupplierSpecificFactory
import worker.models.myrecord.EncounterItem

abstract class EncountersFactory {

    abstract fun getExpectedEncounters(): List<EncounterItem>

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<EncountersFactory>() {

        override val map: HashMap<String, (() -> EncountersFactory)>
                by lazy {
                    hashMapOf(
                            "MICROTEST" to { EncountersFactoryMicrotest() as EncountersFactory }
                    )
                }
    }
}