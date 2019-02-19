package mocking.organDonation.models

import models.Patient

data class OrganDonationRegistration(
        var identifier: String,
        var nhsNumber: String,
        var nameFull: String,
        var name: Name,
        var dateOfBirth: String,
        var addressFull: String,
        var address: Address,
        var emailAddresss: String,
        var decision: OrganDonationRegistrationDecision,
        var decisionDetails: DecisionDetails?,
        var faithDeclaration: String
) {
    companion object {

        fun optOut(patient: Patient,
                   organDonationDemographics: OrganDonationDemographics? = null): OrganDonationRegistration {
            val demographics = organDonationDemographics ?: OrganDonationDemographics()

            return OrganDonationRegistration(
                    identifier = patient.organDonationRegistrationId,
                    nhsNumber = patient.formattedNHSNumber(),
                    nameFull = patient.formattedFullName(),
                    name = mocking.data.organDonation.OrganDonationRegistrationDataBuilder.getName(patient),
                    dateOfBirth = patient.dateOfBirth,
                    addressFull = patient.address.full(),
                    address = mocking.data.organDonation.OrganDonationRegistrationDataBuilder.getAddress(patient),
                    emailAddresss = patient.contactDetails.emailAddress ?: "",
                    decision = OrganDonationRegistrationDecision.OptOut,
                    decisionDetails = null,
                    faithDeclaration = demographics.faithDeclaration.toString()
            )
        }

        fun optIn(patient: Patient,
                  organDonationDemographics: OrganDonationDemographics? = null): OrganDonationRegistration {
            val registration = optOut(patient, organDonationDemographics)
            registration.decision = OrganDonationRegistrationDecision.OptIn
            registration.decisionDetails = DecisionDetails(true,
                    hashMapOf("heart" to "NotStated",
                            "lungs" to "NotStated",
                            "fingers" to "NotStated",
                            "toes" to "NotStated"))
            return registration
        }

        fun some(patient: Patient, organsToDonate: HashMap<String, String>,
                 organDonationDemographics: OrganDonationDemographics? = null): OrganDonationRegistration {
            val registration = optIn(patient, organDonationDemographics)
            registration.decisionDetails = DecisionDetails(false, organsToDonate)
            return registration
        }
    }
}
