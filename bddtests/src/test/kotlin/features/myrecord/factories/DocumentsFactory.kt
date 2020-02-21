package features.myrecord.factories

import constants.Supplier
import mocking.MockingClient
import mocking.SupplierSpecificFactory
import models.Patient

abstract class DocumentsFactory {

    abstract fun disabled(patient: Patient)
    abstract fun enabledWithNoDocuments(patient: Patient)
    abstract fun enabledWithDocuments(patient: Patient, isLarge: Boolean = false,
                                      mockUnavailableDocument: Boolean = false, hasInvalidType: Boolean = false)
    abstract fun enabledWithDocumentsWithNoNameOrTerm(patient: Patient, isLarge: Boolean = false)
    abstract fun enabledWithLettersWithNoNameOrTerm(patient: Patient, isLarge: Boolean = false)
    abstract fun enabledWithNullPageCount()
    abstract fun enabledWithNullSize()
    abstract fun enabledWithDocumentsWithUnknownDate(patient: Patient, isLarge: Boolean = false)

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<DocumentsFactory>() {

        override val map: HashMap<Supplier, (() -> DocumentsFactory)>
            by lazy {
                hashMapOf(
                        Supplier.EMIS to { DocumentsFactoryEmis() },
                        Supplier.TPP to { DocumentsFactoryTpp() })
            }
    }
}