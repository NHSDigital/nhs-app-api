package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import models.KeyValuePair
import net.serenitybdd.core.Serenity
import pages.assertIsVisible
import pages.organDonation.OrganDonationCheckDetailsPage
import utils.SerenityHelpers

open class OrganDonationCheckDetailsDefinitions {

    lateinit var organDonationCheckDetailsPage: OrganDonationCheckDetailsPage

    @When("I select the privacy statement link on the Organ Donation Check Details page")
    fun iSelectThePrivacyStatementLinkOnTheOrganDonationCheckDetailsPage() {
        organDonationCheckDetailsPage.privacyStatementLink.assertIsVisible().click()
    }

    @When("^I confirm that my details are accurate, and accept the privacy statement for organ donation")
    fun iConfirmThatMyDetailsAreAccurateAndAcceptThePrivacyStatement() {
        organDonationCheckDetailsPage.accuracyCheckBox.click()
        organDonationCheckDetailsPage.privacyStatementCheckBox.click()
    }

    @Then("^the Organ Donation Check Details page is displayed")
    fun theOrganDonationCheckDetailsPageIsDisplayed() {
        val patient = SerenityHelpers.getPatient()
        organDonationCheckDetailsPage.assertDisplayed()
        organDonationCheckDetailsPage.assertPersonalDetailsSection(patient)
    }

    @Then("^the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page")
    fun choiceOfNotWishingToDonateOrgansIsDisplayedOnTheOrganDonationCheckDetailsPage() {
        organDonationCheckDetailsPage.yourDecisionModule.assertDecisionIsNo()
    }

    @Then("^the choice of wishing to donate organs is displayed on the Organ Donation Check Details page")
    fun choiceOfWishingToDonateOrgansIsDisplayedOnTheOrganDonationCheckDetailsPage() {
        organDonationCheckDetailsPage.yourDecisionModule.assertDecisionIsYes()
    }

    @Then("^my specific organ donation choices are displayed on the Organ Donation Check Details page")
    fun mySpecificOrganDonationChoicesAreDisplayedOnTheOrganDonaitonCheckDetailsPage(){
        val organsToDonate = Serenity.sessionVariableCalled<ArrayList<KeyValuePair<String, Boolean>>>(
                ORGAN_DONATION_DECISION_SOME_ORGANS)
        organDonationCheckDetailsPage.yourDecisionModule.assertDecisionIsSome(organsToDonate)
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

    @Then("^my choice of '(.*)' to share my faith and beliefs is displayed on the Organ Donation Check Details page")
    fun myChoiceToShareMyFaithAndBeliefsIsDisplayedOnTheOrganDonaitonCheckDetailsPage(option : String){
        organDonationCheckDetailsPage.faithAndBeliefsModule.assertChoice(option)
    }

    @Then("a validation message is shown if both or either of the required conditions for organ donation are not " +
            "checked")
    fun aValidationMessageIsShownIfBothOrEitherOfTheRequiredConditionsForOrganDonationAreNotChecked() {

        val problem = "There's a problem"
        val accuracyValidationMessage = "You must confirm that the information provided is true, complete and accurate."
        val privacyStatementValidationMessage = "You must confirm that you have read the privacy statement " +
                "and consent to your information being used accordingly."

        organDonationCheckDetailsPage.accuracyCheckBox.assertIsVisible()

        organDonationCheckDetailsPage.accuracyCheckBox.assertUnchecked()
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertUnchecked()
        organDonationCheckDetailsPage.clickSubmit()

        organDonationCheckDetailsPage.validationBanner.assertVisible(
                arrayListOf(problem, "$accuracyValidationMessage\n$privacyStatementValidationMessage"))

        organDonationCheckDetailsPage.accuracyCheckBox.click()
        organDonationCheckDetailsPage.accuracyCheckBox.assertChecked()
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertUnchecked()
        organDonationCheckDetailsPage.clickSubmit()

        organDonationCheckDetailsPage.validationBanner.assertVisible(
                arrayListOf(problem, privacyStatementValidationMessage))

        organDonationCheckDetailsPage.accuracyCheckBox.click()
        organDonationCheckDetailsPage.accuracyCheckBox.assertUnchecked()
        organDonationCheckDetailsPage.privacyStatementCheckBox.click()
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertChecked()
        organDonationCheckDetailsPage.clickSubmit()

        organDonationCheckDetailsPage.validationBanner.assertVisible(
                arrayListOf(problem, accuracyValidationMessage))
    }
}