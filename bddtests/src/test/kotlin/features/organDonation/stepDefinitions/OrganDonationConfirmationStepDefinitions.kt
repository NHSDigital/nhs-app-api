package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.assertIsVisible
import mocking.data.organDonation.OrganDonationSerenityHelpers
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
        organDonationConfirmationPage.decisionModule.assertDecisionIsSome(
                OrganDonationSerenityHelpers.getOrganDonationDecisionSomeOrgansUpdated()
        )
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
        organDonationConfirmationPage.decisionModule.assertDecisionIsSome(
                OrganDonationSerenityHelpers.getOrganDonationDecisionSomeOrgansUpdated()
        )
    }

    @Then("^the existing decision to opt in to organ donation with some organs is displayed$")
    fun theExistingDecisionToOptInToSomeOrganDonationIsDisplayed() {
        organDonationConfirmationPage.decisionModule.assertDecisionIsSome(
                OrganDonationSerenityHelpers.getOrganDonationDecisionSomeOrgansExisting()
        )
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

    @Then("^the Organ Donation page is displayed with my existing decision to opt-in$")
    fun theOrganDonationPageIsDisplayedWithMyExistingDecisionToOptIn() {
        organDonationConfirmationPage.assertDisplayed()
        organDonationConfirmationPage.decisionModule.assertDecisionIsYes()
    }

    @Then("^the Organ Donation page is displayed with my existing decision to opt-out$")
    fun theOrganDonationPageIsDisplayedWithMyExistingDecisionToOptOut() {
        organDonationConfirmationPage.assertDisplayed()
        organDonationConfirmationPage.decisionModule.assertDecisionIsNo()
    }

    @Then("^the Organ Donation page is displayed with my existing decision of some$")
    fun theOrganDonationPageIsDisplayedWithMyExistingDecisionOfSome() {
        organDonationConfirmationPage.assertDisplayed()
        organDonationConfirmationPage.decisionModule.assertDecisionIsSome(
                OrganDonationSerenityHelpers.getOrganDonationDecisionSomeOrgansExisting())
    }

    @Then("^the decision to opt out of organ donation has been successfully updated$")
    fun theDecisionToOptOutOfOrganDonationHasBeenSuccessfullyUpdated(){
        organDonationConfirmationPage.assertCreatedBanner()
        organDonationConfirmationPage.decisionModule.assertDecisionIsNo()
    }

    @Then("^the decision to opt in to organ donation has been successfully updated$")
    fun theDecisionToOptInToOrganDonationHasBeenSuccessfullyUpdated(){
        organDonationConfirmationPage.assertCreatedBanner()
        organDonationConfirmationPage.decisionModule.assertDecisionIsYes()
    }

    @Then("^the decision to opt in to organ donation with some organs has been successfully updated$")
    fun theDecisionToOptInToOrganDonationWithSomeOrgansHasBeenSuccessfullyUpdated(){
        organDonationConfirmationPage.assertCreatedBanner()
        val organsToDonate = OrganDonationSerenityHelpers.getOrganDonationDecisionSomeOrgansUpdated()
        organDonationConfirmationPage.decisionModule.assertDecisionIsSome(organsToDonate)
    }}
