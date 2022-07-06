package mocking.emis.immunisations

data class ImmunisationResponseModel(
        var medicalRecord: ImmunisationMedicalRecord
)

data class ImmunisationMedicalRecord(
        var immunisations: MutableList<ImmunisationResponse> = arrayListOf()
)

data class ImmunisationResponse(
        var term: String,
        var effectiveDate: EffectiveDate?,
        var associatedText: MutableList<AssociatedText> = arrayListOf()
)

data class EffectiveDate(
        var datePart: String,
        var value: String
)

data class AssociatedText(
        var text: String,
        var textType: String
)
