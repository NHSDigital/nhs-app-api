package features.myrecord.factories

import constants.Supplier
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

        override val map: HashMap<Supplier, (() -> AllergiesFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { AllergiesFactoryEmis() },
                            Supplier.TPP to { AllergiesFactoryTpp() },
                            Supplier.VISION to { AllergiesFactoryVision() },
                            Supplier.MICROTEST to { AllergiesFactoryMicrotest() })
                }

    }
}
