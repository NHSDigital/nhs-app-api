package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDonationSerenityHelpers
import pages.organDonation.OrganDonationSpecificOrganChoicePage

open class OrganDonationSpecificOrganChoiceStepDefinitions {

    lateinit var organDonationYourChoicePage: OrganDonationSpecificOrganChoicePage

    @When("^I choose which organs to donate")
    fun iChooseWhichOrgansToDonate() {
        val organsToDonate =
                OrganDonationSerenityHelpers.getOrganDonationDecisionSomeOrgansUpdated()
        organsToDonate.forEach { organ ->
            organDonationYourChoicePage.chooseOption(organ.key, organ.value)
        }
        OrganDonationSerenityHelpers.setOrganDonationDecisionSomeOrgansExisting(organsToDonate)
    }

    @Then("^the Organ Donation Specific Organ Choice page is displayed")
    fun theOrganDonationYourChoicePageIsDisplayed() {
        organDonationYourChoicePage.assertDisplayed()
    }

    @Then("^a validation message is shown if a user attempts to continue without selecting a decision for all organs")
    fun aValidationMessageIsShownIfAUserAttemptsToContinueWithoutSelectingADecisionForAllOrgans() {

        val errorMessage = arrayListOf("There's a problem",
                "Make a decision for all categories to continue")

        organDonationYourChoicePage.organOptions.forEach { option -> option.assertAllUnselected() }
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertVisible(errorMessage)

        val organToDonate =
                OrganDonationSerenityHelpers.getOrganDonationDecisionSomeOrgansUpdated().first()

        organDonationYourChoicePage.chooseOption(organToDonate.key, organToDonate.value)
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertVisible(errorMessage)

    }

    @Then("^a validation message is shown if a user attempts to continue with all specific organ options set to no")
    fun aValidationMessageIsShownIfAUserAttemptsToContinueWithAllSpecificOrganOptionsSetToNo() {
        organDonationYourChoicePage.organOptions.forEach { option -> option.select(false) }
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertVisible(arrayListOf("There's a problem",
                "At least one category must be set to yes to continue"))
    }

    @Then("^my previous decisions are displayed on the Organ Donation Specific Organ Choice page")
    fun myPreviousDecisionsAreDisplayedOnTheOrganDonationSpecificOrganChoicePage() {
        val organsToDonate =
                OrganDonationSerenityHelpers.getOrganDonationDecisionSomeOrgansExisting()
        organsToDonate.forEach { organ ->
            organDonationYourChoicePage.assertOrganOption(organ.key, organ.value)
        }
    }

    @Then("^no options on the Organ Donation Specific Organ Choice page are selected")
    fun noOptionsOnTheOrganDonationSpecificOrganChoicePageAreSelected() {
        organDonationYourChoicePage.organOptions.forEach { option -> option.assertAllUnselected() }
    }
}
