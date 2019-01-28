package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
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
    
    @When("I select the register to be a blood donor link on the Organ Donation Confirmation page")
    fun iSelectThePrivacyStatementLinkOnTheOrganDonationCheckDetailsPage() {
        organDonationConfirmationPage.registerBloodDonorLink.assertIsVisible().click()
    }
}
