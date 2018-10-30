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

abstract class MedicationsFactory {

    protected val twentyMonths: Long = 20
    protected val tenMonths: Long = 10
    protected val oneMonth: Long = 1

    val mockingClient = MockingClient.instance

    abstract fun enabledAndNoMedicationsMock(patient:Patient)

    abstract fun enabled(patient:Patient)

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
