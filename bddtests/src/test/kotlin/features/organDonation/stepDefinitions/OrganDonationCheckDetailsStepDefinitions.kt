package features.organDonation.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.organDonation.models.OrganDonationDemographics
import pages.assertIsVisible
import pages.organDonation.OrganDonationAdditionalDetailsModule
import pages.organDonation.OrganDonationCheckDetailsPage
import utils.SerenityHelpers
import utils.getOrFail

open class OrganDonationCheckDetailsStepDefinitions {

    lateinit var organDonationCheckDetailsPage: OrganDonationCheckDetailsPage

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
        organDonationCheckDetailsPage.assertContactDetailsSection(patient)
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
    fun mySpecificOrganDonationChoicesAreDisplayedOnTheOrganDonationCheckDetailsPage(){
        organDonationCheckDetailsPage.yourDecisionModule.assertDecisionIsSome(
                OrganDonationSerenityHelpers.SOME_ORGANS_UPDATED
                .getOrFail())
    }

    @Then("^my ethnicity and religion are recorded on the Organ Donation Check Details page")
    fun myEthnicityAndReligionIsRecordedOnTheOrganDonationCheckDetailsPage() {
        val demographics = OrganDonationSerenityHelpers.DEMOGRAPHICS_UPDATED
                .getOrFail<OrganDonationDemographics>()
        organDonationCheckDetailsPage.additionalDetailsModule.assertEthnicityAndReligion(demographics.ethnicity
                .value, demographics.religion.value)
    }

    @Then("^my ethnicity and religion are recorded as not chosen on the Organ Donation Check Details page")
    fun myEthnicityAndReligionIsRecordedAsNotChosenOnTheOrganDonationCheckDetailsPage() {
        organDonationCheckDetailsPage.additionalDetailsModule.assertEthnicityAndReligion(
                OrganDonationAdditionalDetailsModule.didNotAnswer,
                OrganDonationAdditionalDetailsModule.didNotAnswer)

    }

    @Then("^my choice of whether to share my faith and beliefs is displayed on the Organ Donation Check Details page")
    fun myChoiceOfWhetherToShareMyFaithAndBeliefsIsDisplayedOnTheOrganDonationCheckDetailsPage(){
        val demographics = OrganDonationSerenityHelpers.DEMOGRAPHICS_UPDATED
                .getOrFail<OrganDonationDemographics>()
        organDonationCheckDetailsPage.faithAndBeliefsModule.assertChoice(demographics.faithDeclaration)
    }

    @Then("^my choice of '(.*)' to share my faith and beliefs is displayed on the Organ Donation Check Details page")
    fun myChoiceToShareMyFaithAndBeliefsIsDisplayedOnTheOrganDonationCheckDetailsPage(option : String){
        organDonationCheckDetailsPage.faithAndBeliefsModule.assertChoice(option)
    }

    @Then("^my decision to withdraw is recorded on the Organ Donation Check Details page$")
    fun withdrawDecisionIsRecordedOnTheOrganDonationCheckDetailsPage(){
        organDonationCheckDetailsPage.yourDecisionModule.assertDecisionIsWithdrawn()
    }

    @Then("^there is no additional details section displayed on the Organ Donation Check Details page$")
    fun thereIsNoAdditionalDetailsSectionDisplayedOnTheOrganDonationCheckDetailsPage(){
        organDonationCheckDetailsPage.additionalDetailsModule.assertNotDisplayed()
    }

    @Then("^the additional details section is displayed on the Organ Donation Check Details page$")
    fun theAdditionalDetailsSectionDisplayedOnTheOrganDonationCheckDetailsPage(){
        myEthnicityAndReligionIsRecordedOnTheOrganDonationCheckDetailsPage()
    }

    @Then("a validation message is shown if both or either of the required conditions for organ donation are not " +
            "checked")
    fun aValidationMessageIsShownIfBothOrEitherOfTheRequiredConditionsForOrganDonationAreNotChecked() {

        val problem = "There's a problem"
        val accuracyValidationMessage = "Check your information. Confirm if it is accurate."
        val privacyStatementValidationMessage = "Read the privacy statement. Confirm if you give your consent."

        organDonationCheckDetailsPage.accuracyCheckBox.assertIsVisible()
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertIsVisible()

        verifyValidationWhenBothAreNotChecked(problem, accuracyValidationMessage, privacyStatementValidationMessage)
        verifyValidationWhenOnlyAccuracyIsChecked(problem, privacyStatementValidationMessage)
        verifyValidationWhenOnlyPrivacyIsChecked(problem, accuracyValidationMessage)
    }

    private fun verifyValidationWhenBothAreNotChecked(problem: String,
                                                      accuracyValidationMessage: String,
                                                      privacyStatementValidationMessage: String) {
        organDonationCheckDetailsPage.accuracyCheckBox.assertUnchecked()
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertUnchecked()
        organDonationCheckDetailsPage.clickSubmit()

        organDonationCheckDetailsPage.accuracyCheckBox.assertInlineError(accuracyValidationMessage)
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertInlineError(privacyStatementValidationMessage)
        organDonationCheckDetailsPage.validationBanner.assertVisible(
                arrayListOf(problem, "$accuracyValidationMessage\n$privacyStatementValidationMessage"))
    }

    private fun verifyValidationWhenOnlyAccuracyIsChecked(problem: String, privacyStatementValidationMessage: String) {
        organDonationCheckDetailsPage.accuracyCheckBox.click()
        organDonationCheckDetailsPage.accuracyCheckBox.assertChecked()
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertUnchecked()
        organDonationCheckDetailsPage.clickSubmit()

        organDonationCheckDetailsPage.accuracyCheckBox.assertNoInlineError()
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertInlineError(privacyStatementValidationMessage)
        organDonationCheckDetailsPage.validationBanner.assertVisible(
                arrayListOf(problem, privacyStatementValidationMessage))

        // revert selection
        organDonationCheckDetailsPage.accuracyCheckBox.click()
    }

    private fun verifyValidationWhenOnlyPrivacyIsChecked(problem: String, accuracyValidationMessage: String) {
        organDonationCheckDetailsPage.accuracyCheckBox.assertUnchecked()
        organDonationCheckDetailsPage.privacyStatementCheckBox.click()
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertChecked()
        organDonationCheckDetailsPage.clickSubmit()

        organDonationCheckDetailsPage.accuracyCheckBox.assertInlineError(accuracyValidationMessage)
        organDonationCheckDetailsPage.privacyStatementCheckBox.assertNoInlineError()
        organDonationCheckDetailsPage.validationBanner.assertVisible(
                arrayListOf(problem, accuracyValidationMessage))

        // revert selection
        organDonationCheckDetailsPage.privacyStatementCheckBox.click()
    }
}
