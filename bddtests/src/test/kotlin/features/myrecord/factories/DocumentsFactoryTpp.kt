package features.myrecord.factories

import features.myrecord.stepDefinitions.V2MedicalRecordDocumentsStepDefinitions
import mocking.data.myrecord.TppDcrDocumentData
import mocking.data.myrecord.TppDocumentData
import mocking.tpp.models.RequestPatientRecordReply
import models.ExpectedDocument
import models.Patient
import utils.SerenityHelpers

private const val DATE_FOR_DOCUMENT_DAY = 18

class DocumentsFactoryTpp: DocumentsFactory() {

    override fun disabled(patient: Patient) {
        TODO("not implemented")
    }

    override fun enabledWithNoDocuments(patient: Patient) {
        mockingClient.forTpp {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppDcrDocumentData.getMultipleDcrEventsForTppWithNoDocuments())
        }
    }

    override fun enabledWithNullPageCount() {
        TODO("not implemented")
    }

    override fun enabledWithNullSize() {
        TODO("not implemented")
    }

    override fun enabledWithDocuments(patient: Patient, isLarge: Boolean, mockUnavailableDocument: Boolean,
                                      hasInvalidType: Boolean) {
        setExpectedAndAvailableDocs(
                patient,
                TppDcrDocumentData.getMultipleDcrEventsForTppDcrDocuments(hasInvalidType))

        mockingClient.forTpp {
            myRecord.documentRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppDocumentData.getDefaultDocumentData())
        }
    }

    override fun enabledWithDocumentsWithNoNameOrTerm(patient: Patient, isLarge: Boolean) {
        setExpectedAndAvailableDocs(
                patient,
                TppDcrDocumentData.getMultipleDcrEventsForTppDcrDocuments())
    }

    override fun enabledWithLettersWithNoNameOrTerm(patient: Patient, isLarge: Boolean) {
        setExpectedAndAvailableDocs(
                patient,
                TppDcrDocumentData.getMultipleLetterDcrEventsForTppDcrDocuments())
    }

    override fun enabledWithDocumentsWithUnknownDate(patient: Patient, isLarge: Boolean) {
        TODO("not implemented")
    }

    private fun setSerenityVariable(key: Any, value: Any) {
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(key, value)
    }

    private fun getExpectedDocumentsFromEmisDocuments(documents: RequestPatientRecordReply)
            : List<ExpectedDocument> {
        val expectedDocuments = mutableListOf<ExpectedDocument>()
        for (document in documents.event) {
            for (item in document.Item) {
                val expectedDocument = ExpectedDocument(
                        item.binaryDataId,
                        null,
                        (DATE_FOR_DOCUMENT_DAY).toString() + " February 2018",
                        mutableListOf("View"))
                if (item.type === "Attachment") {
                    expectedDocument.term = "Document"
                } else {
                    expectedDocument.term = "Letter"
                }
                expectedDocuments.add(expectedDocument)
            }
        }
        return expectedDocuments

    }

    private fun setExpectedAndAvailableDocs(patient: Patient, docs: RequestPatientRecordReply) {

        val expectedDocuments = getExpectedDocumentsFromEmisDocuments(docs)
        setSerenityVariable(
                V2MedicalRecordDocumentsStepDefinitions.SerenityVariable.EXPECTED_DOCUMENTS,
                expectedDocuments)
        mockingClient.forTpp {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithSuccess(docs)
        }

        val availableDocument = expectedDocuments[0]
        setSerenityVariable(
                V2MedicalRecordDocumentsStepDefinitions.SerenityVariable.AVAILABLE_DOCUMENT,
                availableDocument)
    }
}
