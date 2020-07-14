package features.myrecord.factories

import features.myrecord.stepDefinitions.V2MedicalRecordDocumentsStepDefinitions.SerenityVariable
import mocking.data.myrecord.DocumentData
import mocking.data.myrecord.DocumentsData
import mocking.data.myrecord.DocumentsData.DEFAULT_NUMBER_OF_DOCUMENTS
import mocking.defaults.EmisMockDefaults
import mocking.emis.documents.DocumentsResponse
import models.ExpectedDocument
import models.Patient
import utils.SerenityHelpers

const val LARGE_DOCUMENT_SIZE = 4
const val REGULAR_DOCUMENT_SIZE = 1
private const val DATE_FOR_DOCUMENT_DAY = 18

class DocumentsFactoryEmis: DocumentsFactory() {

    override fun disabled(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.documentsRequest(patient)
                .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithNoDocuments(patient: Patient) {
        mockingClient.forEmis.mock {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                .respondWithSuccess(DocumentsData.getNoDocumentData())
        }
    }

    override fun enabledWithNullPageCount() {
        val documents = DocumentsData.getDefaultDocumentsData()

        mockingClient.forEmis.mock {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                    .respondWithNullPageCount()
        }

        val expectedDocuments = getExpectedDocumentsFromEmisDocuments(false,
                documents = documents.medicalRecord.documents)
        setSerenityVariable(SerenityVariable.EXPECTED_DOCUMENTS, arrayListOf<ExpectedDocument>(
                expectedDocuments[expectedDocuments.lastIndex]))
    }

    override fun enabledWithNullSize() {
        val documents = DocumentsData.getDefaultDocumentsData(hasSize = false)

        mockingClient.forEmis.mock {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                    .respondWithSuccess(documents)
        }

        val expectedDocuments = getExpectedDocumentsFromEmisDocuments(false,
                documents = documents.medicalRecord.documents, includeSize = false)
        setSerenityVariable(SerenityVariable.EXPECTED_DOCUMENTS, arrayListOf<ExpectedDocument>(expectedDocuments[0]))
    }

    override fun enabledWithDocuments(patient: Patient, documentStatus: DocumentStatus?) {
        var documents = DocumentsData.getDefaultDocumentsData(
                hasInvalidType = documentStatus == DocumentStatus.HasInvalidType)
        val isLarge = documentStatus == DocumentStatus.IsLarge
        var expectedDocuments = getExpectedDocumentsFromEmisDocuments(
                isLarge, documents.medicalRecord.documents, true)

        if (isLarge) {
            documents = DocumentsData.getLargeDocumentData()
            expectedDocuments = getExpectedDocumentsFromEmisDocuments(isLarge, documents.medicalRecord.documents)
        }

        setSerenityVariable(SerenityVariable.EXPECTED_DOCUMENTS, expectedDocuments)

        val availableDocument = expectedDocuments[0]
        setSerenityVariable(SerenityVariable.AVAILABLE_DOCUMENT, availableDocument)

        if (documentStatus == DocumentStatus.MockUnavailableDocument) {
            val unavailableDocument = expectedDocuments[DEFAULT_NUMBER_OF_DOCUMENTS - 1]
            setSerenityVariable(SerenityVariable.UNAVAILABLE_DOCUMENT, unavailableDocument)
        }

        mockingClient.forEmis.mock {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                    .respondWithSuccess(documents)
        }
        mockingClient.forEmis.mock {
            myRecord.documentRequest(EmisMockDefaults.patientEmis, availableDocument.id)
                    .respondWithSuccess(DocumentData.getDefaultDocumentData())
        }
    }

    override fun enabledWithDocumentsWithNoNameOrTerm(patient: Patient, isLarge: Boolean) {
        val documents = if (isLarge) DocumentsData.getLargeDocumentData()
            else DocumentsData.getDefaultDocumentsData(false, false)

        val expectedDocuments = getExpectedDocumentsFromEmisDocuments(false, documents.medicalRecord.documents,
                false, false)

        setSerenityVariable(SerenityVariable.EXPECTED_DOCUMENTS, expectedDocuments)

        val availableDocument = expectedDocuments[0]
        setSerenityVariable(SerenityVariable.AVAILABLE_DOCUMENT, availableDocument)

        mockingClient.forEmis.mock {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                    .respondWithSuccess(documents)
        }
        mockingClient.forEmis.mock {
            myRecord.documentRequest(EmisMockDefaults.patientEmis, availableDocument.id)
                    .respondWithSuccess(DocumentData.getDefaultDocumentData())
        }
    }

    override fun enabledWithLettersWithNoNameOrTerm(patient: Patient, isLarge: Boolean) {
        //Not required for emis
    }

    override fun enabledWithDocumentsWithUnknownDate(patient: Patient, isLarge: Boolean) {
        val documents = if (isLarge) DocumentsData.getLargeDocumentData()
        else DocumentsData.getDefaultDocumentsData(true, true)
        documents.medicalRecord.documents[0].observation.effectiveDate = null

        val expectedDocuments = getExpectedDocumentsFromEmisDocuments(isLarge, documents.medicalRecord.documents,
                true, true)
        expectedDocuments[documents.medicalRecord.documents.lastIndex].date = "Unknown Date"
        setSerenityVariable(SerenityVariable.EXPECTED_DOCUMENTS, expectedDocuments)

        val availableDocument = expectedDocuments[0]
        setSerenityVariable(SerenityVariable.AVAILABLE_DOCUMENT, availableDocument)

        mockingClient.forEmis.mock {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                    .respondWithSuccess(documents)
        }
        mockingClient.forEmis.mock {
            myRecord.documentRequest(EmisMockDefaults.patientEmis, availableDocument.id)
                    .respondWithSuccess(DocumentData.getDefaultDocumentData())
        }
    }

    private fun setSerenityVariable(key: Any, value: Any) {
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(key, value)
    }

    private fun getExpectedDocumentsFromEmisDocuments(isLarge: Boolean,
        documents: List<DocumentsResponse>,
        includeName: Boolean = true, includeTerm: Boolean = true, includeSize: Boolean = true)
            : List<ExpectedDocument> {

        val expectedDocuments= mutableListOf<ExpectedDocument>()

        for(documentNumber in 0..(DEFAULT_NUMBER_OF_DOCUMENTS-1)){
            var typeAndSize = "(${documents[documentNumber].extension.toUpperCase()})"

            if (includeSize) {
                if (isLarge) {
                    typeAndSize = "(${documents[documentNumber].extension.toUpperCase()}, ${LARGE_DOCUMENT_SIZE}MB)"
                } else {
                    typeAndSize = "(${documents[documentNumber].extension.toUpperCase()}, " +
                            "${REGULAR_DOCUMENT_SIZE}MB)"
                }
            }

            val expectedDocument = ExpectedDocument(
                documents[documentNumber].documentGuid,
                typeAndSize,
                    (DATE_FOR_DOCUMENT_DAY+documentNumber).toString() + " February 2018"  ,
                mutableListOf("View"))

            if (includeName) {
                expectedDocument.name = "Name ${documentNumber + 1}"
            }
            if (includeTerm) {
                expectedDocument.term = "Letter ${documentNumber + 1}"
            }
            expectedDocuments.add(expectedDocument)
        }
        return expectedDocuments.sortedByDescending { it.date}
    }
}
