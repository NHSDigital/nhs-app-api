package mocking.organDonation.models

data class Registration(
        var id: String,
        var resourceType: String,
        var identifier: List<Identifier>,
        var name: List<Name>,
        var gender: String,
        var birthdate: String,
        var ethnicCategory: CodeableConcept,
        var religiousAffliation: CodeableConcept,
        var address: List<Address>,
        var telecom: List<Identifier>,
        var organDonationDecision: String,
        var faithDeclaration: String
)

data class Identifier(
        var system: String,
        var value: String
)

data class Name(
        var prefix: List<String>,
        var given: List<String>,
        var family: String
)

data class Address(
        var line: List<String>,
        var postalCode: String
)
