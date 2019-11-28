package mocking.emis.documents

data class DocumentsResponseModel(
        var medicalRecord: DocumentsMedicalRecord
)

data class DocumentsMedicalRecord(
        var documents: MutableList<DocumentsResponse> = arrayListOf()
)

data class DocumentsResponse(
        var documentGuid: String,
        var size: Long?,
        var extension: String,
        var available: Boolean,
        var observation: Observation
)

data class EffectiveDate(
        var datePart: String,
        var value: String
)

data class Observation(
        var term: String? = null,
        var associatedText: MutableList<AssociatedText> = arrayListOf(),
        var effectiveDate: EffectiveDate?
)

data class AssociatedText(
        var text: String
)