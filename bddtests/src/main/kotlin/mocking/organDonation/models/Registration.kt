package mocking.organDonation.models

import models.Patient

data class Resource(
        var id: String,
        var resourceType: String,
        var identifier: List<Identifier>,
        var name: List<Name>,
        var nameFull: String,
        var gender: String,
        var birthdate: String,
        var ethnicCategory: CodeableConcept,
        var religiousAffiliation: CodeableConcept,
        var address: List<Address>,
        var addressFull: String,
        var telecom: List<Identifier>,
        var organDonationDecision: String,
        var faithDeclaration: String,
        var donationWishes: HashMap<String,String>? = null
)

data class Identifier(
        var system: String,
        var value: String
)

data class Name(
        var prefix: List<String>,
        var given: List<String>,
        var family: String
) {
    companion object {
        fun fromPatient (patient: Patient) :Name{
           val prefix = listOf(patient.title)
           val given = listOf(patient.firstName)
           val  family = patient.surname
            return Name(prefix,given,family)
        }
    }
}

data class Address(
        var line: List<String>,
        var postalCode: String
){
    companion object {
        fun fromPatient (patient: Patient) :Address{
            val line = listOf(
                    patient.address.houseNameFlatNumber!!,
                    patient.address.numberStreet!!)
            val postalCode = patient.address.postcode!!
            return Address(line,postalCode)
        }
    }
}
