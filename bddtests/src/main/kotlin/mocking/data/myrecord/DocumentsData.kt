package mocking.data.myrecord

import mocking.emis.documents.AssociatedText
import mocking.emis.documents.DocumentsMedicalRecord
import mocking.emis.documents.DocumentsResponse
import mocking.emis.documents.DocumentsResponseModel
import mocking.emis.documents.EffectiveDate
import mocking.emis.documents.Observation

object DocumentsData {

    const val DEFAULT_NUMBER_OF_DOCUMENTS = 3
    private const val DOCUMENT_SIZE = 1000000

    fun getNoDocumentData() : DocumentsResponseModel {
        return DocumentsResponseModel(
            medicalRecord = DocumentsMedicalRecord(
                documents = mutableListOf()
            )
        )
    }

    fun getDefaultDocumentsData(includeName: Boolean = true) : DocumentsResponseModel {
        val documents = mutableListOf<DocumentsResponse>()
        var nameFormat = "Name %d"
        if (!includeName) {
            nameFormat = ""
        }

        for(documentNumber in 1..DEFAULT_NUMBER_OF_DOCUMENTS){
            documents.add(DocumentsResponse(
                "document-$documentNumber",
                DOCUMENT_SIZE,
                "pdf",
                true,
                Observation(
                    "History $documentNumber",
                    mutableListOf(AssociatedText(String.format(nameFormat, documentNumber))),
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
