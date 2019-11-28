package mocking.data.myrecord

import mocking.emis.documents.AssociatedText
import mocking.emis.documents.DocumentsMedicalRecord
import mocking.emis.documents.DocumentsResponse
import mocking.emis.documents.DocumentsResponseModel
import mocking.emis.documents.EffectiveDate
import mocking.emis.documents.Observation
import org.joda.time.DateTime

object DocumentsData {

    const val DEFAULT_NUMBER_OF_DOCUMENTS = 3
    private const val LARGE_DOCUMENT_SIZE = 4000000L
    private const val REGULAR_DOCUMENT_SIZE = 1000000L
    private const val DATE_FOR_DOCUMENT_YEAR = 2018
    private const val DATE_FOR_DOCUMENT_MONTH = 2
    private const val DATE_FOR_DOCUMENT_DAY = 17

    fun getNoDocumentData() : DocumentsResponseModel {
        return DocumentsResponseModel(
            medicalRecord = DocumentsMedicalRecord(
                documents = mutableListOf()
            )
        )
    }

    fun getDefaultDocumentsData(includeName: Boolean = true, includeTerm: Boolean = true,
                                hasInvalidType: Boolean = false, hasSize: Boolean = true): DocumentsResponseModel {
        val documents = mutableListOf<DocumentsResponse>()
        var nameFormat = "Name %d"
        var termFormat = "Letter %d"
        var type = "pdf"
        var size: Long? = REGULAR_DOCUMENT_SIZE.toLong()

        if (!includeName) {
            nameFormat = ""
        }

        if(!hasSize) {
            size = null
        }

        if (!includeTerm) {
            termFormat = ""
        }

        if (hasInvalidType) {
            type = "tga"
        }

        val date = DateTime().withDate(DATE_FOR_DOCUMENT_YEAR, DATE_FOR_DOCUMENT_MONTH,
                DATE_FOR_DOCUMENT_DAY)

        for(documentNumber in 1..DEFAULT_NUMBER_OF_DOCUMENTS){
            documents.add(DocumentsResponse(
                "document-$documentNumber",
                size,
                type,
                true,
                Observation(
                    String.format(termFormat, documentNumber),
                    mutableListOf(AssociatedText(String.format(nameFormat, documentNumber))),
                    EffectiveDate("YearMonthDay", date.plusDays(documentNumber).toString())
                )))
        }

        return DocumentsResponseModel(
            medicalRecord = DocumentsMedicalRecord(
                documents = documents
            )
        )
    }

    fun getLargeDocumentData() : DocumentsResponseModel {
        val documents = mutableListOf<DocumentsResponse>()

        val date = DateTime().withDate(DATE_FOR_DOCUMENT_YEAR, DATE_FOR_DOCUMENT_MONTH,
                DATE_FOR_DOCUMENT_DAY)

        for(documentNumber in 1..DEFAULT_NUMBER_OF_DOCUMENTS){
            documents.add(DocumentsResponse(
                    "document-$documentNumber",
                    LARGE_DOCUMENT_SIZE,
                    "pdf",
                    true,
                    Observation(
                            null,
                            mutableListOf(AssociatedText(String.format("", documentNumber))),
                            EffectiveDate("YearMonthDay", date.plusDays(documentNumber).toString())
                    )))
        }

        return DocumentsResponseModel(
                medicalRecord = DocumentsMedicalRecord(
                        documents = documents
                )
        )
    }
}
