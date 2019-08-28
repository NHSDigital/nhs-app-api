package features.myrecord.factories

import mocking.MockingClient
import mocking.SupplierSpecificFactory
import models.Patient
import worker.models.myrecord.MedicalHistoryItem

abstract class MedicalHistoryFactory {

    abstract fun enabledWithBlankRecord(patient:Patient)
    abstract fun enabledWithRecords(patient:Patient)
    abstract fun getExpectedMedicalHistory(): List<MedicalHistoryItem>

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<MedicalHistoryFactory>() {

        override val map: HashMap<String, (() -> MedicalHistoryFactory)>
            by lazy {
                hashMapOf(
                        "MICROTEST" to { MedicalHistoryFactoryMicrotest() as MedicalHistoryFactory }                 )
            }

    }
}
