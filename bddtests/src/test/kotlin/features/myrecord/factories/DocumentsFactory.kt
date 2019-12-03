package features.myrecord.factories

import mocking.MockingClient
import mocking.SupplierSpecificFactory
import models.Patient

abstract class DocumentsFactory {

    abstract fun disabled(patient: Patient)
    abstract fun enabledWithNoDocuments(patient: Patient)
    abstract fun enabledWithDocuments(patient: Patient, isLarge: Boolean = false,
                                      mockUnavailableDocument: Boolean = false)
    abstract fun enabledWithDocumentsWithNoName(patient: Patient, isLarge: Boolean = false)

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<DocumentsFactoryEmis>() {

        override val map: HashMap<String, (() -> DocumentsFactoryEmis)>
            by lazy {
                hashMapOf("EMIS" to { DocumentsFactoryEmis() })
            }

    }
}