package features.termsAndConditions.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.termsAndConditions.steps.TermsAndConditionsSteps
import mocking.CosmosDb
import net.thucydides.core.annotations.Steps
import java.time.OffsetDateTime

class TermsAndConditionsStepDefinitions {

    @Steps
    lateinit var termsAndConditions: TermsAndConditionsSteps


    @Given("^I am on the Terms and conditions page$")
    fun iAmOnTheTermsAndConditionsPage() {
        assert(termsAndConditions.mainBodyTextVisible())
        assert(termsAndConditions.tcCheckBoxVisible())
        assert(termsAndConditions.continueButtonVisible())
    }

    @Given("^I have not already accepted terms and conditions$")
    fun iHaveNotAcceptedTermsAndConditions() {
        CosmosDb.clearTermsAndConditionsAcceptance()
    }

    @Given("^I have already accepted terms and conditions$")
    fun iHaveAlreadyAcceptedTermsAndConditions() {
        CosmosDb.clearTermsAndConditionsAcceptance()
        CosmosDb.addTermsAndConditionsAcceptance(OffsetDateTime.now())
    }

    @When("^I click the back arrow$")
    fun iClickTheBackArrow() {
        termsAndConditions.termsAndConditionsPage.tcBackButton.click()
    }

    @When("^I click the continue button on Terms and Conditions$")
    fun iClickTheContinueButtonOnTermsAndConditions() {
        termsAndConditions.termsAndConditionsPage.continueButton.click()
    }

    @When("^I check the agree to terms and conditions checkbox$")
    fun iAgreeToTheTermsAndConditions() {
        termsAndConditions.termsAndConditionsPage.termsAndConditionsLabel.click()
    }

    @Then("^I see error messages indicating I have not yet accepted the terms and conditions$")
    fun iSeeErrorMessages() {
        termsAndConditions.secondaryErrorMessageVisible()
        termsAndConditions.mainErrorMessageVisible()
    }
}
