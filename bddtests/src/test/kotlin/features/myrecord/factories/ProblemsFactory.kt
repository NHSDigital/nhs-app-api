package features.myrecord.factories

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

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<ProblemsFactory>() {

        override val map: HashMap<String, (() -> ProblemsFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { ProblemsFactoryEmis() },
                            "VISION" to { ProblemsFactoryVision() },
                            "MICROTEST" to { ProblemsFactoryMicrotest() })
                }

    }
}
