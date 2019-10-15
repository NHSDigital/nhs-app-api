package features.myrecord.factories

import features.myrecord.stepDefinitions.MyRecordDocumentsStepDefinitions
import mocking.data.myrecord.DocumentData
import mocking.data.myrecord.DocumentsData
import mocking.defaults.EmisMockDefaults
import models.Patient
import utils.SerenityHelpers

class DocumentsFactoryEmis: DocumentsFactory() {
    override fun disabled(patient: Patient) {
        mockingClient.forEmis {
            myRecord.documentsRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun enabledWithNoDocuments(patient: Patient) {
        mockingClient.forEmis {
            myRecord.documentRequest(EmisMockDefaults.patientEmis)
                    .respondWithSuccess(DocumentData.getDefaultDocumentData())
        }
    }

    override fun enabledWithDocuments(patient: Patient) {
        val documents = DocumentsData.getMultipleDocuments()
        val documentsSize = documents.medicalRecord.documents.size
        val availableDocumentId = "document-1"

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                MyRecordDocumentsStepDefinitions.SerenityVariable.NUMBER_OF_DOCUMENTS,
                documentsSize)
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                MyRecordDocumentsStepDefinitions.SerenityVariable.AVAILABLE_DOCUMENT_ID,
                availableDocumentId)
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                MyRecordDocumentsStepDefinitions.SerenityVariable.INVALID_DOCUMENT_ID,
                "document-$documentsSize")

        mockingClient.forEmis {
            myRecord.documentsRequest(EmisMockDefaults.patientEmis)
                    .respondWithSuccess(documents)
        }
        mockingClient.forEmis {
            myRecord.documentRequest(EmisMockDefaults.patientEmis, availableDocumentId)
                    .respondWithSuccess(DocumentData.getDefaultDocumentData())
        }
    }

    override fun withOneInvalidDocument(patient: Patient) {
        mockingClient.forEmis {
            myRecord.documentRequest(
                patient,
                SerenityHelpers.getValueOrNull(MyRecordDocumentsStepDefinitions.SerenityVariable.INVALID_DOCUMENT_ID)!!
            ).respondWithBadRequest("The request is invalid", "Document Guid")
        }
    }
}