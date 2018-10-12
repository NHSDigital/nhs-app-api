package features.prescriptions.factories

import features.sharedSteps.SerenityHelpers
import features.sharedSteps.SupplierSpecificFactory
import mocking.MockingClient
import mocking.data.prescriptions.IPrescriptionLoader
import models.Patient

abstract class PrescriptionsFactory(gpSupplier:String) {

    val mockingClient = MockingClient.instance
    val patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSupplier)

    abstract fun disableAtGPLevel()
    abstract val getPrescriptionsLoader: IPrescriptionLoader<*>

    companion object : SupplierSpecificFactory<PrescriptionsFactory>() {

        override val map: HashMap<String, (() -> PrescriptionsFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { PrescriptionsFactoryEmis() },
                            "TPP" to { PrescriptionsFactoryTpp() },
                            "VISION" to { PrescriptionsFactoryVision() })
                }

    }
}