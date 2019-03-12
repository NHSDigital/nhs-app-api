package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDecisions
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.getOrFail
import mocking.data.organDonation.set
import pages.assertIsVisible
import pages.organDonation.OrganDonationViewRegistrationPage

open class OrganDonationViewRegistrationStepDefinitions {

    private lateinit var organDonationViewRegistrationPage: OrganDonationViewRegistrationPage

    @When("I select the 'Register to be a blood donor' link on the Organ Donation View Registration page$")
    fun iSelectThePrivacyStatementLinkOnTheOrganDonationCheckDetailsPage() {
        organDonationViewRegistrationPage.otherThings.link(
                organDonationViewRegistrationPage.registerBloodDonorLinkTitle)
                .assertIsVisible().click()
    }

    @When("^I select the 'Share that you are a donor' link on the Organ Donation View Registration page$")
    fun iSelectTheShareThatYouAreADonorLinkOnTheOrganDonationViewRegistrationPage() {
        organDonationViewRegistrationPage.nextSteps.link(organDonationViewRegistrationPage.shareLinkTitle)
                .assertIsVisible().click()
    }

    @When("^I select the 'Tell your family' link on the Organ Donation View Registration page$")
    fun iSelectTheTellYourFamilyLinkOnTheOrganDonationViewRegistrationPage() {
        organDonationViewRegistrationPage.nextSteps.link(organDonationViewRegistrationPage.tellFamilyLinkTitle)
                .assertIsVisible().click()
    }

    @When("^I choose to amend my Organ Donation decision$")
    fun iChooseToAmendMyDecision() {
        OrganDonationSerenityHelpers.IS_AMEND_JOURNEY.set(true)
        organDonationViewRegistrationPage.amendDecisionLink.click()
    }

    @When("^I choose to reaffirm my organ donation decision$")
    fun iChooseToReaffirmMyOrganDonationDecision(){
        OrganDonationSerenityHelpers.IS_AMEND_JOURNEY.set(true)
        organDonationViewRegistrationPage.reaffirmDecisionLink.assertIsVisible().click()
    }

    @Then("^the Organ Donation View Registration page is displayed$")
    fun theOrganDonationViewRegistrationPageIsDisplayed() {
        organDonationViewRegistrationPage.assertDisplayed()
    }

    @Then("^the decision to opt out of organ donation has been successfully created$")
    fun theDecisionToOptOutOfOrganDonationHasBeenSuccessfullyCreated() {
        organDonationViewRegistrationPage.assertCreatedBanner()
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsNo()
        organDonationViewRegistrationPage.otherThings.assertLinksPresent()
        organDonationViewRegistrationPage.nextSteps.assertDisplayedWithText()
    }

    @Then("^the decision to opt in to organ donation has been successfully created$")
    fun theDecisionToOptInToOrganDonationHasBeenSuccessfullyCreated() {
        organDonationViewRegistrationPage.assertCreatedBanner()
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsYes()
        organDonationViewRegistrationPage.otherThings.assertLinksPresent()
        organDonationViewRegistrationPage.nextSteps.assertLinksPresent()
    }

    @Then("^the decision to opt in to organ donation with some organs has been successfully created$")
    fun theDecisionToOptInToOrganDonationWithSomeOrgansHasBeenSuccessfullyCreated() {
        organDonationViewRegistrationPage.assertCreatedBanner()
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsSome(
                OrganDonationSerenityHelpers.SOME_ORGANS_UPDATED.getOrFail()
        )
        organDonationViewRegistrationPage.otherThings.assertLinksPresent()
        organDonationViewRegistrationPage.nextSteps.assertLinksPresent()
    }

    @Then("^the decision to opt out of organ donation is displayed$")
    fun theDecisionToOptOutOfOrganDonationIsDisplayed() {
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsNo()
    }

    @Then("^the decision to opt in to organ donation with all organs is displayed$")
    fun theDecisionToOptInToAllOrganDonationIsDisplayed() {
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsYes()
    }

    @Then("^the decision to opt in to organ donation with some organs is displayed$")
    fun theDecisionToOptInToSomeOrganDonationIsDisplayed() {
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsSome(
                OrganDonationSerenityHelpers.SOME_ORGANS_UPDATED.getOrFail()
        )
    }

    @Then("^the existing decision to opt in to organ donation with some organs is displayed$")
    fun theExistingDecisionToOptInToSomeOrganDonationIsDisplayed() {
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsSome(
                OrganDonationSerenityHelpers.SOME_ORGANS_EXISTING.getOrFail()
        )
    }

    @Then("^the choice of an organ donation appointed representative is displayed$")
    fun theChoiceOfAnOrganDonationAppointedRepresentativeIsDisplayed() {
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsAppointedRepresentative()
    }

    @Then("the organ donation decision has been submitted and is to be processed$")
    fun theOrganDonationDecisionHasBeenSubmittedAndIsToBeProcessed() {
        organDonationViewRegistrationPage.assertDecisionSubmitted()
    }

    @Then("the organ donation decision has been found and is to be processed$")
    fun theOrganDonationDecisionHasBeenFoundAndIsToBeProcessed() {
        organDonationViewRegistrationPage.assertDecisionFound()
    }

    @Then("^the Organ Donation View Registration page is displayed with my existing decision to opt-in$")
    fun theOrganDonationPageIsDisplayedWithMyExistingDecisionToOptIn() {
        organDonationViewRegistrationPage.assertDisplayed()
        organDonationViewRegistrationPage.otherThings.assertNotDisplayed()
        organDonationViewRegistrationPage.nextSteps.assertLinksPresent()
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsYes()
    }

    @Then("^the Organ Donation View Registration page is displayed with my existing decision to opt-out$")
    fun theOrganDonationPageIsDisplayedWithMyExistingDecisionToOptOut() {
        organDonationViewRegistrationPage.assertDisplayed()
        organDonationViewRegistrationPage.otherThings.assertNotDisplayed()
        organDonationViewRegistrationPage.nextSteps.assertDisplayedWithText()
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsNo()
    }

    @Then("^the Organ Donation View Registration page is displayed with my existing decision of some$")
    fun theOrganDonationPageIsDisplayedWithMyExistingDecisionOfSome() {
        organDonationViewRegistrationPage.assertDisplayed()
        organDonationViewRegistrationPage.otherThings.assertNotDisplayed()
        organDonationViewRegistrationPage.nextSteps.assertLinksPresent()
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsSome(
                OrganDonationSerenityHelpers.SOME_ORGANS_EXISTING.getOrFail())
    }

    @Then("^the decision to opt out of organ donation has been successfully updated$")
    fun theDecisionToOptOutOfOrganDonationHasBeenSuccessfullyUpdated() {
        organDonationViewRegistrationPage.assertCreatedBanner()
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsNo()
    }

    @Then("^the decision to opt in to organ donation has been successfully updated$")
    fun theDecisionToOptInToOrganDonationHasBeenSuccessfullyUpdated() {
        organDonationViewRegistrationPage.assertCreatedBanner()
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsYes()
    }

    @Then("^the decision to opt in to organ donation with some organs has been successfully updated$")
    fun theDecisionToOptInToOrganDonationWithSomeOrgansHasBeenSuccessfullyUpdated() {
        organDonationViewRegistrationPage.assertCreatedBanner()
        val organsToDonate = OrganDonationSerenityHelpers.SOME_ORGANS_UPDATED
                .getOrFail<OrganDecisions>()
        organDonationViewRegistrationPage.decisionModule.assertDecisionIsSome(organsToDonate)
    }
}
