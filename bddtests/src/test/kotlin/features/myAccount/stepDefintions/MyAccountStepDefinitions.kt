package features.myAccount.stepDefintions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import utils.SerenityHelpers
import pages.MyAccountPage


class MyAccountStepDefinitions {

    lateinit var myAccount: MyAccountPage

    @When("^I click the Terms of use link$")
    fun iClickTheTermsAndConditionsLink() {
        myAccount.termsOfUseLink.click()
    }

    @When("^I click the Privacy policy link$")
    fun iClickThePrivacyPolicyLink() {
        myAccount.privacyPolicyLink.click()
    }

    @When("^I click the Cookies policy link$")
    fun iClickTheCookiesPolicyLink() {
        myAccount.cookiesPolicyLink.click()
    }

    @When("^I click the Open source licences link$")
    fun iClickTheOpenSourceLicencesLink() {
        myAccount.openSourceLicencesLink.click()
    }

    @When("^I click the Help and support link$")
    fun iClickTheHelpAndSupportLink() {
        myAccount.helpAndSupportLink.click()
    }

    @When("^I click the Accessibility statement link$")
    fun iClickTheAccessibilityStatementLink() {
        myAccount.accessibilityStatementLink.element.click()
    }

    @Then("^I am on the My Account page$")
    fun iAmOnTheMyAccountPage() {
        myAccount.signOutButton.assertIsVisible()
        myAccount.isAboutUsHeaderVisible()
        myAccount.assertAllLinksVisible()
    }

    @Then("^I see my personal details")
    fun iSeeMyPersonalDetails() {

        val patient = SerenityHelpers.getPatient()
        myAccount.assertPersonalDetailsVisible(patient.formattedFullName(),
                patient.formattedDateOfBirth(),
                patient.formattedNHSNumber())
    }
}
