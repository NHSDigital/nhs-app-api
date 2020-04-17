package features.myrecord.factories

import constants.ErrorResponseCodeTpp
import features.myrecord.stepDefinitions.V2MedicalRecordDocumentsStepDefinitions
import mocking.data.myrecord.TppDcrDocumentData
import mocking.data.myrecord.TppDocumentData
import mocking.tpp.models.Error
import mocking.tpp.models.RequestPatientRecordReply
import models.ExpectedDocument
import models.Patient
import utils.SerenityHelpers

private const val DATE_FOR_DOCUMENT_DAY = 18

class DocumentsFactoryTpp: DocumentsFactory() {

    override fun disabled(patient: Patient) {
        val errorMsg = "You don't have access to this online service"
        val disabledTppError = Error(errorCode = ErrorResponseCodeTpp.NO_ACCESS,
                                     userFriendlyMessage = errorMsg)
        mockingClient.forTpp {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithError(disabledTppError)
        }
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

    override fun enabledWithDocuments(patient: Patient,
        isLarge: Boolean,
        mockUnavailableDocument: Boolean,
        hasInvalidType: Boolean,
        stillUploading: Boolean,
        hasNonViewableType: Boolean) {
        setExpectedAndAvailableDocs(
                patient,
                TppDcrDocumentData.getMultipleDcrEventsForTppDcrDocuments(hasInvalidType, hasNonViewableType))

        setExpectedBinaryResponse(patient, isLarge, hasInvalidType, stillUploading, hasNonViewableType)
    }

    override fun enabledWithDocumentsWithNoNameOrTerm(patient: Patient, isLarge: Boolean) {
        setExpectedAndAvailableDocs(
                patient,
                TppDcrDocumentData.getMultipleDcrEventsForTppDcrDocuments())

        setExpectedBinaryResponse(patient, isLarge)
    }

    override fun enabledWithLettersWithNoNameOrTerm(patient: Patient, isLarge: Boolean) {
        setExpectedAndAvailableDocs(
                patient,
                TppDcrDocumentData.getMultipleLetterDcrEventsForTppDcrDocuments())

        setExpectedBinaryResponse(patient, isLarge)
    }

    override fun enabledWithDocumentsWithUnknownDate(patient: Patient, isLarge: Boolean) {
        TODO("not implemented")
    }

    private fun setSerenityVariable(key: Any, value: Any) {
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(key, value)
    }

    private fun getExpectedDocumentsFromTppDocuments(documents: RequestPatientRecordReply)
        : List<ExpectedDocument> {
        return documents.event
            .flatMap { it.Item }
            .map {
                ExpectedDocument(
                    it.binaryDataId,
                    null,
                    "${(DATE_FOR_DOCUMENT_DAY)} February 2018",
                    mutableListOf("View"),
                    if (it.type === "Attachment")  "Document" else "Letter"
                )
            }.toMutableList()
    }

    private fun setExpectedAndAvailableDocs(patient: Patient, docs: RequestPatientRecordReply) {
        val expectedDocuments = getExpectedDocumentsFromTppDocuments(docs)

        setSerenityVariable(
                V2MedicalRecordDocumentsStepDefinitions.SerenityVariable.EXPECTED_DOCUMENTS,
                expectedDocuments)

        mockingClient.forTpp {
            myRecord.patientRecordRequest(patient.tppUserSession!!)
                    .respondWithSuccess(docs)
        }

        setSerenityVariable(
                V2MedicalRecordDocumentsStepDefinitions.SerenityVariable.AVAILABLE_DOCUMENT,
                expectedDocuments.first())
    }

    private fun setExpectedBinaryResponse(patient: Patient,
        isLarge: Boolean,
        hasInvalidType: Boolean = false,
        stillUploading: Boolean = false,
        hasNonViewableType: Boolean = false) {
        if (isLarge) {
            mockingClient.forTpp {
                myRecord.documentRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.FILE_SIZE_TOO_LARGE,
                        "File exceeds 2MB"))
            }
            return
        }

        if (stillUploading) {
            mockingClient.forTpp {
                myRecord.documentRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.FILE_STILL_UPLOADING,
                        "File has not finished uploading"))
            }
            return
        }

        val documentData = when {
            hasInvalidType -> TppDocumentData.getDocumentData("tga")
            hasNonViewableType -> TppDocumentData.getDocumentData("pdf")
            else -> TppDocumentData.getDocumentData()
        }

        mockingClient.forTpp {
            myRecord.documentRequest(patient.tppUserSession!!)
                .respondWithSuccess(documentData)
        }
    }
}
