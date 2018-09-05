package features.myAccount.stepDefintions

import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myAccount.steps.MyAccountSteps
import net.thucydides.core.annotations.Steps


class MyAccountStepDefinitions {

    @Steps
    lateinit var myAccount: MyAccountSteps

    @Then("^I am on the My Account page$")
    fun iAmOnTheMyAccountPage() {
        myAccount.assertSignoutButtonVisible()
        myAccount.assertAboutUsHeaderVisible()
        myAccount.myAccountPage.assertAllLinksVisible()
    }

    @When("^I click the Terms and conditions link$")
    fun iClickTheTermsAndConditionsLink() {
        myAccount.goToTermsAndConditions()
    }

    @When("^I click the Privacy policy link$")
    fun iClickThePrivacyPolicyLink() {
        myAccount.goToPrivacyPolicy()
    }

    @When("^I click the Cookies policy link$")
    fun iClickTheCookiesPolicyLink() {
        myAccount.goToCookiesPolicy()
    }

    @When("^I click the Open source licenses link$")
    fun iClickTheOpenSourceLicensesLink() {
        myAccount.goToOpenSourceLicenses()
    }

    @When("^I click the Help and support link$")
    fun iClickTheHelpAndSupportLink() {
        myAccount.goToHelpAndSupport()
    }
}
