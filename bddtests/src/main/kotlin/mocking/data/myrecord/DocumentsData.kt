package mocking.data.myrecord

import mocking.emis.documents.AssociatedText
import mocking.emis.documents.DocumentsMedicalRecord
import mocking.emis.documents.DocumentsResponse
import mocking.emis.documents.DocumentsResponseModel
import mocking.emis.documents.EffectiveDate
import mocking.emis.documents.Observation

object DocumentsData {

    private const val NUMBER_OF_DOCUMENTS = 3
    private const val DOCUMENT_SIZE = 1000000

    fun getDefaultDocumentsData() : DocumentsResponseModel {
        return DocumentsResponseModel(
                medicalRecord = DocumentsMedicalRecord(
                        documents = mutableListOf()
                )
        )
    }

    fun getMultipleDocuments(): DocumentsResponseModel {

        val documents = mutableListOf<DocumentsResponse>()

        for(documentNumber in 1..NUMBER_OF_DOCUMENTS){
            documents.add(DocumentsResponse(
                    "document-$documentNumber",
                    DOCUMENT_SIZE,
                    "pdf",
                    true,
                    Observation(
                            "History $documentNumber",
                            mutableListOf(AssociatedText("Name $documentNumber")),
                            EffectiveDate("YearMonthDay", "2018-02-18T14:23:44.927")
                    )))
        }

        return DocumentsResponseModel(
                medicalRecord = DocumentsMedicalRecord(
                        documents = documents
                )
        )
    }
}
