package features.myrecord.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient
import net.serenitybdd.core.Serenity
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrNull
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MedicationsData
import worker.models.myrecord.MyRecordResponse
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

    fun getResult() {

        try {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrNull<String>()

            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .myRecord.getMyRecord(patientId)
            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

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
