package features.myrecord.factories

import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient
import worker.models.myrecord.TestResultItem

abstract class TestResultsFactory {

    abstract fun disabled(patient: Patient)
    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun enabledWithRecords(patient: Patient)
    abstract fun errorRetrieving(patient: Patient)
    abstract fun noAccess(patient: Patient)
    abstract fun getExpectedTestResults() : List<TestResultItem>

    val mockingClient = MockingClient.instance


    companion object : SupplierSpecificFactory<TestResultsFactory>() {

        override val map: HashMap<String, (() -> TestResultsFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { TestResultsFactoryEmis() },
                            "TPP" to { TestResultsFactoryTpp() },
                            "VISION" to { TestResultsFactoryVision() },
                            "MICROTEST" to { TestResultsFactoryMicrotest() })
                }

    }
}