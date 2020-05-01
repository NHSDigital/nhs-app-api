package mocking.organDonation.models

import mocking.data.organDonation.OrganDonationSerenityHelpers
import models.Patient
import utils.set

data class OrganDonationWithdrawRequest(
        var identifier: String,
        var nhsNumber: String,
        var name: Name,
        var gender: String,
        var dateOfBirth: String,
        var addressFull: String,
        var address: Address,
        var WithdrawReasonId: String
) {
    companion object {
        fun withdrawDecision(patient: Patient): OrganDonationWithdrawRequest{
            val withdrawalReasons: KeyValuePair<String, String> = KeyValuePair("3", "Religious Grounds")
            OrganDonationSerenityHelpers.ORGAN_DONATION_WITHDRAWAL_REASON.set(withdrawalReasons.value)

            return OrganDonationWithdrawRequest(
                    identifier = patient.organDonationRegistrationId,
                    nhsNumber = patient.formattedNHSNumber(),
                    name = Name.fromPatient(patient),
                    gender = patient.sex.toString(),
                    dateOfBirth = patient.age.dateOfBirth,
                    addressFull = patient.contactDetails.address.full(),
                    address = Address.fromPatient(patient),
                    WithdrawReasonId = withdrawalReasons.key
            )
        }
    }
}