package features.myrecord.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import mocking.microtest.myRecord.MyRecordModuleCounts
import mocking.microtest.myRecord.TestResultOptions
import models.Patient

abstract class MyRecordFactory {

    val mockingClient = MockingClient.instance

    abstract fun disabled(patient:Patient)
    abstract fun disabledForProxy(patient:Patient, actingOnBehalfOf: Patient)
    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun enabledWithData(
            patient: Patient, myRecordModuleCounts: MyRecordModuleCounts, testResultOptions: TestResultOptions)
    abstract fun enabledWithAllRecords(patient: Patient)
    abstract fun respondWithForbidden(patient: Patient)

    companion object : SupplierSpecificFactory<MyRecordFactory>() {

        override val map: HashMap<Supplier, (() -> MyRecordFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { MyRecordFactoryEmis() },
                            Supplier.TPP to { MyRecordFactoryTpp() },
                            Supplier.VISION to { MyRecordFactoryVision() },
                            Supplier.MICROTEST to { MyRecordFactoryMicrotest() })
                }

    }
}
