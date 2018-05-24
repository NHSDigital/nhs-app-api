package mocking.emis.models

data class PatientIdentifier(
        var identifierValue: String? = null,
        var identifierType: IdentifierType? = null
)