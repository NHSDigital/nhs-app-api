package mocking.emis.demographics

import mocking.emis.models.IdentifierType

data class PatientIdentifier(
        var identifierValue: String? = null,
        var identifierType: IdentifierType? = null
)
