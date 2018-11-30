package features.myrecord.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient

abstract class ImmunisationsFactory {

    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun enabledWithRecords(patient: Patient)
    abstract fun errorRetrieving(patient: Patient)
    abstract fun noAccess(patient: Patient)

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<ImmunisationsFactory>() {

        override val map: HashMap<String, (() -> ImmunisationsFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { ImmunisationsFactoryEmis() },
                            "VISION" to { ImmunisationsFactoryVision() })
                }

    }
}
