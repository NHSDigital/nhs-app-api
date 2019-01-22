package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.organDonation.OrganDonationFaithAndBeliefsPage

open class OrganDonationFaithAndBeliefsStepDefinitions {

    lateinit var organDonationFaithAndBeliefsPage: OrganDonationFaithAndBeliefsPage

    @When("^I select the option '(.*)' to share my organ donation faith and beliefs")
    fun iSelectTheOptionToShareMyOrganDonaitonFaithAndBeliefs(option:String){
        organDonationFaithAndBeliefsPage.radioButtons.button(option).select()
        organDonationFaithAndBeliefsPage.radioButtons.assertSelected(option)
    }

    @Then("^the Organ Donation Faith And Beliefs page is displayed")
    fun theOrganDonationFaithAndBeliefsPageIsDisplayed() {
        organDonationFaithAndBeliefsPage.assertIsDisplayed()
    }

    @Then("^no options on the Organ Donation Faith And Beliefs page are selected")
    fun noOptionsOnTheOrganDonationFaithAndBeliefsPageAreSelected() {
        organDonationFaithAndBeliefsPage.radioButtons.assertAllUnselected()
    }

    @Then("a validation message is shown if a user attempts to continue without selecting a faith and belief option")
    fun aValidationMessageIsShownIfAUserAttemptsToContinueWithoutSelectingAFaithAndBeliefOption() {

        organDonationFaithAndBeliefsPage.radioButtons.assertAllUnselected()
        organDonationFaithAndBeliefsPage.clickContinue()
        organDonationFaithAndBeliefsPage.validationBanner.assertVisible(arrayListOf("There's a problem",
                "You cannot continue without making a selection"))
    }
}
