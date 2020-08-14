package features.myrecord.factories

import constants.Supplier
import mocking.MockingClient
import mocking.SupplierSpecificFactory
import models.Patient
import worker.models.myrecord.MedicationsData
import java.util.*

const val TWENTY_MONTHS: Long = 20
const val TEN_MONTHS: Long = 10
const val ONE_MONTH: Long = 1

abstract class MedicationsFactory {

    val mockingClient = MockingClient.instance

    abstract fun enabledWithBlankRecord(patient:Patient)
    abstract fun enabledWithRecords(patient:Patient)
    abstract fun getExpectedMedications(): MedicationsData
    abstract fun respondWithBadData(patient: Patient)

    companion object : SupplierSpecificFactory<MedicationsFactory>() {


        override val map: HashMap<Supplier, () -> MedicationsFactory>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { MedicationsFactoryEmis() },
                            Supplier.TPP to { MedicationsFactoryTpp() },
                            Supplier.VISION to { MedicationsFactoryVision() },
                            Supplier.MICROTEST to { MedicationsFactoryMicrotest() })
                }

    }
}
