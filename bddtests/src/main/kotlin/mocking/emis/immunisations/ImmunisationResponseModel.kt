package mocking.emis.immunisations

data class ImmunisationResponseModel(
        var medicalRecord: ImmunisationMedicalRecord
)

data class ImmunisationMedicalRecord(
        var immunisations: MutableList<ImmunisationResponse> = arrayListOf()
)

data class ImmunisationResponse(
        var term: String,
        var effectiveDate: EffectiveDate?
)

data class EffectiveDate(
        var datePart: String,
        var value: String
)