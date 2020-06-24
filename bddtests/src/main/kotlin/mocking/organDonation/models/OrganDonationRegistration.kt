package mocking.organDonation.models

import mocking.data.organDonation.OrganDecisions
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
                    nameFull = patient.formattedFullName(false),
                    name = Name.fromPatient(patient),
                    dateOfBirth = patient.age.dateOfBirth,
                    addressFull = patient.contactDetails.address.full(),
                    address = Address.fromPatient(patient),
                    emailAddresss = patient.contactDetails.emailAddress,
                    decision = OrganDonationRegistrationDecision.OptOut,
                    decisionDetails = null,
                    faithDeclaration = demographics.faithDeclaration.toString()
            )
        }

        fun optIn(patient: Patient,
                  organDonationDemographics: OrganDonationDemographics): OrganDonationRegistration {
            val registration = optOut(patient, organDonationDemographics)
            registration.decision = OrganDonationRegistrationDecision.OptIn
            registration.decisionDetails = DecisionDetails(true,
                    hashMapOf("heart" to "NotStated",
                            "lungs" to "NotStated",
                            "fingers" to "NotStated",
                            "toes" to "NotStated"))
            registration.faithDeclaration  = organDonationDemographics.faithDeclaration.toString()
            return registration
        }

        fun some(patient: Patient, organsToDonate: OrganDecisions,
                 organDonationDemographics: OrganDonationDemographics): OrganDonationRegistration {
            val registration = optIn(patient, organDonationDemographics)
            registration.decisionDetails = DecisionDetails(false, createDecisionMap(organsToDonate))
            return registration
        }

        private fun createDecisionMap(organsToDonate: OrganDecisions): HashMap<String, String> {
            val decision = hashMapOf<String, String>()
            organsToDonate.optIn.forEach { organ -> decision[organ] = "yes" }
            organsToDonate.optOut.forEach { organ -> decision[organ] = "no" }
            return decision
        }
    }
}
