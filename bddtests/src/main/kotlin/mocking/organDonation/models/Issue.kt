package mocking.organDonation.models

data class Issue(
        var code: String = "",
        var details: CodeableConcept,
        var diagnostics: String = ""
)
