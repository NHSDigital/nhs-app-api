package features.myrecord.factories

import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient
import worker.models.myrecord.AllergyItem

abstract class AllergiesFactory {

    abstract fun disabled(patient: Patient)
    abstract fun enabledWithRecords(patient: Patient, count: Int)
    abstract fun getExpectedAllergies(): List<AllergyItem>

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<AllergiesFactory>() {

        override val map: HashMap<String, (() -> AllergiesFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { AllergiesFactoryEmis() },
                            "TPP" to { AllergiesFactoryTpp() },
                            "VISION" to { AllergiesFactoryVision() },
                            "MICROTEST" to { AllergiesFactoryMicrotest() })
                }

    }
}
