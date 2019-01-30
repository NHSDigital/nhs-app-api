package mocking.organDonation.models

import models.KeyValuePair
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

        fun optOut(patient: Patient): OrganDonationRegistration {
            return OrganDonationRegistration(
                    identifier = "123",
                    nhsNumber = patient.formattedNHSNumber(),
                    nameFull = patient.formattedFullName(),
                    name = mocking.data.organDonation.OrganDonationRegistrationDataBuilder.getName(patient),
                    dateOfBirth = patient.dateOfBirth,
                    addressFull = patient.address.full(),
                    address = mocking.data.organDonation.OrganDonationRegistrationDataBuilder.getAddress(patient),
                    emailAddresss = patient.contactDetails.emailAddress ?: "",
                    decision = OrganDonationRegistrationDecision.OptOut,
                    decisionDetails = null,
                    faithDeclaration = patient.organDonationDemographics.faithDeclaration.toString()
            )
        }

        fun optIn(patient: Patient): OrganDonationRegistration {
            val registration = optOut(patient)
            registration.decision = OrganDonationRegistrationDecision.OptIn
            registration.decisionDetails = DecisionDetails(true,
                    hashMapOf("heart" to "NotStated",
                            "lungs" to "NotStated",
                            "fingers" to "NotStated",
                            "toes" to "NotStated"))
            return registration
        }

        fun some(patient: Patient, organsToDonate: ArrayList<KeyValuePair<String, Boolean>>)
                : OrganDonationRegistration {
            val registration = optIn(patient)
            val decision = hashMapOf<String,String>()
            organsToDonate.forEach { organ-> decision.put(organ.key, if(organ.value) "yes" else "no") }
            registration.decisionDetails = DecisionDetails(false, decision)
            return registration
        }
    }
}