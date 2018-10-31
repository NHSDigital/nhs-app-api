package features.myrecord.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient

abstract class DemographicsFactory {

    val mockingClient = MockingClient.instance

    abstract fun enabledFunctionality(patient: Patient)

    abstract fun disabledFunctionality(patient: Patient)

    companion object : SupplierSpecificFactory<DemographicsFactory>() {

        override val map: HashMap<String, (() -> DemographicsFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { DemographicsFactoryEmis() },
                            "TPP" to { DemographicsFactoryTpp() },
                            "VISION" to { DemographicsFactoryVision() })
                }

    }
}
