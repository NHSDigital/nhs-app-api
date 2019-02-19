package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.getOrFail
import mocking.organDonation.models.FaithDeclaration
import mocking.organDonation.models.OrganDonationDemographics
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
        organDonationFaithAndBeliefsPage.assertDisplayed()
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

    @Then("^the previous option on the Organ Donation Faith And Beliefs page is selected")
    fun thePreviousOptionOnTheOrganDonationFaithAndBeliefsPageIsSelected() {
        val faithDeclaration = OrganDonationSerenityHelpers.DEMOGRAPHICS
                .getOrFail<OrganDonationDemographics>().faithDeclaration
        when (faithDeclaration) {
            FaithDeclaration.No ->
                organDonationFaithAndBeliefsPage.radioButtons.assertSelected("No")
            FaithDeclaration.Yes ->
                organDonationFaithAndBeliefsPage.radioButtons.assertSelected("Yes")
            FaithDeclaration.NotStated ->
                organDonationFaithAndBeliefsPage.radioButtons.assertSelected("Prefer not to say")
        }
    }
}
