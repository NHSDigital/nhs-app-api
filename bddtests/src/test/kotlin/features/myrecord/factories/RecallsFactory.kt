package features.myrecord.factories

import mocking.MockingClient
import mocking.SupplierSpecificFactory
import worker.models.myrecord.RecallItem

abstract class RecallsFactory {

    abstract fun getExpectedRecalls(): List<RecallItem>

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<RecallsFactory>() {

        override val map: HashMap<String, (() -> RecallsFactory)>
            by lazy {
                hashMapOf(
                        "MICROTEST" to { RecallsFactoryMicrotest() as RecallsFactory }
                )
            }

    }
}
