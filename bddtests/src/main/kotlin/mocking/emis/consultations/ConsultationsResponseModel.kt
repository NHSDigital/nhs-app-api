package mocking.emis.consultations

data class ConsultationsResponseModel(
        var medicalRecord: ConsultationMedicalRecord
)

data class ConsultationMedicalRecord(
        var consultations: MutableList<ConsultationResponse> = arrayListOf()
)

data class ConsultationResponse(
        var location: String,
        var consultantName: String,
        var sections: MutableList<Section> = arrayListOf(),
        var effectiveDate: EffectiveDate
)

data class Section(
        var header: String,
        var observations: MutableList<Observation> = arrayListOf()
)

data class EffectiveDate(
        var datePart: String,
        var value: String
)

data class Observation(
        var term: String,
        var associationText: MutableList<AssociatedText> = arrayListOf()
)

data class AssociatedText(
        var text: String
)
