package features.myrecord.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient
import worker.models.myrecord.ImmunisationItem

abstract class ImmunisationsFactory {

    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun enabledWithRecords(patient: Patient)
    abstract fun errorRetrieving(patient: Patient)
    abstract fun noAccess(patient: Patient)
    abstract fun getExpectedImmunisations(): List<ImmunisationItem>
    abstract fun respondWithACorruptedResponse(patient: Patient)

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<ImmunisationsFactory>() {

        override val map: HashMap<Supplier, (() -> ImmunisationsFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { ImmunisationsFactoryEmis() },
                            Supplier.VISION to { ImmunisationsFactoryVision() },
                            Supplier.MICROTEST to { ImmunisationsFactoryMicrotest() })
                }

    }
}
