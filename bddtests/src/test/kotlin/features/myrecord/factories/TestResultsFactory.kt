package features.myrecord.factories

import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient

abstract class TestResultsFactory {

    abstract fun disabled(patient: Patient)
    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun enabledWithRecords(patient: Patient)
    abstract fun errorRetrieving(patient: Patient)
    abstract fun noAccess(patient: Patient)

    val mockingClient = MockingClient.instance


    companion object : SupplierSpecificFactory<TestResultsFactory>() {

        override val map: HashMap<String, (() -> TestResultsFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { TestResultsFactoryEmis() },
                            "TPP" to { TestResultsFactoryTpp() },
                            "VISION" to { TestResultsFactoryVision() })
                }

    }
}