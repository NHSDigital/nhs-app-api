package features.organDonation.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.data.organDonation.OrganDonationSerenityHelpers
import pages.organDonation.OrganDonationViewWithdrawRegistrationPage
import pages.organDonation.OrganDonationWithdrawDecisionPage
import utils.getOrFail

class OrganDonationWithdrawStepDefinitions {

    private lateinit var withdrawDecisionPage: OrganDonationWithdrawDecisionPage
    private lateinit var viewWithdrawRegistrationPage: OrganDonationViewWithdrawRegistrationPage

    @When("^I select an organ donation withdrawal reason from the list$")
    fun iSelectAnOrganDonationWithdrawalReasonFromTheList(){
        withdrawDecisionPage.withdrawalReasonList.selectByText(
                OrganDonationSerenityHelpers.ORGAN_DONATION_WITHDRAWAL_REASON.getOrFail())
    }

    @Then("^the Organ Donation Withdraw Decision page is displayed")
    fun theOrganDonationWithdrawDecisionPageIsDisplayed(){
        withdrawDecisionPage.assertDisplayed()
        withdrawDecisionPage.withdrawalReasonList.assertSelected(withdrawDecisionPage.defaultDropDownValue)
    }

    @Then("^the Organ Donation View Registration page is displayed with my decision to withdraw$")
    fun organDonationDecisionIssuccessfullyWithdrawnFromODR(){
        viewWithdrawRegistrationPage.assertDisplayed()
        viewWithdrawRegistrationPage.assertDecisionWithdrawn()
        viewWithdrawRegistrationPage.otherThings.assertOnlyBloodLinkPresent()
        viewWithdrawRegistrationPage.nextSteps.assertNotDisplayed()
    }

    @Then("^the organ donation withdraw decision reasons are shown sorted alphabetically$")
    fun theOrganDonationWithdrawDecisionReasonsAreSortedAlphabetically(){
        val expectedDropDownContent =
                OrganDonationSerenityHelpers.REFERENCE_WITHDRAWAL_REASONS.getOrFail<ArrayList<String>>()
        withdrawDecisionPage.withdrawalReasonList.assertSortedContent(
                withdrawDecisionPage.defaultDropDownValue,expectedDropDownContent)
    }

    @Then("^I am shown validation error on the page advising me to choose an organ donation withdrawal reason$")
    fun iAmShownValidationErrorAdvisingMeToChooseWithdrawalReason(){
        withdrawDecisionPage.assertWithdrawalListValidationError()
    }
}
