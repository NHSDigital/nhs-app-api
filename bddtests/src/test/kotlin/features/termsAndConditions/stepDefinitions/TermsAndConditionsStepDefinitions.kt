package features.termsAndConditions.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.termsAndConditions.steps.TermsAndConditionsSteps
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps


class TermsAndConditionsStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var termsAndConditions: TermsAndConditionsSteps

    @When("^I click the continue button")
    fun iClickTheContinueButton() {
        termsAndConditions.continueWithTermsAndConditions()
    }

    @When("^I check the agree to terms and conditions checkbox")
    fun iCheckTheAgreeTermsCheckbox() {
        termsAndConditions.agreeToTermsAndConditions()
    }

    @When("^I click on Privacy policy")
    fun iClickOnPrivacyPolicy() {
        termsAndConditions.viewPrivacyPolicy()
    }

    @When("^I click on Cookies policy")
    fun iClickOnCookiesPolicy() {
        termsAndConditions.viewCookiesPolicy()
    }

    @When("^I click on Terms of use")
    fun iClickOnTermsOfUsePolicy() {
        termsAndConditions.viewTermsOfUse()
    }

    @Then("^I see error messages indicating I have not yet accepted the terms and conditions")
    fun iSeeErrorMessages() {
        termsAndConditions.secondaryErrorMessageVisible()
        termsAndConditions.mainErrorMessageVisible()
    }

    @Given("^I am on the Terms and conditions page$")
    fun iAmOnTheTermsAndConditionsPage() {
        assert(termsAndConditions.mainBodyTextVisible())
        assert(termsAndConditions.termsOfUseLinkVisible())
        assert(termsAndConditions.privacyPolicyLinkVisible())
        assert(termsAndConditions.cookiesPolicyLinkVisible())
        assert(termsAndConditions.tcCheckBoxVisible())
        assert(termsAndConditions.continueButtonVisible())
    }

}
