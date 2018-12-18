package mocking.organDonation.models

data class OrganDonationErrorResponse(
        var code: String,
        var details: CodeableConcept,
        var diagnostics: String
)
