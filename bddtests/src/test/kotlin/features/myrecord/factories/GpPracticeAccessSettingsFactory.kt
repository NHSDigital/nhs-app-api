package features.myrecord.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import mockingFacade.linkedProfiles.FeaturesEnabledFacade
import models.Patient

abstract class GpPracticeAccessSettingsFactory {

    val mockingClient = MockingClient.instance

    abstract fun enabled(patient: Patient)
    abstract fun enabledViaProxy(
            callingPatient: Patient,
            actingOnBehalfOf: Patient,
            featuresEnabled: FeaturesEnabledFacade)

    companion object : SupplierSpecificFactory<GpPracticeAccessSettingsFactory>() {

        override val map: HashMap<Supplier, (() -> GpPracticeAccessSettingsFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to
                                    { GpPracticeAccessSettingsFactoryEmis() as GpPracticeAccessSettingsFactory }
                    )
                }
    }
}
