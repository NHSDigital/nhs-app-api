package mocking.organDonation.models

import models.Patient

data class OrganDonationAdditionalDetails(val ethnicityId:String? = null , val religionId:String? = null) {

    companion object {
        fun fromPatient(patient: Patient): OrganDonationAdditionalDetails {
            return OrganDonationAdditionalDetails(
                    patient.organDonationDemographics.ethnicity.key,
                    patient.organDonationDemographics.religion.key
            )
        }
    }
}