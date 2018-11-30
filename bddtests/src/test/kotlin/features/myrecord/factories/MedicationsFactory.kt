package features.myrecord.factories

import features.myrecord.stepDefinitions.HTTP_EXCEPTION
import features.sharedSteps.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse
import java.util.*

const val TWENTY_MONTHS: Long = 20
const val TEN_MONTHS: Long = 10
const val ONE_MONTH: Long = 1

abstract class MedicationsFactory {

    val mockingClient = MockingClient.instance

    abstract fun enabledWithBlankRecord(patient:Patient)
    abstract fun enabledWithRecords(patient:Patient)

    fun getResult() {

        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .myRecord.getMyRecord()
            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    companion object : SupplierSpecificFactory<MedicationsFactory>() {


        override val map: HashMap<String, () -> MedicationsFactory>
                by lazy {
                    hashMapOf(
                            "EMIS" to { MedicationsFactoryEmis() },
                            "TPP" to { MedicationsFactoryTpp() },
                            "VISION" to { MedicationsFactoryVision() })
                }

    }
}
