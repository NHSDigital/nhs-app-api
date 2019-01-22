package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import pages.organDonation.OrganDonationConfirmationPage

open class OrganDonationConfirmationStepDefinitions {

    lateinit var organDonationConfirmationPage: OrganDonationConfirmationPage

    @Then("^the Organ Donation Confirmation page is displayed$")
    fun theOrganDonationConfirmationPageIsDisplayed(){
        organDonationConfirmationPage.title.assertSingleElementPresent().assertIsVisible()
    }

    @Then("^the decision to opt out of organ donation has been successfully created$")
    fun theDecisionToOptOutOfOrganDonationHasBeenSuccessfullyCreated(){
        organDonationConfirmationPage.assertDecisionIsNo()
        organDonationConfirmationPage.assertSuccessBanner()
    }

    @Then("^the decision to opt in to organ donation has been successfully created$")
    fun theDecisionToOptInToOrganDonationHasBeenSuccessfullyCreated(){
        organDonationConfirmationPage.assertDecisionIsYes()
        organDonationConfirmationPage.assertSuccessBanner()
    }
}
