package mocking.emis.models

data class MedicalRecordSettings(
        var recordAccessScheme: String = "DetailedCodedCareRecord",
        var allergiesEnabled: Boolean = true,
        var consultationsEnabled: Boolean = true,
        var immunisationsEnabled: Boolean = true,
        var documentsEnabled: Boolean = true,
        var medicationEnabled: Boolean = true,
        var problemsEnabled: Boolean = true,
        var testResultsEnabled: Boolean = true
)
