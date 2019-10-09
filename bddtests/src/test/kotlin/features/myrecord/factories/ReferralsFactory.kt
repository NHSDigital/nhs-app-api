package features.myrecord.factories

import mocking.MockingClient
import mocking.SupplierSpecificFactory
import worker.models.myrecord.ReferralItem

abstract class ReferralsFactory {

    abstract fun getExpectedReferrals(): List<ReferralItem>

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<ReferralsFactory>() {

        override val map: HashMap<String, (() -> ReferralsFactory)>
                by lazy {
                    hashMapOf(
                            "MICROTEST" to { ReferralsFactoryMicrotest() as ReferralsFactory }
                    )
                }
    }
}