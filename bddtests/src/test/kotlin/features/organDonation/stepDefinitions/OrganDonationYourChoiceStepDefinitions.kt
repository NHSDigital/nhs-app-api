package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.organDonation.OrganDonationYourChoicePage

open class OrganDonationYourChoiceStepDefinitions {

    lateinit var organDonationYourChoicePage: OrganDonationYourChoicePage


    @When("^I select the option to donate all my organs")
    fun iSelectTheOptionToDonateAllMyOrgans() {
        organDonationYourChoicePage.radioButtons.button(organDonationYourChoicePage.allOfMyOrgans).select()
    }

    @Then("^the Organ Donation Your Choice page is displayed")
    fun theOrganDonationYourChoicePageIsDisplayed() {
        organDonationYourChoicePage.yourChoiceTitle.assertIsVisible()
        organDonationYourChoicePage.assertOptions()
        organDonationYourChoicePage.radioButtons.assertSelected(organDonationYourChoicePage.allOfMyOrgans)
    }
}
