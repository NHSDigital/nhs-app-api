package features.myrecord.factories

import constants.Supplier
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

        override val map: HashMap<Supplier, (() -> TestResultsFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { TestResultsFactoryEmis() },
                            Supplier.TPP to { TestResultsFactoryTpp() },
                            Supplier.VISION to { TestResultsFactoryVision() },
                            Supplier.MICROTEST to { TestResultsFactoryMicrotest() })
                }

    }
}