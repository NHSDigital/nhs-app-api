package mocking.data.organDonation

import mocking.organDonation.models.Address
import mocking.organDonation.models.CodeableConcept
import mocking.organDonation.models.Coding
import mocking.organDonation.models.Identifier
import mocking.organDonation.models.Name
import mocking.organDonation.models.Resource
import models.KeyValuePair
import models.Patient

object OrganDonationRegistrationDataBuilder {

    fun optIn(patient: Patient): Resource {
        val resource = build(patient)
        resource.organDonationDecision = "opt-in"
        resource.donationWishes = hashMapOf(
                "all" to "yes",
                "heart" to "not-stated",
                "lungs" to "not-stated",
                "kidney" to "not-stated",
                "liver" to "not-stated",
                "corneas" to "not-stated",
                "pancreas" to "not-stated",
                "tissue" to "not-stated",
                "smallBowel" to "not-stated")
        return resource
    }

    fun optOut(patient: Patient): Resource {
        val resource = build(patient)
        resource.organDonationDecision = "opt-out"
        return resource
    }

    fun optInSome(patient: Patient, someOrgans: HashMap<String, String>): Resource {
        val resource = build(patient)
        resource.organDonationDecision = "opt-in"
        val donationWishes = hashMapOf<String, String>()
        someOrgans.forEach{organChoice -> donationWishes.put(map(organChoice.key), organChoice.value)}
        donationWishes["all"] = "no"
        resource.donationWishes = donationWishes
        return resource
    }

    fun appointRepresentative(patient: Patient): Resource {
        val resource = build(patient)
        resource.organDonationDecision = "app-rep"
        return resource
    }

    private val mapUiLabelToMock = hashMapOf("Small bowel" to "smallBowel")

    private fun map(input:String):String{
        if(mapUiLabelToMock.containsKey(input))
        {
            return mapUiLabelToMock[input]!!
        }
        return input
    }

    private fun build(patient:Patient): Resource{
        return Resource(
                id = "AD02745157",
                resourceType = "Registration",
                identifier = listOf(Identifier(
                        system = "https://fhir.nhs.uk/Id/nhs-number",
                        value = patient.nhsNumbers.first())),
                name = listOf(getName(patient)),
                nameFull = patient.formattedFullName(),
                gender = patient.sex.toString(),
                birthdate = patient.dateOfBirth,
                ethnicCategory = getCodeableConcept(patient.organDonationDemographics.ethnicity),
                religiousAffiliation = getCodeableConcept(patient.organDonationDemographics.religion),
                address = listOf(getAddress(patient)),
                addressFull = patient.address.full(),
                telecom = listOf(Identifier(
                        system = "phone",
                        value = patient.contactDetails.telephoneNumber!!)),
                organDonationDecision = patient.organDonationDecision,
                faithDeclaration = patient.organDonationDemographics.faithDeclaration.toString()
        )
    }

    fun getName(patient: Patient): Name {
        return Name(
                prefix = listOf(patient.title),
                given = listOf(patient.firstName),
                family = patient.surname)

    }

    fun getAddress(patient: Patient): Address {
        return Address(
                line = listOf(
                        patient.address.houseNameFlatNumber!!,
                        patient.address.numberStreet!!),
                postalCode = patient.address.postcode!!)
    }

    private fun getCodeableConcept(pair: KeyValuePair<String, String>): CodeableConcept {
        return CodeableConcept(listOf(Coding(pair.key, pair.value)))
    }
}
