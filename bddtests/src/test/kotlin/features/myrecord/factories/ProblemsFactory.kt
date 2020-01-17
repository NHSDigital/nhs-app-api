package features.myrecord.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient
import worker.models.myrecord.ProblemItem

abstract class ProblemsFactory {

    abstract fun disabled(patient: Patient)
    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun enabledWithRecords(patient: Patient)
    abstract fun errorRetrieving(patient: Patient)
    abstract fun noAccess(patient: Patient)
    abstract fun getExpectedProblems(): List<ProblemItem>
    abstract fun badDataResponse(patient: Patient)

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<ProblemsFactory>() {

        override val map: HashMap<Supplier, (() -> ProblemsFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { ProblemsFactoryEmis() },
                            Supplier.VISION to { ProblemsFactoryVision() },
                            Supplier.MICROTEST to { ProblemsFactoryMicrotest() })
                }

    }
}
