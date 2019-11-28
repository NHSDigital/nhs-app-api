package features.myrecord.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient
import worker.models.myrecord.ConsultationItem

abstract class ConsultationsFactory {

    abstract fun disabled(patient: Patient)
    abstract fun enabledWithRecords(patient: Patient)
    abstract fun getExpectedConsultations(): List<ConsultationItem>
    abstract fun enabledWithBlankRecord(patient: Patient)
    abstract fun errorRetrieving(patient: Patient)
    abstract fun recordWithBadConsultationsData(patient: Patient)

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<ConsultationsFactory>() {

        override val map: HashMap<Supplier, (() -> ConsultationsFactory)>
                by lazy {
                    hashMapOf(
                            Supplier.EMIS to { ConsultationsFactoryEmis() },
                            Supplier.TPP to { ConsultationsFactoryTpp() },
                            Supplier.VISION to { ConsultationsFactoryVision() },
                            Supplier.MICROTEST to { ConsultationsFactoryMicrotest() })
                }

    }

}