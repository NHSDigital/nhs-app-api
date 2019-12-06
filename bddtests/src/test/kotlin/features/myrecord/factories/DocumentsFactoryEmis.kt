package features.myrecord.factories

import features.myrecord.stepDefinitions.MyRecordDocumentsStepDefinitions.SerenityVariable
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

class DocumentsFactoryEmis: DocumentsFactory() {

    override fun disabled(patient: Patient) {
        mockingClient.forEmis {
            myRecord.documentsRequest(patient)
                .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithNoDocuments(patient: Patient) {
        mockingClient.forEmis {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                .respondWithSuccess(DocumentsData.getNoDocumentData())
        }
    }

    override fun enabledWithDocuments(patient: Patient, isLarge: Boolean, mockUnavailableDocument: Boolean) {
        val documents = DocumentsData.getDefaultDocumentsData()

        val expectedDocuments = getExpectedDocumentsFromEmisDocuments(isLarge, documents.medicalRecord.documents)
        setSerenityVariable(SerenityVariable.EXPECTED_DOCUMENTS, expectedDocuments)

        val availableDocument = expectedDocuments[0]
        setSerenityVariable(SerenityVariable.AVAILABLE_DOCUMENT, availableDocument)

        if (mockUnavailableDocument) {
            val unavailableDocument = expectedDocuments[DEFAULT_NUMBER_OF_DOCUMENTS - 1]
            setSerenityVariable(SerenityVariable.UNAVAILABLE_DOCUMENT, unavailableDocument)
        }

        mockingClient.forEmis {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                .respondWithSuccess(documents)
        }
        mockingClient.forEmis {
            myRecord.documentRequest(EmisMockDefaults.patientEmis, availableDocument.id)
                .respondWithSuccess(DocumentData.getDefaultDocumentData())
        }
    }

    override fun enabledWithDocumentsWithNoNameOrTerm(patient: Patient, isLarge: Boolean) {
        val documents = if (isLarge) DocumentsData.getLargeDocumentData()
            else DocumentsData.getDefaultDocumentsData(false, false)

        val expectedDocuments = getExpectedDocumentsFromEmisDocuments(isLarge, documents.medicalRecord.documents,
                false, false)
        setSerenityVariable(SerenityVariable.EXPECTED_DOCUMENTS, expectedDocuments)

        val availableDocument = expectedDocuments[0]
        setSerenityVariable(SerenityVariable.AVAILABLE_DOCUMENT, availableDocument)

        mockingClient.forEmis {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                    .respondWithSuccess(documents)
        }
        mockingClient.forEmis {
            myRecord.documentRequest(EmisMockDefaults.patientEmis, availableDocument.id)
                    .respondWithSuccess(DocumentData.getDefaultDocumentData())
        }
    }

    private fun setSerenityVariable(key: Any, value: Any) {
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(key, value)
    }

    private fun getExpectedDocumentsFromEmisDocuments(isLarge: Boolean,
        documents: List<DocumentsResponse>,
        includeName: Boolean = true, includeTerm: Boolean = true): List<ExpectedDocument> {
        val size = if (isLarge) LARGE_DOCUMENT_SIZE else REGULAR_DOCUMENT_SIZE

        return documents.mapIndexed(fun(index, document): ExpectedDocument {
            val expectedDocument = ExpectedDocument(
                document.documentGuid,
                "(${document.extension.toUpperCase()}, ${size}MB)",
                "18 February 2018",
                mutableListOf("View"))
            if (includeName) {
                expectedDocument.name = "Name ${index + 1}"
            }
            if (includeTerm) {
                expectedDocument.term = "Letter ${index + 1}"
            }
            return expectedDocument
        })
    }
}