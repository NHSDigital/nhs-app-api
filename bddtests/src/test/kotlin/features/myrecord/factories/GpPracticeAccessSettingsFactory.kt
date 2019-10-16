package features.myrecord.factories

import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient

abstract class GpPracticeAccessSettingsFactory {

    val mockingClient = MockingClient.instance

    abstract fun enabled(patient: Patient)
    abstract fun enabledViaProxy(callingPatient: Patient, actingOnBehalfOf: Patient)

    companion object : SupplierSpecificFactory<GpPracticeAccessSettingsFactory>() {

        override val map: HashMap<String, (() -> GpPracticeAccessSettingsFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { GpPracticeAccessSettingsFactoryEmis() as GpPracticeAccessSettingsFactory }
                    )
                }
    }
}
