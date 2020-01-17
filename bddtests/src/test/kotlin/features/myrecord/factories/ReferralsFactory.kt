package features.myrecord.factories

import constants.Supplier
import mocking.MockingClient
import mocking.SupplierSpecificFactory
import worker.models.myrecord.ReferralItem

abstract class ReferralsFactory {

    abstract fun getExpectedReferrals(): List<ReferralItem>
    abstract fun respondWithCorruptedReponse()

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<ReferralsFactory>() {

        override val map: HashMap<Supplier, (() -> ReferralsFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.MICROTEST to { ReferralsFactoryMicrotest() as ReferralsFactory }
                    )
                }
    }
}