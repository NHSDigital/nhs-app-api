package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.getOrFail
import mocking.organDonation.models.FaithDeclaration
import mocking.organDonation.models.OrganDonationDemographics
import org.junit.Assert
import pages.organDonation.OrganDonationFaithAndBeliefsPage

open class OrganDonationFaithAndBeliefsStepDefinitions {

    lateinit var organDonationFaithAndBeliefsPage: OrganDonationFaithAndBeliefsPage

    @When("^I select the option '(.*)' to share my organ donation faith and beliefs")
    fun iSelectTheOptionToShareMyOrganDonationFaithAndBeliefs(option: String) {
        organDonationFaithAndBeliefsPage.radioButtons.button(option).select()
        organDonationFaithAndBeliefsPage.radioButtons.assertSelected(option)
    }

    @When("^I select an option in sharing my organ donation faith and beliefs")
    fun iSelectAnOptionInSharingMyOrganDonationFaithAndBeliefs() {
        val faith = OrganDonationSerenityHelpers.DEMOGRAPHICS_UPDATED
                .getOrFail<OrganDonationDemographics>().faithDeclaration
        organDonationFaithAndBeliefsPage.selectOption(faith)
    }

    @Then("^the Organ Donation Faith And Beliefs page is displayed")
    fun theOrganDonationFaithAndBeliefsPageIsDisplayed() {
        organDonationFaithAndBeliefsPage.assertDisplayed()
    }

    @Then("^the Organ Donation 'Examples of end of life wishes' is collapsed, and can be expanded")
    fun theOrganDonationExamplesOfEndOfLifeWishesIsCollapsedAndCanBeExpanded() {
        organDonationFaithAndBeliefsPage.endOfLifeWishes.assertCollapsed()
        organDonationFaithAndBeliefsPage.endOfLifeWishes.expand()
        organDonationFaithAndBeliefsPage.endOfLifeWishes.assertLabel("Examples of end of life wishes")
        organDonationFaithAndBeliefsPage.endOfLifeWishes.assertContent(
                "Requesting a faith representative for your family\n" +
                        "When to say prayers\n" +
                        "Rituals or traditions regards washing and dressing\n" +
                        "Being buried within a certain time period")
        organDonationFaithAndBeliefsPage.endOfLifeWishes.collapse()
        organDonationFaithAndBeliefsPage.endOfLifeWishes.assertCollapsed()
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
                "Respond to the faith/belief declaration. Choose yes, no or prefer not to say."))
    }

    @Then("^the previous option on the Organ Donation Faith And Beliefs page is selected")
    fun thePreviousOptionOnTheOrganDonationFaithAndBeliefsPageIsSelected() {
        val faithDeclaration = OrganDonationSerenityHelpers.DEMOGRAPHICS_EXISTING
                .getOrFail<OrganDonationDemographics>().faithDeclaration

        val optionMap = mapOf(
                FaithDeclaration.No to organDonationFaithAndBeliefsPage.noOption,
                FaithDeclaration.Yes to organDonationFaithAndBeliefsPage.yesOption,
                FaithDeclaration.NotStated to organDonationFaithAndBeliefsPage.preferNotToSayOption
        )
        Assert.assertTrue("Test setup incorrect, expected mapping for $faithDeclaration",
                optionMap.containsKey(faithDeclaration))
        organDonationFaithAndBeliefsPage.radioButtons.assertSelected(optionMap[faithDeclaration]!!)
    }
}

