package mocking.data.organDonation

import models.KeyValuePair
import mocking.organDonation.models.Address
import mocking.organDonation.models.CodeableConcept
import mocking.organDonation.models.Coding
import mocking.organDonation.models.Entry
import mocking.organDonation.models.Identifier
import mocking.organDonation.models.Name
import mocking.organDonation.models.Resource
import models.Patient

object OrganDonationRegistration {

    fun getOrganDonationRegistrationData(patient: Patient): List<Entry<Resource>> {

        val resource = Resource(
                id = "AD02745157",
                resourceType = "Registration",
                identifier = listOf(Identifier(
                        system = "https://fhir.nhs.uk/Id/nhs-number",
                        value = patient.nhsNumbers.first())),
                name = getName(patient),
                nameFull = patient.formattedFullName(),
                gender = patient.sex.toString(),
                birthdate = patient.dateOfBirth,
                ethnicCategory = getCodeableConcept(patient.organDonationDemographics.ethnicity),
                religiousAffiliation = getCodeableConcept(patient.organDonationDemographics.religion),
                address = getAddress(patient),
                addressFull = patient.address.full(),
                telecom = listOf(Identifier(
                        system = "phone",
                        value = patient.contactDetails.telephoneNumber!!)),
                organDonationDecision = patient.organDonationDecision,
                faithDeclaration = patient.organDonationDemographics.faithDeclaration.toString()
        )

        return listOf(Entry(resource))
    }

    private fun getName(patient: Patient): List<Name> {
        return listOf(Name(
                prefix = listOf(patient.title),
                given = listOf(patient.firstName),
                family = patient.surname)
        )
    }

    private fun getAddress(patient: Patient): List<Address> {
        return listOf(Address(
                line = listOf(
                        patient.address.houseNameFlatNumber!!,
                        patient.address.numberStreet!!),
                postalCode = patient.address.postcode!!))
    }

    private fun getCodeableConcept(pair: KeyValuePair<String, String>): CodeableConcept {
        return CodeableConcept(listOf(Coding(pair.key, pair.value)))
    }
}
