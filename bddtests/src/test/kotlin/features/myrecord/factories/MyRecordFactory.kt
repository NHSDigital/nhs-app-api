package features.myrecord.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient

abstract class MyRecordFactory {

    val mockingClient = MockingClient.instance

    abstract fun disabled(patient:Patient)
    abstract fun disabledForProxy(patient:Patient, actingOnBehalfOf: Patient)
    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun enabledWithNoDcrAccess(patient: Patient)
    abstract fun enabledWithData(patient: Patient)
    abstract fun enabledWithAllRecords(patient: Patient)
    abstract fun respondWithForbidden(patient: Patient)

    companion object : SupplierSpecificFactory<MyRecordFactory>() {

        override val map: HashMap<Supplier, (() -> MyRecordFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { MyRecordFactoryEmis() },
                            Supplier.TPP to { MyRecordFactoryTpp() },
                            Supplier.VISION to { MyRecordFactoryVision() })
                }

    }
}
