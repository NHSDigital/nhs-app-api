package mocking.data.organDonation

import mocking.organDonation.models.OrganDonationRegistrationRequest
import mocking.organDonation.models.KeyValuePair
import mocking.organDonation.models.OrganDonationDemographics
import net.serenitybdd.core.Serenity
import org.junit.Assert

class OrganDonationSerenityHelpers {

    companion object {

        private const val ORGAN_DONATION_DECISION = "ORGAN_DONATION_DECISION"
        private const val ORGAN_DONATION_DECISION_SOME_ORGANS_EXISTING = "ORGAN_DONATION_DECISION_SOME_ORGANS_EXISTING"
        private const val ORGAN_DONATION_DECISION_SOME_ORGANS_UPDATED = "ORGAN_DONATION_DECISION_SOME_ORGANS_UPDATED"
        private const val ORGAN_DONATION_DECISION_OPT_IN = "ORGAN_DONATION_DECISION_OPT_IN"
        private const val ORGAN_DONATION_EXPECTED_REGISTRATION_ID = "ORGAN_DONATION_EXPECTED_REGISTRATION_ID"
        private const val ORGAN_DONATION_DEMOGRAPHICS = "ORGAN_DONATION_DEMOGRAPHICS"
        const val ORGAN_DONATION_REFERENCE_ETHNICITIES = "ORGAN_DONATION_REFERENCE_ETHNICITIES"
        const val ORGAN_DONATION_REFERENCE_RELIGIONS = "ORGAN_DONATION_REFERENCE_RELIGIONS"

        fun setOrganDonationDecision(decision: OrganDonationRegistrationRequest) {
            Serenity.setSessionVariable(ORGAN_DONATION_DECISION).to(decision)
        }

        fun getOrganDonationDecision(): OrganDonationRegistrationRequest {
            return getValueOrFail(ORGAN_DONATION_DECISION)
        }

        fun setOrganDonationDecisionSomeOrgansExisting(decision: ArrayList<KeyValuePair<String, Boolean>>) {
            Serenity.setSessionVariable(ORGAN_DONATION_DECISION_SOME_ORGANS_EXISTING).to(decision)
        }

        fun getOrganDonationDecisionSomeOrgansExisting(): ArrayList<KeyValuePair<String, Boolean>> {
            return getValueOrFail(ORGAN_DONATION_DECISION_SOME_ORGANS_EXISTING)
        }

        fun setOrganDonationDecisionSomeOrgansUpdated(decision: ArrayList<KeyValuePair<String, Boolean>>) {
            Serenity.setSessionVariable(ORGAN_DONATION_DECISION_SOME_ORGANS_UPDATED).to(decision)
        }

        fun getOrganDonationDecisionSomeOrgansUpdated(): ArrayList<KeyValuePair<String, Boolean>> {
            return getValueOrFail(ORGAN_DONATION_DECISION_SOME_ORGANS_UPDATED)
        }

        fun setIfOrganDonationDecisionIsOptIn(decision: Boolean) {
            Serenity.setSessionVariable(ORGAN_DONATION_DECISION_OPT_IN).to(decision)
        }

        fun getIfOrganDonationDecisionIsOptIn(): Boolean {
            return getValueOrFail(ORGAN_DONATION_DECISION_OPT_IN)
        }

        fun setRegistrationId(registrationId: String) {
            Serenity.setSessionVariable(ORGAN_DONATION_EXPECTED_REGISTRATION_ID).to(registrationId)
        }

        fun getRegistrationID(): String {
            return getValueOrFail(ORGAN_DONATION_EXPECTED_REGISTRATION_ID)
        }

        fun setOrganDonationDemographics(organDonationDemographics: OrganDonationDemographics) {
            Serenity.setSessionVariable(ORGAN_DONATION_DEMOGRAPHICS).to(organDonationDemographics)
        }

        fun getOrganDonationDemographics(): OrganDonationDemographics {
            return getValueOrFail(ORGAN_DONATION_DEMOGRAPHICS)
        }

        private fun <T>getValueOrFail(key:String): T {
            Assert.assertTrue("Test setup incorrect, $key to be set",
                    Serenity.hasASessionVariableCalled(key))
            return Serenity.sessionVariableCalled<T>(key)
        }
    }
}
