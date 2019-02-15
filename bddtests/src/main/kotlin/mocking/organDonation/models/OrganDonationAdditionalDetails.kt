package mocking.organDonation.models

data class OrganDonationAdditionalDetails(val ethnicityId:String? = null , val religionId:String? = null) {

    companion object {
        fun getAdditionalDetails(organDonationDemographics: OrganDonationDemographics? = null):
                OrganDonationAdditionalDetails {
            val demographics = organDonationDemographics ?: OrganDonationDemographics()
            return OrganDonationAdditionalDetails(
                    demographics.ethnicity.key,
                    demographics.religion.key
            )
        }
    }
}
