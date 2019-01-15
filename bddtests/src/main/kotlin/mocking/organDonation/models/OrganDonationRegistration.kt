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
        var decisionDetails: String?,
        var faithDeclaration: String
) {
    companion object {

        fun fromPatient(patient: Patient): OrganDonationRegistration {
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
    }
}