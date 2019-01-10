package features.termsAndConditions.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.termsAndConditions.steps.UpdatedTermsAndConditionsSteps
import mocking.CosmosDb
import net.thucydides.core.annotations.Steps
import java.time.OffsetDateTime

class UpdatedTermsAndConditionsStepDefinitions {

    @Steps
    lateinit var updatedTermsAndConditions: UpdatedTermsAndConditionsSteps

    @When("^I click the continue button on Updated Terms and Conditions$")
    fun iClickTheContinueButtonOnUpdatedTermsAndConditions() {
        updatedTermsAndConditions.updatedTermsAndConditionsPage.continueButton.click()
    }

    @Given("^I see the updated Terms and conditions page$")
    fun iSeeTheUpdatedTermsAndConditionsPage() {
        updatedTermsAndConditions.thePageHeaderIsVisible()
    }

    @Given("^I have previously accepted terms and conditions and updated terms and conditions exist$")
    fun iHavePreviouslyAcceptedTermsAndConditionsAndUpdatedAcceptanceIsRequired() {
        CosmosDb.clearTermsAndConditionsAcceptance()
        CosmosDb.addTermsAndConditionsAcceptance(OffsetDateTime.parse("2018-11-11T00:00:00+00:00"))
    }

    @Then("^I see error messages indicating I have not yet accepted the updated terms and conditions$")
    fun iSeeErrorMessageIndicatingIHaventAcceptedTheUpdatedTermsAndConditions() {
        updatedTermsAndConditions.secondaryErrorMessageVisible()
        updatedTermsAndConditions.mainErrorMessageVisible()
    }
}