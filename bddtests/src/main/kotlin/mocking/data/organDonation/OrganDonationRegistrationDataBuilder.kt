package mocking.data.organDonation

import mocking.organDonation.models.Address
import mocking.organDonation.models.CodeableConcept
import mocking.organDonation.models.Coding
import mocking.organDonation.models.FaithDeclaration
import mocking.organDonation.models.Identifier
import mocking.organDonation.models.Name
import mocking.organDonation.models.Resource
import mocking.organDonation.models.KeyValuePair
import mocking.organDonation.models.OrganDonationDemographics
import models.Patient

object OrganDonationRegistrationDataBuilder {

    fun optIn(patient: Patient, organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val resource = build(patient, organDonationDemographics)
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

    fun optOut(patient: Patient, organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val resource = build(patient, organDonationDemographics)
        resource.organDonationDecision = "opt-out"
        return resource
    }

    fun optInSome(patient: Patient, organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val resource = build(patient, organDonationDemographics)
        resource.organDonationDecision = "opt-in"
        val organsToDonate = OrganDecisions(
                optIn = arrayListOf("Heart", "Kidney", "Liver", "Pancreas", "Small bowel"),
                optOut = arrayListOf("Lungs", "Corneas", "Tissue"))
        OrganDonationSerenityHelpers.SOME_ORGANS_EXISTING.set(organsToDonate)
        resource.donationWishes = createDonationWishes(organsToDonate)
        return resource
    }

    fun optInSomeNotAllDecided(patient: Patient, organDonationDemographics: OrganDonationDemographics? = null):
            Resource {
        val resource = build(patient, organDonationDemographics)
        resource.organDonationDecision = "opt-in"
        val organsToDonate = OrganDecisions(
                optIn = arrayListOf("Heart", "Kidney", "Liver", "Lungs", "Pancreas"),
                optOut = arrayListOf("Corneas"),
                notStated = arrayListOf("Tissue", "Small bowel"))
        OrganDonationSerenityHelpers.SOME_ORGANS_EXISTING.set(organsToDonate)
        resource.donationWishes = createDonationWishes(organsToDonate)
        return resource
    }

    private fun createDonationWishes(organList:OrganDecisions): HashMap<String, String> {
        val donationWishes = hashMapOf<String, String>()
        organList.optIn.forEach { organChoice -> donationWishes.put(map(organChoice), "yes") }
        organList.optOut.forEach { organChoice -> donationWishes.put(map(organChoice), "no") }
        organList.notStated.forEach { organChoice -> donationWishes.put(map(organChoice), "not-stated") }
        donationWishes["all"] = "no"
        return donationWishes
    }

    fun appointRepresentative(patient: Patient): Resource {
        val resource = build(patient)
        resource.organDonationDecision = "app-rep"
        return resource
    }

    fun someOrgansListUpdated(): OrganDecisions{
        val organsToDonate = OrganDecisions(
                optIn = arrayListOf("Heart", "Kidney", "Liver","Pancreas"),
                optOut = arrayListOf("Lungs", "Corneas", "Tissue", "Small bowel"))
        OrganDonationSerenityHelpers.SOME_ORGANS_UPDATED.set(organsToDonate)
        return organsToDonate
    }

    private val mapUiLabelToMock = hashMapOf("Small bowel" to "smallBowel")

    private fun map(input: String): String {
        if (mapUiLabelToMock.containsKey(input)) {
            return mapUiLabelToMock[input]!!
        }
        return input
    }

    private fun build(patient: Patient, organDonationDemographics: OrganDonationDemographics? = null): Resource {
        val demographics = organDonationDemographics ?: OrganDonationDemographics()
        OrganDonationSerenityHelpers.DEMOGRAPHICS_EXISTING.set(demographics)

        return Resource(
                id = patient.organDonationRegistrationId,
                resourceType = "Registration",
                identifier = listOf(Identifier(
                        system = "https://fhir.nhs.uk/Id/nhs-number",
                        value = patient.nhsNumbers.first())),
                name = listOf(Name.fromPatient(patient)),
                nameFull = patient.formattedFullName(),
                gender = patient.sex.toString(),
                birthdate = patient.dateOfBirth,
                ethnicCategory = getCodeableConcept(demographics.ethnicity),
                religiousAffiliation = getCodeableConcept(demographics.religion),
                address = listOf(Address.fromPatient(patient)),
                addressFull = patient.address.full(),
                telecom = listOf(Identifier(
                        system = "phone",
                        value = patient.contactDetails.telephoneNumber!!)),
                organDonationDecision = "opt-out",
                faithDeclaration = getFaithDeclaration(demographics.faithDeclaration))
    }

    private fun getFaithDeclaration(faithDeclaration: FaithDeclaration): String {
        return when (faithDeclaration) {
            FaithDeclaration.No -> "no"
            FaithDeclaration.Yes -> "yes"
            FaithDeclaration.NotStated -> "not-stated"
            FaithDeclaration.None -> ""
        }
    }

    private fun getCodeableConcept(pair: KeyValuePair<String, String>): CodeableConcept {
        return CodeableConcept(listOf(Coding(pair.key, pair.value)))
    }
}

data class OrganDecisions(var optIn:ArrayList<String>,
                          var optOut: ArrayList<String>,
                          var notStated: ArrayList<String> = arrayListOf())
