package mocking.organDonation.models

data class CodeableConcept(
        var coding: List<Coding>
)

data class Coding(
        var code: String,
        var display: String
)
