package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.organDonation.OrganDonationCheckDetailsPage
import utils.SerenityHelpers

open class OrganDonationCheckDetailsDefinitions {

    lateinit var organDonationCheckDetailsPage: OrganDonationCheckDetailsPage

    @When("I select the privacy statement link on the Organ Donation Check Details page")
    fun iSelectThePrivacyStatementLinkOnTheOrganDonationCheckDetailsPage() {
        organDonationCheckDetailsPage.privacyStatementLink.assertIsVisible().click()
    }

    @Then("^the Organ Donation Check Details page is displayed")
    fun theOrganDonationCheckDetailsPageIsDisplayed() {
        val patient = SerenityHelpers.getPatient()
        organDonationCheckDetailsPage.title.assertIsVisible()
        organDonationCheckDetailsPage.assertPersonalDetailsSection(patient)
        organDonationCheckDetailsPage.assertConfirmationCheckBoxes()
    }

    @Then("^the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page")
    fun choiceOfNotWishingToDonateOrgansIsDisplayedOnTheOrganDonationCheckDetailsPage() {
        organDonationCheckDetailsPage.yourDecisionModule.assertDecisionIsNo()
    }

    @Then("^my ethnicity is recorded on the Organ Donation Check Details page")
    fun myEthnicityIsRecordedOnTheOrganDonationCheckDetailsPage() {
        val patient = SerenityHelpers.getPatient()
        organDonationCheckDetailsPage.assertEthnicity(patient.organDonationDemographics.ethnicity.value)
    }

    @Then("^my ethnicity is recorded as not chosen on the Organ Donation Check Details page")
    fun myEthnicityIsRecordedAsNotChosenOnTheOrganDonationCheckDetailsPage() {
        organDonationCheckDetailsPage.assertEthnicity("")
    }

    @Then("^my religion is recorded on the Organ Donation Check Details page")
    fun myReligionIsRecordedOnTheOrganDonationCheckDetailsPage() {
        val patient = SerenityHelpers.getPatient()
        organDonationCheckDetailsPage.assertReligion(patient.organDonationDemographics.religion.value)
    }

    @Then("^my religion is recorded as not chosen on the Organ Donation Check Details page")
    fun myReligionIsRecordedAsNotChosenOnTheOrganDonationCheckDetailsPage() {
        organDonationCheckDetailsPage.assertReligion("")
    }
}
