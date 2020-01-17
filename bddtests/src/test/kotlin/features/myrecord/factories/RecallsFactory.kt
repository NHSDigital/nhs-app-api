package features.myrecord.factories

import constants.Supplier
import mocking.MockingClient
import mocking.SupplierSpecificFactory
import worker.models.myrecord.RecallItem

abstract class RecallsFactory {

    abstract fun getExpectedRecalls(): List<RecallItem>
    abstract fun respondWithCorruptedResponse()

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<RecallsFactory>() {

        override val map: HashMap<Supplier, (() -> RecallsFactory)>
            by lazy {
                hashMapOf(
                        Supplier.MICROTEST to { RecallsFactoryMicrotest() as RecallsFactory }
                )
            }

    }
}
