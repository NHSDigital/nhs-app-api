package mocking.emis.testResults

data class TestResultResponseModel(
        var medicalRecord: TestResultMedicalRecord
)

data class TestResultMedicalRecord(
        var testResults: MutableList<TestResultResponse> = arrayListOf()
)

data class TestResultResponse(
        var value: TestResultValue,
        var childValues: MutableList<TestResultValue> = arrayListOf()
)

data class EffectiveDate(
        var datePart: String,
        var value: String
)

data class TestResultValue(
        var effectiveDate: EffectiveDate,
        var term: String,
        var textValue: String,
        var numericUnits: String,
        var range: TestResultRange?
)

data class TestResultRange(
        var minimumText: String,
        var maximumText: String
)
