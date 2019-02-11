package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import models.KeyValuePair
import net.serenitybdd.core.Serenity
import cucumber.api.java.en.When
import pages.assertIsVisible
import pages.organDonation.OrganDonationConfirmationPage

open class OrganDonationConfirmationStepDefinitions {

    lateinit var organDonationConfirmationPage: OrganDonationConfirmationPage

    @When("I select the register to be a blood donor link on the Organ Donation Confirmation page")
    fun iSelectThePrivacyStatementLinkOnTheOrganDonationCheckDetailsPage() {
        organDonationConfirmationPage.registerBloodDonorLink.assertIsVisible().click()
    }

    @Then("^the Organ Donation Confirmation page is displayed$")
    fun theOrganDonationConfirmationPageIsDisplayed(){
        organDonationConfirmationPage.assertDisplayed()
    }

    @Then("^the decision to opt out of organ donation has been successfully created$")
    fun theDecisionToOptOutOfOrganDonationHasBeenSuccessfullyCreated(){
        organDonationConfirmationPage.assertCreatedBanner()
        organDonationConfirmationPage.decisionModule.assertDecisionIsNo()
    }

    @Then("^the decision to opt in to organ donation has been successfully created$")
    fun theDecisionToOptInToOrganDonationHasBeenSuccessfullyCreated(){
        organDonationConfirmationPage.assertCreatedBanner()
        organDonationConfirmationPage.decisionModule.assertDecisionIsYes()
    }

    @Then("^the decision to opt in to organ donation with some organs has been successfully created$")
    fun theDecisionToOptInToOrganDonationWithSomeOrgansHasBeenSuccessfullyCreated(){
        organDonationConfirmationPage.assertCreatedBanner()
        val organsToDonate = Serenity.sessionVariableCalled<ArrayList<KeyValuePair<String, Boolean>>>(
                ORGAN_DONATION_DECISION_SOME_ORGANS)
        organDonationConfirmationPage.decisionModule.assertDecisionIsSome(organsToDonate)
    }

    @Then("^the decision to opt out of organ donation is displayed$")
    fun theDecisionToOptOutOfOrganDonationIsDisplayed() {
        organDonationConfirmationPage.decisionModule.assertDecisionIsNo()
    }

    @Then("^the decision to opt in to organ donation with all organs is displayed$")
    fun theDecisionToOptInToAllOrganDonationIsDisplayed() {
        organDonationConfirmationPage.decisionModule.assertDecisionIsYes()
    }

    @Then("^the decision to opt in to organ donation with some organs is displayed$")
    fun theDecisionToOptInToSomeOrganDonationIsDisplayed() {
        val organsToDonate = Serenity.sessionVariableCalled<ArrayList<KeyValuePair<String, Boolean>>>(
                ORGAN_DONATION_DECISION_SOME_ORGANS)
        organDonationConfirmationPage.decisionModule.assertDecisionIsSome(organsToDonate)
    }

    @Then("^the choice of an organ donation appointed representative is displayed$")
    fun theChoiceOfAnOrganDonationAppointedRepresentativeIsDisplayed() {
        organDonationConfirmationPage.decisionModule.assertDecisionIsAppointedRepresentative()
    }

    @Then("the organ donation decision has been submitted and is to be processed")
    fun theOrganDonationDecisionHasBeenSubmittedAndIsToBeProcessed(){
        organDonationConfirmationPage.assertDecisionSubmitted()
    }

    @Then("the organ donation decision has been found and is to be processed")
    fun theOrganDonationDecisionHasBeenFoundAndIsToBeProcessed(){
        organDonationConfirmationPage.assertDecisionFound()
    }
}
