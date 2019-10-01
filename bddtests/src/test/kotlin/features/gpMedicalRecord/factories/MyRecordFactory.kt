package features.gpMedicalRecord.factories

import mocking.MockingClient
import mocking.SupplierSpecificFactory
import mocking.microtest.myRecord.MyRecordModuleCounts
import models.Patient

abstract class MyRecordFactory {

    val mockingClient = MockingClient.instance

    abstract fun disabled(patient:Patient)
    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun enabledWithData(patient: Patient, myRecordModuleCounts: MyRecordModuleCounts)
    abstract fun enabledWithAllRecords(patient: Patient)
    abstract fun respondWithForbidden(patient: Patient)

    companion object : SupplierSpecificFactory<MyRecordFactory>() {

        override val map: HashMap<String, (() -> MyRecordFactory)>
                by lazy {
                    hashMapOf(
                            "EMIS" to { MyRecordFactoryEmis() },
                            "TPP" to { MyRecordFactoryTpp() },
                            "VISION" to { MyRecordFactoryVision() },
                            "MICROTEST" to { MyRecordFactoryMicrotest() })
                }

    }
}
