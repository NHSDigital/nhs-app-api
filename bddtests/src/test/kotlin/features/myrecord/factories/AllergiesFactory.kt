package features.myrecord.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient

abstract class AllergiesFactory {

    abstract fun disabled(patient: Patient)
    abstract fun enabledWithRecords(patient: Patient, count: Int)

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<AllergiesFactory>() {

        override val map: HashMap<String, (() -> AllergiesFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { AllergiesFactoryEmis() },
                            "TPP" to { AllergiesFactoryTpp() },
                            "VISION" to { AllergiesFactoryVision() })
                }

    }
}
