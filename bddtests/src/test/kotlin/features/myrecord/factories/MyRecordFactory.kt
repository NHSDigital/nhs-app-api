package features.myrecord.factories

import features.sharedSteps.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient

abstract class MyRecordFactory {

    val mockingClient = MockingClient.instance

    abstract fun disabled(patient:Patient)
    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun enabledWithData(patient: Patient, numAllergies: Int)

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
