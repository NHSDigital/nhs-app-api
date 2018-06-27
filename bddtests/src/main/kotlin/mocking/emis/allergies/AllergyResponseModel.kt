package mocking.emis.allergies

data class AllergyResponseModel(
        var medicalRecord: AllergyMedicalRecord
)

data class AllergyMedicalRecord(
        var allergies: MutableList<AllergyResponse> = arrayListOf()
)

data class AllergyResponse(
        var term: String,
        var effectiveDate: EffectiveDate
)

data class EffectiveDate(
        var datePart: String,
        var value: String
)