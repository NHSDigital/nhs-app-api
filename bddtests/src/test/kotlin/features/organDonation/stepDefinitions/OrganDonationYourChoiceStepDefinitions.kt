package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.organDonation.OrganDonationYourChoicePage

open class OrganDonationYourChoiceStepDefinitions {

    lateinit var organDonationYourChoicePage: OrganDonationYourChoicePage

    @When("^I select the option to donate all my organs")
    fun iSelectTheOptionToDonateSomeOfMyOrgans() {
        organDonationYourChoicePage.radioButtons.button(organDonationYourChoicePage.allOfMyOrgans).select()
    }

    @When("^I select the option to donate some of my organs")
    fun iSelectTheOptionToDonateAllMyOrgans() {
        organDonationYourChoicePage.radioButtons.button(organDonationYourChoicePage.someOfMyOrgans).select()
    }

    @Then("^the Organ Donation Your Choice page is displayed")
    fun theOrganDonationYourChoicePageIsDisplayed() {
        organDonationYourChoicePage.assertDisplayed()
    }

    @Then("^a validation message is shown if a user attempts to continue without choosing to donate all or some organs")
    fun aValidationMessageIsShownIfAUserAttemptsToContinueWithoutChoosingToDonateAllOrSomeOrgans() {
        organDonationYourChoicePage.assertOptions()
        organDonationYourChoicePage.radioButtons.assertAllUnselected()
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertVisible(arrayListOf("There's a problem",
                "Make a decision to continue"))
    }
}
